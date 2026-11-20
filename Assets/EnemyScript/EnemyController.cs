using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    public Slider hpSlider;

    private float maxHP = 100f;
    private float currentHP;
    private int power = 30;
    private int exp = 5;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP =maxHP;

        hpSlider.maxValue=maxHP;
        hpSlider.value = currentHP;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの視点から体力バーが正面に見えるようにする
        GameObject.Find("Canvas").transform.LookAt(GameObject.Find("Player").transform);
    }

    //ダメージを受けた時の処理
    public void TakeDamage(float damage)
    {
        currentHP-=damage;
        hpSlider.value = currentHP;

        //体力が０になったら
        if (hpSlider.value <= 0f)
        {
            // オブジェクトを非アクティブ化
            // this.gameObject.SetActive(false);
            this.GetComponent<MoveEnemy2>().walkSpeed = 0f;
            animator.SetFloat("Speed", 0.0f);
            animator.SetTrigger("Death");
        }
    }

    //倒れた後にフェードアウト
    public void OnAnimationEnd()
    {
        GetComponent<Fade>().FadeOut();
    }
}
