using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ADOFAIData
{
    public string pathData;
    public Settings settings;
    public List<object> actions;
}

[Serializable]
public class Settings
{
    public int version;
    public string artist;
    public string specialArtistType;
    public string artistPermission;
    public string song;
    public string author;
    public string separateCountdownTime;
    public string previewImage;
    public string previewIcon;
    public string previewIconColor;
    public int previewSongStart;
    public int previewSongDuration;
    public string seizureWarning;
    public string levelDesc;
    public string levelTags;
    public string artistLinks;
    public int difficulty;
    public string songFilename;
    public float bpm;
    public int volume;
    public int offset;
    public int pitch;
    public string hitsound;
    public int hitsoundVolume;
    public int countdownTicks;
    public string trackColorType;
    public string trackColor;
    public string secondaryTrackColor;
    public int trackColorAnimDuration;
    public string trackColorPulse;
    public int trackPulseLength;
    public string trackStyle;
    public string trackAnimation;
    public int beatsAhead;
    public string trackDisappearAnimation;
    public int beatsBehind;
    public string backgroundColor;
    public string bgImage;
    public string bgImageColor;
    public List<int> parallax;
    public string bgDisplayMode;
    public string lockRot;
    public string loopBG;
    public int unscaledSize;
    public string relativeTo;
    public List<int> position;
    public int rotation;
    public int zoom;
    public string bgVideo;
    public string loopVideo;
    public int vidOffset;
    public string floorIconOutlines;
    public string stickToFloors;
    public string planetEase;
    public int planetEaseParts;
}

public class FileManager : MonoBehaviour
{
    public static FileManager _instance;
    public static FileManager Instance => _instance;

    public string fileName;
    public LevelManager levelManager;
    public AudioType audioType;
    public Conductor conductor;

    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // ���� ��� ����
        string filePath = Path.Combine(Application.streamingAssetsPath, $"{fileName}/{fileName}.adofai");

        // ���� ���� ���� Ȯ�� �� �б�
        if (File.Exists(filePath))
        {
            // ������ ��� �ؽ�Ʈ �б�
            string fileContent = File.ReadAllText(filePath);

            // JSON �����͸� JObject�� �Ľ�
            JObject jsonObject = JObject.Parse(FixJsonFormat(fileContent));
            var settings = jsonObject["settings"];
            Conductor.bpm = float.Parse(settings["bpm"].ToString());
            Conductor.offset = float.Parse(settings["offset"].ToString());// - 30f;
            FollowCamera.Instance.zoomSize = float.Parse(settings["zoom"].ToString()) / 100.0f;
            if (settings["relativeTo"]?.ToString() == "Tile")
            {
                FollowCamera.Instance.IsTargetTile = true;


                var offset = settings["position"]?.ToString();
                if (offset != null)
                {
                    JArray positionArray = JArray.Parse(offset);
                    if (positionArray.Count == 2)
                    {
                        float x = positionArray[0].ToObject<float>();
                        float y = positionArray[1].ToObject<float>();
                        FollowCamera.Instance.transform.position = new Vector3(x, y) * 1.5f + new Vector3(0, 0, -10);
                    }
                }
            }
            if (jsonObject["angleData"] != null)
            {
                levelManager.LoadLevel(settings, (JArray)jsonObject["angleData"], (JArray)jsonObject["actions"]);
            }
            else
            {
                levelManager.LoadLevel(settings, jsonObject["pathData"].ToString(), (JArray)jsonObject["actions"]);
            }
            LoadBGM(settings["songFilename"].ToString(), float.Parse(settings["volume"]?.ToString()) / 200.0f);

            PlanetController.startOK = true;

        }
        else
        {
            Debug.LogError("������ ã�� �� �����ϴ�: " + filePath);
        }

    }

    void LoadBGM(string songName, float volume)
    {
        int lastIndex = 0;
        for (int i = songName.Length - 1; i >= 0; i--)
        {
            if (songName[i] == '.')
            {
                lastIndex = i;
                break;
            }
        }
        conductor.song.clip = MainUI.bgmDict[songName.Substring(0, lastIndex)];
        conductor.song.volume = volume;
    }
    // �߸��� JSON ������ �����ϴ� �Լ�
    private string FixJsonFormat(string json)
    {
        // �ߺ��Ǵ� �޸� ����
        json = Regex.Replace(json, ",,", ",");

        // JSON ���ڿ����� ���ʿ��� ���� ����
        json = json.Trim();

        return json;
    }
}
