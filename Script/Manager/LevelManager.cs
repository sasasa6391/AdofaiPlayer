using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class LevelManager : MonoBehaviour
{
    public class Tile
    {
        string name;
        Vector3 rot;
        public Tile(string name, Vector3 rot)
        {
            this.name = name;
            this.rot = rot;
        }
    }

    public Conductor conductor;


    public static Dictionary<string, string> TileTypeToString = new Dictionary<string, string>
        {
            { "Standard", "greyscale" },
            { "Basic", "basic" },
            { "Neon", "neon" },
            { "NeonLight", "neonLight" },
            { "Gems", "neonLight" },
       };

    public static Dictionary<char, int> PathToAngle = new Dictionary<char, int>
        {
            { 'R', 0 },
            { 'p', 15 },
            { 'J', 30 },
            { 'E', 45 },
            { 'T', 60  },
            { 'o', 75 },
            { 'U', 90 },
            { 'q', 105 },
            { 'G', 120 },
            { 'Q', 135 },
            { 'H', 150 },
            { 'W', 165 },
            { 'L', 180 },
            { 'x', 195 },
            { 'N', 210 },
            { 'Z', 225 },
            { 'F', 240 },
            { 'V', 255 },
            { 'D', 270 },
            { 'Y', 285 },
            { 'B', 300 },
            { 'C', 315 },
            { 'M', 330 },
            { 'A', 345 },
            { '!', 360 }
    };


    public static Dictionary<int, char> AngleToPath = new Dictionary<int, char>
        {
            { 0 , 'R'},
            { 15 , 'p'},
            { 30 , 'J'},
            { 45 , 'E'},
            { 60 , 'T' },
            { 75 , 'o'},
            { 90 , 'U'},
            { 105, 'q' },
            { 120, 'G' },
            { 135, 'Q' },
            { 150, 'H' },
            { 165, 'W' },
            { 180, 'L' },
            { 195, 'x' },
            { 210, 'N' },
            { 225, 'Z' },
            { 240, 'F' },
            { 255, 'V' },
            { 270, 'D' },
            { 285, 'Y' },
            { 300, 'B' },
            { 315, 'C' },
            { 330, 'M' },
            { 345, 'A' },
            { 999, '!' }
    };

    public string Input;

    public static string listChar = "";
    public static List<Floor> listFloor = new List<Floor>();
    public static bool IsLegacyFlash = false;
    public Transform floorParent;

    public static int currentIndex = 0;

    public TextMeshProUGUI textTitle;
    public static Floor CurrentFloor => listFloor[Mathf.Clamp(currentIndex, 0, listFloor.Count - 1)];

    public TrackAnimationType trackAppearType;
    public float TrackAppearDelay;
    public TrackAnimationType2 trackDisappearType;
    public float TrackDisappearDelay;

    public void Awake()
    {
        DOTween.SetTweensCapacity(10000, 500);
    }

    public void Initialize()
    {
        listFloor.Clear();
        currentIndex = 0;
    }

    public JToken settingData;
    public void LoadLevel(JToken settingData, string pathData, JArray actions)
    {
        Initialize();
        PlanetController.Initialize();
        Conductor.Initialize();
        FloorEventScheduler.Initialize();


        //listChar = Input;

        string pattern = @"\([^)]*\)";
        string result = Regex.Replace(settingData["artist"].ToString(), pattern, "");

        this.settingData = settingData;
        textTitle.text = result + " - " + settingData["song"].ToString();
        var unscaledSize = settingData["unscaledSize"] == null ? 100f : float.Parse(settingData["unscaledSize"]?.ToString());
        var bg = Resources.Load<Sprite>($"{FileManager.Instance.fileName}/{settingData["bgImage"]?.ToString().Split('.')[0]}");
        if (settingData["bgImage"]?.ToString() == "black.png")
        {
            FollowCamera.Instance.bg_Tex.color = new UnityEngine.Color(0, 0, 0, 0);
        }
        else if (bg != null)
        {
            FollowCamera.Instance.bg_Tex.sprite = bg;
            FollowCamera.Instance.bg_Tex.transform.localScale = Vector3.one * unscaledSize / 100.0f;
        }
        SFXManager.Instance.hitSoundType = Enum.Parse<HitSound>(settingData["hitsound"]?.ToString());
        SFXManager.Instance.hitSoundVolume = float.Parse(settingData["hitsoundVolume"]?.ToString());
        listChar = pathData;
        IsLegacyFlash = settingData["legacyFlash"] == null || settingData["legacyFlash"]?.ToString() == "True";
        GenerateFloor(actions);
    }
    public void LoadLevel(JToken settingData, JArray pathData, JArray actions)
    {
        Initialize();

        foreach (var e in pathData)
        {
            var intV = int.Parse(e.ToString());
            if (intV < 0)
            {
                intV += 360;
            }

            listChar += AngleToPath[intV];
        }

        //listChar = Input;
        this.settingData = settingData;

        //listChar = pathData;

        GenerateFloor(actions);
    }
    public void GenerateFloor(JArray actions)
    {

        TrackColorType trackColorType = Enum.Parse<TrackColorType>(settingData["trackColorType"]?.ToString());
        var firstColor = Utils.HexToColorWithOpacity(settingData["trackColor"].ToString(), 100.0f);
        var secondColor = Utils.HexToColorWithOpacity(settingData["secondaryTrackColor"].ToString(), 100.0f);
        var trackColorAnimation = float.Parse(settingData["trackColorAnimDuration"].ToString());
        var trackPulseLength = int.Parse(settingData["trackPulseLength"].ToString());
        var trackStyle = settingData["trackStyle"].ToString();
        var trackColorPulse = Enum.Parse<TrackColorPulse>(settingData["trackColorPulse"]?.ToString());

        trackAppearType = Enum.Parse<TrackAnimationType>(settingData["trackAnimation"]?.ToString());
        trackDisappearType = Enum.Parse<TrackAnimationType2>(settingData["trackDisappearAnimation"]?.ToString());
        TrackAppearDelay = float.Parse(settingData["beatsAhead"]?.ToString());
        TrackDisappearDelay = float.Parse(settingData["beatsBehind"]?.ToString());


        actions = new JArray(
            actions
                .Select((obj, index) => new { Obj = obj, Index = index })
                .OrderBy(x => int.Parse(x.Obj["floor"]?.ToString()))
                .ThenBy(x => (x.Obj["duration"] != null) ? float.Parse(x.Obj["duration"]?.ToString()) : 0.0f)
                .ThenBy(x => x.Index)
                .Select(x => x.Obj)
        );

        int colorDir = -1;
        if (trackColorType != TrackColorType.Glow)
        {
            colorDir = 1;
        }

        float colorIndex = 0;

        Vector3 currentPos = Vector3.zero;
        {
            var floor = Instantiate(Resources.Load($"Floor/Tile_{0}"), currentPos, Quaternion.identity, floorParent).GetComponent<Floor>();
            floor.floorRenderer.sprite = Resources.Load<Sprite>($"Sprite/tiles_editor_{TileTypeToString[trackStyle]}_{Floor.angleMap[0]}");

            float timing = 0.0f;
            if (trackColorPulse != TrackColorPulse.None)
            {
                if (trackColorType == TrackColorType.Glow)
                {
                    timing = colorIndex / trackPulseLength;

                    floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);

                    if (colorIndex == trackPulseLength)
                    {
                        colorDir = -colorDir;
                    }
                    else if (colorIndex == 0)
                    {
                        colorDir = -colorDir;
                    }
                }
                else
                {
                    timing = (colorIndex % trackPulseLength) / trackPulseLength;

                    floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                }
            }
            else
            {
                floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
            }
            colorIndex += colorDir;

            listFloor.Add(floor.GetComponent<Floor>());
        }
        List<Vector3> pos = new List<Vector3>();
        pos.Add(Vector3.zero);

        for (int i = 0; i < listChar.Length; i++)
        {
            var currentAngle = PathToAngle[listChar[i]];

            var diffVector = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;

            if (currentAngle == 360)
            {
                currentPos = pos[i - 1];
            }
            else
            {
                currentPos += (diffVector) * 1.5f;
            }
            pos.Add(currentPos);
        }


        for (int i = 0; i < listChar.Length; i++)
        {
            var currentAngle = PathToAngle[listChar[i]];

            if (i == listChar.Length - 1)
            {
                int tmpAngle = 0;

                var tileString = $"Floor/Tile_0";
                tmpAngle = 0;
                /*
                var tileString = $"Floor/Tile_{(PathToAngle[listChar[i - 1]] - PathToAngle[listChar[i - 2]] + 90) % 360}";
                tmpAngle = (PathToAngle[listChar[i - 1]] - PathToAngle[listChar[i - 2]] + 90) % 360;
                if (currentAngle == 360)
                {
                    tileString = $"Floor/Tile_{(PathToAngle[listChar[i - 1]] - PathToAngle[listChar[i - 2]] + 270) % 360}";
                    tmpAngle = (PathToAngle[listChar[i - 1]] - PathToAngle[listChar[i - 2]] + 270) % 360;
                    currentAngle = PathToAngle[listChar[i - 1]] + 180;
                }
                */

                var go = Instantiate(Resources.Load(tileString), pos[i + 1], Quaternion.identity, floorParent);
                var floor = go.GetComponent<Floor>();


                floor.spriteAngle = (int)tmpAngle;
                floor.startPos = floor.transform.position;
                floor.isMidSpin = PathToAngle[listChar[i]] == 360;
                floor.transform.eulerAngles = new Vector3(0, 0, floor.InitAngle + currentAngle);
                floor.GetComponent<SpriteRenderer>().sortingOrder = 9000 - i;
                floor.floorRenderer.sprite = Resources.Load<Sprite>($"Sprite/tiles_editor_{TileTypeToString[trackStyle]}_{Floor.angleMap[tmpAngle]}");
                float timing = 0.0f;
                if (trackColorPulse != TrackColorPulse.None)
                {
                    if (trackColorType == TrackColorType.Glow)
                    {
                        timing = colorIndex / trackPulseLength;

                        floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);

                        if (colorIndex == trackPulseLength)
                        {
                            colorDir = -colorDir;
                        }
                        else if (colorIndex == 0)
                        {
                            colorDir = -colorDir;
                        }
                    }
                    else
                    {
                        timing = (colorIndex % trackPulseLength) / trackPulseLength;

                        floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                    }
                }
                else
                {
                    floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                }
                colorIndex += colorDir;

                listFloor.Add(floor);
            }
            else
            {

                var angleDiff = PathToAngle[listChar[i + 1]] - currentAngle;

                if (currentAngle == 360)
                {
                    angleDiff = PathToAngle[listChar[i + 1]] - PathToAngle[listChar[i - 1]] + 180;
                    currentAngle = PathToAngle[listChar[i - 1]] + 180;
                }

                var spawnAngle = (angleDiff + 360) % 360;

                if (PathToAngle[listChar[i + 1]] == 360)
                {
                    spawnAngle = 360;
                }
                var tileString = $"Floor/Tile_{spawnAngle}";

                var go = Instantiate(Resources.Load(tileString), pos[i + 1], Quaternion.identity, floorParent);
                var floor = go.GetComponent<Floor>();
                floor.spriteAngle = spawnAngle;
                floor.startPos = floor.transform.position;
                floor.isMidSpin = PathToAngle[listChar[i]] == 360;
                floor.transform.eulerAngles = new Vector3(0, 0, floor.InitAngle + currentAngle);
                floor.startAngle = floor.transform.eulerAngles.z;
                floor.floorRenderer.sortingOrder = 30000 - i * 3;
                floor.floorRenderer.sprite = Resources.Load<Sprite>($"Sprite/tiles_editor_{TileTypeToString[trackStyle]}_{Floor.angleMap[spawnAngle]}");


                float timing = 0.0f;
                if (trackColorPulse != TrackColorPulse.None)
                {
                    if (trackColorType == TrackColorType.Glow)
                    {
                        timing = colorIndex / trackPulseLength;

                        floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);

                        if (colorIndex == trackPulseLength)
                        {
                            colorDir = -colorDir;
                        }
                        else if (colorIndex == 0)
                        {
                            colorDir = -colorDir;
                        }
                    }
                    else
                    {
                        timing = (colorIndex % trackPulseLength) / trackPulseLength;

                        floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                    }
                }
                else
                {
                    floor.SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                }
                colorIndex += colorDir;

                listFloor.Add(floor);
            }
        }

        int currIdx = 0;
        int currIdx2 = 0;
        int currIdx3 = 0;
        int currIdx4 = 0;
        int prevIndex = 0;
        float tmpBpm = Conductor.bpm;
        bool swiriCW = false;

        // JSON 파일을 라인 단위로 읽기
        foreach (var e in actions)
        {

            // eventType 필드 추출
            string eventType = e["eventType"]?.ToString();
            // floor 필드 추출
            string floor = e["floor"]?.ToString();
            if (eventType != null)
            {
                var floorIndex = int.Parse(floor);
                var floorObj = listFloor[floorIndex];
                switch (eventType)
                {
                    case "Twirl":
                        floorObj.events.Add(new Event_Twirl());
                        floorObj.SetSwiri(swiriCW);
                        while (currIdx2 < floorIndex)
                        {
                            listFloor[currIdx2].isCW = swiriCW;
                            currIdx2++;
                        }
                        swiriCW = !swiriCW;
                        break;
                }
            }

        }

        while (currIdx2 < listFloor.Count)
        {
            listFloor[currIdx2].isCW = swiriCW;
            currIdx2++;
        }

        for (int i = 0; i < listFloor.Count - 1; i++)
        {
            listFloor[i].nextFloor = listFloor[i + 1];
        }
        for (int i = 1; i < listFloor.Count; i++)
        {
            listFloor[i].prevFloor = listFloor[i - 1];
        }

        for (int i = 0; i < listFloor.Count; i++)
        {
            listFloor[i].maxTickAngle = listFloor[i].GetRotateAngle();
        }


        currentPos = Vector3.zero;

        Vector3[] posOffset = new Vector3[listChar.Length + 10];
        for (int i = 0; i < posOffset.Length; i++)
        {
            posOffset[i] = Vector3.zero;
        }
        foreach (var e in actions)
        {
            {
                // eventType 필드 추출
                string eventType = e["eventType"]?.ToString();
                // floor 필드 추출
                string floor = e["floor"]?.ToString();
                if (eventType != null)
                {
                    var floorIndex = int.Parse(floor);
                    var offset = e["positionOffset"]?.ToString();
                    if (eventType == "PositionTrack")
                    {
                        posOffset[floorIndex] = Vector3.zero;
                        if (offset != null)
                        {
                            JArray positionArray = JArray.Parse(offset);
                            if (positionArray.Count == 2)
                            {
                                float x = positionArray[0].ToObject<float>();
                                float y = positionArray[1].ToObject<float>();
                                posOffset[floorIndex] += new Vector3(x, y, 0);
                            }
                        }
                    }
                }
            }
        }

        pos.Clear();
        pos.Add(Vector3.zero);

        for (int i = 0; i < listChar.Length; i++)
        {
            var currentAngle = PathToAngle[listChar[i]];

            var diffVector = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;

            if (currentAngle == 360)
            {
                currentPos = pos[i - 1];
            }
            else
            {
                currentPos += (diffVector + posOffset[i + 1]) * 1.5f;
            }
            listFloor[i + 1].startPos = currentPos;
            listFloor[i + 1].transform.position = currentPos;

            if (posOffset[i + 1].magnitude > 0.1f)
            {
                listFloor[i].isMoveFloor = true;
            }
            pos.Add(currentPos);
        }


        colorIndex = 0;

        var camTargetPos = Vector3.zero;

        // JSON 파일을 라인 단위로 읽기
        foreach (var e in actions)
        {
            // eventType 필드 추출
            string eventType = e["eventType"]?.ToString();
            // floor 필드 추출
            string floor = e["floor"]?.ToString();
            if (eventType != null)
            {
                var floorIndex = int.Parse(floor);
                var floorObj = listFloor[floorIndex];
                switch (eventType)
                {
                    case "SetFilter":
                        string filter = e["filter"]?.ToString();
                        string enabled = e["enabled"]?.ToString();
                        string disableOthers = e["disableOthers"]?.ToString();
                        float Intensity = float.Parse(e["intensity"]?.ToString());
                        var filterEvent = new Event_SetFilter(filter, Intensity, enabled == "Enabled" || enabled == "True" ? true : false, disableOthers == "Enabled" || disableOthers == "True");

                        if (disableOthers == "Enabled" || disableOthers == "True")
                        {
                            floorObj.events_filterOff.Add(filterEvent);
                        }
                        else
                        {
                            floorObj.events.Add(filterEvent);
                        }
                        break;
                    case "SetSpeed":
                        SpeedType type = Enum.Parse<SpeedType>(e["speedType"]?.ToString());
                        float beatsPerMinute = float.Parse(e["beatsPerMinute"]?.ToString());
                        float bpmMultiplier = float.Parse(e["bpmMultiplier"]?.ToString());
                        floorObj.events.Add(new Event_SetSpeed(type, beatsPerMinute, bpmMultiplier));

                        while (currIdx < floorIndex)
                        {
                            listFloor[currIdx].currBpm = tmpBpm;
                            currIdx++;
                        }
                        prevIndex = floorIndex;
                        var tmpBpm2 = tmpBpm;
                        if (type == SpeedType.Bpm)
                        {
                            tmpBpm = beatsPerMinute;
                        }
                        else
                        {
                            tmpBpm *= bpmMultiplier;
                        }
                        floorObj.SetSpeedImage(tmpBpm2 < tmpBpm);
                        break;
                    case "MoveCamera":
                        {
                            Ease easeType = Enum.Parse<Ease>(e["ease"]?.ToString());
                            float duration = float.Parse(e["duration"]?.ToString());
                            float zoom = e["zoom"] == null ? 0.0f : float.Parse(e["zoom"]?.ToString());
                            float rotation = e["rotation"] == null ? 0.0f : float.Parse(e["rotation"]?.ToString());
                            string relativeTo = e["relativeTo"]?.ToString();
                            var offset = e["position"]?.ToString();
                            Vector3 position = floorObj.transform.position;
                            Vector3 pOffset = Vector3.zero;
                            if (offset != null)
                            {
                                JArray positionArray = JArray.Parse(offset);
                                if (positionArray.Count == 2)
                                {
                                    float x = positionArray[0].ToObject<float>();
                                    float y = positionArray[1].ToObject<float>();
                                    pOffset = new Vector3(x, y, 0) * 1.5f;
                                }
                            }
                            floorObj.events.Add(new Event_MoveCamera(duration, easeType, Mathf.Max(zoom, 0.1f), rotation, relativeTo, position, pOffset));
                            break;
                        }
                    case "Flash":
                        {
                            {
                                Ease easeType = Enum.Parse<Ease>(e["ease"]?.ToString() == null ? "Linear" : e["ease"]?.ToString());
                                float duration = float.Parse(e["duration"]?.ToString());
                                string startColor = e["startColor"]?.ToString();
                                string endColor = e["endColor"]?.ToString();
                                float startOpacity = float.Parse(e["startOpacity"]?.ToString());
                                float endOpacity = float.Parse(e["endOpacity"]?.ToString());
                                string plane = e["plane"]?.ToString();

                                floorObj.events.Add(new Event_Flash(plane, easeType, duration, Utils.HexToColorWithOpacity(startColor, startOpacity), Utils.HexToColorWithOpacity(endColor, endOpacity)));
                            }
                            break;
                        }
                    case "CustomBackground":
                        {
                            string bgImage = e["bgImage"]?.ToString();
                            string color = e["color"]?.ToString();
                            string imageColor = e["imageColor"]?.ToString();
                            var unscaledSize = e["unscaledSize"] == null ? 100f : float.Parse(e["unscaledSize"]?.ToString());

                            floorObj.events.Add(new Event_CustomBackground(bgImage, Utils.HexToColorWithOpacity(color, 100.0f), Utils.HexToColorWithOpacity(imageColor, 100.0f), unscaledSize));
                            break;
                        }
                    case "MoveTrack":
                        {
                            int rfloorIndex1 = floorIndex;

                            var startArray = JArray.Parse(e["startTile"]?.ToString());
                            if (startArray[1].ToString() == "Start")
                            {
                                rfloorIndex1 = 0;
                            }
                            else if (startArray[1].ToString() == "End")
                            {
                                rfloorIndex1 = listFloor.Count - 1;
                            }

                            int startIndex = rfloorIndex1 + startArray[0].ToObject<int>();


                            int rfloorIndex2 = floorIndex;

                            var endArray = JArray.Parse(e["endTile"]?.ToString());

                            if (endArray[1].ToString() == "Start")
                            {
                                rfloorIndex2 = 0;
                            }
                            else if (endArray[1].ToString() == "End")
                            {
                                rfloorIndex2 = listFloor.Count - 1;
                            }

                            int endIndex = rfloorIndex2 + endArray[0].ToObject<int>();

                            if (startIndex > endIndex)
                            {
                                int tmp = endIndex;
                                endIndex = startIndex;
                                startIndex = tmp;
                            }

                            float duration = float.Parse(e["duration"]?.ToString());

                            var offset = e["positionOffset"]?.ToString();
                            Vector3 offsetV = Vector3.zero;
                            if (offset != null)
                            {
                                JArray positionArray = JArray.Parse(offset);
                                if (positionArray.Count == 2)
                                {
                                    float x = positionArray[0].ToObject<float>();
                                    float y = positionArray[1].ToObject<float>();
                                    offsetV = new Vector3(x, y) * 1.5f;
                                }
                            }
                            var delay = float.Parse(e["angleOffset"]?.ToString());
                            var angleOffset = e["rotationOffset"] == null ? 0.0f : float.Parse(e["rotationOffset"]?.ToString());
                            var endOpacity = e["opacity"] == null ? 1.0f : float.Parse(e["opacity"]?.ToString()) / 100.0f;
                            Ease easeType = Enum.Parse<Ease>(e["ease"]?.ToString() == null ? "Linear" : e["ease"]?.ToString());
                            floorObj.events.Add(new Event_MoveTrack(startIndex, endIndex, duration, offsetV, angleOffset, endOpacity, delay, easeType));
                            break;
                        }
                    case "AnimateTrack":
                        {
                            while (currIdx3 < floorIndex)
                            {
                                var floorAppear = listFloor[currIdx3].AddComponent<FloorAppearEvent>();
                                floorAppear.floor = listFloor[currIdx3];
                                floorAppear.animType = trackAppearType;
                                floorAppear.delayBeat = TrackAppearDelay;
                                listFloor[currIdx3].appearEvent = floorAppear;

                                var floorDisappear = new FloorDisappearEvent();
                                floorDisappear.floor = listFloor[currIdx3];
                                floorDisappear.animType = trackDisappearType;
                                floorDisappear.delayBeat = TrackDisappearDelay;
                                listFloor[currIdx3].disappearEvent = floorDisappear;
                                FloorEventScheduler.floorDisappearEvents.Add(floorDisappear);
                                currIdx3++;
                            }
                            TrackAnimationType AppearAnimation = Enum.Parse<TrackAnimationType>(e["trackAnimation"]?.ToString());
                            TrackAnimationType2 DisappearAnimation = Enum.Parse<TrackAnimationType2>(e["trackDisappearAnimation"]?.ToString());
                            var beatsAhead = float.Parse(e["beatsAhead"]?.ToString());
                            var beatsBehind = float.Parse(e["beatsBehind"]?.ToString());
                            trackAppearType = AppearAnimation;
                            trackDisappearType = DisappearAnimation;
                            TrackAppearDelay = beatsAhead;
                            TrackDisappearDelay = beatsBehind;


                            break;
                        }
                    case "HallOfMirrors":
                        var str = e["enabled"]?.ToString();
                        floorObj.events.Add(new Event_HallOfMirrors(str == "Enabled" || str == "True"));
                        break;
                    case "ColorTrack":
                        int initIndex = currIdx4;
                        colorIndex = 0;
                        int dir = -1;
                        if (trackColorType != TrackColorType.Glow && trackColorType != TrackColorType.Rainbow)
                        {
                            dir = 1;
                        }

                        while (currIdx4 < floorIndex)
                        {
                            listFloor[currIdx4].floorRenderer.sprite = Resources.Load<Sprite>($"Sprite/tiles_editor_{TileTypeToString[trackStyle]}_{Floor.angleMap[listFloor[currIdx4].spriteAngle]}");


                            float timing = 0.0f;
                            if (trackColorPulse != TrackColorPulse.None)
                            {
                                if (trackColorType == TrackColorType.Glow)
                                {
                                    timing = colorIndex / trackPulseLength;

                                    listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);

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

                                    listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                                }
                            }
                            else
                            {
                                listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                            }
                            listFloor[currIdx4].setColorIndex = initIndex;
                            colorIndex += dir;
                            currIdx4++;
                        }
                        trackColorType = Enum.Parse<TrackColorType>(e["trackColorType"]?.ToString());
                        firstColor = Utils.HexToColorWithOpacity(e["trackColor"]?.ToString(), 100f);
                        secondColor = Utils.HexToColorWithOpacity(e["secondaryTrackColor"]?.ToString(), 100f);
                        trackColorAnimation = float.Parse(e["trackColorAnimDuration"]?.ToString());
                        trackPulseLength = int.Parse(e["trackPulseLength"].ToString());
                        trackStyle = e["trackStyle"].ToString();
                        trackColorPulse = Enum.Parse<TrackColorPulse>(e["trackColorPulse"]?.ToString());

                        /*
                        floorObj.events.Add(new Event_RecolorTrack(9999, floorIndex, floorIndex, trackColorType, firstColor, secondColor, trackColorAnimation, trackPulseLength, trackStyle, trackColorPulse));
                        currIdx4++;
                        */
                        break;
                    case "RecolorTrack":
                        {

                            int rfloorIndex1 = floorIndex;

                            var startArray = JArray.Parse(e["startTile"]?.ToString());
                            if (startArray[1].ToString() == "Start")
                            {
                                rfloorIndex1 = 0;
                            }
                            else if (startArray[1].ToString() == "End")
                            {
                                rfloorIndex1 = listFloor.Count - 1;
                            }

                            int startIndex = rfloorIndex1 + startArray[0].ToObject<int>();


                            int rfloorIndex2 = floorIndex;

                            var endArray = JArray.Parse(e["endTile"]?.ToString());

                            if (endArray[1].ToString() == "Start")
                            {
                                rfloorIndex2 = 0;
                            }
                            else if (endArray[1].ToString() == "End")
                            {
                                rfloorIndex2 = listFloor.Count - 1;
                            }

                            int endIndex = rfloorIndex2 + endArray[0].ToObject<int>();


                            if (startIndex > endIndex)
                            {
                                int tmp = endIndex;
                                endIndex = startIndex;
                                startIndex = tmp;
                            }

                            var R_trackColorType = Enum.Parse<TrackColorType>(e["trackColorType"]?.ToString());
                            var R_firstColor = Utils.HexToColorWithOpacity(e["trackColor"]?.ToString(), 100f);
                            var R_secondColor = Utils.HexToColorWithOpacity(e["secondaryTrackColor"]?.ToString(), 100f);
                            var R_trackColorAnimation = float.Parse(e["trackColorAnimDuration"]?.ToString());
                            var R_trackColorPulse = Enum.Parse<TrackColorPulse>(e["trackColorPulse"]?.ToString());
                            var R_trackPulseLength = int.Parse(e["trackPulseLength"].ToString());
                            var R_trackStyle = e["trackStyle"].ToString();

                            floorObj.events.Add(new Event_RecolorTrack(floorIndex, startIndex, endIndex, R_trackColorType, R_firstColor, R_secondColor, R_trackColorAnimation, R_trackPulseLength, R_trackStyle, R_trackColorPulse));
                        }
                        break;
                    case "Bloom":
                        {
                            string ea = e["enabled"]?.ToString();
                            float threshold = float.Parse(e["threshold"]?.ToString()) / 100.0f;
                            float intensity = float.Parse(e["intensity"]?.ToString()) / 100.0f;
                            string color = e["color"]?.ToString();
                            float duration = e["duration"] == null ? 0.0f : float.Parse(e["duration"]?.ToString());
                            Ease easeType = Enum.Parse<Ease>(e["ease"]?.ToString() == null ? "Linear" : e["ease"]?.ToString());

                            floorObj.events.Add(new Event_Bloom(ea, duration, threshold, intensity, Utils.HexToColorWithOpacity(color), easeType));
                        }
                        break;
                    case "SetHitsound":
                        {
                            var hitsound = Enum.Parse<HitSound>(e["hitsound"]?.ToString());
                            var hitsoundVolume = float.Parse(e["hitsoundVolume"]?.ToString());

                            floorObj.events.Add(new Event_SetHitSound(hitsound, hitsoundVolume));
                        }
                        break;
                }
            }
        }


        while (currIdx < listFloor.Count)
        {
            listFloor[currIdx].currBpm = tmpBpm;
            currIdx++;
        }


        while (currIdx3 < listFloor.Count)
        {
            var floorAppear = listFloor[currIdx3].AddComponent<FloorAppearEvent>();
            floorAppear.floor = listFloor[currIdx3];
            floorAppear.animType = trackAppearType;
            floorAppear.delayBeat = TrackAppearDelay;
            listFloor[currIdx3].appearEvent = floorAppear;

            var floorDisappear = new FloorDisappearEvent();
            floorDisappear.floor = listFloor[currIdx3];
            floorDisappear.animType = trackDisappearType;
            floorDisappear.delayBeat = TrackDisappearDelay;
            listFloor[currIdx3].disappearEvent = floorDisappear;

            FloorEventScheduler.floorDisappearEvents.Add(floorDisappear);

            currIdx3++;
        }
        {
            int initIndex = currIdx4;
            colorIndex = 0;
            int dir = -1;
            if (trackColorType != TrackColorType.Glow)
            {
                dir = 1;
            }

            while (currIdx4 < listFloor.Count)
            {
                listFloor[currIdx4].floorRenderer.sprite = Resources.Load<Sprite>($"Sprite/tiles_editor_{TileTypeToString[trackStyle]}_{Floor.angleMap[listFloor[currIdx4].spriteAngle]}");

                float timing = 0.0f;

                if (trackColorPulse != TrackColorPulse.None)
                {
                    if (trackColorType == TrackColorType.Glow)
                    {
                        timing = colorIndex / trackPulseLength;

                        listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);

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

                        listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                    }
                }
                else
                {
                    listFloor[currIdx4].SetColor(firstColor, secondColor, trackColorAnimation, timing, trackColorType, trackColorPulse);
                }

                listFloor[currIdx4].setColorIndex = initIndex;
                colorIndex += dir;
                currIdx4++;
            }
        }
        float startTime = 0.0f;
        for (int i = 0; i < listFloor.Count; i++)
        {
            listFloor[i].index = i;

            foreach (var e in listFloor[i].events)
            {
                e.bpm = listFloor[i].currBpm;
            }
            listFloor[i].startTime = startTime;

            if (listFloor[i].isMidSpin == true)
            {
                listFloor[i].startTime += ((listFloor[i - 1].maxTickAngle - listFloor[i - 2].maxTickAngle) / 180.0f) / (listFloor[i - 1].currBpm / 60.0f);
            }
            if (i == 0)
            {

            }
            else if (i + 1 < listFloor.Count && listFloor[i + 1].isMidSpin == true)
            {
            }
            else
            {
                startTime += (listFloor[i].maxTickAngle / 180.0f) / (listFloor[i].currBpm / 60.0f);
            }
            if (i >= 1)
            {
                listFloor[i - 1].endTime = listFloor[i].startTime;
            }
        }

        foreach (var e in listFloor)
        {
            e.GetComponent<FloorAppearEvent>().FloorSetup();
        }



        //CalculateFloorEntryTimes();
    }



}
