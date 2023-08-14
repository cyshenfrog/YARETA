using System.Collections.Generic;
using UnityEngine;

public class StreamingObject : MonoBehaviour
{
    public Rect[] Rects => GetRects();

    private Rect[] GetRects()
    {
        List<Rect> Rects = new List<Rect>();
        var colliders = GetComponentsInChildren<BoxCollider>();
        if (colliders.Length != 0)
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

    public RTreeData GetRTreeData(string prefabPath)
    {
        return new RTreeData(gameObject.GetInstanceID(), transform.position, transform.rotation, prefabPath);
        //return new RTreeData(new Envelope(Rects[0].xMin, Rects[0].yMin, Rects[0].xMax, Rects[0].yMax), prefabPath, transform.localRotation);
    }
}