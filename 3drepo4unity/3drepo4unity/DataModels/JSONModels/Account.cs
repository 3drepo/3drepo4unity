using System;

namespace RepoForUnity.DataModels.JSONModels
{

    /// <summary>
    /// Defines the parameters required to form a login request
    /// </summary>
    [Serializable]
    internal class LoginParameters
    {
        public string username;
        public string password;
    }

    /// <summary>
    /// Managed equivalent of the JSON object returned in response to a login request
    /// </summary>
    [Serializable]
    internal class LoginResponse
    {
        public string username;
        public UserRole[] roles;
    }

    /// <summary>
    /// User role returned by login
    /// </summary>
    [Serializable]
    internal class UserRole
    {
        public string db;
        public string role;
    }
}
