
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("HP Slider")]
    public Slider hpSlider;
    [SerializeField]
    private MoveEnemy2 moveEnemy2;

    [Header("HP設定")]
    [SerializeField] private float maxHP = 100f;
    [Header("カメラ設定")]
    [SerializeField] private Transform mainCameraTransform;

    [Header("HP SliderのCanvas")]
    [SerializeField] private Transform hpCanvasTransform; private float currentHP;
    private int power = 30;
    [Header("経験値")]
    [SerializeField] private int exp = 5; // 敵ごとにPrefabで設定(敵1:5、敵2:15、敵3:40)

    [Header("属性効果")]
    [SerializeField] private float burnDuration = 3f;   // 燃える時間
    [SerializeField] private float burnRate = 0.2f;     // 1秒あたり、矢のダメージの何割か
    [SerializeField] private float slowDuration = 3f;   // 鈍足の時間
    [SerializeField] private float slowMul = 0.5f;      // 移動速度の倍率

    private Coroutine burnCo, slowCo;
    private float baseWalkSpeed = -1f;

    public static event System.Action<int> onAnyEnemyDied;

    private Animator animator;
    private bool isDead = false;
    public DropTable dropTable;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentHP = maxHP;

        currentHP = maxHP;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

    }

    void LateUpdate()
    {
        if (hpCanvasTransform == null || mainCameraTransform == null)
            return;

        // Canvasからカメラへの方向を取得
        Vector3 direction =
            mainCameraTransform.position - hpCanvasTransform.position;

        // 上下方向の角度を変更しない
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            // Canvas全体の角度だけ変更
            hpCanvasTransform.rotation =
                Quaternion.LookRotation(-direction);
        }
    }
    // ダメージ処理
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0f, currentHP);

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        // HPが0になったら
        if (currentHP <= 0f)
        {
            isDead = true;

            onAnyEnemyDied?.Invoke(exp);

            MoveEnemy2 moveEnemy = GetComponent<MoveEnemy2>();

            if (moveEnemy != null)
            {
                moveEnemy.walkSpeed = 0f;
            }

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetTrigger("Death");
            }
        }
    }

    public void ApplyElement(Element element, float baseDamage)
    {
        if (isDead) return;

        switch (element)
        {
            case Element.Fire:
                if (burnCo != null) StopCoroutine(burnCo);   // 重ねがけは、時間を更新
                burnCo = StartCoroutine(Burn(baseDamage * burnRate));
                break;
            case Element.Ice:
                if (slowCo != null) StopCoroutine(slowCo);
                slowCo = StartCoroutine(Slow());
                break;
        }
    }

    System.Collections.IEnumerator Burn(float dmgPerSec)
    {
        for (float t = 0f; t < burnDuration; t += 1f)
        {
            yield return new WaitForSeconds(1f);
            if (isDead) yield break;
            TakeDamage(dmgPerSec);
        }
    }

    System.Collections.IEnumerator Slow()
    {
        MoveEnemy2 move = GetComponent<MoveEnemy2>();
        if (move == null) yield break;

        if (baseWalkSpeed < 0f) baseWalkSpeed = move.walkSpeed; // 元の速度を、最初の1回だけ控える
        move.walkSpeed = baseWalkSpeed * slowMul;

        yield return new WaitForSeconds(slowDuration);

        if (!isDead) move.walkSpeed = baseWalkSpeed;            // 死んでいたら、0のままにする
    }

    // 倒れた後にフェードアウト
    public void OnAnimationEnd()
    {
        GetComponent<Fade>().FadeOut();
    }
}
