
using System.Collections.Generic;
using UnityEngine;

public class CubeDestroy : MonoBehaviour
{
    [Header("爆発エフェクト")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("ダメージ設定")]
    [SerializeField] private float explosionRadius = 4f;

    // 距離ごとのダメージ
    [SerializeField]
    private float[] damageByDistance =
    {
        30f,  // 0～1m
        20f,  // 1～2m
        10f,  // 2～3m
        5f    // 3～4m
    };

    [Header("プレイヤー吹っ飛ばし設定")]
    [SerializeField] private float playerKnockbackForce = 8f;
    [SerializeField] private float playerKnockbackUpward = 4f;

    private bool isExploded = false;

    // 矢が衝突したとき
    private void OnCollisionEnter(Collision collision)
    {
        if (isExploded) return;

        // ヒットボム専用の矢を取得
        HitBombArrowProjectile arrow =
            collision.gameObject.GetComponentInParent<HitBombArrowProjectile>();

        // ヒットボム以外なら処理しない
        if (arrow == null)
        {
            Debug.Log(
                "ヒットボム以外との衝突：" +
                collision.gameObject.name
            );
            return;
        }

        Debug.Log(
            "ヒットボムが命中：" +
            collision.gameObject.name
        );

        // 爆発処理
        Explode();

        // 命中した矢を削除
        Destroy(arrow.gameObject);
    }

    // 爆発処理
    private void Explode()
    {
        if (isExploded) return;

        isExploded = true;

        // 爆発エフェクトを生成
        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            Debug.Log("爆発エフェクトを生成しました！");
        }
        else
        {
            Debug.LogError(
                "Explosion Prefabが設定されていません！"
            );
        }

        // 周囲の敵とプレイヤーにダメージ・吹っ飛ばし
        DamageNearbyTargets();

        // Cubeを削除
        Destroy(gameObject);
    }

    // 爆発範囲内の敵とプレイヤーを取得
    private void DamageNearbyTargets()
    {
        // 爆発地点
        Vector3 explosionPosition = transform.position;

        // 爆発範囲内のColliderを取得
        Collider[] hitColliders = Physics.OverlapSphere(
            explosionPosition,
            explosionRadius
        );

        // 重複処理を防止
        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        HashSet<PlayerHealth> damagedPlayers =
            new HashSet<PlayerHealth>();

        foreach (Collider col in hitColliders)
        {
            // 敵を取得
            EnemyHealth enemy =
                col.GetComponentInParent<EnemyHealth>();

            // プレイヤーを取得
            PlayerHealth player =
                col.GetComponentInParent<PlayerHealth>();

            // 爆心地からの水平距離
            Vector2 explosionXZ = new Vector2(
                explosionPosition.x,
                explosionPosition.z
            );

            // =================================
            // 敵へのダメージ・吹っ飛ばし
            // =================================

            if (enemy != null && damagedEnemies.Add(enemy))
            {
                float distance = Vector2.Distance(
                    explosionXZ,
                    new Vector2(
                        enemy.transform.position.x,
                        enemy.transform.position.z
                    )
                );

                float damage = GetDistanceDamage(distance);

                if (damage > 0)
                {
                    // 敵の吹っ飛ばし処理
                    EnemyKnockback knockback =
                        enemy.GetComponent<EnemyKnockback>();

                    if (knockback != null)
                    {
                        knockback.ApplyKnockback(
                            explosionPosition
                        );

                        Debug.Log(
                            "敵を吹っ飛ばしました：" +
                            enemy.name
                        );
                    }
                    else
                    {
                        Debug.LogWarning(
                            "EnemyKnockbackが見つかりません：" +
                            enemy.name
                        );
                    }

                    // 敵のダメージ処理
                    enemy.TakeDamage(damage);

                    Debug.Log(
                        "敵への爆発ダメージ：" +
                        enemy.name +
                        " / 距離：" + distance.ToString("F2") +
                        "m / ダメージ：" + damage
                    );
                }
            }

            // =================================
            // プレイヤーへのダメージ・吹っ飛ばし
            // =================================

            if (player != null && damagedPlayers.Add(player))
            {
                float distance = Vector2.Distance(
                    explosionXZ,
                    new Vector2(
                        player.transform.position.x,
                        player.transform.position.z
                    )
                );

                float damage = GetDistanceDamage(distance);

                if (damage > 0)
                {
                    // プレイヤーのダメージ処理
                    player.TakeDamage(damage);

                    // プレイヤーの移動スクリプトを取得
                    PlayerController movement =
                        player.GetComponentInParent<
                            PlayerController>();

                    // プレイヤーの吹っ飛ばし処理
                    if (movement != null)
                    {
                        movement.ApplyKnockback(
                            explosionPosition,
                            playerKnockbackForce,
                            playerKnockbackUpward
                        );

                        Debug.Log(
                            "プレイヤーを吹っ飛ばしました！"
                        );
                    }
                    else
                    {
                        Debug.LogWarning(
                            "PlayerControllerが見つかりません！"
                        );
                    }

                    Debug.Log(
                        "プレイヤーへの爆発ダメージ：" +
                        player.name +
                        " / 距離：" + distance.ToString("F2") +
                        "m / ダメージ：" + damage
                    );
                }
            }
        }
    }

    // 距離に応じたダメージを取得
    private float GetDistanceDamage(float distance)
    {
        // 爆発範囲外
        if (distance >= explosionRadius)
        {
            return 0f;
        }

        // 距離に応じてダメージを決定
        int damageIndex = Mathf.FloorToInt(distance);

        // 配列の範囲外を防止
        if (damageIndex < 0 ||
            damageIndex >= damageByDistance.Length)
        {
            return 0f;
        }

        return damageByDistance[damageIndex];
    }

    // Sceneビューで爆発範囲を表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
