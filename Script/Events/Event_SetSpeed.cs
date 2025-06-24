using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpeedType
{
    Bpm = 0,
    Multiplier = 1
}

public class Event_SetSpeed : FloorEvent
{

    public SpeedType type;
    public float beatsPerMinute;
    public float bpmMultiplier;

    public Event_SetSpeed(SpeedType type, float beatsPerMinute, float bpmMultiplier)
    {
        this.type = type;
        this.beatsPerMinute = beatsPerMinute;
        this.bpmMultiplier = bpmMultiplier;
    }

    public override void EventStart()
    {
        if (type == SpeedType.Bpm)
        {
            Conductor.bpm = beatsPerMinute;
        }
        else
        {
            Conductor.bpm *= bpmMultiplier;
        }
    }
}
