
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("HP Slider")]
    public Slider hpSlider;

    [Header("HP設定")]
    [SerializeField] private float maxHP = 100f;
    [Header("カメラ設定")]
    [SerializeField] private Transform mainCameraTransform;

    [Header("HP SliderのCanvas")]
    [SerializeField] private Transform hpCanvasTransform; private float currentHP;
    private int power = 30;
    [Header("経験値")]
    [SerializeField] private int exp = 5; // 敵ごとにPrefabで設定(敵1:5、敵2:15、敵3:40)

    public static event System.Action<int> onAnyEnemyDied;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();

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

    // 倒れた後にフェードアウト
    public void OnAnimationEnd()
    {
        GetComponent<Fade>().FadeOut();
    }
}
