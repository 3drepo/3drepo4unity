using RepoForUnity.DataModels.JSONModels;
using RepoForUnity.Utility;
using System.Collections.Generic;
using UnityEngine;
namespace RepoForUnity
{
    public class Model
    {
        private Dictionary<string, SuperMeshInfo> superMeshes;
        private RepoWebClientInterface repoHttpClient;
        public readonly GameObject root;
        public readonly string name, teamspace, modelId, revisionId, units;
        public readonly Vector3 offset, surveyPoint;
        public readonly Vector2 latLong;

        //Angle (in degrees from north, clockwise)
        public readonly float angleFromNorth = 0;
        public readonly bool hasSurveyPoints = false;


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

        public string GetSubMeshID(string supermesh, int index)
        {
            string ret = null;

            if(superMeshes.ContainsKey(supermesh) && superMeshes[supermesh].indexToID.Length > index)
            {
                ret = superMeshes[supermesh].indexToID[index];
            }

            return ret;
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
