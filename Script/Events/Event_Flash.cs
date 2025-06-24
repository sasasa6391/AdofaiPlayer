using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//{ "floor": 348, "eventType": "Flash", "duration": 4, "plane": "Background", "startColor": "000000", "startOpacity": 20, "endColor": "000000", "endOpacity": 40, "angleOffset": 0, "eventTag": "", },
public class Event_Flash : FloorEvent
{
    public string plane;
    public float duration;
    public Color startColor;
    public Color endColor;

    public Ease easeType;
    public MeshRenderer bg;
    public MeshRenderer fg;

    public Event_Flash(string plane, Ease easeType, float duration, Color startColor, Color endColor)
    {
        this.plane = plane;
        this.easeType = easeType;
        this.duration = duration;
        this.startColor = startColor;
        this.endColor = endColor;
        bg = FollowCamera.Instance.bg;
        fg = FollowCamera.Instance.fg;
    }

    public override void EventStart()
    {
        if (LevelManager.IsLegacyFlash == true)
        {
            PlayFlash_Legacy();
        }

        else
        {
            PlayFlash();
        }
    }

    public void PlayFlash()
    {

        Material mat;

        if (plane == "Background")
        {
            FollowCamera.Instance.bgColorTween?.Kill();
            fg.material.color = new Color(1, 1, 1, 0);
            mat = bg.material;
            var realDuration = Mathf.Max(duration * 60.0f / bpm, 0.4f);

            mat.color = startColor;
            FollowCamera.Instance.bgColorTween = mat.DOColor(endColor, realDuration).SetEase(easeType).OnComplete(() =>
            {
                mat.color = endColor;
            }).SetLink(FollowCamera.Instance.gameObject);
        }
        else if (plane == "Foreground")
        {
            FollowCamera.Instance.fgColorTween?.Kill();
            bg.material.color = new Color(0, 0, 0, 0);

            mat = fg.material;

            endColor = new Color(Mathf.Min(1.0f, endColor.r), Mathf.Min(1.0f, endColor.g), Mathf.Min(1.0f, endColor.b), endColor.a);

            var realDuration = Mathf.Max(duration * 60.0f / bpm, 0.4f);

            mat.color = startColor;
            FollowCamera.Instance.fgColorTween = mat.DOColor(endColor, realDuration).SetEase(easeType).OnComplete(() =>
            {
                mat.color = endColor;
            }).SetLink(FollowCamera.Instance.gameObject);
        }
    }

    public void PlayFlash_Legacy()
    {
        Material mat;

        FollowCamera.Instance.fgColorTween?.Kill();
        FollowCamera.Instance.bgColorTween?.Kill();

        if (plane == "Background")
        {
            fg.material.color = new Color(1, 1, 1, 0);
            bg.material.color = new Color(0, 0, 0, 0);
            mat = bg.material;

            var realDuration = duration * 60.0f / bpm;

            mat.color = startColor;
            FollowCamera.Instance.bgColorTween = mat.DOColor(endColor, realDuration).SetEase(easeType).OnComplete(() =>
            {
                mat.color = endColor;
            }).SetLink(FollowCamera.Instance.gameObject);
        }
        else if (plane == "Foreground")
        {
            bg.material.color = new Color(0, 0, 0, 0);
            fg.material.color = new Color(1, 1, 1, 0);
            mat = fg.material;

            endColor = new Color(Mathf.Min(1.0f, endColor.r), Mathf.Min(1.0f, endColor.g), Mathf.Min(1.0f, endColor.b), endColor.a);

            var realDuration = duration * 60.0f / bpm;

            mat.color = startColor;
            FollowCamera.Instance.fgColorTween = mat.DOColor(endColor, realDuration).SetEase(easeType).OnComplete(() =>
            {
                mat.color = endColor;
            }).SetLink(FollowCamera.Instance.gameObject);
        }
    }
}
