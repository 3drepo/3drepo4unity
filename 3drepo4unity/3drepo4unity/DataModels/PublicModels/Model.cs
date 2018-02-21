using UnityEngine;
namespace RepoForUnity
{
    public class Model
    {
        private GameObject[] gos;
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

        public GameObject[] ModelObjects
        {
            get
            {
                return gos;
            }
        }        

        public Model(string name, GameObject[] gos, Vector3 offset)
        {
            this.name = name;
            this.gos = gos;
            this.offset = offset;
            root = new GameObject(name);
            foreach(var go in gos)
            {
                go.transform.parent = root.transform;
            }
        }
    }

    internal class SuperMeshInfo
    {
        internal int nSubMeshes;

    }
}
