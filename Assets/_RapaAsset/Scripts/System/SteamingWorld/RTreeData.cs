using RTree;
using System;
using UnityEngine;

[Serializable]
public struct RTreeData : ISpatialData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public string PrefabPath;
    public int Key;

    public RTreeData(int key, Vector3 position, Quaternion rotation, string prefabPath)
    {
        Key = key;
        Position = position;
        Rotation = rotation;
        PrefabPath = prefabPath;
    }

    public Envelope Envelope => new Envelope(Position.x, Position.z);
}