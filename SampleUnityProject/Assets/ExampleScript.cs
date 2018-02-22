using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExampleScript : MonoBehaviour {
    private RepoForUnity.RepoClient client;

    public string username;
    public string password;
    public string teamspace;
    public string modelID;

    // Use this for initialization
    void Start () {
        client = new RepoForUnity.RepoClient();
        Login();
	}
	
    void Login()
    {
        //Login
        if (client.Connect(username, password))
        {
            var tsList = "Connected! Teamspaces this user has access to: ";
            foreach(var ts in client.GetTeamspaces()){
                tsList += ts + ", ";
            }

            Debug.Log(tsList);
            LoadModel();
        }
        else
        {
            //You can catch WebException for details.
            Debug.Log("Failed...");
        }
    }

    void LoadModel()
    {
        /**
         * This method illustrates how to load a model from 3D Repo.
         * Note that the function returns an array of models.
         * Federated models will return each of the sub models as a model,
         * whereas a normal model will always return an array with 1 member.
         * 
         * Note that 3D Repo uses mesh batching, where multiple meshes are batched together
         * into a single unity mesh. This results in requiring some special logic within the shader
         * to render the material properties correctly.
         * 
         * Sample shaders and shader controller are provided within this project to demonstrate how 
         * to create a basic shader that works with 3D Repo meshes.
         */
        var models = client.LoadModel(teamspace, modelID, Shader.Find("3DRepo/Standard"), Shader.Find("3DRepo/StandardTransparent"), AttachShaderComponent);


        DisplayModelInfo(models);
        IdentifyAMesh(models);
        
        


    }

    void DisplayModelInfo(RepoForUnity.Model[] models)
    {
        foreach(var model in models)
        {
            Debug.Log("Model " + model.teamspace + "." + model.modelId + " is called " + model.name + " modelled in " + model.units);
        }
    }

    void IdentifyAMesh(RepoForUnity.Model[] models)
    {
        /**
         * The following illustrate how you can identify a sub mesh within a supermesh after
         * you have found it's supermesh ID and index.
         * 
         * As meshes are batched into supermeshes, identifying a single mesh can be tricky.
         * All meshes are encoded with an index within the UV2 element. 
         * 
         * In order to find a submesh from view (e.g. if you're trying to implement object selection), first 
         * do a Raycast to the point to identify index from the UV2.y value.
         * 
         * Then identify the supermesh it belongs to. This is the name of the parent GameObject.
         */
         
        var model = models.FirstOrDefault(item => item.modelId == "148333e9-e189-473c-9ac6-cc6adc790ab6");
        var meshID = model.GetSubMeshID("153cf665-2c84-4ff9-a9e2-ba495af8e6dc", 0);
        Debug.Log("["+model.teamspace + "." + model.name+"]The first mesh within 153cf665-2c84-4ff9-a9e2-ba495af8e6dc is " + meshID);

        GetMetadataInfo(model, meshID);
    }

    void GetMetadataInfo(RepoForUnity.Model model, string meshID)
    {
        /**
         * Once you have identified an object, you can fetch it's metadata properties 
         */
        var metadataArr = model.GetMetadataInfo(meshID);
        if(metadataArr != null)
        {
            Debug.Log(metadataArr.Length + " pieces of metadata found.");
            foreach (var meta in metadataArr[0])
            {
                Debug.Log(meta);
            }
        }
        
    }

    public static void AttachShaderComponent(GameObject obj, int height, int width)
    {
        obj.AddComponent<RepoShaderController>().SetTextureDimension(height, width);
    }
}
