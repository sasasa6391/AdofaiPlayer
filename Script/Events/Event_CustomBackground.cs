using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event_CustomBackground : FloorEvent
{
    public Color color;
    public string bgImage;
    public Color imageColor;
    public SpriteRenderer bg;
    public SpriteRenderer customBackground;
    public float unscaledSize = 0.0f;
    public Event_CustomBackground(string bgImage, Color color, Color imageColor, float unscaledSize)
    {
        //bg = FollowCamera.Instance.bg;
        customBackground = FollowCamera.Instance.bg_Tex;
        this.color = color;
        this.bgImage = bgImage;
        this.imageColor = imageColor;
        this.unscaledSize = unscaledSize / 100.0f;
    }

    public override void EventStart()
    {
        /*
        FollowCamera.Instance.bgColorTween?.Kill();
        bg.color = color;
        */
        customBackground.sprite = Resources.Load<Sprite>($"{FileManager.Instance.fileName}/{bgImage.Split('.')[0]}");
        customBackground.color = imageColor;
        customBackground.transform.localScale = Vector3.one * unscaledSize;


    }
}
