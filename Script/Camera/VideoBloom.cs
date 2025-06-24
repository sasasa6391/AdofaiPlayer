using UnityEngine;

[ExecuteInEditMode]
public class BloomEffect : MonoBehaviour
{
    public Material bloomMaterial;
    public float threshold = 0.1f;
    public float intensity = 1.0f;

    public void SetBloomParam(float threshold, float intensity)
    {
        bloomMaterial.SetFloat("_Threshold", threshold);
        bloomMaterial.SetFloat("_Intensity", intensity);
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (bloomMaterial != null)
        {
            // Apply bloom effect
            Graphics.Blit(src, dest, bloomMaterial);
        }
        else
        {
            // Just copy the source to the destination
            Graphics.Blit(src, dest);
        }
    }
}