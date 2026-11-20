using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("体力設定")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("敵の体力：" + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log(
            "敵がダメージを受けました！ 残りHP：" +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敵を倒しました！");

        Destroy(gameObject);
    }
}
