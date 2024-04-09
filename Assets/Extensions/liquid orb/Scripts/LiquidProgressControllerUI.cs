using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class LiquidProgressControllerUI : MonoBehaviour
{
    public CanvasRenderer LiquidRenderer;
    [Range(0, 1)] public float progress;
    public float animationDuration = .5f;

    // Update is called once per frame
    void Update()
    {
        if (LiquidRenderer.GetMaterial() != null && !Application.isPlaying)
        {
            LiquidRenderer.GetMaterial().SetFloat("_Progress", progress);
        }
    }

    public void Fill(float f)
    {
        if (LiquidRenderer.GetMaterial() != null)
        {
            LiquidRenderer.GetMaterial().DOFloat(f, "_Progress", animationDuration);
            progress = f;
        }
    }

}
