using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Conductor : MonoBehaviour
{

    public AudioSource song;

    public HitSound hitSound = HitSound.Kick;

    [Range(0f, 2f)]
    public float hitSoundVolume = 1f;

    public static float calibration_i => 0.0f;

    [NonSerialized]
    public double lastHit;

    [NonSerialized]
    public double crotchet;

    public static float offset;

    public static float bpm = 110.0f;

    public int countdownTicks = 4;

    public float countdownSpeedMultiplier = 1f;

    public float adjustedCountdownTicks => (float)countdownTicks / countdownSpeedMultiplier;

    public static float currentTime = 0.0f;
    public static float currentBeat = 0.0f;
    public float ctime = 0.0f;
    public float dtime = 0.0f;
    public float eTime = 0.0f;

    public bool paused = false;

    public TextMeshProUGUI bpmUI;

    private void Awake()
    {
        // VSync 비활성화
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
        bpmUI.gameObject.SetActive(false);
    }
    public static void Initialize()
    {
        currentTime = 0.0f;
        currentBeat = 0.0f;
    }
    
    public void LateUpdate()
    {
        if(paused == true)
        {
            return;
        }

        if (song.isPlaying == true)
        {
            if (bpmUI != null)
            {
                bpmUI.text = $"BPM : {bpm.ToString("F2")}";
            }
            ctime = currentTime;
            dtime = LevelManager.CurrentFloor.startTime;
            eTime = LevelManager.CurrentFloor.endTime;
        }
    }

    /*
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Time.timeScale = 0; // 게임 일시정지
            song.Pause();
            paused = true;
        }
        else
        {
            Time.timeScale = 1; // 게임 재개
            song.UnPause();
            paused = false;
        }
    }
    */


}
