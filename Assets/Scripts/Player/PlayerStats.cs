using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType { Damage, Range, MaxHp, MoveSpeed, ChargeSpeed, Multishot, Fire, Ice }
public enum Element { None, Fire, Ice }

public class PlayerStats : MonoBehaviour
{
    [Header("基本値(ここを調整する)")]
    public int baseMaxHp = 100;
    public float baseDamage = 20f;
    public float baseMoveSpeed = 5f;
    public float baseMinRange = 10f;
    public float baseMaxRange = 25f;
    public float invincibleTime = 1f; // 被弾後の無敵時間

    [Header("強化で変わる値")]
    public float damageMul = 1f;
    public float rangeMul = 1f;
    public float moveSpeedMul = 1f;
    public float chargeSpeedMul = 1f;
    public int maxHpBonus = 0;
    public int arrowCount = 1;
    public float arrowDamageScale = 1f; // 連射中は0.7
    public Element element = Element.None;

    public int hp;

    // プレイヤーのコードは、これらを読む
    public int MaxHp => baseMaxHp + maxHpBonus;
    public float Damage => baseDamage * damageMul * arrowDamageScale;
    public float MoveSpeed => baseMoveSpeed * moveSpeedMul;
    public float MinRange => baseMinRange * rangeMul;
    public float MaxRange => baseMaxRange * rangeMul;
    public float ChargeSpeed => chargeSpeedMul; // charge += Time.deltaTime * ChargeSpeed

    public event Action onChanged; // HPバー
    public event Action onDeath;   // ゲームオーバー画面

    float lastHitTime = -999f;

    void Awake() { hp = MaxHp; }

    public void Apply(UpgradeType t, float amount)
    {
        switch (t)
        {
            case UpgradeType.Damage: damageMul += amount; break;
            case UpgradeType.Range: rangeMul += amount; break;
            case UpgradeType.MoveSpeed: moveSpeedMul += amount; break;
            case UpgradeType.ChargeSpeed: chargeSpeedMul += amount; break;
            case UpgradeType.MaxHp: maxHpBonus += (int)amount; hp = Mathf.Min(hp + (int)amount, MaxHp); break;
            case UpgradeType.Multishot: arrowCount = 2; arrowDamageScale = amount; break;
            case UpgradeType.Fire: element = Element.Fire; break;
            case UpgradeType.Ice: element = Element.Ice; break;
        }
        onChanged?.Invoke();
    }

    public void TakeDamage(int dmg)
    {
        if (hp <= 0) return;
        if (Time.time - lastHitTime < invincibleTime) return; // 無敵中
        lastHitTime = Time.time;

        hp = Mathf.Max(hp - dmg, 0);
        onChanged?.Invoke();
        if (hp == 0) onDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, MaxHp);
        onChanged?.Invoke();
    }

    public void ResetStats() // 死亡時
    {
        damageMul = rangeMul = moveSpeedMul = chargeSpeedMul = 1f;
        maxHpBonus = 0;
        arrowCount = 1;
        arrowDamageScale = 1f;
        element = Element.None;
        hp = MaxHp;
        onChanged?.Invoke();
    }

    // 確認用の表示(完成したら消す)
    public bool showDebug = true;
    void OnGUI()
    {
        if (!showDebug) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height * 0.025f) };
        string text =
            $"HP {hp}/{MaxHp}\n攻撃力 {Damage:F0}\n射程 {MinRange:F0}〜{MaxRange:F0}\n" +
            $"移動速度 {MoveSpeed:F1}\nチャージ速度 x{chargeSpeedMul:F1}\n矢の本数 {arrowCount}  属性 {element}";
        GUI.Label(new Rect(Screen.width - 400, 20, 380, 300), text, style);
    }
}

