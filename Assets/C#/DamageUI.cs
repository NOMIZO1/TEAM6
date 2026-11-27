using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    [SerializeField,Header("オブジェクトが消えるまでの時間")]
    private float DeleteTime = 1.5f; //オブジェクトが消えるまでの時間
    [SerializeField, Header("オブジェクトの移動範囲")]
    private float MoveRange = 50.0f; //オブジェクトの移動範囲
    [SerializeField, Header("透明度")]
    private float EndAlpha = 0.2f;   //透明度

    private float TimeCnt;
    private TextMesh NowText;
    // Start is called before the first frame update
    void Start()
    {
        TimeCnt = 0.0f;
        Destroy(this.gameObject,DeleteTime);
        NowText=gameObject.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Camera.main.transform);
        TimeCnt += Time.deltaTime;
        gameObject.transform.localPosition += 
            new Vector3(0, MoveRange / DeleteTime * Time.deltaTime, 0);
        float alpha=1.0f-(1.0f-EndAlpha)*(TimeCnt/DeleteTime);
        if (alpha <= 0.0f) alpha = 0.0f;
        NowText.color = 
            new Color(NowText.color.r, NowText.color.g, NowText.color.b, alpha);
    }
}
