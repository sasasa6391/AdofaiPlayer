using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorEventScheduler : MonoBehaviour
{
    public static Queue<FloorAppearEvent> floorAppearEventQueue = new Queue<FloorAppearEvent>();
    public static List<FloorDisappearEvent> floorDisappearEvents = new List<FloorDisappearEvent>();
    public static List<FloorDisappearEvent> disappearUpdateList = new List<FloorDisappearEvent>();
    public int _appearIdx = 0;
    public int _disappearIdx = 0;
    public float _currentTimeAppear = 0.0f;
    public float _currentTimeDisappear = 0.0f;

    public static void Initialize()
    {
        floorAppearEventQueue.Clear();
        floorDisappearEvents.Clear();
        disappearUpdateList.Clear();
        FloorAppearEvent.Initialize();
    }
    public void ManualUpdate(float delta, float bpm)
    {
        if (floorAppearEventQueue.Count > 0)
        {
            while (floorAppearEventQueue.Count > 0 && floorAppearEventQueue.Peek().animType == TrackAnimationType.None)
            {
                floorAppearEventQueue.Dequeue();
            }
            if (floorAppearEventQueue.Count > 0)
            {
                var peekF = floorAppearEventQueue.Peek();
                _currentTimeAppear += Time.deltaTime * Mathf.Max(peekF.floor.currBpm, Conductor.bpm) / 60.0f;
                if (_currentTimeAppear > peekF.floor.maxTickAngle / 360.0f)
                {
                    peekF.Play();
                    floorAppearEventQueue.Dequeue();
                    _currentTimeAppear -= peekF.floor.maxTickAngle / 360.0f;
                }
            }
        }

        while (_disappearIdx != floorDisappearEvents.Count)
        {
            var dFloor = floorDisappearEvents[_disappearIdx].floor;
            if (LevelManager.CurrentFloor.index > dFloor.index)
            {
                disappearUpdateList.Add(floorDisappearEvents[_disappearIdx]);
                _disappearIdx++;
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < disappearUpdateList.Count; i++)
        {
            var e = disappearUpdateList[i];

            e.delayBeat -= delta * bpm / 60.0f;
            if (e.delayBeat <= 0.0f)
            {
                e.Play();
                disappearUpdateList.RemoveAt(i);
                i--;
            }
        }




        /*
        if (_appearIdx != floorAppearEvents.Count)
        {
            var floor = floorAppearEvents[_appearIdx].floor;
            _currentTimeAppear += delta * Mathf.Max(floor.currBpm, Conductor.bpm) / 60.0f;
            while (_currentTimeAppear >= floor.maxTickAngle / 360.0f)
            {
                if(floorAppearEvents[_appearIdx].Check(_currentTimeAppear) == true)
                {
                    _currentTimeAppear -= floor.maxTickAngle / 360.0f;
                }
                else
                {
                    break;
                }
            }
        }
        */

    }

    /*
    public bool CheckAppear()
    {
        if (_appearIdx < floorAppearEvents.Count && floorAppearEvents[_appearIdx].Check() == true)
        {
            _appearIdx++;
            return true;
        }

        return false;
    }

    public static void Setup()
    {
        foreach (var e in floorAppearEvents)
        {
            e.FloorSetup();
        }
    }
    */
}
