using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float fadeTime;
    private void Awake()
    {
        var text = GetComponent<TextMeshProUGUI>();
        var endColor = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
        text.DOColor(endColor, fadeTime).OnComplete(() =>
        {
            Destroy(gameObject);
        }).SetLink(gameObject);
    }
}
