using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType { Damage, Range, MaxHp, MoveSpeed, ChargeSpeed, Multishot, Fire, Ice }
public enum Element { None, Fire, Ice }

public class PlayerStats : MonoBehaviour
{
    public float damageMul = 1f;
    public float rangeMul = 1f;
    public float moveSpeedMul = 1f;
    public float chargeSpeedMul = 1f;
    public int maxHp = 100;
    public int hp = 100;
    public int arrowCount = 1;
    public float arrowDamageScale = 1f; // 連射中は0.7
    public Element element = Element.None;

    public bool showDebug = true;

    public void Apply(UpgradeType t)
    {
        switch (t)
        {
            case UpgradeType.Damage: damageMul += 0.1f; break;
            case UpgradeType.Range: rangeMul += 0.1f; break;
            case UpgradeType.MoveSpeed: moveSpeedMul += 0.1f; break;
            case UpgradeType.ChargeSpeed: chargeSpeedMul += 0.1f; break;
            case UpgradeType.MaxHp: maxHp += 20; hp = Mathf.Min(hp + 20, maxHp); break;
            case UpgradeType.Multishot: arrowCount = 2; arrowDamageScale = 0.7f; break;
            case UpgradeType.Fire: element = Element.Fire; break;
            case UpgradeType.Ice: element = Element.Ice; break;
        }
    }

    public void ResetStats() // 死亡時
    {
        damageMul = rangeMul = moveSpeedMul = chargeSpeedMul = 1f;
        maxHp = 100; hp = 100;
        arrowCount = 1; arrowDamageScale = 1f;
        element = Element.None;
    }

    void OnGUI()
    {
        if (!showDebug) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height * 0.025f) };
        string text =
            $"HP {hp}/{maxHp}\n" +
            $"攻撃力 x{damageMul:F1}\n" +
            $"射程 x{rangeMul:F1}\n" +
            $"移動速度 x{moveSpeedMul:F1}\n" +
            $"チャージ速度 x{chargeSpeedMul:F1}\n" +
            $"矢の本数 {arrowCount}  属性 {element}";
        GUI.Label(new Rect(Screen.width - 400, 20, 380, 300), text, style);
    }
}
