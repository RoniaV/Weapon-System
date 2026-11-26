using UnityEngine;

/// <summary>Chargeable bullet: its size, collider and explosion radius grow with the load.</summary>
public class WaterLoadBullet : BulletBase
{
    [Header("Load")]
    [SerializeField] float maxLoad = 5f;
    [SerializeField] float minLoad = 1f;
    [Header("Explosion")]
    [SerializeField] float minExplosionRadius = 1f;
    [SerializeField] float maxExplosionRadius = 3f;
    [SerializeField] GameObject feedbackPrefab;
    [Header("Collider")]
    [SerializeField] SphereCollider collisionCollider;
    [SerializeField] float minCollRadius = 0.15f;
    [SerializeField] float maxCollRadius = 1.5f;
    [Header("Visuals")]
    [SerializeField] Transform visuals;
    [SerializeField] Vector3 minSize = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] Vector3 maxSize = new Vector3(3f, 3f, 3f);

    private float currentLoad;
    private float currentExplosionRadius;


    // Does not call base, so the lifetime coroutine never starts for this bullet.
    protected override void OnEnable()
    {
        rb.useGravity = false;

        SetCollider();
        SetVisuals();
        SetExplosionRadius();
    }

    protected override void OnCollisionEnter(Collision coll)
    {
        if (rb.velocity.magnitude > minVelocity)
        {
            GameObject feedback = Instantiate(
                feedbackPrefab,
                transform.position,
                Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized),
                null);
            feedback.transform.localScale = Vector3.one * currentExplosionRadius;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, currentExplosionRadius, Vector3.down * 0.01f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                    damageable.ReceiveDamage(baseDamage);
            }

            gameObject.SetActive(false);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SetCollider();
        SetVisuals();
        SetExplosionRadius();
    }

    public void Load(float loadSpeed)
    {
        currentLoad += Time.deltaTime * loadSpeed;
        currentLoad = Mathf.Clamp(currentLoad, 0f, maxLoad);

        float t = Mathf.InverseLerp(0f, maxLoad, currentLoad);

        SetCollider(t);
        SetVisuals(t);
        SetExplosionRadius(t);
    }

    // Launches only if the minimum load was reached; otherwise the bullet is discarded.
    public override void LaunchBullet(Vector3 direction, float force)
    {
        bool isCharged = currentLoad >= minLoad;

        currentLoad = 0f;

        if (isCharged)
        {
            rb.useGravity = true;

            base.LaunchBullet(direction, force);
        }
        else
            gameObject.SetActive(false);
    }

    private void SetCollider(float t = 0f)
    {
        collisionCollider.radius = Mathf.Lerp(minCollRadius, maxCollRadius, t);
    }

    private void SetVisuals(float t = 0f)
    {
        visuals.localScale = Vector3.Lerp(minSize, maxSize, t);
    }

    private void SetExplosionRadius(float t = 0f)
    {
        currentExplosionRadius = Mathf.Lerp(minExplosionRadius, maxExplosionRadius, t);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * 0.01f), currentExplosionRadius);
    }
}
