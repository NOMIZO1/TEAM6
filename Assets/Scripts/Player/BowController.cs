
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BowController : MonoBehaviour
{
    [Header("Animator設定")]
    [SerializeField] private Animator animator;
    [SerializeField] private string bowLayerName = "BowLayer";

    [Header("弓を構えるアニメーション")]
    [SerializeField]
    private string[] bowReadyAnimations =
    {
        "Bow_Ready_01",
        "Bow_Ready_02",
        "Bow_Ready_03"
    };
    [Header("矢の上下角度調整")]
    [SerializeField] private Transform anchorPoint;
    [Header("3D照準とAnchorPointの位置関係")]
    [SerializeField]
    private Vector3 aimMarkerLocalPosition =
    new Vector3(0f, 0f, 10f);
    [Header("カメラ切り替え設定")]
    [SerializeField] private float verticalRotationOffset = 0f;
    [SerializeField] private string bowReleaseAnimation = "Bow_Release";
    [SerializeField] private float readyAnimationSpeed = 1f;
    [SerializeField] private string speedParameter = "BowAnimationSpeed";

    [Header("矢の設定")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Transform arrowDirectionPoint;

    [SerializeField] private float arrowSpeed = 30f;
    [SerializeField] private float maxArrowSpeed = 60f;

    [Header("矢本体の角度補正")]
    [SerializeField] private Vector3 arrowRotationOffset = Vector3.zero;

    [Header("3D照準オブジェクト")]
    [SerializeField] private GameObject aimMarker;
    [Header("チャージによる矢の射程距離")]
    [SerializeField] private float minArrowRange = 5f;
    [SerializeField] private float maxArrowRange = 30f;
    [Header("3D照準の位置・角度")]
    [SerializeField] private float aimMarkerDistance = 10f;
    [SerializeField] private Vector3 aimMarkerRotationOffset = Vector3.zero;

    [Header("照準設定")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float maxAimDistance = 200f;

    [Header("チャージゲージ")]
    [SerializeField] private Image chargeGauge;
    [SerializeField] private float maxChargeTime = 3f;
    [Header("3D円形チャージゲージ")]
    [SerializeField] private Circular3DGauge aimGauge;
    [Header("カメラ設定")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollow mainCameraFollow;

    [Header("カーソル設定")]
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D bowCursor;
    [SerializeField] private Texture2D chargedBowCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private float chargeTime;
    private float chargeRate;

    private int bowLayerIndex = -1;

    private Coroutine bowCoroutine;

    private GameObject loadedArrow;

    private bool isHoldingBow;
    private bool isReleasing;
    private bool isArrowLoaded;

    void Start()
    {
        // Animator取得
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (aimMarker != null)
        {
            aimMarker.SetActive(false);
        }
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (aimCamera == null)
        {
            aimCamera = mainCamera;
        }

        if (mainCameraFollow != null)
        {
            mainCameraFollow.enabled = true;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Animatorのレイヤー取得
        if (animator != null)
        {
            bowLayerIndex = animator.GetLayerIndex(bowLayerName);

            if (bowLayerIndex < 0)
            {
                Debug.LogError("BowLayerが見つかりません！");
            }
            else
            {
                animator.SetLayerWeight(bowLayerIndex, 0f);
            }
        }
        else
        {
            Debug.LogError("Animatorが設定されていません！");
        }

        // 初期状態
        if (chargeGauge != null)
        {
            chargeGauge.fillAmount = 0f;
            chargeGauge.gameObject.SetActive(false);
        }

        if (aimMarker != null)
        {
            aimMarker.SetActive(false);
        }


        SetNormalCursor();
    }

    void Update()
    {
        // 左クリックで弓を構える
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHoldingBow && !isReleasing)
            {
                StartBow();
            }
        }

        // 左クリックを離すと矢を発射
        if (Input.GetMouseButtonUp(0))
        {
            if (isHoldingBow)
            {
                ReleaseBow();
            }
        }

        if (isHoldingBow)
        {
            UpdateCharge();
        }

        // 構えている間、矢を弓の位置に追従させる
        FollowArrow();
    }
    // =====================================
    // 3D照準オブジェクトをカメラに追従
    // =====================================

    // =====================================
    // 3D照準オブジェクトを左右のみ追従
    // =====================================

    // =====================================
    // カメラの上下角度と矢の角度を連動
    // =====================================

    // =====================================
    // カメラ・弓・矢・3D照準の角度を同期
    // =====================================

    // =====================================
    // カメラ・弓・矢・3D照準の角度と位置を同期
    // =====================================
    void LateUpdate()
    {
        if (!isHoldingBow || mainCamera == null)
            return;

        Transform cam = mainCamera.transform;

        float cameraPitch = cam.eulerAngles.x;
        float cameraYaw = cam.eulerAngles.y;

        if (cameraPitch > 180f)
        {
            cameraPitch -= 360f;
        }

        float pitch = cameraPitch + verticalRotationOffset;

        Quaternion aimRotation = Quaternion.Euler(
            pitch,
            cameraYaw,
            0f
        );

        if (anchorPoint != null)
        {
            anchorPoint.rotation = aimRotation;
        }

        if (aimMarker != null && anchorPoint != null)
        {
            Vector3 markerPosition =
                anchorPoint.TransformPoint(aimMarkerLocalPosition);

            Quaternion markerRotation =
                anchorPoint.rotation *
                Quaternion.Euler(aimMarkerRotationOffset);

            aimMarker.transform.SetPositionAndRotation(
                markerPosition,
                markerRotation
            );
        }
    }    // =====================================
    // 弓を構える
    // =====================================

    void StartBow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null)
        {
            Debug.LogError("ArrowPrefabまたはArrowSpawnPointが設定されていません！");
            return;
        }

        isHoldingBow = true;
        isArrowLoaded = false;

        // 一人称カメラに切り替え
        Debug.Log("BowController: 一人称カメラに切り替え");

        // チャージ初期化
        chargeTime = 0f;
        chargeRate = 0f;
        if (aimGauge != null)
        {
            aimGauge.SetProgress(0f);
        }

        // カーソル変更
        SetBowCursor();

        // Animatorレイヤー有効化
        if (animator != null && bowLayerIndex >= 0)
        {
            animator.SetLayerWeight(bowLayerIndex, 1f);

            animator.SetFloat(speedParameter, readyAnimationSpeed);
        }

        // 構えるアニメーション開始
        if (bowCoroutine != null)
        {
            StopCoroutine(bowCoroutine);
        }

        bowCoroutine = StartCoroutine(PlayBowReadySequence());
    }

    // =====================================
    // 構えるアニメーション
    // =====================================

    IEnumerator PlayBowReadySequence()
    {
        if (animator == null || bowLayerIndex < 0)
            yield break;

        for (int i = 0; i < bowReadyAnimations.Length; i++)
        {
            string animationName = bowReadyAnimations[i];

            if (string.IsNullOrEmpty(animationName))
                continue;

            animator.Play(animationName, bowLayerIndex, 0f);

            // 最初のアニメーションで矢をセット
            if (i == 0)
            {
                LoadArrow();
            }

            yield return null;

            AnimatorStateInfo stateInfo =
                animator.GetCurrentAnimatorStateInfo(bowLayerIndex);

            float waitTime =
                stateInfo.length / Mathf.Max(readyAnimationSpeed, 0.01f);

            yield return new WaitForSeconds(waitTime);
        }

        bowCoroutine = null;
    }

    // =====================================
    // 矢をセット
    // =====================================

    void LoadArrow()
    {
        if (loadedArrow != null)
        {
            Destroy(loadedArrow);
        }

        loadedArrow = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            arrowSpawnPoint.rotation,
            arrowSpawnPoint
        );

        loadedArrow.transform.localPosition = Vector3.zero;

        if (arrowDirectionPoint != null)
        {
            loadedArrow.transform.rotation =
                arrowDirectionPoint.rotation *
                Quaternion.Euler(arrowRotationOffset);
        }
        else
        {
            loadedArrow.transform.rotation =
                arrowSpawnPoint.rotation *
                Quaternion.Euler(arrowRotationOffset);
        }

        Rigidbody rb = loadedArrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isArrowLoaded = true;
    }

    // =====================================
    // 矢を弓の位置に追従
    // =====================================

    void FollowArrow()
    {
        if (loadedArrow == null || !isArrowLoaded)
            return;

        loadedArrow.transform.position = arrowSpawnPoint.position;

        if (arrowDirectionPoint != null)
        {
            loadedArrow.transform.rotation =
                arrowDirectionPoint.rotation *
                Quaternion.Euler(arrowRotationOffset);
        }
        else
        {
            loadedArrow.transform.rotation =
                arrowSpawnPoint.rotation *
                Quaternion.Euler(arrowRotationOffset);
        }
    }

    // =====================================
    // チャージ処理
    // =====================================

    void UpdateCharge()
    {
        chargeTime += Time.deltaTime;

        chargeRate = Mathf.Clamp01(chargeTime / maxChargeTime);

        // 従来のUIゲージも使用する場合
        if (chargeGauge != null)
        {
            chargeGauge.fillAmount = chargeRate;
        }

        // 3D円形ゲージを更新
        if (aimGauge != null)
        {
            aimGauge.SetProgress(chargeRate);
        }

        // 最大チャージでカーソル変更
        if (chargeRate >= 1f)
        {
            SetChargedCursor();
        }
        else
        {
            SetBowCursor();
        }
    }

    // =====================================
    // 弓を離す
    // =====================================

    void ReleaseBow()
    {
        if (!isHoldingBow)
            return;

        isHoldingBow = false;
        isReleasing = true;

        // 照準オブジェクトを非表示
   
        // チャージゲージを非表示
        if (chargeGauge != null)
        {
            chargeGauge.gameObject.SetActive(false);
        }

        // アニメーションを停止
        if (bowCoroutine != null)
        {
            StopCoroutine(bowCoroutine);
            bowCoroutine = null;
        }

        // 矢を発射
        if (isArrowLoaded)
        {
            ShootArrow();
        }


        // カーソルを戻す
        SetNormalCursor();

        // 発射アニメーション
        PlayBowRelease();
    }

    // =====================================
    // 矢を発射
    // =====================================

    void ShootArrow()
    {
        if (loadedArrow == null)
            return;

        Rigidbody rb = loadedArrow.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        loadedArrow.transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;

        // 照準位置を取得
        Vector3 targetPosition;

        if (aimMarker != null)
        {
            targetPosition = aimMarker.transform.position;
        }
        else
        {
            targetPosition = mainCamera.transform.position
                           + mainCamera.transform.forward * maxArrowRange;
        }

        // 発射方向
        Vector3 direction =
            (targetPosition - loadedArrow.transform.position).normalized;

        // チャージ量から射程距離を計算
        float arrowRange = Mathf.Lerp(
            minArrowRange,
            maxArrowRange,
            chargeRate
        );

        // 射程距離に応じて発射速度を計算
        float arrowSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * arrowRange);

        // 矢の向きを設定
        loadedArrow.transform.rotation =
            Quaternion.LookRotation(direction) *
            Quaternion.Euler(arrowRotationOffset);

        // 矢を発射
        rb.AddForce(direction * arrowSpeed, ForceMode.VelocityChange);

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        loadedArrow = null;
        isArrowLoaded = false;
    }
    // =====================================
    // 発射アニメーション
    // =====================================

    void PlayBowRelease()
    {
        if (animator == null || bowLayerIndex < 0)
        {
            isReleasing = false;
            return;
        }

        animator.Play(bowReleaseAnimation, bowLayerIndex, 0f);

        StartCoroutine(WaitForReleaseAnimation());
    }

    IEnumerator WaitForReleaseAnimation()
    {
        yield return null;

        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(bowLayerIndex);

        float waitTime =
            stateInfo.length / Mathf.Max(readyAnimationSpeed, 0.01f);

        yield return new WaitForSeconds(waitTime);

        animator.SetLayerWeight(bowLayerIndex, 0f);

        isReleasing = false;
    }

    // =====================================
    // カメラ切り替え
    // =====================================

    // =====================================
    // カーソル設定
    // =====================================

    void SetNormalCursor()
    {
        Cursor.SetCursor(
            normalCursor,
            cursorHotspot,
            CursorMode.Auto
        );
    }

    void SetBowCursor()
    {
        Cursor.SetCursor(
            bowCursor,
            cursorHotspot,
            CursorMode.Auto
        );
    }

    void SetChargedCursor()
    {
        Cursor.SetCursor(
            chargedBowCursor,
            cursorHotspot,
            CursorMode.Auto
        );
    }

    // =====================================
    // 終了処理
    // =====================================

    void OnDisable()
    {
        if (bowCoroutine != null)
        {
            StopCoroutine(bowCoroutine);
            bowCoroutine = null;
        }

        if (loadedArrow != null)
        {
            Destroy(loadedArrow);
            loadedArrow = null;
        }

        isHoldingBow = false;
        isArrowLoaded = false;
        isReleasing = false;

        if (chargeGauge != null)
        {
            chargeGauge.gameObject.SetActive(false);
        }

        if (animator != null && bowLayerIndex >= 0)
        {
            animator.SetLayerWeight(bowLayerIndex, 0f);
        }

        SetNormalCursor();
    }
}
