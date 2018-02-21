using RepoForUnity.DataModels.JSONModels;
using UnityEngine;

namespace RepoForUnity.DataModels
{
    internal class Account
    {
        internal readonly string username;
        private string[] teamspaces;

        public string[] Teamspaces
        {
            get
            {
                return teamspaces;
            }
        }

        internal Account(LoginResponse response)
        {
            username = response.username;
            teamspaces = new string[response.roles.Length];
            for(int i = 0; i < response.roles.Length; ++i)
            {
                teamspaces[i] = response.roles[i].db;
            }
        }
    }
}
