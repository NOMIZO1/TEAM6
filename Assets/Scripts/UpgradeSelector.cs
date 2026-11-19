using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeDef
{
    public UpgradeType type;
    public string title;
    public string description;
    public int weight = 10;     // 重み(出やすさ)
    public int maxCount = 3;    // 取得回数の上限
    public string group = "";   // 同じgroupは同時に1つだけ(属性付与など)
}

public class UpgradeSelector : MonoBehaviour
{
    public PlayerStats stats;
    public List<UpgradeDef> all = new List<UpgradeDef>();
    public bool useDebugGUI = true; // UIが完成したらfalse

    // UI担当はこれを購読して、渡された候補をカードに表示する
    public event Action<List<UpgradeDef>> onShow;
    public event Action onClose;

    readonly Dictionary<UpgradeType, int> counts = new Dictionary<UpgradeType, int>();
    List<UpgradeDef> current;
    int pending;      // 連続レベルアップの待ち数
    float shownAt;

    public bool IsShowing => current != null;

    void Reset() { all = Defaults(); }

    static List<UpgradeDef> Defaults() => new List<UpgradeDef>
    {
        new UpgradeDef{ type=UpgradeType.Damage,      title="矢の攻撃力アップ", description="矢のダメージ +10%",            weight=10, maxCount=5 },
        new UpgradeDef{ type=UpgradeType.Range,       title="射程アップ",       description="矢の最大射程 +10%",            weight=10, maxCount=3 },
        new UpgradeDef{ type=UpgradeType.MaxHp,       title="最大HPアップ",     description="最大HP +20、同時に20回復",     weight=10, maxCount=5 },
        new UpgradeDef{ type=UpgradeType.MoveSpeed,   title="移動速度アップ",   description="移動速度 +10%",                weight=8,  maxCount=3 },
        new UpgradeDef{ type=UpgradeType.ChargeSpeed, title="チャージ速度アップ", description="弓を引き絞る速さ +10%",       weight=10, maxCount=3 },
        new UpgradeDef{ type=UpgradeType.Multishot,   title="連射",             description="矢が2本出る(1本の威力は70%)", weight=3,  maxCount=1 },
        new UpgradeDef{ type=UpgradeType.Fire,        title="火属性付与",       description="矢が敵を燃やす",               weight=3,  maxCount=1, group="element" },
        new UpgradeDef{ type=UpgradeType.Ice,         title="氷属性付与",       description="矢が敵を遅くする",             weight=3,  maxCount=1, group="element" },
    };

    int Count(UpgradeType t) => counts.TryGetValue(t, out var c) ? c : 0;

    // PlayerExp.onLevelUp からこれを呼ぶ
    public void OnLevelUp()
    {
        Debug.Log($"OnLevelUp 呼ばれた pending={pending}");
        pending++;
        if (!IsShowing) Show();
    }

    void Show()
    {
        var pool = all.FindAll(IsAvailable);
        if (pool.Count == 0) { Finish(); return; } // 候補なし

        current = new List<UpgradeDef>();
        while (current.Count < 3 && pool.Count > 0)
        {
            int total = 0;
            foreach (var u in pool) total += u.weight;
            int r = UnityEngine.Random.Range(0, total);
            foreach (var u in pool)
            {
                if (r < u.weight) { current.Add(u); pool.Remove(u); break; }
                r -= u.weight;
            }
        }

        Time.timeScale = 0f;
        shownAt = Time.unscaledTime;
        onShow?.Invoke(current);
    }

    bool IsAvailable(UpgradeDef u)
    {
        if (Count(u.type) >= u.maxCount) return false;
        if (u.group != "")
            foreach (var o in all)
                if (o != u && o.group == u.group && Count(o.type) > 0) return false;
        return true;
    }

    // UIのカードが押されたら、番号(0〜2)を渡す
    public void Select(int index)
    {
        if (!IsShowing || index < 0 || index >= current.Count) return;
        if (Time.unscaledTime - shownAt < 0.3f) return; // 誤操作の防止

        var u = current[index];
        counts[u.type] = Count(u.type) + 1;
        stats.Apply(u.type);

        current = null;
        pending--;
        if (pending > 0) Show(); else Finish();
    }

    void Finish()
    {
        current = null;
        pending = 0;
        Time.timeScale = 1f;
        onClose?.Invoke();
    }

    public int GetCount(UpgradeType t) => Count(t);

    public void ResetAll() // 死亡時
    {
        counts.Clear();
        stats.ResetStats();
        Finish();
    }

    void Update()
    {
        if (!IsShowing) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) Select(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Select(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Select(2);
    }

    // UI完成までの仮表示
    void OnGUI()
    {
        if (!useDebugGUI || !IsShowing) return;

        var list = current;

        float cardW = Screen.width * 0.26f;   // カードの幅
        float cardH = Screen.height * 0.60f;  // カードの高さ
        float gap = Screen.width * 0.03f;   // カードの間隔
        float totalW = cardW * list.Count + gap * (list.Count - 1);
        float startX = (Screen.width - totalW) / 2f;
        float y = (Screen.height - cardH) / 2f;

        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = Mathf.RoundToInt(Screen.height * 0.035f); // 文字の大きさ
        style.wordWrap = true;
        style.alignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < list.Count; i++)
        {
            var u = list[i];
            string label = $"[{i + 1}] {u.title}\n\n{u.description}\n\n取得 {Count(u.type)}/{u.maxCount}";
            var rect = new Rect(startX + i * (cardW + gap), y, cardW, cardH);

            if (GUI.Button(rect, label, style))
            {
                Select(i);
                return;
            }
        }
    }
}
