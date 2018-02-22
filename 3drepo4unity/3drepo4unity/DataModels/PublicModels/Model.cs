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
            Debug.Log("Getting metadata info for: " + nodeID);
            Debug.Log("Getting metadata info for: " + meshInfo.ContainsKey(nodeID));
            Debug.Log("Getting metadata info for: " + meshInfo[nodeID].meta);
            Debug.Log("Getting metadata info for: " + meshInfo[nodeID].meta.Length);
            if (meshInfo.ContainsKey(nodeID) && meshInfo[nodeID].meta != null && meshInfo[nodeID].meta.Length > 0)
            {
                Debug.Log("Instantiating res... Length is : " + meshInfo[nodeID].meta.Length);
                res = new Dictionary<string, object>[meshInfo[nodeID].meta.Length];
                for(int i = 0; i < meshInfo[nodeID].meta.Length; ++i)
                {
                    res[i] = repoHttpClient.GetMetadataByID(teamspace, modelId,  meshInfo[nodeID].meta[i]).metadata;
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
            Debug.Log("Populated node: " + node._id + " size of meshInfo: " + meshInfo.Count);
            meshInfo[node._id] = node;
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
