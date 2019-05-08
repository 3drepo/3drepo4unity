using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class GetBoundingBox : MonoBehaviour {
    public ExampleScript rootScript = null;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                var meshName = Regex.Replace(hitInfo.collider.name, @"_[\d]+$", ""); ;
                var uvCoord = hitInfo.textureCoord2;
                var ns = hitInfo.collider.transform.root.name;
                var nsArr = ns.Split('.');
                GetBoundingBoxInfo(nsArr[1], meshName, (int) uvCoord.y);
            }
        }
	}

    void GetBoundingBoxInfo(string ns, string superMeshId, int ind)
    {
        var model = rootScript.models.FirstOrDefault(item => item.modelId == ns);

        if (model != null)
        {
            var subMeshId = model.GetSubMeshId(superMeshId, ind);
            var bounds = model.GetMeshBoundingBox(subMeshId);
            Debug.Log("Model: " + ns + " Mesh ID: " + subMeshId + " , bounds: " + bounds);
        }
        else
        {
            Debug.LogError("[GetBoundingBox.cs] Cannot find model " + ns);
        }
    }
}
