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
using RepoForUnity.DataModels.JSONModels;
using System.Linq;
using UnityEngine;

namespace RepoForUnity.Utility
{
    internal class RepoWebClientInterface : HttpClient
    {
        /**
         * A interface between the 3D Repo.io API service and the 3D Repo 4 Unity library. 
         * By default, the URL of the API service is set to Production environment of 3D Repo IO.
         * @params domain URL of the API service.
         */
        internal RepoWebClientInterface(string domain = "https://api1.www.3drepo.io/api") 
            : base(domain)
        {
           
        }


        /**
         * Log into 3D Repo IO. The API service URL will be based on the one given when this
         * object was constructed.
         * @param username user name 
         * @param password password for this user
         * @return upon success returns an Account object with information about this user, null otherwise.
         */
        internal Account Login(string username, string password)
        {
            LoginParameters loginParams = new LoginParameters();
            loginParams.username = username;
            loginParams.password = password;

            var response = HttpPostJson<LoginParameters, LoginResponse>(domain + "login", loginParams);

            Account account = null;

            if (response != null)
                account = new Account(response);
            
            return account;
        }

        internal Account GetCurrentAccount()
        {
            var response = HttpGetJson<LoginResponse>(domain + "me");
            Account account = null;
            if(response != null)
            {
                account = new Account(response);
            }

            return account;
        }


        /**
         * Get Unity asset information about the model specified. This tells you where are
         * the Asset bundles and some information regarding the model.
         * @param teamspace name of the teamspace the model resides
         * @param modelId Id of the model to fetch
         * @param revisionId revision ID for the specific revision to fetch (null for latest revision)
         * @return returns ModelAssetInfo object, containing asset information.
         */
        internal ModelAssetInfo GetUnityAssetInfo(string teamspace, string modelId, string revisionId)
        {
            string uri = null;
            if (revisionId == null || revisionId == "")
                uri = teamspace + "/" + modelId + "/revision/master/head/unityAssets.json";
            else
                uri = teamspace + "/" + modelId + "/revision/" + revisionId + "/unityAssets.json";

            return HttpGetJson<ModelAssetInfo>(domain + uri);
        }

        /**
         *  Get Metadata information by the ID of the metadata object
         * @param teamspace name of the teamspace the model resides
         * @param modelId Id of the model to fetch
         * @param metadataId ID of the metadata object
         * @returns a Metadata object containing all the metadata information
         */
        internal Metadata GetMetadataById(string teamspace, string modelId, string metadataId)
        {
            string uri = teamspace + "/" + modelId + "/meta/" + metadataId + ".json";
            var metadataArr =  HttpGetJson<MetadataWrapper>(domain + uri);
            return metadataArr.meta.Length > 0 ?  metadataArr.meta[0]  : null;

        }

        /**
         * Get all Metadata Object that contains the field specified.
         * @param teamspace name of the teamspace the model resides
         * @param modelId Id of the model to fetch
         * @param revisionId revision ID for the specific revision to fetch (null for latest revision)
         * @param metaField metadataField to search for
         * @return returns an array of results, containing metadata objects that has this field, and also the value of metaField.
         */
        internal MetaSearchResult[] GetAllMetadataWithField(string teamspace, string modelId, string revisionId, string metaField)
        {
            string uri = teamspace + "/" + modelId + "/revision/" + (revisionId == null ? "master/head" : revisionId) + "/meta/findObjsWith/" + metaField + ".json";
            return HttpGetJson<MetaSearchWrapper>(domain + uri).data;
        }

        /**
         * Load an asset bundle from a URL
         * @param assetURI endpoint for the asset bundle url.
         * @return returns a Unity Asset Bundle object if found, null otherwise. 
         */
        internal AssetBundle LoadBundle(string assetURI)
        {
            var response = HttpGetURI(domain + assetURI);
            return AssetBundle.LoadFromStream(response);
        }

        /**
         * Load an bundlle JSON file, containing information about the bundled meshes.
         * @param jsonURI endpoint for the json bundle url.
         * @return returns an Asset Mapping object containing information on how submeshes are mapped into supermeshes.
         */
        internal AssetMapping LoadBundleJSON(string jsonURI)
        {
            return HttpGetJson<AssetMapping>(domain + jsonURI);
        }

        /**
         * Load model settings of the given model. 
         * This contains information such as units, survey points, name of the model
         * @param teamspace name of the teamspace the model resides
         * @param modelId Id of the model to fetch
         * @return returns a ModelSettings object containing the model information
         */
        internal ModelSettings LoadModelSettings(string teamspace, string modelId)
        {
            return HttpGetJson<ModelSettings>(domain + teamspace + "/" + modelId + ".json");
        }

        /**
         * Fetch the tree for the specified model 
         * @param teamspace name of the teamspace the model resides
         * @param modelId Id of the model to fetch
         * @param revisionId revision ID for the specific revision to fetch (null for latest revision)
         * @return returns A tree structure for the given model
         */
        internal TreeWrapper FetchTree(string teamspace, string modelId, string revisionId)
        {
            return HttpGetJson<TreeWrapper>(domain + teamspace + "/" + modelId + "/revision/" +(revisionId == null? "master/head" : revisionId)+ "/fullTree.json");
        }

        /**
         * Check versioning compatibility between this library and the API service
         * @return returns true if they are compatible, false otherwise.
         */
        internal bool VersionCheck()
        {
            var versionInfo =  HttpGetJson<Version>(domain + "version");
            //FIXME: current production version does not support unitydll yet. Remove the first conditino once new version has been released.
            return versionInfo.unitydll == null || ( versionInfo.unitydll.current == Globals.version || versionInfo.unitydll.supported.FirstOrDefault(v => v == Globals.version) != null);
        }
    }
}
