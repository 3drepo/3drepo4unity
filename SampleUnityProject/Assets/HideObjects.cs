using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideObjects : MonoBehaviour {

    public ExampleScript rootScript = null;
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            HideObject();
        }
	}

    void HideObject()
    {
        string objectModel = "027be63e-8759-43a9-a7e8-a198dea841e0";
        string objectId = "6b6d7837-ef2e-43b8-a608-788581a95531";

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
    
    void HideObjectsByLocation(RepoForUnity.MeshLocation[] locations)
    {
        if(locations.Length > 0)
        {
            foreach (var entry in locations)
            {
                Debug.Log(entry.superMeshID + ", " + entry.index);
                var gameObj = GameObject.Find(entry.superMeshID);
                var shaderController = gameObj.GetComponent<RepoShaderController>();
                if(shaderController != null)
                {
                    shaderController.HideObject(entry.index);
                }
            }
        }
    }
}
