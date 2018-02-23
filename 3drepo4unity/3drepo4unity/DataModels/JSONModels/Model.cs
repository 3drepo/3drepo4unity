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

using System;
using System.Collections.Generic;

namespace RepoForUnity.DataModels.JSONModels
{
    [Serializable]
    internal class ModelAssetInfo
    {
        public AssetInfo[] models;
    }

    [Serializable]
    internal class AssetInfo
    {
        public string[] assets;
        public string[] vrAssets;
        public string[] jsonFiles;
        public double[] offset;
        public string model;
        public string database;
    }

    [Serializable]
    internal class Mapping
    {
        public string name;
        public string sharedID;
        public float[] min;
        public float[] max;
        public string[] usage;
    }

    [Serializable]
    internal class AssetMapping
    {
        public int numberOfIDs;
        public int maxGeoCount;
        public Mapping[] mapping;
    }

    [Serializable]
    internal class ModelSettings
    {
        public string name;
        public ModelSettingsProperties properties;
        public SurveyPoint[] surveyPoints;
        public float angleFromNorth;
    }

    [Serializable]
    internal class SurveyPoint
    {
        public float[] position;
        public float[] latLong;
    }

    [Serializable]
    internal class ModelSettingsProperties
    {
        public string unit;
    }


    [Serializable]
    internal class TreeWrapper
    {
        public FullTree mainTree;
    }

    [Serializable]
    internal class FullTree
    {
        public TreeNode nodes;
    }

    [Serializable]
    public class TreeNode
    {
        public string name;
        public string path;
        public string _id;
        public string shared_id;
        public string type;
        public TreeNode[] children;
        public string[] meta;
    }

    [Serializable]
    internal class MetadataWrapper
    {
        public Metadata[] meta;
    }

    [Serializable]
    public class Metadata
    {
        public Dictionary<String, object> metadata;
    }

    [Serializable]
    internal class MetaSearchWrapper
    {
        public MetaSearchResult[] data;
    }

    [Serializable]
    public class MetaSearchResult
    {
        public string _id;
        public SearchValue metadata;
        public string[] parents;
    }
    
    [Serializable]
    public class SearchValue
    {
        public object value;
    }
}