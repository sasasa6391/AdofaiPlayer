using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Event_Bloom : FloorEvent
{
    public string enabled;
    public float duration;
    public float threadHold;
    public float intensity;
    public Color color;

    public Ease easeType;
    public Material material;

    public Event_Bloom(string enabled, float duration, float threadHold, float intensity, Color color, Ease easeType)
    {
        this.enabled = enabled;
        this.duration = duration;
        this.threadHold = threadHold;
        this.intensity = intensity;
        this.color = color;
        this.easeType = easeType;
        material = FollowCamera.Instance.bloom.bloomMaterial;
    }

    public override void EventStart()
    {

        if (enabled == "True" || enabled == "Enabled")
        {
            FollowCamera.Instance.bloom.enabled = true;

            FollowCamera.Instance.bloomColorTween?.Kill();

            var realDuration = Mathf.Max(duration * 60.0f / bpm, 0.4f);

            FollowCamera.Instance.bloom.SetBloomParam(threadHold, intensity);

            FollowCamera.Instance.bloomColorTween = material.DOColor(color, realDuration).SetEase(easeType).OnComplete(() =>
            {
                material.color = color;
            }).SetLink(FollowCamera.Instance.gameObject);
        }
        else
        {
            FollowCamera.Instance.bloom.enabled = false;
        }
    }
}
