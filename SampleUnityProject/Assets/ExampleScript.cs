/*
 *	Copyright (C) 2018 3D Repo Ltd
 *
 *	This program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU Affero General Public License as
 *	published by the Free Software Foundation, either version 3 of the
 *	License, or (at your option) any later version.
 *
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU Affero General Public License for more details.
 *
 *	You should have received a copy of the GNU Affero General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  Author: Sebastian J Friston
 */

using System.Linq;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    private RepoForUnity.RepoClient client;

    public string apikey;
    public string teamspace;
    public string modelID;

    internal RepoForUnity.Model[] models;

    // Use this for initialization
    private void Start()
    {
        client = new RepoForUnity.RepoClient();
        Login();
    }

    private void Login()
    {
        //Login
        if (client.Connect(apikey))
        {
            Debug.Log("Connected!");
            //NOTE: to find out the which teamspaces/models the user have access to, make an API call to get the user's account information
            //       see https://3drepo.github.io/3drepo.io/#api-Account-listInfo
            LoadModel();
        }
        else
        {
            //You can catch WebException for details.
            Debug.Log("Failed...");
        }
    }

    private void LoadModel()
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
        models = client.LoadModel(teamspace, modelID, Shader.Find("3DRepo/Standard"), Shader.Find("3DRepo/StandardTransparent"), AttachShaderComponent, true);

        //DisplayModelInfo(models);
        //IdentifyAMeshAndFetchMetadata(models);
        //SearchMetadata(models);
    }

    private void DisplayModelInfo(RepoForUnity.Model[] models)
    {
        foreach (var model in models)
        {
            Debug.Log("Model " + model.teamspace + "." + model.modelId + " is called " + model.name + " modelled in " + model.units);
        }
    }

    private void SearchMetadata(RepoForUnity.Model[] models)
    {
        /**
         * The following illustrates how you would perform a metadata search.
         * Here, I search for all metadata which has a property "Floor".
         */
        var model = models.FirstOrDefault(item => item.modelId == "148333e9-e189-473c-9ac6-cc6adc790ab6");
        var results = model.GetAllMetadataWithField("Floor");
        Debug.Log(results.Length + " entries have the property \"Floor\"");
    }

    private void IdentifyAMeshAndFetchMetadata(RepoForUnity.Model[] models)
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
        var meshID = model.GetSubMeshId("153cf665-2c84-4ff9-a9e2-ba495af8e6dc", 0);
        Debug.Log("[" + model.teamspace + "." + model.name + "]The first mesh within 153cf665-2c84-4ff9-a9e2-ba495af8e6dc is " + meshID);

        GetMetadataInfo(model, meshID);
    }

    private void GetMetadataInfo(RepoForUnity.Model model, string meshID)
    {
        /**
         * Once you have identified an object, you can fetch it's metadata properties
         */
        var metadataArr = model.GetMetadataInfo(meshID);
        if (metadataArr != null)
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