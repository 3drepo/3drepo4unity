using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutMesh : MonoBehaviour
{
    public ExampleScript rootScript = null;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            CutOutMesh();
        }
    }

    private void CutOutMesh()
    {
        var objectModel = rootScript.subModelID;
        var objectId = "19050393-9e9c-4a40-b4d3-62cefa4bbaf2";

        if (!rootScript)
        {
            Debug.LogError("[CutMesh.cs] Cannot find root script - check it is attached!");
        }

        var model = rootScript.models.FirstOrDefault(item => item.modelId == objectModel);

        if (model != null)
        {
            var mesh = GetMesh(model.GetMeshLocation(objectId));
            DisplayMesh(objectModel + "." + objectId, mesh);
        }
        else
        {
            Debug.LogError("[CutMesh.cs] Cannot find model " + objectModel);
        }
    }

    private void DisplayMesh(string name, Mesh mesh)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = transform;
        obj.transform.localScale = new Vector3(1, 1, -1); //Unity's Z goes in different direction to 3D Repo geometry
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

        //hide all other models
        foreach (var model in rootScript.models)
        {
            model.root.SetActive(false);
        }
    }

    private Mesh GetMesh(RepoForUnity.MeshLocation[] meshLocations)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normal = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (var entry in meshLocations)
        {
            var go = GameObject.Find(entry.superMeshID);
            var meshFilter = go.GetComponent<MeshFilter>();
            var superMesh = meshFilter.mesh;
            Dictionary<int, int> vIndexChange = new Dictionary<int, int>();
            for (int i = 0; i < superMesh.uv2.Length; ++i)
            {
                if (superMesh.uv2[i].y == entry.index)
                {
                    vIndexChange[i] = vertices.Count;
                    vertices.Add(superMesh.vertices[i]);
                    normal.Add(superMesh.normals[i]);
                }
            }

            for (int i = 0; i < superMesh.triangles.Length; ++i)
            {
                var index = superMesh.triangles[i];
                if (vIndexChange.ContainsKey(index))
                {
                    triangles.Add(vIndexChange[index]);
                }
            }
        }

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normal.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }
}