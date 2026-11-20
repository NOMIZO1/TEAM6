using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    private Renderer[] renderers;
    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();   
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        float duration = 2f;
        float timer = 0f;

        //各rendererのMaterialを取得
        Material[][] materials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;
        }

        //最初の色を保存
        Color[][]colors = new Color[materials.Length][];

        for (int i = 0;i < materials.Length; i++)
        {
            colors[i] = new Color[materials[i].Length];
            for(int j = 0; j < materials[i].Length; j++)
            {
                colors[i][j] = materials[i][j].color;
            }
        }

        while (timer < duration)
        {
            timer+=Time.deltaTime;

            float alpha=Mathf.Lerp(1f,0f,timer/duration);

            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < materials[i].Length; j++)
                {
                    Color color = colors[i][j];
                    color.a = alpha;

                    materials[i][j].color= color;
                }
            }

            yield return null;
        }
        //フェードアウト後破壊
        Destroy(gameObject);
    }
}
