using System.Collections;
using System.Collections.Generic;
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
        var models = client.LoadModel(teamspace, modelID, Shader.Find("3DRepo/Standard"), Shader.Find("3DRepo/StandardTransparent"), AttachShaderComponent);
    }

    public static void AttachShaderComponent(GameObject obj, int height, int width)
    {
        obj.AddComponent<RepoShaderController>().SetTextureDimension(height, width);
    }
}
