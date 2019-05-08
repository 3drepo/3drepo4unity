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
 *
 */

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
            if(!client.VersionCheck())
            {
                throw new RepoUnsupportedException();
            }
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
         * Load a Model. This may return multiple models if the specified model is a federation
         * @params teamspace the teamspace this model resides in
         * @params modelId ID of the model
         * @param revisionId revision ID to load (null for latest)
         * @param opaqueShader a Unity Opaque shader to be attached to the opaque mesh objects
         * @param transparentShader a Unity Opaque shader to be attached to the translucent mesh objects
         * @params callback a callback function to attach a controller to control the shader (null if not needed)
         * @return returns the an array of models, containing the model information
         */
        public Model[] LoadModel(string teamspace, string model, string revisionId,
            Shader opaqueShader, Shader transparentShader, AddShaderControllerCallback callback, bool addPhysCollider = false)
        {
            return modelManager.LoadModel(teamspace, model, revisionId, opaqueShader, transparentShader, callback, addPhysCollider);
        }

        /**
         * Load a 3D Model into Unity.
         * @params teamspace teamspace where the model resides
         * @param model where the model resides
         * @return returns the object, Model, containing the model information
         */
        public Model[] LoadModel(string teamspace, string model, Shader opaqueShader, Shader transparentShader, AddShaderControllerCallback callback, bool addPhysCollider = false)
        {
            return LoadModel(teamspace, model, null, opaqueShader, transparentShader, callback);
        }
    }
}
