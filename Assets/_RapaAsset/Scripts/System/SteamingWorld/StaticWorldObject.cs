using RTree;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticWorldObject : MonoBehaviour
{
    private Guid Guid = Guid.NewGuid();
    public Rect[] Rects => GetRects();

    private Rect[] GetRects()
    {
        List<Rect> Rects = new List<Rect>();
        var colliders = GetComponentsInChildren<BoxCollider>();
        if (colliders != null)
        {
            foreach (var box in colliders)
            {
                Rects.Add(new Rect(box.center.x - box.size.x / 2, box.center.z - box.size.z / 2, box.size.x, box.size.z));
            }
        }
        else
            Rects.Add(new Rect(transform.position.x, transform.position.z, 0, 0));
        return Rects.ToArray();
    }

    public RTreeData ToRTreeData()
    {
        return new RTreeData(new Envelope(Rects[0].xMin, Rects[0].yMin, Rects[0].xMax, Rects[0].yMax), Guid);
    }
}