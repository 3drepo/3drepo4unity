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

using RepoForUnity.DataModels;
using RepoForUnity.DataModels.JSONModels;
using System.Linq;
using UnityEngine;

namespace RepoForUnity.Utility
{
    internal class RepoWebClientInterface : HttpClient
    {
        internal RepoWebClientInterface(string domain = "https://api1.www.3drepo.io/api") 
            : base(domain)
        {
           
        }

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

        internal ModelAssetInfo GetUnityAssetInfo(string teamspace, string modelID, string revisionId)
        {
            string uri = null;
            if (revisionId == null || revisionId == "")
                uri = teamspace + "/" + modelID + "/revision/master/head/unityAssets.json";
            else
                uri = teamspace + "/" + modelID + "/revision/" + revisionId + "/unityAssets.json";

            return HttpGetJson<ModelAssetInfo>(domain + uri);
        }

        internal Metadata GetMetadataByID(string teamspace, string modelId, string metadataId)
        {
            string uri = teamspace + "/" + modelId + "/meta/" + metadataId + ".json";
            var metadataArr =  HttpGetJson<MetadataWrapper>(domain + uri);
            return metadataArr.meta.Length > 0 ?  metadataArr.meta[0]  : null;

        }

        internal MetaSearchResult[] GetAllMetadataWithField(string teamspace, string modelId, string revisionId, string metaField)
        {
            string uri = teamspace + "/" + modelId + "/revision/" + (revisionId == null ? "master/head" : revisionId) + "/meta/findObjsWith/" + metaField + ".json";
            return HttpGetJson<MetaSearchWrapper>(domain + uri).data;
        }

        internal AssetBundle LoadBundle(string assetURI)
        {
            var response = HttpGetURI(domain + assetURI);
            return AssetBundle.LoadFromStream(response);
        }

        internal AssetMapping LoadBundleJSON(string jsonURI)
        {
            return HttpGetJson<AssetMapping>(domain + jsonURI);
        }

        internal ModelSettings LoadModelSettings(string teamspace, string model)
        {
            return HttpGetJson<ModelSettings>(domain + teamspace + "/" + model + ".json");
        }

        internal TreeWrapper FetchTree(string teamspace, string modelId, string revisionId)
        {
            return HttpGetJson<TreeWrapper>(domain + teamspace + "/" + modelId + "/revision/" +(revisionId == null? "master/head" : revisionId)+ "/fullTree.json");
        }

        internal bool VersionCheck()
        {
            var versionInfo =  HttpGetJson<Version>(domain + "version");
            //FIXME: current production version does not support unitydll yet. Remove the first conditino once new version has been released.
            return versionInfo.unitydll == null || ( versionInfo.unitydll.current == Globals.version || versionInfo.unitydll.supported.FirstOrDefault(v => v == Globals.version) != null);
        }
    }
}
