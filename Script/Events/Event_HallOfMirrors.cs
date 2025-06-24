using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_HallOfMirrors : FloorEvent
{
    public Event_HallOfMirrors(bool enabled)
    {
        this.enabled = enabled; 
    }
    public bool enabled;
    public override void EventStart()
    {
        FollowCamera.Instance.homEffect.SetupHomEffect(enabled);
    }
}
