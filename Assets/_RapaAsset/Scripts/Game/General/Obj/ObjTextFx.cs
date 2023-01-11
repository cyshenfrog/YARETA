using System;
using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class ObjTextFx : MonoBehaviour
{
    public Text Text;
    private Vector3 initScale;

    private void Awake()
    {
        initScale = transform.localScale;
    }

    private void LateUpdate()
    {
        transform.rotation = GameRef.MainCam.transform.rotation;
    }

    public void Init(string text, int level, Vector3 pos)
    {
        StartCoroutine(_Init(text, level, pos));
    }

    private IEnumerator _Init(string text, int level, Vector3 pos)
    {
        Text.text = "";
        Text.color = Color.clear;
        transform.localScale = initScale;
        transform.position = pos;
        switch (level)
        {
            case 0:
            default:
                Text.transform.localScale *= UnityEngine.Random.Range(0.4f, 0.7f);
                Text.color = Color.white;
                break;

            case 1:
                Text.transform.localScale *= 0.7f;
                Text.color = Color.blue;
                break;

            case 2:
                Text.transform.localScale *= 1f;
                Text.color = Color.magenta;
                break;

            case 3:
                Text.transform.localScale *= 1.2f;
                Text.color = Color.yellow;
                break;
        }

        yield return 0;
        Text.text = text;
        Text.DOFade(.5f, 1)
            .SetDelay(0.5f)
            .OnComplete(() => { Despawn(); });
        Text.transform.DOLocalMoveY(0.1f, 1)
            .SetRelative(true);
    }

    private void Despawn()
    {
        transform.DOScale(.05f, 3f)
            .SetEase(Ease.InQuad);
        Text.DOFade(0, 3f)
            .OnComplete(() =>
            {
                LeanPool.Despawn(transform);
                Text.rectTransform.anchoredPosition = Vector2.zero;
            });
    }
}