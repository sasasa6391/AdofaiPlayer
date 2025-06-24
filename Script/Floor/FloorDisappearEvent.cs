using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class FloorDisappearEvent 
{
    public Floor floor;
    public TrackAnimationType2 animType;
    public float delayBeat;
    public float currentBeat = 0.0f;

    public bool Invoked = false;
    
    public bool Check()
    {
        if (floor.endTime + (delayBeat * 60.0f) / (Conductor.bpm) <= Conductor.currentTime)
        {
            Invoked = true;
            Play();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Play()
    {
        if(Invoked == true)
        {
            return;
        }
        Invoked = true;
        float duration = 0.0f;

        {
            float num2 = 60f / (floor.currBpm);
            duration = Mathf.Min(num2 * 0.5f, 0.5f);
        }

        var currAngle = floor.transform.eulerAngles.z;
        float angle = Random.Range(-75f, 75f);

        switch (animType)
        {
            case TrackAnimationType2.Retract:
                floor.disappearEvent = null;
                floor.MoveToBack();
                break;
            case TrackAnimationType2.Scatter:
                {
                    float num3 = Random.Range(-4f, 4f);
                    float num4 = Random.Range(-4f, 4f);
                    floor.MoveToBack();
                    floor.transform.DOMove(floor.transform.position + new Vector3(num3, num4), duration).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        floor.disappearEvent = null;
                    }).SetLink(floor.gameObject);
                    floor.transform.DORotate(new Vector3(0, 0, currAngle + angle), duration).SetEase(Ease.OutSine).SetLink(floor.gameObject);
                    break;
                }
            case TrackAnimationType2.Scatter_Far:
                {
                    float num = Random.Range(-8f, 8f);
                    float num2 = Random.Range(-8f, 8f);
                    floor.MoveToBack();
                    floor.transform.DOMove(floor.transform.position + new Vector3(num, num2), duration).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        floor.disappearEvent = null;
                    }).SetLink(floor.gameObject);
                    floor.transform.DORotate(new Vector3(0, 0, currAngle + angle), duration).SetEase(Ease.OutSine).SetLink(floor.gameObject);
                    break;
                }
            case TrackAnimationType2.Shrink:
                floor.transform.DOScale(Vector3.zero, duration).OnComplete(() =>
                {
                    floor.disappearEvent = null;
                    floor.MoveToBack();
                }).SetLink(floor.gameObject);
                break;
            case TrackAnimationType2.Shrink_Spin:
                floor.transform.DOScale(Vector3.zero, duration);
                floor.transform.DORotate(new Vector3(0, 0, currAngle - 180.0f), duration).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    floor.disappearEvent = null;
                    floor.MoveToBack();
                }).SetLink(floor.gameObject);
                break;
            case TrackAnimationType2.Fade:
                floor.disappearEvent = null;
                floor.TweenOpacity(0f, duration);
                break;
        }
    }
}
