using System.Collections.Generic;
using UnityEngine;
namespace RepoForUnity
{
    public class Model
    {
        private Dictionary<string, SuperMeshInfo> superMeshes;
        private GameObject root;
        private string name;
        private Vector3 offset;

        public string ModelName
        {
            get
            {
                return name;
            }
        }

        public GameObject RootObject
        {
            get
            {
                return root;
            }
        }        

        internal Model(string name, Dictionary<string, SuperMeshInfo> superMeshes, Vector3 offset)
        {
            this.name = name;
            this.superMeshes = superMeshes;
            this.offset = offset;
            root = new GameObject(name);
            foreach(var smesh in superMeshes)
            {
                smesh.Value.gameObj.transform.parent = root.transform;
            }
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
