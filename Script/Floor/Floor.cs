using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Floor : MonoBehaviour
{


    // Static dictionary to hold the angle value-key pairs
    public static Dictionary<int, string> angleMap = new Dictionary<int, string>
    {
        { 0, "4" },
        { 15, "7" },
        { 30, "6" },
        { 45, "5" },
        { 60, "2" },
        { 75, "8" },
        { 90, "0" },
        { 105, "9" },
        { 120, "1" },
        { 135, "3" },
        { 150, "11" },
        { 165, "10" },
        { 180, "12" },
        { 195, "10" },
        { 210, "11" },
        { 225, "3" },
        { 240, "1" },
        { 255, "9" },
        { 270, "0" },
        { 285, "8" },
        { 300, "2" },
        { 315, "5" },
        { 330, "6" },
        { 345, "7" },
        { 360, "midspin" } // midspin value
    };


    public static List<Color> rainbowColors = new List<Color>
        {
            new Color32(255,0,0,0),
            new Color32(255,128,0,0),
            new Color32(255,255,0,0),
            new Color32(0,255,0,0),
            new Color32(0,0,255,0),
            new Color32(75,0,130,0),
            new Color32(128,0,128,0)
        };

    public int index;
    public Floor prevFloor;
    public Floor nextFloor;
    public FloorAppearEvent appearEvent;
    public FloorDisappearEvent disappearEvent;
    public GameObject Glow;
    public bool isMidSpin = false;
    public float InitAngle;
    public bool isMoveFloor = false;

    [Header("Game Properties")]
    public int seqID;

    public double marginScale = 1.0;
    public float radiusScale = 1f;
    public bool speedChange = false;
    public bool swiri = false;
    public float currBpm = 0.0f;
    public float startTime = 0.0f;
    public float endTime = 0.0f;
    public float maxTickAngle = 0.0f;
    public bool isCW = false;
    public int spriteAngle = 0;

    public Vector3 startPos;
    public float startAngle;

    public SpriteRenderer floorRenderer;
    public SpriteRenderer otherSp;
    public SpriteRenderer otherSp2;

    public bool dontChangeMySprite;

    public List<FloorEvent> events = new List<FloorEvent>();

    public List<FloorEvent> events_filterOff = new List<FloorEvent>();

    public Color firstColor;
    public Color secondColor;
    public float colorAnimation;
    public float colorCurrentRate = 0.0f;
    public float offsetColorRate = 0.0f;
    public TrackColorType trackColorType;
    public TrackColorPulse trackColorPulse;
    public bool IsFade = false;
    public float colorDir = 1.0f;
    public int setColorIndex = 0;
    private float minColorLerp = 0.0f;
    private bool firstColorStart = false;

    public GameObject onlyEditorCanvas;

    public void SetColor(Color firstColor, Color secondColor, float colorAnimation, float timing, TrackColorType trackColorType, TrackColorPulse colorPulse)
    {
        if (trackColorType == TrackColorType.Rainbow) colorDir = 1.0f;
        else colorDir = -1.0f;


        trackColorPulse = colorPulse;
        colorCurrentRate = minColorLerp;
        this.firstColor = firstColor;
        this.secondColor = secondColor;
        this.colorAnimation = colorAnimation;
        this.trackColorType = trackColorType;
        this.offsetColorRate = timing;
        floorRenderer.color = new Color(firstColor.r, firstColor.g, firstColor.b, floorRenderer.color.a);

    }

    public void Update()
    {
        if (firstColorStart == false && LevelManager.currentIndex + 250 > index)
        {
            firstColorStart = true;
            StartCoroutine(ColorFlicking());
        }
    }

    private void Awake()
    {
        Glow.gameObject.SetActive(false);
        Glow.transform.localScale = Vector3.one * 1.0f;
        Glow.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        floorRenderer = GetComponent<SpriteRenderer>();
        tmpV = Vector3.zero;
    }

    public void SetSortingOrder(int order)
    {
        floorRenderer.sortingOrder = order;
        if (otherSp != null) otherSp.sortingOrder = floorRenderer.sortingOrder;
        if (otherSp2 != null) otherSp2.sortingOrder = floorRenderer.sortingOrder;
    }
    public void MoveToBack()
    {
        if (disappearEvent != null) return;

        SetSortingOrder(-index);
        //gameObject.SetActive(false);
        //transform.DOScale(0.0f, 0.2f).SetEase(Ease.OutCubic);
        //SetSortingOrder(97);
    }

    public IEnumerator ColorFlicking()
    {
        //float rate = timing / colorAnimation;
        while (true)
        {
            if (IsFade == true)
            {
                yield return null;
            }

            if (index < LevelManager.currentIndex)
            {
                break;
            }

            switch (trackColorType)
            {
                case TrackColorType.Rainbow:
                    {

                        if (LevelManager.CurrentFloor.index + 250 < index)
                        {
                            break;
                        }

                        colorCurrentRate += Time.deltaTime * colorDir * 1.0f / colorAnimation;
                        if (offsetColorRate + colorCurrentRate >= 1.0f)
                        {
                            colorCurrentRate %= 1.0f;
                        }

                        var currentA = floorRenderer.color.a;

                        firstColor.a = currentA;

                        var finalColorRate = (colorCurrentRate + offsetColorRate) % 1.0f;
                        Color.RGBToHSV(firstColor, out var _, out var S, out var V);
                        floorRenderer.color = Color.HSVToRGB(finalColorRate, S, V).WithAlpha(currentA);
                        break;
                    }
                case TrackColorType.Glow:
                    {
                        colorCurrentRate += Time.deltaTime * colorDir * 1.0f / colorAnimation;

                        if (colorCurrentRate + offsetColorRate <= minColorLerp)
                        {
                            colorCurrentRate = -offsetColorRate;
                            colorDir = -colorDir;
                        }
                        else if (colorCurrentRate + offsetColorRate >= 1.0f)
                        {
                            colorCurrentRate = 1.0f - offsetColorRate;
                            colorDir = -colorDir;
                        }

                        if (LevelManager.CurrentFloor.index + 250 < index)
                        {
                            break;
                        }
                        var currentA = floorRenderer.color.a;

                        var finalColorRate = colorCurrentRate + offsetColorRate;
                        var newColor = Color.Lerp(firstColor, secondColor, finalColorRate);
                        newColor.a = currentA;
                        floorRenderer.color = newColor;
                        break;
                    }
                case TrackColorType.Blink:
                    {
                        colorCurrentRate += Time.deltaTime * colorDir * 1.0f / colorAnimation;

                        if (colorCurrentRate + offsetColorRate <= minColorLerp)
                        {
                            colorCurrentRate = 1.0f - offsetColorRate;
                        }
                        var currentA = floorRenderer.color.a;

                        if (LevelManager.CurrentFloor.index + 250 < index)
                        {
                            break;
                        }
                        var finalColorRate = colorCurrentRate + offsetColorRate;
                        var newColor = Color.Lerp(firstColor, secondColor, finalColorRate);
                        newColor.a = currentA;
                        floorRenderer.color = newColor;
                        break;
                    }
            }
            yield return null;
        }
    }


    public void SetSpeedImage(bool IsSpeedUp)
    {
        if (swiri)
        {
            otherSp2 = null;
            Destroy(transform.Find("swirl_red").gameObject);
        }
        string path = "snail";
        if (IsSpeedUp == true)
        {
            path = "rabbit";
        }
        speedChange = true;
        var tf = Instantiate(Resources.Load($"Floor/{path}"), transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, transform).GetComponent<Transform>();
        tf.localScale = Vector3.one * 0.8f;
        tf.GetComponent<SpriteRenderer>().sortingOrder = floorRenderer.sortingOrder + 1;
        otherSp = tf.GetComponent<SpriteRenderer>();
    }

    public void SetSwiri(bool IsCW)
    {
        if (speedChange == true) return;

        swiri = true;
        var tf = Instantiate(Resources.Load("Floor/swirl_red"), transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, transform).GetComponent<Transform>();
        tf.GetComponent<SpriteRenderer>().sortingOrder = floorRenderer.sortingOrder + 1;
        if (IsCW == false)
        {
            tf.eulerAngles = new Vector3(0, 180, 0);
        }
        tf.name = "swirl_red";
        otherSp = tf.GetComponent<SpriteRenderer>();
        otherSp2 = tf.GetChild(0).GetComponent<SpriteRenderer>();
        otherSp2.sortingOrder = floorRenderer.sortingOrder;
    }


    // 두 벡터 간의 각도를 구하는 메소드
    public float AngleBetweenVectors()
    {
        if (this == LevelManager.listFloor[0])
        {
            return 180 - LevelManager.PathToAngle[LevelManager.listChar[0]];
        }


        Vector3 a = (prevFloor.transform.position - transform.position).normalized;

        Vector3 b = (nextFloor.transform.position - transform.position).normalized;



        return Quaternion.FromToRotation(Vector3.forward, b - a).eulerAngles.z * (PlanetController.IsCW == false ? -1.0f : 1.0f);
    }


    public static Vector3 tmpV;

    public float GetRotateAngle()
    {
        if (this == LevelManager.listFloor[0])
        {
            return 180 - LevelManager.PathToAngle[LevelManager.listChar[0]];
        }

        if (nextFloor == null)
        {
            return 0.0f;
        }
        Vector3 a = (prevFloor.transform.position - transform.position).normalized;
        Vector3 a2 = Vector3.zero;

        if (nextFloor.isMidSpin == true)
        {
            if (prevFloor.isMidSpin == true)
            {
                a2 = tmpV;
            }
            else
            {
                a2 = (prevFloor.prevFloor.transform.position - prevFloor.transform.position).normalized;
                tmpV = a2;
            }
        }

        Vector3 b = (nextFloor.transform.position - transform.position).normalized;
        Vector3 b2 = Vector3.zero;
        if (nextFloor.isMidSpin == true)
        {
            if (nextFloor.nextFloor != null)
            {
                b2 = (nextFloor.nextFloor.transform.position - prevFloor.transform.position).normalized;
            }
            else
            {
                b2 = (nextFloor.transform.position - prevFloor.transform.position).normalized;
            }
        }

        float angle = Vector3.Angle(a, b);




        if (nextFloor.isMidSpin == true)

        {
            var a3 = (prevFloor.prevFloor.transform.position - prevFloor.transform.position).normalized;
            var b3 = (transform.position - prevFloor.transform.position).normalized;

            var angle2 = Vector3.Angle(a2, b2);
            var angle3 = Vector3.Angle(a3, b3);

            if ((isCW == false && swiri == false) || (isCW == true && swiri == true))
            {
                if (Vector3.Cross(a2, b2).z > 0)
                {
                    angle2 = 360.0f - angle2;
                }
                if (Vector3.Cross(a3, b3).z > 0)
                {
                    angle3 = 360.0f - angle3;
                }
            }
            else
            {
                if (Vector3.Cross(a2, b2).z < 0)
                {
                    angle2 = 360.0f - angle2;
                }
                if (Vector3.Cross(a3, b3).z < 0)
                {
                    angle3 = 360.0f - angle3;
                }
            }

            if (angle2 < angle3)
            {
                Debug.Log(index);
                return 360.0f + angle2;
            }
            return angle2;
        }

        else if (isCW == false)
        {
            if (Vector3.Cross(a, b).z > 0)
            {
                angle = 360.0f - angle;
            }
        }
        else
        {
            if (Vector3.Cross(a, b).z < 0)
            {
                angle = 360.0f - angle;
            }
        }

        if (angle < 0.1f)
        {
            return 359.99999f;
        }


        return angle;
    }

    public void SetMove(float duration, Vector3 offset, float angleOffset, float endOpacity, Ease ease)
    {
        /*
        if (appearEvent != null && appearEvent.animType != TrackAnimationType.None)
        {
            appearEvent.Play();
            return;
        }
        */

        var sPos = transform.position;
        var sAngle = transform.eulerAngles.z;
        var ePos = startPos + offset;
        var eAngle = startAngle + angleOffset;

        TweenOpacity(endOpacity, duration, ease);

        DOVirtual.Float(0.0f, 1.0f, duration, x =>
        {
            transform.position = Vector3.Lerp(sPos, ePos, x);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(sAngle, eAngle, x));
        }).SetEase(ease).SetLink(gameObject);
    }

    Tweener opaTweener = null;

    public void TweenOpacity(float endOpacity, float duration, Ease ease = Ease.Linear, bool IsAppearFade = false)
    {
        var startOpacity = floorRenderer.color.a;
        var glowCompo = Glow.GetComponent<SpriteRenderer>();
        opaTweener?.Kill();
        if (duration == 0.0f)
        {
            floorRenderer.color = new Color(floorRenderer.color.r, floorRenderer.color.g, floorRenderer.color.b, endOpacity);
            if (otherSp != null)
            {
                otherSp.color = new Color(otherSp.color.r, otherSp.color.g, otherSp.color.b, endOpacity);
            }
            if (otherSp2 != null)
            {
                otherSp2.color = new Color(otherSp2.color.r, otherSp2.color.g, otherSp2.color.b, endOpacity);
            }
            if (IsAppearFade != true)
                glowCompo.color = new Color(glowCompo.color.r, glowCompo.color.g, glowCompo.color.b, endOpacity);
            return;
        }

        opaTweener = DOVirtual.Float(0.0f, 1.0f, duration, x =>
        {
            floorRenderer.color = new Color(floorRenderer.color.r, floorRenderer.color.g, floorRenderer.color.b, Mathf.Lerp(startOpacity, endOpacity, x));
            if (otherSp != null)
            {
                otherSp.color = new Color(otherSp.color.r, otherSp.color.g, otherSp.color.b, Mathf.Lerp(startOpacity, endOpacity, x));
            }
            if (otherSp2 != null)
            {
                otherSp2.color = new Color(otherSp2.color.r, otherSp2.color.g, otherSp2.color.b, Mathf.Lerp(startOpacity, endOpacity, x));
            }
            if (IsAppearFade != true)
                glowCompo.color = new Color(glowCompo.color.r, glowCompo.color.g, glowCompo.color.b, Mathf.Lerp(startOpacity, endOpacity, x));
        }).SetEase(ease).SetLink(gameObject);
    }

    public void PlayFloorEvents()
    {
        if (index == 1123)
        {
            Debug.Log("OK");
        }
        foreach (var e in events_filterOff)
        {
            e.EventStart();
        }
        foreach (var e in events)
        {
            e.EventStart();
        }
    }

    public void OnRayCastEnter()
    {
        if (onlyEditorCanvas.activeSelf == false)
        {
            onlyEditorCanvas.gameObject.SetActive(true);
            SetEditorCanvas(false);
        }
    }
    public void OnRayCastExit()
    {
        if (onlyEditorCanvas.activeSelf == true)
        {
            onlyEditorCanvas.gameObject.SetActive(false);
        }
    }

    public void SetEditorCanvas(bool isShift)
    {
        onlyEditorCanvas.transform.GetChild(0).gameObject.SetActive(isShift == false);
        onlyEditorCanvas.transform.GetChild(1).gameObject.SetActive(isShift == true);
    }

}
