using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerExp : MonoBehaviour
{
    public UpgradeSelector selector; // Inspectorで設定

    void Awake()
    {
        if (selector != null) onLevelUp.AddListener(selector.OnLevelUp);
    }

    public int level = 1;
    public int exp = 0;
    public UnityEvent onLevelUp;

    public int Required => 10 + 5 * level;

    public void AddExp(int amount)
    {
        exp += amount;
        while (exp >= Required)
        {
            exp -= Required;
            level++;
            onLevelUp.Invoke(); // 選択画面を開く処理をここにつなぐ
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) AddExp(10); // 仮
    }
}
