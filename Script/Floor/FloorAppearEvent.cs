using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAppearEvent : MonoBehaviour
{
    public Floor floor;
    public TrackAnimationType animType;
    public float delayBeat;
    public bool invoked = false;

    public static int currentIndex = 0;

    public static void Initialize()
    {
        currentIndex = 0;
    }
    public float _currentTime = 0.0f;
    private void LateUpdate()
    {

        if (currentIndex == floor.index)
        {
            var t = floor.startTime - delayBeat * (60.0f / floor.currBpm);

            if (t <= Conductor.currentTime || animType == TrackAnimationType.None)
            {
                currentIndex = floor.index + 1;
                FloorEventScheduler.floorAppearEventQueue.Enqueue(this);
            }
        }
    }

    public Vector3 toRotate;
    public Vector3 toPosition;


    public void FloorSetup()
    {
        toRotate = new Vector3(0, 0, floor.startAngle);
        toPosition = floor.startPos;

        switch (animType)
        {
            case TrackAnimationType.Extend:
                floor.transform.localScale = Vector2.zero;
                break;
            case TrackAnimationType.Grow:
                floor.transform.localScale = Vector2.zero;
                break;
            case TrackAnimationType.Grow_Spin:
                floor.transform.localScale = Vector2.zero;
                floor.transform.eulerAngles = floor.transform.eulerAngles + Vector3.forward * -180f;
                break;
            case TrackAnimationType.Assemble:
                {
                    float num4 = Random.Range(-4f, 4f);
                    float num5 = Random.Range(-4f, 4f);
                    float num6 = Random.Range(-75f, 75f);
                    floor.transform.position = floor.transform.position + new Vector3(num4, num5);
                    floor.transform.eulerAngles = floor.transform.eulerAngles + Vector3.forward * num6;
                    break;
                }
            case TrackAnimationType.Assemble_Far:
                {
                    float num = Random.Range(-8f, 8f);
                    float num2 = Random.Range(-8f, 8f);
                    float num3 = Random.Range(-75f, 75f);
                    floor.transform.position = floor.transform.position + new Vector3(num, num2);
                    floor.transform.eulerAngles = floor.transform.eulerAngles + Vector3.forward * num3;
                    break;
                }
            case TrackAnimationType.Fade:
                floor.TweenOpacity(0f, 0f, Ease.Linear, true);
                break;
            case TrackAnimationType.Drop:
                floor.transform.position = floor.transform.position + Vector3.up * 8f;
                floor.transform.localScale = Vector3.zero;
                break;
            case TrackAnimationType.Rise:
                floor.transform.position = floor.transform.position - Vector3.up * 8f;
                floor.transform.localScale = Vector3.zero;
                break;
        }
    }
    public void Play()
    {
        floor.appearEvent = null;
        invoked = true;

        if (animType == TrackAnimationType.None)
        {
            return;
        }

        float duration = 0.0f;

        {
            bool num = animType == TrackAnimationType.Drop || animType == TrackAnimationType.Rise;
            float num2 = 60f / (floor.currBpm);
            float num3 = delayBeat;
            if (num)
            {
                num3 *= 2f;
            }
            if (num)
            {
                duration = num2 * num3;
            }
            else
            {
                duration = Mathf.Min(num2 * 0.5f, 0.5f);
            }
        }



        if (animType == TrackAnimationType.Assemble || animType == TrackAnimationType.Assemble_Far)
        {
            floor.transform.DORotate(toRotate, duration).SetEase(Ease.OutSine).SetLink(gameObject);
        }
        else if (animType == TrackAnimationType.Grow)
        {
            floor.transform.DOScale(Vector3.one, duration).SetLink(gameObject);
        }
        else if (animType == TrackAnimationType.Grow_Spin)
        {
            floor.transform.DOScale(Vector3.one, duration).SetLink(gameObject);
            floor.transform.DORotate(toRotate, duration).SetEase(Ease.OutSine).SetLink(gameObject);
        }
        else if (animType == TrackAnimationType.Fade)
        {
            floor.TweenOpacity(1f, duration, Ease.Linear);
        }
        else if (animType == TrackAnimationType.Drop || animType == TrackAnimationType.Rise)
        {
            floor.transform.DOScale(Vector3.one, duration / 8f).SetLink(gameObject);
        }
        Ease ease = ((animType == TrackAnimationType.Drop || animType == TrackAnimationType.Rise) ? Ease.Linear : Ease.OutSine);
        floor.transform.DOMove(toPosition, duration).SetEase(ease).SetLink(gameObject);
    }
}

