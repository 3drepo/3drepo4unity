﻿using System;
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
    public class MetadataWrapper
    {
        public Metadata[] meta;
    }

    [Serializable]
    public class Metadata
    {
        public Dictionary<String, object> metadata;
    }
}