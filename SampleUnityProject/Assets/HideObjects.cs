using System.Linq;
using UnityEngine;

/**
 * This file demostrate how a developer can code something to hide a mesh.
 * To run this, enable the game object in your scene and hit '1' to trigger the hide object. This should hide the tree from view.
 * Hidden functionality is implemented within the shader. This example only demostate how you can hide a single object within a supermesh
 * This can then be adapted to hide multiple meshes within a supermesh by implementing the shader to support multiple indices (potentially by means of introducing a new texture)
 *
 * This can also be adapted to do highlighting by o.Albedo to base on the index.
 */

public class HideObjects : MonoBehaviour
{
    public ExampleScript rootScript = null;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            HideObject();
        }
    }

    private void HideObject()
    {
        var objectModel = rootScript.subModelID;
        var objectId = "19050393-9e9c-4a40-b4d3-62cefa4bbaf2";

        if (!rootScript)
        {
            Debug.LogError("[HideObjects.cs] Cannot find root script - check it is attached!");
        }

        var model = rootScript.models.FirstOrDefault(item => item.modelId == objectModel);

        if (model != null)
        {
            HideObjectsByLocation(model.GetMeshLocation(objectId));
        }
    }

    private void HideObjectsByLocation(RepoForUnity.MeshLocation[] locations)
    {
        if (locations.Length > 0)
        {
            foreach (var entry in locations)
            {
                Debug.Log(entry.superMeshID + ", " + entry.index);
                var gameObj = GameObject.Find(entry.superMeshID);
                var shaderController = gameObj.GetComponent<RepoShaderController>();
                if (shaderController != null)
                {
                    shaderController.HideObject(entry.index);
                }
            }
        }
    }
}