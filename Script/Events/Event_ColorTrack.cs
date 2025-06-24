using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TrackColorType
{
    Single = 0,
    Stripes = 1,
    Glow = 2,
    Blink = 3,
    Switch = 4,
    Rainbow = 5,
    Volume = 6
}
public enum TrackColorPulse
{
    None = 0,
    Forward = 1,
    Backward = 2
}


public class Event_ColorTrack : FloorEvent
{
    public TrackColorType trackColorType;
    public Color trackColor;
    public Color trackSecnodColor;
    public float trackColorAnimDuration;
    //public float trackColorPulse;
    public float trackPulseLength;
    public string trackStyle;

    public Event_ColorTrack(TrackColorType trackColorType, Color trackColor, Color trackSecnodColor, float trackColorAnimDuration, int trackPulseLength, string trackStyle)
    {
        this.trackColorType = trackColorType;
        this.trackColor = trackColor;
        this.trackSecnodColor = trackSecnodColor;
        this.trackColorAnimDuration = trackColorAnimDuration;
        this.trackPulseLength = trackPulseLength;
        this.trackStyle = trackStyle;
    }

    public override void EventStart()
    {
    }
}
