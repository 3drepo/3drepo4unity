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

using System;

namespace RepoForUnity.DataModels.JSONModels
{
    /// <summary>
    /// Defines the parameters required to form a login request
    /// </summary>
    [Serializable]
    internal class LoginParameters
    {
        public string username = null;
        public string password = null;
    }

    /// <summary>
    /// Managed equivalent of the JSON object returned in response to a login request
    /// </summary>
    [Serializable]
    internal class LoginResponse
    {
        public string username = null;
    }

    /// <summary>
    /// User role returned by login
    /// </summary>
    [Serializable]
    internal class UserRole
    {
        public string db = null;
        public string role = null;
    }

    [Serializable]
    internal class Version
    {
        public string VERSION = null;
        public PluginVersionInfo unitydll = null;
    }

    [Serializable]
    internal class PluginVersionInfo
    {
        public string current = null;
        public string[] supported = null;
    }
}