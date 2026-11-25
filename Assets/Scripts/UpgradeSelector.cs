using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeDef
{
    public UpgradeType type;
    public string title;
    public string description;   // 例: "矢のダメージ +{0}%" ({0}に数値が入る)
    public float amount = 0.1f;  // 効果の大きさ(攻撃力なら 0.1 = 10%、最大HPなら 20)
    public bool percent = true;  // trueなら表示は amount×100
    public int weight = 10;      // 重み(出やすさ)
    public int maxCount = 3;     // 取得回数の上限
    public string group = "";    // 同じgroupは同時に1つだけ(属性付与など)

    public string DescText
    {
        get
        {
            float v = percent ? amount * 100f : amount;
            return description.Replace("{0}", v.ToString("0"));
        }
    }
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

    // カーソルの状態を、選択画面を出す前に保存しておく
    CursorLockMode prevLock;
    bool prevVisible;
    bool cursorSaved;

    public bool IsShowing => current != null;

    void Reset() { all = Defaults(); }

    static List<UpgradeDef> Defaults() => new List<UpgradeDef>
    {
        new UpgradeDef{ type=UpgradeType.Damage,      title="矢の攻撃力アップ", description="矢のダメージ +{0}%",            amount=0.3f, weight=10, maxCount=5 },
        new UpgradeDef{ type=UpgradeType.Range,       title="射程アップ",       description="矢の最大射程 +{0}%",            amount=0.1f, weight=10, maxCount=3 },
        new UpgradeDef{ type=UpgradeType.MaxHp,       title="最大HPアップ",     description="最大HP +{0}、同時に{0}回復",    amount=20f, percent=false, weight=10, maxCount=5 },
        new UpgradeDef{ type=UpgradeType.MoveSpeed,   title="移動速度アップ",   description="移動速度 +{0}%",                amount=5f, weight=8,  maxCount=3 },
        new UpgradeDef{ type=UpgradeType.ChargeSpeed, title="チャージ速度アップ", description="弓を引き絞る速さ +{0}%",       amount=0.1f, weight=10, maxCount=3 },
        new UpgradeDef{ type=UpgradeType.Multishot,   title="連射",             description="矢が2本出る(1本の威力は{0}%)", amount=0.7f, weight=3,  maxCount=1 },
        new UpgradeDef{ type=UpgradeType.Fire,        title="火属性付与",       description="矢が敵を燃やす",               weight=3,  maxCount=1, group="element" },
        new UpgradeDef{ type=UpgradeType.Ice,         title="氷属性付与",       description="矢が敵を遅くする",             weight=3,  maxCount=1, group="element" },
    };

    int Count(UpgradeType t) => counts.TryGetValue(t, out var c) ? c : 0;

    // PlayerExp.onLevelUp からこれを呼ぶ
    public void OnLevelUp()
    {
        // 選択画面を出す前の、カーソルの状態を保存する(最初の1回だけ)
        if (pending == 0 && !IsShowing)
        {
            prevLock = Cursor.lockState;
            prevVisible = Cursor.visible;
            cursorSaved = true;
        }

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        stats.Apply(u.type, u.amount);

        current = null;
        pending--;
        if (pending > 0) Show(); else Finish();
    }

    void Finish()
    {
        current = null;
        pending = 0;
        Time.timeScale = 1f;

        if (cursorSaved) // 保存したときだけ、カーソルを元に戻す
        {
            Cursor.lockState = prevLock;
            Cursor.visible = prevVisible;
            cursorSaved = false;
        }

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

        float cardW = Screen.width * 0.26f;
        float cardH = Screen.height * 0.60f;
        float gap = Screen.width * 0.03f;
        float totalW = cardW * list.Count + gap * (list.Count - 1);
        float startX = (Screen.width - totalW) / 2f;
        float y = (Screen.height - cardH) / 2f;

        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = Mathf.RoundToInt(Screen.height * 0.035f);
        style.wordWrap = true;
        style.alignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < list.Count; i++)
        {
            var u = list[i];
            string label = $"[{i + 1}] {u.title}\n\n{u.DescText}\n\n取得 {Count(u.type)}/{u.maxCount}";
            var rect = new Rect(startX + i * (cardW + gap), y, cardW, cardH);

            if (GUI.Button(rect, label, style))
            {
                Select(i);
                return;
            }
        }
    }
}
