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
 *
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

        /**
         * Model Manager manages the construction of the Model Object.
         * @param repoHttpClient takes a  RepoWebClientInterface connected to an API service
         */
        internal ModelManager(RepoWebClientInterface repoHttpClient)
        {
            this.repoHttpClient = repoHttpClient;
        }

        /**
         * Load a Model. This may return multiple models if the specified model is a federation
         * @params teamspace the teamspace this model resides in
         * @params modelId ID of the model
         * @param revisionId revision ID to load (null for latest)
         * @param opaqueShader a Unity Opaque shader to be attached to the opaque mesh objects
         * @param transparentShader a Unity Opaque shader to be attached to the translucent mesh objects
         * @params callback a callback function to attach a controller to control the shader (null if not needed)
         * @return returns the an array of models, containing the model information
         */
        internal Model[] LoadModel(string teamspace, string modelId, string revisionId,
            Shader opaqueShader, Shader transparentShader, AddShaderControllerCallback callback)
        {
            List<Model> models = new List<Model>();
            this.opaqueShader = opaqueShader;
            transShader = transparentShader;
            attachComponentCallback = callback;

            var assetInfo = repoHttpClient.GetUnityAssetInfo(teamspace, modelId, revisionId);

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
                throw new RepoModelLoadingException("Failed to fetch Unity Asset Information for model: " + teamspace + "." + modelId);
            }

            return models.ToArray();
        }

        /**
         * Load model given the model Info. This is typically called by the other LoadModel function.
         * @param modelInfo an object containing URIs to model information, this is obtained by  RepoWebInterface::GetUnityAssetInfo()
         * @return returns a model object containing model information
         */
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
            Dictionary<string, List<MeshLocation>> meshToLocations = new Dictionary<string, List<MeshLocation>>();

            var revisionId = ExtractRevisionIdFromURI(assetBundlesURI[0]);

            var settings = repoHttpClient.LoadModelSettings(modelInfo.database, modelInfo.model);

            //TODO: this can be done asynchronously
            for (int i = 0; i < modelInfo.jsonFiles.Length; ++i)
            {
                var info =  ProcessSuperMeshInfo(repoHttpClient.LoadBundleJSON(modelInfo.jsonFiles[i]), meshToLocations);
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
                supermeshes, meshToLocations, new Vector3((float)modelInfo.offset[0], (float)modelInfo.offset[1], (float)modelInfo.offset[2]),
                repoHttpClient);
        }

        /**
         * A utility function to extract revision ID from a URI
         * @params uri uri to extract from
         * @return returns the extracted revision ID. null if uri points to latest revision.
         */
        private string ExtractRevisionIdFromURI(string uri)
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

        /**
         * Given an AssetMapping object, process the information Super mesh information
         * @params assetMapping an AssetMapping object to digest
         * @return returns a SuperMeshInfo object containing the digested information. 
         */
        private SuperMeshInfo ProcessSuperMeshInfo(AssetMapping assetMapping, Dictionary<string, List<MeshLocation>> meshLocations)
        {
            SuperMeshInfo info = new SuperMeshInfo();
            info.nSubMeshes = assetMapping.mapping.Length;
            info.indexToId = new string[info.nSubMeshes];

            if (assetMapping.mapping.Length > 0)
            {
                var supermeshId = assetMapping.mapping[0].usage[0];
                info.name = supermeshId.Remove(supermeshId.LastIndexOf('_'));

                for(int i = 0; i < assetMapping.mapping.Length; ++i)
                {
                    var meshId = assetMapping.mapping[i].name;
                    info.indexToId[i] = meshId;
                    if (!meshLocations.ContainsKey(meshId))
                    {
                        meshLocations[meshId] = new List<MeshLocation>();
                    }
                    meshLocations[meshId].Add(new MeshLocation(supermeshId, i));
                }
            }

            return info;
        }

        /**
         * Given a SuperMeshInfo with game objects, attach the relevant shaders to the game objects.
         * This function will find all MeshRenderers and attach the specified Opaque/Transparent shader
         * where appropriate
         * @param info SuperMeshInfo object that contains the GameObjects to attach shaders to
         */
        private void AttachShader(SuperMeshInfo info)
        {
            if (info.gameObj == null) return;

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
                attachComponentCallback?.Invoke(r.gameObject, width, height);

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
