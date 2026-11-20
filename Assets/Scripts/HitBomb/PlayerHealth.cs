using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("プレイヤー体力設定")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("プレイヤーの体力：" + currentHealth);
    }

    // ダメージ処理
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log(
            "プレイヤーがダメージを受けました！ 残りHP：" +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 回復処理
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(
            currentHealth + amount,
            maxHealth
        );

        Debug.Log("プレイヤー回復！ 現在HP：" + currentHealth);
    }

    // 倒されたとき
    private void Die()
    {
        Debug.Log("プレイヤーが倒されました！");
    }
}
