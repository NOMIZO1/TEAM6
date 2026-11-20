using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnableRoot : MonoBehaviour
{
    [Header("燃焼設定")]
    [SerializeField] private float burnDuration = 5f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float damageAmount = 5f;
    [SerializeField] private float damageRadius = 4f;

    [Header("エフェクト")]
    [SerializeField] private Renderer rootRenderer;
    [SerializeField] private GameObject fireEffect;

    private bool isBurning = false;

    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        if (rootRenderer != null)
        {
            originalColor = rootRenderer.material.color;
        }

        if (fireEffect != null)
        {
            fireEffect.SetActive(false);
        }
    }

    public void Ignite()
    {
        if (isBurning) return;

        isBurning = true;

        Debug.Log("木の根が燃え始めました！");

        // 木の根の色を変更
        if (rootRenderer != null)
        {
            rootRenderer.material.color = Color.red;
        }

        // 炎エフェクトを表示
        if (fireEffect != null)
        {
            fireEffect.SetActive(true);

            ParticleSystem particle =
            fireEffect.GetComponent<ParticleSystem>();

            // 子オブジェクトも含めて検索
            if (particle == null)
            {
                particle =
                    fireEffect.GetComponentInChildren<ParticleSystem>(true);
            }

            if (particle != null)
            {
                particle.Clear(true);
                particle.Play(true);

                Debug.Log("RootFireEffectの再生を開始しました！");
            }
            else
            {
                Debug.LogError("Particle Systemが見つかりません！");
            }
        }

        StartCoroutine(BurningProcess());
    }

    IEnumerator BurningProcess()
    {
        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {
            // 周囲の敵にダメージ
            DamageNearbyEnemies();

            yield return new WaitForSeconds(damageInterval);

            elapsedTime += damageInterval;
        }

        // 燃焼終了
        isBurning = false;

        if (rootRenderer != null)
        {
            rootRenderer.material.color = originalColor;
        }

        if (fireEffect != null)
        {
            ParticleSystem particle =
            fireEffect.GetComponent<ParticleSystem>();

            if (particle == null)
            {
                particle =
                    fireEffect.GetComponentInChildren<ParticleSystem>(true);
            }

            if (particle != null)
            {
                particle.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }

            fireEffect.SetActive(false);
        }

        Debug.Log("木の根の燃焼が終了しました。");
    }

    void DamageNearbyEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            damageRadius
        );

        // 同じ敵に複数回ダメージを与えない
        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider col in enemies)
        {
            EnemyHealth enemy =
                col.GetComponentInParent<EnemyHealth>();

            if (enemy != null && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(damageAmount);
            }
        }
    }

    // Sceneビューで炎上範囲を表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            damageRadius
        );
    }
}
