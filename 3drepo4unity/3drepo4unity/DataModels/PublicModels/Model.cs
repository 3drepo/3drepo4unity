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

using RepoForUnity.DataModels.JSONModels;
using RepoForUnity.Utility;
using System.Collections.Generic;
using UnityEngine;
namespace RepoForUnity
{
    public class Model
    {
        private Dictionary<string, SuperMeshInfo> superMeshes;
        private Dictionary<string, TreeNode> meshInfo;
        private RepoWebClientInterface repoHttpClient;
        private Dictionary<string, string> sharedIdToUniqueId;
        private TreeNode treeRoot = null;
        public readonly GameObject root;
        public readonly string name, teamspace, modelId, revisionId, units;
        public readonly Vector3 offset, surveyPoint;
        public readonly Vector2 latLong;

        //Angle (in degrees from north, clockwise)
        public readonly float angleFromNorth = 0;
        public readonly bool hasSurveyPoints = false;

        /**
         *  Obtain the Tree structure of the model
         */
        public TreeNode Tree
        {
            get
            {
                if(treeRoot == null)
                {
                    FetchTree();
                }

                return treeRoot;
            }
        }

        /**
         * Get an object's unqiue ID based on a shared id
         * @params sharedId a shared ID
         * @return returns the corresponding unique id if none is found, return null; 
         */
        public string SharedIdtoUniqueId(string sharedId)
        {
            if (sharedIdToUniqueId == null) FetchTree();
            
            return sharedIdToUniqueId.ContainsKey(sharedId) ? sharedIdToUniqueId[sharedId] : null;
        }

        /**
         * Get Metadata object based on it's unique ID. 
         * @params nodeId The Id of the metadata object to fetch
         * @return returns Key value pairs containing metadata information.
         */
        public Dictionary<string, object>[] GetMetadataInfo(string nodeId)
        {
            if (meshInfo == null) FetchTree();
            Dictionary<string, object>[] res = null;
            if (meshInfo.ContainsKey(nodeId) && meshInfo[nodeId].meta != null && meshInfo[nodeId].meta.Length > 0)
            {
                res = new Dictionary<string, object>[meshInfo[nodeId].meta.Length];
                for(int i = 0; i < meshInfo[nodeId].meta.Length; ++i)
                {
                    res[i] = repoHttpClient.GetMetadataById(teamspace, modelId, meshInfo[nodeId].meta[i]).metadata;
                }
            }

            return res;
        }

        /**
         * Given the supermesh name and the index, returns the unique ID of the submesh
         * @params supermesh name of the supermesh (an UUID)
         * @params index index of the mesh (typically retrieved from UV2.y upon a raycast)
         * @return returns the unique ID of the submesh found, null otherwise.
         */
        public string GetSubMeshId(string supermesh, int index)
        {
            string ret = null;

            if (superMeshes.ContainsKey(supermesh) && superMeshes[supermesh].indexToId.Length > index)
            {
                ret = superMeshes[supermesh].indexToId[index];
            }

            return ret;
        }

        /**
         * Perform a metadata search, returning all metadata objects with the specified field name
         * @param field name of the field to search for
         * @return returns an array of metadata objects that contains this field, with the value of the field. 
         */
        public MetaSearchResult[] GetAllMetadataWithField(string field)
        {
            return repoHttpClient.GetAllMetadataWithField(teamspace, modelId, revisionId, field);
        }


        internal Model(
            string teamspace, 
            string modelId, 
            string revisionId, 
            ModelSettings settings, 
            Dictionary<string, SuperMeshInfo> superMeshes, 
            Vector3 offset,
            RepoWebClientInterface repoHttpClient)
        {
            this.teamspace = teamspace;
            this.modelId = modelId;
            this.revisionId = revisionId;
            this.superMeshes = superMeshes;
            this.offset = offset;
            this.repoHttpClient = repoHttpClient;

            root = new GameObject(teamspace + "." + modelId);
            foreach(var smesh in superMeshes)
            {
                smesh.Value.gameObj.transform.parent = root.transform;
            }

            units = settings.properties.unit;
            angleFromNorth = settings.angleFromNorth;


            if(hasSurveyPoints = (settings.surveyPoints != null && settings.surveyPoints.Length > 0))
            {
                surveyPoint = new Vector3(settings.surveyPoints[0].position[0], settings.surveyPoints[0].position[1], settings.surveyPoints[0].position[2]);
                latLong = new Vector2(settings.surveyPoints[0].latLong[0], settings.surveyPoints[0].latLong[1]);
            }

            name = settings.name;
                
        }

        /*
         * Fetch the tree information of this model. 
         */
        private void FetchTree()
        {
            var treeWrapper = repoHttpClient.FetchTree(teamspace, modelId, revisionId);

            treeRoot = treeWrapper.mainTree.nodes;

            meshInfo = new Dictionary<string, TreeNode>();
            sharedIdToUniqueId = new Dictionary<string, string>();
            PopulateMeshInfo(treeRoot);
        }

        /**
         * Populate all mesh entry caches with the tree node information.
         * This is called recursively by FetchTree() 
         */
        private void PopulateMeshInfo(TreeNode node)
        {
            meshInfo[node._id] = node;
            sharedIdToUniqueId[node.shared_id] = node._id;
            if(node.children != null)
            {
                foreach (var child in node.children)
                {
                    PopulateMeshInfo(child);
                }
            }
            
        }
    }

    internal class SuperMeshInfo
    {
        internal string name;
        internal int nSubMeshes;
        internal GameObject gameObj;
        internal string[] indexToId; //UV2 number to sub mesh Id
    }
}
