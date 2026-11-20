using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAttack : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float attackRange = 10f;

    void Start()
    {
        Debug.Log("PanelAttackが起動しました！");
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("左クリックを検知しました！");
            Attack();
        }
    }

    void Attack()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Cameraが登録されていません！");
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
        );

        // Rayの方向をSceneビューに表示
        Debug.DrawRay(
            ray.origin,
            ray.direction * attackRange,
            Color.red,
            5f
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            Debug.Log("命中：" + hit.collider.name);

            PanelBlock panel =
                hit.collider.GetComponentInParent<PanelBlock>();

            if (panel != null)
            {
                Debug.Log("パネルブロックを検知！");
                panel.TakeDamage();
            }
            else
            {
                Debug.Log("パネルブロック以外に命中しました。");
            }
        }
        else
        {
            Debug.Log("Raycastが何にも当たっていません。");

            Debug.Log("Rayの開始位置：" + ray.origin);
            Debug.Log("Rayの方向：" + ray.direction);
        }
    }
}
