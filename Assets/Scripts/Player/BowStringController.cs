
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BowStringController : MonoBehaviour
{
    [Header("弓の糸の位置")]
    [SerializeField] private Transform stringTop;
    [SerializeField] private Transform stringMiddle;
    [SerializeField] private Transform stringBottom;

    [Header("引いたときの位置")]
    [SerializeField] private Transform pulledPosition;

    [Header("連動させるオブジェクト")]
    [SerializeField] private Transform followObject;

    [Header("速度")]
    [SerializeField] private float pullSpeed = 12f;

    private LineRenderer lineRenderer;

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private bool isPulling = false;

    private Vector3 topOffset;
    private Vector3 middleOffset;
    private Vector3 bottomOffset;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 3;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = true;

        originalPosition = stringMiddle.position;
        targetPosition = originalPosition;

        if (followObject != null)
        {
            topOffset = followObject.InverseTransformPoint(stringTop.position);
            middleOffset = followObject.InverseTransformPoint(stringMiddle.position);
            bottomOffset = followObject.InverseTransformPoint(stringBottom.position);
        }

        UpdateString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PullString();

        if (Input.GetMouseButtonUp(0))
            ReturnString();
    }

    void LateUpdate()
    {
        // 弓本体に追従
        if (followObject != null)
        {
            stringTop.position = followObject.TransformPoint(topOffset);
            stringBottom.position = followObject.TransformPoint(bottomOffset);

            if (!isPulling)
            {
                stringMiddle.position =
                    followObject.TransformPoint(middleOffset);
            }
        }

        // 引いた位置に追従
        if (isPulling && pulledPosition != null)
        {
            targetPosition = pulledPosition.position;

            stringMiddle.position = Vector3.Lerp(
                stringMiddle.position,
                targetPosition,
                Time.deltaTime * pullSpeed
            );
        }

        UpdateString();
    }

    public void PullString()
    {
        if (pulledPosition == null)
            return;

        isPulling = true;
        targetPosition = pulledPosition.position;
    }

    public void ReturnString()
    {
        isPulling = false;

        if (followObject != null)
        {
            stringMiddle.position =
                followObject.TransformPoint(middleOffset);
        }
        else
        {
            stringMiddle.position = originalPosition;
        }

        targetPosition = stringMiddle.position;
    }

    private void UpdateString()
    {
        if (stringTop == null ||
            stringMiddle == null ||
            stringBottom == null)
            return;

        lineRenderer.SetPosition(0, stringTop.position);
        lineRenderer.SetPosition(1, stringMiddle.position);
        lineRenderer.SetPosition(2, stringBottom.position);
    }
}
