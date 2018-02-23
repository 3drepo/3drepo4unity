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
        private TreeNode treeRoot = null;
        public readonly GameObject root;
        public readonly string name, teamspace, modelId, revisionId, units;
        public readonly Vector3 offset, surveyPoint;
        public readonly Vector2 latLong;
        public readonly Dictionary<string, string> sharedIdToUniqueId;


        //Angle (in degrees from north, clockwise)
        public readonly float angleFromNorth = 0;
        public readonly bool hasSurveyPoints = false;

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

        public Dictionary<string, object>[] GetMetadataInfo(string nodeID)
        {
            if (meshInfo == null) FetchTree();
            Dictionary<string, object>[] res = null;
            if (meshInfo.ContainsKey(nodeID) && meshInfo[nodeID].meta != null && meshInfo[nodeID].meta.Length > 0)
            {
                res = new Dictionary<string, object>[meshInfo[nodeID].meta.Length];
                for(int i = 0; i < meshInfo[nodeID].meta.Length; ++i)
                {
                    res[i] = repoHttpClient.GetMetadataByID(teamspace, modelId, meshInfo[nodeID].meta[i]).metadata;
                }
            }

            return res;
        }

        public string GetSubMeshID(string supermesh, int index)
        {
            string ret = null;

            if (superMeshes.ContainsKey(supermesh) && superMeshes[supermesh].indexToID.Length > index)
            {
                ret = superMeshes[supermesh].indexToID[index];
            }

            return ret;
        }

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

        private void FetchTree()
        {
            var treeWrapper = repoHttpClient.FetchTree(teamspace, modelId, revisionId);

            treeRoot = treeWrapper.mainTree.nodes;

            meshInfo = new Dictionary<string, TreeNode>();
            PopulateMeshInfo(treeRoot);
        }

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
        internal string[] indexToID; //UV2 number to sub mesh ID
    }
}
