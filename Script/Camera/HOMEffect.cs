using UnityEngine;

public class HOMEffect : MonoBehaviour
{
    public Camera overlayCamera; // ��ø ī�޶�
    public GameObject quad; // �̹����� ǥ���� Quad ������Ʈ
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
        // homRenderTexture �ʱ�ȭ �˻�
        if (homRenderTexture != null)
        {
            homRenderTexture.Release();
        }

        // ���ο� RenderTexture ����
        homRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        homRenderTexture.name = $"HOM {Screen.width}x{Screen.height}";

        RenderTexture active = RenderTexture.active;
        RenderTexture.active = homRenderTexture;
        GL.Clear(clearDepth: true, clearColor: true, Color.clear);
        RenderTexture.active = active;


        // Quad�� ���� �ؽ�ó�� ����
        quad.GetComponent<MeshRenderer>().material.mainTexture = homRenderTexture;

        // ī�޶� ����Ʈ ũ�� ����
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
        // ȭ�� ũ�� ���� �� RenderTexture �����
        if (Screen.width != homRenderTexture.width || Screen.height != homRenderTexture.height)
        {
            homRenderTextureNeedsRecreation = true;
            SetupHOMRenderTexture();
        }
    }
}