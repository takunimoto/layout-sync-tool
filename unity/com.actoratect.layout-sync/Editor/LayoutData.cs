using System;
using System.Collections.Generic;
using UnityEngine;

namespace Actoratect.LayoutSync
{
    [Serializable]
    public class LayoutJson
    {
        public string version;
        public Metadata metadata;
        public List<LayoutObject> objects;
    }

    [Serializable]
    public class Metadata
    {
        public string layoutName;
        public string exportDate;
        public string sourceApp;
        public CoordinateInfo coordinate;
    }

    [Serializable]
    public class CoordinateInfo
    {
        public string upAxis;
        public string unit;
        public string handedness;
    }

    [Serializable]
    public class LayoutObject
    {
        public string id;
        public string name;
        public string assetName;
        public string sequence;
        public string path;
        public string parentPath;
        public ModelPath modelPath;
        public TransformData transform;
    }

    [Serializable]
    public class ModelPath
    {
        public string maya;
        public string unity;
    }

    [Serializable]
    public class TransformData
    {
        public float[] position;
        public float[] rotation;
        public float[] scale;
        
        public Vector3 GetPosition() => new Vector3(position[0], position[1], position[2]);
        public Vector3 GetRotation() => new Vector3(rotation[0], rotation[1], rotation[2]);
        public Vector3 GetScale() => new Vector3(scale[0], scale[1], scale[2]);
    }
}