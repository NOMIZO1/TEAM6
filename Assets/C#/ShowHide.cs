using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    [SerializeField, Header("続けますか？")]
    GameObject コンテテキスト;

    [SerializeField,Header("CanvasGroup")]
    CanvasGroup cg;

    [SerializeField]
    float 待ち時間 = 3f;

    [SerializeField]
    float 表示時間 = 0.8f;

    [SerializeField]
    float 開始サイズ = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        //最初は非表示
        コンテテキスト.SetActive(false);
        cg.alpha=0f;
        コンテテキスト.transform.localScale = Vector3.one;
        StartCoroutine(表示());
    }

    IEnumerator 表示()
    {
        yield return new WaitForSeconds(待ち時間);//何秒後にテキスト表示
        コンテテキスト.SetActive(true);
        float time = 0f;
     
        while (time<表示時間)
        {
            time += Time.deltaTime;
            float t=time/表示時間;
            cg.alpha=t;
            コンテテキスト.transform.localScale=
                Vector3.one*Mathf.Lerp(開始サイズ,1f,t);
            yield return null;
        }
        cg.alpha = 1f;
        コンテテキスト.transform.localScale = Vector3.one;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
