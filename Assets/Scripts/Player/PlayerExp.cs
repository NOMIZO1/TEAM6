using UnityEngine;
using UnityEngine.Events;

public class PlayerExp : MonoBehaviour
{
    public UpgradeSelector selector; // Inspectorで設定

    public int level = 1;
    public int exp = 0;
    public UnityEvent onLevelUp;

    public int Required => 10 + 5 * level;

    void Awake()
    {
        if (selector != null) onLevelUp.AddListener(selector.OnLevelUp);
    }

    [SerializeField] float expDelay = 1.5f;

    void OnEnable() { EnemyController.onAnyEnemyDied += OnEnemyDied; }
    void OnDisable() { EnemyController.onAnyEnemyDied -= OnEnemyDied; }
    void OnEnemyDied(int amount)
    {
        StartCoroutine(AddExpDelayed(amount));
    }

    System.Collections.IEnumerator AddExpDelayed(int amount)
    {
        yield return new WaitForSeconds(expDelay);
        AddExp(amount);
    }
        

    public void AddExp(int amount)
    {
        exp += amount;
        while (exp >= Required)
        {
            exp -= Required;
            level++;
            onLevelUp.Invoke();
        }
    }

    // 死亡後のやり直し用
    public void ResetExp()
    {
        level = 1;
        exp = 0;
    }
}