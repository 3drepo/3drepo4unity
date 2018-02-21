using UnityEngine;
namespace RepoForUnity
{
    public class Model
    {
        private GameObject[] gos;
        private string name;

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

        public Model(string name, GameObject[] gos)
        {
            this.name = name;
            this.gos = gos;
        }
    }

    internal class SuperMeshInfo
    {
        internal int nSubMeshes;

    }
}
