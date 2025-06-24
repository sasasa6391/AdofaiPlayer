using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_SetFilter : FloorEvent
{
    public string filter;
    public bool enabled;
    public bool disableOthers;
    public float Intensity;

    public Event_SetFilter(string filter, float Intensity, bool enabled, bool disableOthers)
    {
        this.filter = filter;
        this.enabled = enabled;
        this.disableOthers = disableOthers;
        this.Intensity = Intensity;
    }
    
    public override void EventStart()
    {
        if(disableOthers == true)
        {
            FollowCamera.Instance.DisableAllFilters();
        }

        FollowCamera.Instance.SetFilter(filter, Intensity, enabled);
    }
}
