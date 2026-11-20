using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBlock : MonoBehaviour
{
    [SerializeField] private DoorController door;

    private bool isActivated = false;

    public void TakeDamage()
    {
        if (isActivated) return;

        isActivated = true;

        // ブロックが反応したことを通知
        door.OpenDoor();

        // ブロックの色を変更
        GetComponent<Renderer>().material.color =
            Color.yellow;

        Debug.Log("パネルブロックが起動しました！");
    }
}
