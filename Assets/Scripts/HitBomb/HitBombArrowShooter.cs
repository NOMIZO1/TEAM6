using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBombArrowShooter : MonoBehaviour
{
    [Header("矢の設定")]
    [SerializeField] private GameObject hitBombArrowPrefab;
    [SerializeField] private Camera playerCamera;

    [Header("発射設定")]
    [SerializeField] private float shootSpeed = 20f;
    [SerializeField] private float shootInterval = 0.5f;

    private float nextShootTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextShootTime)
            {
                Shoot();
                nextShootTime = Time.time + shootInterval;
            }
        }
    }

    void Shoot()
    {
        Debug.Log("===== ヒットボム発射開始 =====");

        if (hitBombArrowPrefab == null || playerCamera == null)
        {
            Debug.LogError("矢またはCameraが登録されていません！");
            return;
        }

        Vector3 direction = playerCamera.transform.forward;

        // 矢の発射位置
        Vector3 spawnPosition =
            transform.position
            + direction * 1.5f;

        GameObject arrow = Instantiate(
            hitBombArrowPrefab,
            spawnPosition,
            Quaternion.LookRotation(direction)
        );

        Debug.Log("ヒットボムの矢を生成：" + arrow.name);

        // 専用スクリプトを取得
        HitBombArrowProjectile projectile =
            arrow.GetComponentInChildren<HitBombArrowProjectile>(true);

        if (projectile == null)
        {
            Debug.LogError(
                "HitBombArrowProjectileが取得できません！"
            );

            Destroy(arrow);
            return;
        }

        // 矢を発射
        projectile.Launch(direction, shootSpeed);

        Debug.Log("ヒットボム発射完了！");
    }
}
