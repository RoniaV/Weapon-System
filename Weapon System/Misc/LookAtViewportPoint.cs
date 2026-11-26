using UnityEngine;

/// <summary>Rotates the object towards whatever is under a viewport point (the screen center by default).</summary>
public class LookAtViewportPoint : MonoBehaviour
{
    private const float MaxAimDistance = 100f;

    [SerializeField] Vector2 viewportPoint = new Vector2(0.5f, 0.5f);
    [SerializeField] LayerMask aimLayerMask;
    [SerializeField] float rotationSpeed = 1f;


    public void RotateToPoint()
    {
        Vector3 targetDirection;

        if (TryGetAimPoint(out Vector3 aimPoint))
            targetDirection = (aimPoint - transform.position).normalized;
        else
            targetDirection = Camera.main.transform.forward;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), rotationSpeed);
    }

    public Vector3 GetPointPosition()
    {
        if (TryGetAimPoint(out Vector3 aimPoint))
            return aimPoint;
        else
            return Camera.main.transform.position + (Camera.main.transform.forward * MaxAimDistance);
    }

    private bool TryGetAimPoint(out Vector3 point)
    {
        Ray centerRay = Camera.main.ViewportPointToRay(viewportPoint);

        if (Physics.Raycast(centerRay, out RaycastHit hit, MaxAimDistance, aimLayerMask))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
