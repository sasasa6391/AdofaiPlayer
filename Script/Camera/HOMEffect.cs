using UnityEngine;

public class HOMEffect : MonoBehaviour
{
    public Camera overlayCamera; // 중첩 카메라
    public GameObject quad; // 이미지를 표시할 Quad 오브젝트
    public Camera bgCam;
    public Camera bgStaticCam;
    private RenderTexture homRenderTexture;
    private bool homRenderTextureNeedsRecreation = true;

    private void Start()
    {
        SetupHOMRenderTexture();
    }

    private void SetupHOMRenderTexture()
    {
        // homRenderTexture 초기화 검사
        if (homRenderTexture != null)
        {
            homRenderTexture.Release();
        }

        // 새로운 RenderTexture 생성
        homRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        homRenderTexture.name = $"HOM {Screen.width}x{Screen.height}";

        RenderTexture active = RenderTexture.active;
        RenderTexture.active = homRenderTexture;
        GL.Clear(clearDepth: true, clearColor: true, Color.clear);
        RenderTexture.active = active;


        // Quad의 메인 텍스처로 설정
        quad.GetComponent<MeshRenderer>().material.mainTexture = homRenderTexture;

        // 카메라 뷰포트 크기 설정
        float orthographicSize = overlayCamera.orthographicSize * 2f;
        float aspectRatio = orthographicSize * Screen.width / Screen.height;
        quad.transform.localScale = new Vector3(aspectRatio, orthographicSize, 1f);

    }

    public void SetupHomEffect(bool homEnabled)
    {
        RenderTexture targetTexture = (homEnabled ? homRenderTexture : null);
        Camera.main.targetTexture = targetTexture;
        bgStaticCam.targetTexture = targetTexture;
        bgCam.targetTexture = targetTexture;
        bgCam.clearFlags = homEnabled == true ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
        quad.SetActive(homEnabled);
        overlayCamera.gameObject.SetActive(homEnabled);
    }
    private void Update()
    {
        // 화면 크기 변경 시 RenderTexture 재생성
        if (Screen.width != homRenderTexture.width || Screen.height != homRenderTexture.height)
        {
            homRenderTextureNeedsRecreation = true;
            SetupHOMRenderTexture();
        }
    }
}