using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    public TextMeshProUGUI text_Title;
    public TextMeshProUGUI text_Desc;
    public List<GameObject> group_Difficult;
    public TextMeshProUGUI text_Author;
    public TextMeshProUGUI text_Artist;
    [SerializeField] MainScrollView scrollView = default;
    public List<SongData> songlist;
    public string fileName;
    //[SerializeField] Text selectedItemInfo = default;
    public static Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    public static bool Init = false;
    public static int currentSelected = 0;


    void Start()
    {
        // VSync 비활성화
        QualitySettings.vSyncCount = 0;
        scrollView.OnSelectionChanged(OnSelectionChanged);

        songlist = new List<SongData>();
        songlist.Add(new SongData("<color=#00A2FF>R</color>", "<color=#00ADFA>Plum</color>", "KanoKaz & NumbEr07", "The Journey Hidden in a Mysterious Melody", 8, "R", "BG5", "R"));
        songlist.Add(new SongData("Maelstrom", "Plum", "RedCRP", "맞습니다. 전설의 맵이 더 멋지고, 더 엄격한 판정으로 돌아왔습니다. 재밌게 즐겨주세요! :", 8, "Maelstrom", "Maelstrom", "Maelstrom"));
        songlist.Add(new SongData("Once Again", "Cansol", "NumbEr07", "", 8, "OnceOri", "onceagain2", "Once again-11"));
        songlist.Add(new SongData("Tempest", "Plum", "고사리 & 다피쿤 & -K & Goyeetroll", "아주 강한 폭풍우가 몰아친다!", 10, "Tempest", "a[1]", "Tempest"));
        songlist.Add(new SongData("</size><color=#AA0000>†</color> <color=#FFFFFF>Triple</color> <size=150><color=#FF7770>Cross</color></size>", "<color=#00FFFF>HyuN</color> <size=-1>", "헤르니", "Three crosses are bound to open the purple sea...", 8, "TripleCross", "Ocean", "Triple Cross"));
        songlist.Add(new SongData("<color=#00ffff>The Lost Aria</color>", "Plum", "Etell", "Plum님의 The Lost Aria로 만든 힐링 커스텀 레벨입니다!", 6, "TheLostAria", "포털 이미지", "The Lost Aria"));
        songlist.Add(new SongData("</size><color=#220022>Blackmagik</color> <color=#110011>B</color><color=#220022>l</color><color=#330033>a</color><color=#440044>z</color><color=#550055>i</color><color=#660066>n</color><color=#770077>g</color>", "<color=#770077>C</color><color=#660066>a</color><color=#550055>m</color><color=#440044>e</color><color=#330033>l</color><color=#220022>l</color><color=#110011>i</color><color=#000000>a<size=-10>", "헤르니", "더이상 자세한 설명을 생략하겠다.", 10, "BlackMagikBlazing", "a2312567325_16", "Blackmagik Blazing"));
        songlist.Add(new SongData("<color=#6B6B6B>M</color><color=#767676>u</color><color=#828282>l</color><color=#8E8E8E>t</color><color=#9A9A9A>i</color><color=#A6A6A6>_</color><color=#B2B2B2>a</color><color=#BEBEBE>r</color><color=#CACACA>m</color>", "Frums", "Zagon", "#3", 10, "Multiarm", "3434", "multi_arm"));
        songlist.Add(new SongData("We Want To Run", "Frums", "Nephrolepis & 네온빌런", "장식과 필터를 제대로 활용해보는 맵중 하나라고 생각되네요", 10, "WWR", "20220312164046_1", "01. Frums - We Want To Run"));
        songlist.Add(new SongData("Hello (BPM) 2021", "Camellia", "Nephrolepis & 2seehyun", "제 마지막 맵입니다, 재밌게 플레이해주세요!", 10, "Hello2021", "portal_image", "Hello_BPM_2021_Mix"));
        songlist.Add(new SongData("Hello (BPM) 2022", "Camellia", "Alpha & Lemoni", "레벨에 대해 말해보세요!", 10, "Hello2022", "deco_magicshape_copied", "かめりあ_Camellia_-Hello-_BPM_-2022"));

        if (Init == false)
        {
            Init = true;
            LoadAllBGM(songlist);
        }

        scrollView.UpdateData(songlist.ToArray());
        scrollView.SelectCell(currentSelected);

    }
    private Coroutine playSong = null;

    void OnSelectionChanged(int index)
    {
        currentSelected = index;
        text_Title.text = songlist[index].title;
        text_Desc.text = songlist[index].desc;
        text_Author.text = $"Author : {songlist[index].author}";
        text_Artist.text = $"Artist : {songlist[index].artist}";
        foreach (var e in group_Difficult)
        {
            e.SetActive(false);
        }
        for (int i = 0; i < songlist[index].difficulty; i++)
        {
            group_Difficult[i].SetActive(true);
        }
        fileName = songlist[index].fileName;

        LoadBGM(fileName, songlist[index].songFileName, 0.5f);
        //selectedItemInfo.text = $"Selected item info: index {index}";
    }

    void LoadAllBGM(List<SongData> datas)
    {
        foreach (var e in datas)
        {
            bgmDict.Add(e.songFileName, Resources.Load<AudioClip>($"{e.fileName}/{e.songFileName}"));
        }
    }

    void LoadBGM(string fileName, string songName, float volume)
    {
        StopLoop();

        //AudioClip clip = Resources.Load<AudioClip>($"{fileName}/{songName}");
        // AudioSource에 클립을 설정하고 재생합니다.
        audioSource.clip = bgmDict[songName];
        audioSource.volume = volume;
        playSong = StartCoroutine(PlayPartialAudioLoop());
    }

    public void StartScene()
    {
        SceneManager.LoadScene(fileName);
    }

    public AudioSource audioSource; // 재생할 오디오 소스

    public float startTime = 10f; // 시작 지점 (초)
    public float duration = 20f; // 재생할 시간 (초)

    private bool isPlaying = false;

    private IEnumerator PlayPartialAudioLoop()
    {
        isPlaying = true;

        while (isPlaying)
        {
            audioSource.time = startTime;
            audioSource.Play();
            yield return new WaitForSeconds(duration);

            audioSource.time = startTime;
        }
    }

    public void StopLoop()
    {
        isPlaying = false;
        if (playSong != null)
        {
            StopCoroutine(playSong);
        }
        audioSource.Stop();
    }

}
