using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
public class Tool_MaterialFade : MonoBehaviour
{
    public bool FadeAllMaterial = true;

    [HideIf("FadeAllMaterial")]
    public int[] TargetMaterialID;

    public float Duration = 1;
    public float FadeTo;
    public Ease EaseType = Ease.OutQuad;

    public void Fade()
    {
        Renderer r = GetComponent<Renderer>();
        if (FadeAllMaterial)
        {
            foreach (var m in r.materials)
            {
                m.DOFade(FadeTo, Duration)
                    .SetEase(EaseType);
            }
        }
        else
        {
            foreach (var id in TargetMaterialID)
            {
                r.materials[id].DOFade(FadeTo, Duration)
                    .SetEase(EaseType);
            }
        }
    }
}