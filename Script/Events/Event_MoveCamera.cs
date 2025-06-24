using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Event_MoveCamera : FloorEvent
{
    public float duration;
    public Ease easeType;
    public float zoom;
    public float rotation;
    public string relativeTo;
    public Vector3 position;
    public Vector3 offset;

    public FollowCamera cam;
    public Event_MoveCamera(float duration, Ease easeType, float zoom, float rotation, string relativeTo, Vector3 position, Vector3 offset)
    {
        this.duration = duration;
        this.easeType = easeType;
        this.zoom = zoom / 100.0f;
        this.rotation = rotation;
        this.relativeTo = relativeTo;
        this.position = position;
        this.offset = offset;
        cam = FollowCamera.Instance;
    }

    public override void EventStart()
    {

        if (relativeTo == "LastPosition")
        {
            this.position = cam.transform.position;
        }
        this.position += offset;

        cam.zoomTween?.Kill();
        cam.rotTween?.Kill();
        cam.moveTween?.Kill();
        if (duration == 0.0f)
        {
            cam.zoomSize = zoom;
            cam.rotation = rotation;

            if (relativeTo == "Tile")
            {
                cam.targetPos = position;
                cam.transform.position = position + new Vector3(0, 0, -10);
            }
            else
            {
                cam.DirectFollow(relativeTo);
            }

            return;
        }

        var realDuration = duration * 60.0f / bpm;


        if (cam.IsTargetTile == true)
        {
            if (relativeTo == "Player")
            {
                cam.IsTargetTile = false;
                cam.transform.position = cam.playerTf.transform.position + new Vector3(0, 0, -10);
            }
        }

        var ori = 0.0f;
        if (relativeTo == "Tile")
        {
            cam.IsTargetTile = true;
            cam.targetPos = position;
            cam.moveTween = cam.transform.DOMove(position + new Vector3(0, 0, -10), realDuration).SetEase(easeType).SetLink(FollowCamera.Instance.gameObject);
        }
        else if (relativeTo == "LastPosition")
        {
            ori = cam.rotation;
            if (cam.IsTargetTile == true)
            {
                cam.targetPos = position;
                cam.moveTween = cam.transform.DOMove(position + new Vector3(0, 0, -10), realDuration).SetEase(easeType).SetLink(FollowCamera.Instance.gameObject);
            }
        }

        cam.zoomTween = DOTween.To(() => cam.zoomSize, delegate (float x)
        {
            cam.zoomSize = x;
        }, zoom, realDuration).SetEase(easeType).SetLink(FollowCamera.Instance.gameObject);

        cam.rotTween = DOTween.To(() => cam.rotation, delegate (float x)
        {
            cam.rotation = x;
        }, ori + rotation, realDuration).SetEase(easeType).SetLink(FollowCamera.Instance.gameObject);

    }
}
