using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_RecolorTrack : FloorEvent
{
    public int startTileIndex;
    public int endTileIndex;
    public TrackColorType trackColorType;
    public Color trackColor;
    public Color trackSecnodColor;
    public float trackColorAnimDuration;
    //public float trackColorPulse;
    public float trackPulseLength;
    public string trackStyle;
    public TrackColorPulse trackColorPulse;
    public int tileIndex;

    public Event_RecolorTrack(int tileIndex, int startTileIndex, int endTileIndex, TrackColorType trackColorType, Color trackColor, Color trackSecnodColor, float trackColorAnimDuration, int trackPulseLength, string trackStyle, TrackColorPulse trackColorPulse)
    {
        this.tileIndex = tileIndex;
        this.startTileIndex = startTileIndex;
        this.endTileIndex = endTileIndex;
        this.trackColorType = trackColorType;
        this.trackColor = trackColor;
        this.trackSecnodColor = trackSecnodColor;
        this.trackColorAnimDuration = trackColorAnimDuration;
        this.trackPulseLength = trackPulseLength;
        this.trackStyle = trackStyle;
        this.trackColorPulse = trackColorPulse;

    }

    //listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, (float) colorIndex / (float) trackPulseLength * trackColorAnimation, trackColorType);
    public override void EventStart()
    {
        int colorIndex = 0;
        int dir = -1; 
        if (trackColorType != TrackColorType.Glow)
        {
            dir = 1;
        }
        for (int i = startTileIndex; i <= Mathf.Min(endTileIndex, LevelManager.listFloor.Count - 1); i++)
        {
            //if (LevelManager.listFloor[i].setColorIndex > tileIndex) break;

            LevelManager.listFloor[i].floorRenderer.sprite = ResourceManager.LoadResourceSprite($"Sprite/tiles_editor_{LevelManager.TileTypeToString[trackStyle]}_{Floor.angleMap[LevelManager.listFloor[i].spriteAngle]}");
            float timing = 0.0f;
            if (trackColorPulse != TrackColorPulse.None)
            {
                if (trackColorType == TrackColorType.Glow)
                {
                    timing = (float)Mathf.Clamp(colorIndex, 0, trackPulseLength) / trackPulseLength;
                    if (colorIndex == trackPulseLength)
                    {
                        dir = -dir;
                    }
                    else if (colorIndex == 0)
                    {
                        dir = -dir;
                    }
                }
                else
                {
                    timing = (colorIndex % trackPulseLength) / trackPulseLength;
                }
            }
            LevelManager.listFloor[i].SetColor(trackColor, trackSecnodColor, trackColorAnimDuration, timing, trackColorType, trackColorPulse);
            colorIndex += dir;
        }
    }
}
