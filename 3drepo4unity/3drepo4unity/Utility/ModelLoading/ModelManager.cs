/*
 *	Copyright (C) 2018 3D Repo Ltd
 *
 *	This program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU Affero General Public License as
 *	published by the Free Software Foundation, either version 3 of the
 *	License, or (at your option) any later version.
 *
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU Affero General Public License for more details.
 *
 *	You should have received a copy of the GNU Affero General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *  Author: Sebastian J Friston
 */

using System.Collections.Generic;
using RepoForUnity.DataModels.JSONModels;
using UnityEngine;

namespace RepoForUnity.Utility
{
    internal class ModelManager
    {
        private RepoWebClientInterface repoHttpClient;
        private Shader opaqueShader, transShader;
        private AddShaderControllerCallback attachComponentCallback;
        private double[] worldOffset = null;

        internal ModelManager(RepoWebClientInterface repoHttpClient)
        {
            this.repoHttpClient = repoHttpClient;
        }

        internal Model[] LoadModel(string teamspace, string modelID, string revisionId,
            Shader opaqueShader, Shader transparentShader, AddShaderControllerCallback callback)
        {
            List<Model> models = new List<Model>();
            this.opaqueShader = opaqueShader;
            transShader = transparentShader;
            attachComponentCallback = callback;

            var assetInfo = repoHttpClient.GetUnityAssetInfo(teamspace, modelID, revisionId);

            if (assetInfo != null)
            {
                foreach(var modelAssetInfo in assetInfo.models)
                {
                    var subModel = LoadModel(modelAssetInfo);
                    if (subModel != null)
                        models.Add(subModel);
                }
            }
            else
            {
                throw new RepoModelLoadingException("Failed to fetch Unity Asset Information for model: " + teamspace + "." + modelID);
            }

            return models.ToArray();
        }

        private Model LoadModel(DataModels.JSONModels.AssetInfo modelInfo)
        {

            var assetBundlesURI = modelInfo.vrAssets;
            if (modelInfo.vrAssets == null || modelInfo.vrAssets.Length == 0)
            {
                if(modelInfo.assets != null && modelInfo.assets.Length > 0)
                {
                    Debug.LogWarning("Win 64 bundles are not generated for this model. Using WebGL bundles...");
                    assetBundlesURI = modelInfo.assets;
                }                
                else{
                    throw new RepoModelLoadingException("This model does not have any unity bundles.");
                }
            }

            Vector3 relativeOffset = Vector3.zero;

            if (worldOffset == null)
                worldOffset = modelInfo.offset;
            else
            {
                relativeOffset = new Vector3((float)(modelInfo.offset[0] - worldOffset[0]),
                                                (float)(modelInfo.offset[1] - worldOffset[1]),
                                               (float)-(modelInfo.offset[2] - worldOffset[2])); //unity goes towards the other direction than WebGL
            }
           
            Dictionary<string, SuperMeshInfo> supermeshes = new Dictionary<string, SuperMeshInfo>();

            var revisionId = ExtractRevisionIDFromURI(assetBundlesURI[0]);

            var settings = repoHttpClient.LoadModelSettings(modelInfo.database, modelInfo.model);

            //TODO: this can be done asynchronously
            for (int i = 0; i < modelInfo.jsonFiles.Length; ++i)
            {
                var info =  ProcessSuperMeshInfo(repoHttpClient.LoadBundleJSON(modelInfo.jsonFiles[i]));
                supermeshes[info.name] = info;
            }

            //TODO: this can be done asynchronously
            for (int i = 0; i < assetBundlesURI.Length; ++i)
            {
                AssetBundle bundle =  repoHttpClient.LoadBundle(assetBundlesURI[i]);

                var names = bundle.GetAllAssetNames();
                var bundleObj = bundle.LoadAsset(names[0]) as GameObject;
                var superMeshName = bundleObj.name;
                var gameObject = UnityEngine.Object.Instantiate(bundleObj);
                gameObject.transform.position += relativeOffset;
                gameObject.name = superMeshName;

                supermeshes[superMeshName].gameObj = gameObject;
                AttachShader(supermeshes[superMeshName]);
                bundle.Unload(false);
            }

            return new Model(modelInfo.database, modelInfo.model, revisionId, settings,
                supermeshes, new Vector3((float)modelInfo.offset[0], (float)modelInfo.offset[1], (float)modelInfo.offset[2]),
                repoHttpClient);
        }

        private string ExtractRevisionIDFromURI(string uri)
        {
            string revId = null;
            int index;
            if((index = uri.LastIndexOf("/revision/")) != -1)
            {
                index += "/revision/".Length;
                string substring = uri.Substring(index);
                string rev = substring.Split('/')[0];
                if(rev != "master")
                {
                    revId = rev;
                }
            }

            return revId;
        }

        private SuperMeshInfo ProcessSuperMeshInfo(AssetMapping assetMapping)
        {
            SuperMeshInfo info = new SuperMeshInfo();
            info.nSubMeshes = assetMapping.mapping.Length;
            info.indexToID = new string[info.nSubMeshes];

            if (assetMapping.mapping.Length > 0)
            {
                var supermeshID = assetMapping.mapping[0].usage[0];
                info.name = supermeshID.Remove(supermeshID.LastIndexOf('_'));

                for(int i = 0; i < assetMapping.mapping.Length; ++i)
                {
                    info.indexToID[i] = assetMapping.mapping[i].name;
                }
            }

            return info;
        }

        private void AttachShader(SuperMeshInfo info)
        {
            var renderers = info.gameObj.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                var materials = r.sharedMaterials;
                foreach (var material in materials)
                {
                    material.shader = material.name.Contains("Transparent") ? transShader: opaqueShader;
                }

                string superMesh = r.gameObject.name.Remove(r.gameObject.name.LastIndexOf('_'));
                string model = r.gameObject.transform.root.name;
                
                int height = 0;
                int width = 0;
                CalculateTextureSquare(info.nSubMeshes, out width, out height);
                attachComponentCallback(r.gameObject, width, height);
                
            }
        }

        /**
        * Calculate the the width and length of a square, determined by the length
        */
        private void CalculateTextureSquare(int length, out int width, out int height)
        {
            float num = Mathf.Sqrt(length);
            width = Mathf.RoundToInt(num);
            height = Mathf.CeilToInt(num);
        }

    }
}
