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
using System.Collections.Generic;

namespace RepoForUnity.DataModels.JSONModels
{
    [Serializable]
    internal class ModelAssetInfo
    {
        public AssetInfo[] models = null;
    }

    [Serializable]
    internal class AssetInfo
    {
        public string[] assets = null;
        public string[] vrAssets = null;
        public string[] jsonFiles = null;
        public double[] offset = null;
        public string model = null;
        public string database = null;
    }

    [Serializable]
    internal class Mapping
    {
        public string name = null;
        public string sharedId = null;
        public float[] min = null;
        public float[] max = null;
        public string[] usage = null;
    }

    [Serializable]
    internal class AssetMapping
    {
        public int numberOfIds = 0;
        public int maxGeoCount = 0;
        public Mapping[] mapping = null;
    }

    [Serializable]
    internal class ModelSettings
    {
        public string name = null;
        public ModelSettingsProperties properties = null;
        public SurveyPoint[] surveyPoints = null;
        public float angleFromNorth = 0;
    }

    [Serializable]
    internal class SurveyPoint
    {
        public float[] position = null;
        public float[] latLong = null;
    }

    [Serializable]
    internal class ModelSettingsProperties
    {
        public string unit = null;
    }

    [Serializable]
    internal class TreeWrapper
    {
        public FullTree mainTree = null;
    }

    [Serializable]
    internal class FullTree
    {
        public TreeNode nodes = null;
    }

    [Serializable]
    public class TreeNode
    {
        public string name = null;
        public string path = null;
        public string _id = null;
        public string shared_id = null;
        public string type = null;
        public TreeNode[] children = null;
        public string[] meta = null;
    }

    [Serializable]
    internal class MetadataWrapper
    {
        public Metadata[] meta = null;
    }

    [Serializable]
    public class Metadata
    {
        public Dictionary<String, object> metadata = null;
    }

    [Serializable]
    internal class MetaSearchWrapper
    {
        public MetaSearchResult[] data = null;
    }

    [Serializable]
    public class MetaSearchResult
    {
        public string _id = null;
        public SearchValue metadata = null;
        public string[] parents = null;
    }

    [Serializable]
    public class SearchValue
    {
        public object value = null;
    }
}