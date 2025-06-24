using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_SetHitSound : FloorEvent
{
    private HitSound hitsound;
    private float hitsoundVolume;

    public Event_SetHitSound(HitSound hitsound, float hitsoundVolume)
    {
        this.hitsound = hitsound;
        this.hitsoundVolume = hitsoundVolume;
    }

    public override void EventStart()
    {
        SFXManager.Instance.hitSoundType = hitsound;
        SFXManager.Instance.hitSoundVolume = hitsoundVolume;
    }
}
