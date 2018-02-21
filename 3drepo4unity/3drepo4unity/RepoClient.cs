using RepoForUnity.DataModels;
using RepoForUnity.Utility;
using UnityEngine;

namespace RepoForUnity
{
    public delegate void AddShaderControllerCallback(GameObject obj, int height, int width);
    public class RepoClient
    {
        private RepoWebClientInterface client;
        private ModelManager modelManager;
        private Account account = null;

        /**
         * RepoClient constructor.
         * This instantiates the interfacing layer between the Unity Game and 3D Repo Web Services 
         */
        public RepoClient()
        {
            client = new RepoWebClientInterface();
            modelManager = new ModelManager(client);
        }

        /**
         * Connect to 3D Repo Web Services
         * This will connect to API services for https://www.3drepo.io
         * The user will have to login before any API calls can be made.
         * @params username username to login with
         * @params password password for username.
         * @return returns true upon success
         */
        public bool Connect(string username, string password)
        {
            account = client.Login(username, password);

            return account != null;
        }

        /**
         * Get the list of teamspace the user has access to 
         * @return returns an array of teamspace names
         */
        public string[] GetTeamspaces()
        {
            return account.Teamspaces;
        }

        /**
         * Load a 3D Model into Unity.
         * @params teamspace teamspace where the model resides
         * @param model where the model resides
         * @return returns the object, Model, containing the model information
         */
        public Model[] LoadModel(string teamspace, string model, string revisionId,
            Shader opaqueShader, Shader transparentShader, AddShaderControllerCallback callback)
        {
            return modelManager.LoadModel(teamspace, model, revisionId, opaqueShader, transparentShader, callback);
        }

        /**
         * Load a 3D Model into Unity.
         * @params teamspace teamspace where the model resides
         * @param model where the model resides
         * @return returns the object, Model, containing the model information
         */
        public Model[] LoadModel(string teamspace, string model, Shader opaqueShader, Shader transparentShader, AddShaderControllerCallback callback)
        {
            return LoadModel(teamspace, model, null, opaqueShader, transparentShader, callback);
        }
    }
}
