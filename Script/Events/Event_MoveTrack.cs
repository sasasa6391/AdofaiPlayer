using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_MoveTrack : FloorEvent
{
    public int startTileIndex;
    public int endTileIndex;
    public float duration;

    public Vector3 offset;
    public float angleOffset;
    public float endOpacity;
    public Ease ease;
    public float delay;

    public Event_MoveTrack(int startTileIndex, int endTileIndex, float duration, Vector3 offset, float angleOffset, float endOpacity, float delay, Ease ease)
    {
        this.startTileIndex = startTileIndex;
        this.endTileIndex = endTileIndex;
        this.duration = duration;
        this.offset = offset;
        this.angleOffset = angleOffset;
        this.endOpacity = endOpacity;
        this.delay = delay;
        this.ease = ease;
    }

    public override void EventStart()
    {
        if (delay == 0.0f)
        {
            MoveTrack();
        }
        else
        {
            var rDelay = delay / ((bpm / 60.0f) * 180.0f);

            DOVirtual.DelayedCall(rDelay, () =>
            {
                MoveTrack();
            }).SetLink(FollowCamera.Instance.gameObject);
        }
    }

    public void MoveTrack()
    {
        var realDuration = duration * 60.0f / bpm;

        for (int i = Mathf.Max(0, startTileIndex); i <= Mathf.Min(endTileIndex, LevelManager.listFloor.Count - 1); i++)
        {
            LevelManager.listFloor[i].SetMove(realDuration, offset, angleOffset, endOpacity, ease);
        }
    }
}
