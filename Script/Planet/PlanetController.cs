using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetController : MonoBehaviour
{

    public Transform CanvasFloatingText;
    public Conductor conductor;

    public Planet[] planets = new Planet[2];

    public TextMeshProUGUI tmpText;
    public Planet chosenPlanet;
    public MeshRenderer quad;
    [NonSerialized]
    public double speed = 1.0;

    [NonSerialized]
    public static bool IsCW = false;

    [NonSerialized]
    public RenderTexture homRenderTexture;

    public Camera staticCam;

    public FloorEventScheduler floorEventScheduler;

    public float startRadius
    {
        get
        {
            return 1.5f;
        }
    }


    private void Awake()
    {

        //chosenPlanet.snappedLastAngle = Mathf.PI/2 * (PlanetController.IsCW ? 1.0f : -1.0f);
        startOK = false;

    }

    public static void Initialize()
    {
        IsCW = false;
    }


    private int turn = 0;
    float _currentTIme = 0.0f;
    public float waitTime = 0.0f;

    private bool _start = false;
    public void SongStart()
    {
        tmpText.gameObject.SetActive(false);
        planets[1].SetRotate(true);
        planets[1].angle += Conductor.bpm * 180.0f / 60.0f * Conductor.offset / 1000.0f;
        planets[1].tickAngle -= Conductor.bpm * 180.0f / 60.0f * Conductor.offset / 1000.0f;
        Conductor.currentTime -= Conductor.offset / 1000.0f;
        Conductor.currentBeat -= Conductor.offset / 1000.0f * Conductor.bpm / 60.0f;
        conductor.song.Play();
        _start = true;
    }
    bool _isEnd = false;

    private float _endTime = 0.0f;
    public static bool startOK;

    public int forceFadeFloorIndex = 0;
    public int forceAppearFloorIndex = 300;
    public bool InitFirst = false;
    private void Update()
    {
        if (conductor.paused == true)
            return;

        if (Input.GetKey(KeyCode.Escape) == true)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }
        if (_start == true)
        {
            _currentTIme += Time.deltaTime;
            Conductor.currentTime += Time.deltaTime;
            Conductor.currentBeat += Time.deltaTime * Conductor.bpm / 60.0f;
        }
        else
        {
            waitTime += Time.deltaTime;

            if(startOK == true && InitFirst == false)
            {
                InitFirst = true;
                LevelManager.listFloor[0].Glow.gameObject.SetActive(true);
                LevelManager.listFloor[0].PlayFloorEvents();
            }

            if (waitTime > 1.0f && startOK == true && Input.anyKeyDown == true)
            {
                SongStart();
            }
            else
            {
                return;
            }
        }

        if (_isEnd == true)
        {
            floorEventScheduler.ManualUpdate(Time.deltaTime, Conductor.bpm);
            _endTime += Time.deltaTime;
            if (_endTime > 2.0f)
            {
                SceneManager.LoadScene("MainScene");
                return;
            }
            return;
        }

        if (LevelManager.listFloor.Count - 1 <= LevelManager.currentIndex)
        {
            _isEnd = true;
            return;
        }

        if (LevelManager.CurrentFloor.isMoveFloor == true)
        {
            transform.position = LevelManager.CurrentFloor.transform.position;
        }

        if (turn >= 1)
        {
            foreach (var e in planets)
            {
                e.UpdatePlanet();
            }
            floorEventScheduler.ManualUpdate(Time.deltaTime, Conductor.bpm);

            if (planets[(turn + 1) % 2].tickAngle >= LevelManager.CurrentFloor.maxTickAngle)
            {
                SFXManager.Instance.PlaySFX();

                if (LevelManager.listFloor.Count - 1 == LevelManager.currentIndex + 1)
                {
                    LevelManager.listFloor[LevelManager.currentIndex].MoveToBack();
                    SetNextPos(planets[(turn + 1) % 2].tickAngle - LevelManager.listFloor[LevelManager.currentIndex].maxTickAngle, Conductor.bpm);
                    return;
                }

                if (LevelManager.listFloor[LevelManager.currentIndex + 2].isMidSpin == false)
                {
                    if (LevelManager.listFloor[LevelManager.currentIndex + 1].isMidSpin == true)
                    {
                        if ((LevelManager.currentIndex + 3 < LevelManager.listFloor.Count && LevelManager.listFloor[LevelManager.currentIndex + 3].isMidSpin == false))
                        {
                            SetNextPos(planets[(turn + 1) % 2].tickAngle - LevelManager.listFloor[LevelManager.currentIndex].maxTickAngle, Conductor.bpm);
                            turn++;
                        }
                        else
                        {
                            LevelManager.listFloor[LevelManager.currentIndex + 1].PlayFloorEvents();
                            LevelManager.listFloor[LevelManager.currentIndex].MoveToBack();
                            LevelManager.currentIndex++;
                            LevelManager.CurrentFloor.Glow.gameObject.SetActive(true);

                            if (forceAppearFloorIndex + 1 <= LevelManager.listFloor.Count)
                            {
                                LevelManager.listFloor[forceAppearFloorIndex++].gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        SetNextPos(planets[(turn + 1) % 2].tickAngle - LevelManager.listFloor[LevelManager.currentIndex].maxTickAngle, Conductor.bpm);
                        turn++;
                    }
                }
                else
                {
                    LevelManager.listFloor[LevelManager.currentIndex + 1].PlayFloorEvents();
                    LevelManager.listFloor[LevelManager.currentIndex].MoveToBack();
                    LevelManager.currentIndex++;
                    LevelManager.CurrentFloor.Glow.gameObject.SetActive(true);

                    if (forceAppearFloorIndex + 1 <= LevelManager.listFloor.Count)
                    {
                        LevelManager.listFloor[forceAppearFloorIndex++].gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                floorEventScheduler.ManualUpdate(Time.deltaTime, Conductor.bpm);
            }
        }
        else if (turn == 0 && _currentTIme > Conductor.offset / 1000.0f)
        {
            SFXManager.Instance.PlaySFX();
            SetNextPos(0.0f, Conductor.bpm);
            turn++;
            foreach (var e in planets)
            {
                e.UpdatePlanet();
            }
            floorEventScheduler.ManualUpdate(Time.deltaTime, Conductor.bpm);
        }
        else
        {
            floorEventScheduler.ManualUpdate(Time.deltaTime, Conductor.bpm);
            foreach (var e in planets)
            {
                e.UpdatePlanet();
            }
        }



    }


    public void SetNextPos(float remain, float prevBpm)
    {
        if (LevelManager.listFloor[LevelManager.currentIndex + 1].isMidSpin == true)
        {
            LevelManager.listFloor[LevelManager.currentIndex + 1].PlayFloorEvents();
            LevelManager.listFloor[LevelManager.currentIndex].MoveToBack();
            LevelManager.currentIndex++;
            if (forceAppearFloorIndex + 1 <= LevelManager.listFloor.Count)
            {
                LevelManager.listFloor[forceAppearFloorIndex++].gameObject.SetActive(true);
            }
            LevelManager.CurrentFloor.Glow.gameObject.SetActive(true);
        }

        //Debug.Log($"{LevelManager.currentIndex} : {conductor.beat}");


        if (LevelManager.listFloor.Count - 1 > LevelManager.currentIndex + 1)
        {
            LevelManager.listFloor[LevelManager.currentIndex + 1].PlayFloorEvents();
            LevelManager.listFloor[LevelManager.currentIndex].MoveToBack();
        }

        LevelManager.currentIndex++;

        if (LevelManager.listFloor.Count <= LevelManager.currentIndex)
        {
            return;
        }

        var dest = LevelManager.listFloor[LevelManager.currentIndex].transform.position;
        var text = Instantiate(Resources.Load("AccText")).GetComponent<RectTransform>();
        text.position = dest + new Vector3(0, 0.8f);
        text.SetParent(CanvasFloatingText);
        //Core.Instance.pathManager.transform.GetChild(turn + 1).GetComponentInChildren<SpriteRenderer>().material.EnableKeyword("GLOW_ON");
        transform.position = dest;
        planets[(turn + 1) % 2].transform.position = dest;
        planets[(turn + 1) % 2].SetRotate(false);
        planets[(turn + 1) % 2].tickAngle = 0.0f;
        LevelManager.CurrentFloor.Glow.gameObject.SetActive(true);
        planets[turn % 2].angle = LevelManager.PathToAngle[LevelManager.listChar[LevelManager.currentIndex - 1]] + 180.0f + remain * (Conductor.bpm / prevBpm);
        planets[turn % 2].SetRotate(true);
        planets[turn % 2].tickAngle = remain * (Conductor.bpm / prevBpm);


        if (LevelManager.CurrentFloor.index > 250)
        {
            LevelManager.listFloor[forceFadeFloorIndex++].gameObject.SetActive(false);
        }

        if (forceAppearFloorIndex + 1 <= LevelManager.listFloor.Count)
        {
            LevelManager.listFloor[forceAppearFloorIndex++].gameObject.SetActive(true);
        }
    }

}
