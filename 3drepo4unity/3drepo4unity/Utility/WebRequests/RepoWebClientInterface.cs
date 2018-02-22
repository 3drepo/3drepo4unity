using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepoForUnity.DataModels;
using RepoForUnity.DataModels.JSONModels;
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
    }
}
