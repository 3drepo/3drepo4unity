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
}