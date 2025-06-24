using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class FollowCamera : MonoBehaviour
{
    public Transform playerTf;

    private static FollowCamera _instance = null;

    public CameraFilterPack_TV_ARCADE filter_Aracde;
    public CameraFilterPack_Blizzard filter_Blizzard;
    public CameraFilterPack_Color_GrayScale filter_GrayScale;
    public CameraFilterPack_FX_Glitch1 filter_Glitch;
    public CameraFilterPack_TV_LED filter_LED;
    public CameraFilterPack_Real_VHS filter_VHS;
    public CameraFilterPack_Color_Chromatic_Aberration filter_Aberration;
    public CameraFilterPack_Color_Sepia filter_Sepia;
    public CameraFilterPack_Color_Invert filter_Invert;
    public CameraFilterPack_TV_CompressionFX filter_Compression;
    public CameraFilterPack_Drawing_Paper filter_Drawing;
    public CameraFilterPack_Drawing_Paper filter_Drawing_BG;
    public CameraFilterPack_Distortion_Wave_Horizontal filter_Waves;
    public CameraFilterPack_Pixel_Pixelisation filter_Pixelate;
    public CameraFilterPack_Atmosphere_Rain filter_Rain;
    public CameraFilterPack_Noise_TV filter_Static;
    public CameraFilterPack_Film_Grain filter_Grain;
    public CameraFilterPack_Distortion_FishEye filter_Fisheye;
    public CameraFilterPack_FX_Hexagon_Black filter_HexagonBlack;
    public CameraFilterPack_TV_Posterize filter_Postrize;
    public CameraFilterPack_Sharpen_Sharpen filter_Sharpen;
    public CameraFilterPack_Color_Contrast filter_Contrast;
    public CameraFilterPack_Pixelisation_OilPaint filter_OilPaint;
    public CameraFilterPack_Blur_Blurry filter_Blur;
    public CameraFilterPack_Blur_Focus filter_BlurFocus;
    public CameraFilterPack_Blur_GaussianBlur filter_GaussianBlur;
    public CameraFilterPack_AAA_WaterDrop filter_WaterDrop;
    public CameraFilterPack_Light_Water2 filter_LightWater;
    public CameraFilterPack_Atmosphere_Snow_8bits filter_pixelSnow;
    public CameraMotionBlur cameraMotionBlur;
    public CameraFilterPack_FX_8bits_gb filter_Handheld;
    public CameraFilterPack_TV_50 filter_50s;
    public CameraFilterPack_TV_80 filter_80s;
    public CameraFilterPack_Edge_Neon filter_Neon;
    public CameraFilterPack_FX_Funk filter_Funk;
    public BloomEffect bloom;

    public HOMEffect homEffect;
    public Transform bgParent;
    public MeshRenderer bg;
    public SpriteRenderer bg_Tex;
    public MeshRenderer fg;
    //public MeshRenderer bg;
    //public MeshRenderer fg;
    public static FollowCamera Instance => _instance;

    public int tweenIndex = 0;
    public Tweener zoomTween = null;
    public Tweener rotTween = null;
    public Tweener moveTween = null;
    public Tweener bgColorTween = null;
    public Tweener fgColorTween = null;
    public Tweener bloomColorTween = null;
    public float zoomSize = 1.0f;
    public float rotation = 0.0f;
    public bool IsTargetTile = false;
    public Vector3 targetPos;
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        fg.sortingLayerName = "FGFlash";
        fg.sortingOrder = 4;
    }

    public Conductor conductor;

    private float _camSpeed = 1.0f;

    public void Update()
    {
        _camSpeed = Conductor.bpm * conductor.song.pitch / 120f;
    }
    public void DirectFollow(string relative)
    {
        if (relative == "LastPosition")
        {
            if (IsTargetTile == false)
            {
                //transform.position = playerTf.transform.position + new Vector3(0, 0, -10);
            }
            else
            {
                transform.position = targetPos + new Vector3(0, 0, -10);
            }
        }
        else
        {
            if (IsTargetTile == true)
            {
                IsTargetTile = false;
                transform.position = playerTf.transform.position + new Vector3(0, 0, -10);
            }
        }
    }
    public void LateUpdate()
    {
        var fromPos = transform.position;
        var toPos = playerTf.transform.position;

        if (IsTargetTile)
        {
            toPos = targetPos;
        }
        else
        {
            transform.position = Vector3.Lerp(fromPos, toPos + new Vector3(0, 0, -10), _camSpeed * Time.deltaTime);
        }
        Camera.main.orthographicSize = zoomSize * 5.0f;
        Camera.main.transform.eulerAngles = new Vector3(0, 0, rotation);
        //bgParent.transform.localScale = Vector3.one * zoomSize;
        //customBackGround.transform.localScale = Vector3.one * zoomSize;
    }

    public void DisableAllFilters()
    {
        filter_Aracde.enabled = false;
        filter_Blizzard.enabled = false;
        filter_GrayScale.enabled = false;
        filter_Glitch.enabled = false;
        filter_LED.enabled = false;
        filter_VHS.enabled = false;
        filter_Aberration.enabled = false;
        filter_Compression.enabled = false;
        filter_Sepia.enabled = false;
        filter_Invert.enabled = false;
        filter_Drawing.enabled = false;
        filter_Drawing_BG.enabled = false;
        filter_Waves.enabled = false;
        filter_Pixelate.enabled = false;
        filter_Rain.enabled = false;
        filter_Static.enabled = false;
        filter_Grain.enabled = false;
        filter_Fisheye.enabled = false;
        filter_HexagonBlack.enabled = false;
        filter_Postrize.enabled = false;
        filter_Sharpen.enabled = false;
        filter_Contrast.enabled = false;
        filter_OilPaint.enabled = false;
        filter_Blur.enabled = false;
        filter_BlurFocus.enabled = false;
        filter_GaussianBlur.enabled = false;
        filter_WaterDrop.enabled = false;
        filter_LightWater.enabled = false;
        filter_pixelSnow.enabled = false;
        filter_Handheld.enabled = false;
        filter_50s.enabled = false;
        filter_80s.enabled = false;
        filter_Neon.enabled = false;
        filter_Funk.enabled = false;

        cameraMotionBlur.enabled = false;
    }
    public void SetFilter(string filter, float Intensity, bool enabled)
    {
        Intensity /= 100.0f;

        switch (filter)
        {
            case "Arcade":
                filter_Aracde.enabled = enabled;
                break;
            case "Blizzard":
                filter_Blizzard._Speed = Intensity;
                filter_Blizzard.enabled = enabled;
                break;
            case "Grayscale":
                filter_GrayScale._Fade = Intensity;
                filter_GrayScale.enabled = enabled;
                break;
            case "Glitch":
                filter_Glitch.enabled = enabled;
                break;
            case "LED":
                filter_LED.Size = Mathf.RoundToInt(5f * Intensity);
                filter_LED.enabled = enabled;
                break;
            case "VHS":
                filter_VHS.TRACKING = 0.212f * Intensity;
                filter_VHS.enabled = enabled;
                break;
            case "Aberration":
                filter_Aberration.Offset = Intensity * 0.04f - 0.02f;
                filter_Aberration.enabled = enabled;
                break;
            case "Compression":
                filter_Compression.Parasite = 3f * Intensity;
                filter_Compression.enabled = enabled;
                break;
            case "Sepia":
                filter_Sepia._Fade = Intensity;
                filter_Sepia.enabled = enabled;
                break;
            case "Invert":
                filter_Invert.enabled = enabled;
                break;
            case "Drawing":
                filter_Drawing.enabled = enabled;
                filter_Drawing_BG.enabled = enabled;
                filter_Drawing.Fade_With_Original = Mathf.Clamp(Intensity / 10.0f, 0f, 1f);
                //filter_Drawing_BG.Fade_With_Original = Mathf.Clamp(Intensity / 10.0f, 0f, 1f);
                break;
            case "Waves":
                filter_Waves.enabled = enabled;
                filter_Waves.WaveIntensity = 10f * Intensity;
                break;
            case "Pixelate":
                filter_Pixelate.enabled = enabled;
                filter_Pixelate._Pixelisation = 4f * Intensity;
                break;
            case "Rain":
                filter_Rain.enabled = enabled;
                filter_Rain.Intensity = 0.5f * Intensity;
                break;
            case "Static":
                filter_Static.enabled = enabled;
                filter_Static.Fade = Intensity;
                break;
            case "Grain":
                filter_Grain.enabled = enabled;
                filter_Grain.Value = 32f * Intensity;
                break;
            case "Fisheye":
                filter_Fisheye.enabled = enabled;
                filter_Fisheye.Distortion = Intensity;
                break;
            case "HexagonBlack":
                filter_HexagonBlack.enabled = enabled;
                filter_HexagonBlack.Value = Mathf.Max(0.2f, Intensity);
                break;
            case "Posterize":
                filter_Postrize.enabled = enabled;
                filter_Postrize.Posterize = Intensity * 20f;
                break;
            case "Sharpen":
                filter_Sharpen.enabled = enabled;
                filter_Sharpen.Value2 = Intensity;
                break;
            case "Contrast":
                filter_Contrast.enabled = enabled;
                filter_Contrast.Contrast = Intensity + 1f;
                break;
            case "Blur":
                filter_Blur.enabled = enabled;
                filter_Blur.Amount = Intensity * 2f;
                break;
            case "BlurFocus":
                filter_BlurFocus.enabled = enabled;
                filter_BlurFocus._Size = Mathf.Max(0.10001f, Intensity);
                break;
            case "GaussianBlur":
                filter_GaussianBlur.enabled = enabled;
                filter_GaussianBlur.Size = Mathf.Max(0.10001f, Intensity);
                break;
            case "WaterDrop":
                filter_WaterDrop.enabled = enabled;
                filter_WaterDrop.Distortion = Mathf.Lerp(64f, 8f, Mathf.Clamp(Intensity, 0f, 1f));
                break;
            case "LightWater":
                filter_LightWater.enabled = enabled;
                filter_LightWater.Intensity = Intensity * 2.4f;
                break;
            case "OilPaint":
                filter_OilPaint.enabled = enabled;
                filter_OilPaint.Value = Intensity;
                break;
            case "CameraMotionBlur":
                cameraMotionBlur.enabled = enabled;
                cameraMotionBlur.velocityScale = 0.375f * Intensity;
                break;
            case "PixelSnow":
                filter_pixelSnow.enabled = enabled;
                filter_pixelSnow.Threshold = 0.9f + 0.1f * Intensity;
                break;
            case "Handheld":
                filter_Handheld.enabled = enabled;
                break;
            case "Neon":
                filter_Neon.enabled = enabled;
                break;
            case "EightiesTV":
                filter_80s.enabled = enabled;
                break;
            case "FiftiesTV":
                filter_50s.enabled = enabled;
                break;
            case "Funk":
                filter_Funk.enabled = enabled;
                break;

        }
    }

    /*
    public RenderTexture renderTexture;
    public Material effectMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial != null)
        {
            Graphics.Blit(src, dest, effectMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
    */
}