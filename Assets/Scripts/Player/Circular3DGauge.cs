
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Circular3DGauge : MonoBehaviour
{
    [Header("ゲージ設定")]
    [SerializeField] private SpriteRenderer gaugeSprite;

    [Header("ゲージの色")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullColor = Color.red;

    private Material runtimeMaterial;

    void Awake()
    {
        if (gaugeSprite == null)
            gaugeSprite = GetComponent<SpriteRenderer>();

        runtimeMaterial = new Material(gaugeSprite.material);
        gaugeSprite.material = runtimeMaterial;

        SetProgress(0f);
    }

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (runtimeMaterial == null)
            return;

        Color color = Color.Lerp(
            normalColor,
            fullColor,
            progress
        );

        gaugeSprite.color = color;

        runtimeMaterial.SetFloat("_Fill", progress);
    }

    void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}

