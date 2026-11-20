using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrowShooter : MonoBehaviour
{
    [Header("矢の設定")]
    [SerializeField] private GameObject fireArrowPrefab;
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
        Debug.Log("===== 矢の発射診断開始 =====");

        if (fireArrowPrefab == null || playerCamera == null)
        {
            Debug.LogError("矢またはCameraが登録されていません！");
            return;
        }

        Debug.Log("登録Prefab：" + fireArrowPrefab.name);

        Vector3 direction =
        playerCamera.transform.forward;

        Vector3 spawnPosition =
            playerCamera.transform.position
            + direction * 1.5f;

        GameObject arrow = Instantiate(
            fireArrowPrefab,
            spawnPosition,
            Quaternion.LookRotation(direction)
        );

        Debug.Log("矢を生成：" + arrow.name);

        // 子オブジェクトも含めて検索する
        FireArrowProjectile projectile =
            arrow.GetComponentInChildren<FireArrowProjectile>(true);

        if (projectile == null)
        {
            Debug.LogError(
            "FireArrowProjectileが取得できません！"
        );
            Destroy(arrow);
            return;
        }

        projectile.Launch(direction, shootSpeed);

        Debug.Log("Launch呼び出し完了！");
    }
}
