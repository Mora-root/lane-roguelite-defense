using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -10f);
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private bool followInLateUpdate = true;

    private void Update()
    {
        if (!followInLateUpdate)
        {
            FollowTarget();
        }
    }

    private void LateUpdate()
    {
        if (followInLateUpdate)
        {
            FollowTarget();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void FollowTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime);
    }
}
