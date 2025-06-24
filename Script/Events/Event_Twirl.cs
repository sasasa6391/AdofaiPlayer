using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Twirl : FloorEvent
{
    public override void EventStart()
    {
        PlanetController.IsCW = !PlanetController.IsCW;

    }
}
