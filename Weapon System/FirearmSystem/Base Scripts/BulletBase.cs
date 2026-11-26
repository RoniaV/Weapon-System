using System.Collections;
using UnityEngine;

/// <summary>Base for pooled physics bullets: lifetime, launch and contact damage.</summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class BulletBase : MonoBehaviour
{
    [SerializeField] protected float minVelocity;
    [SerializeField] protected Vector3 centerOfMass;
    [SerializeField] protected float lifetime = 5f;
    [SerializeField] protected float baseDamage = 1f;

    protected Rigidbody rb;


    protected virtual void Awake()
    {
        SetRigidbody();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(LifetimeCoroutine());
    }

    protected virtual void OnCollisionEnter(Collision coll)
    {
        if (rb.velocity.magnitude > minVelocity
            && coll.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.ReceiveDamage(baseDamage);
        }
    }

    protected virtual void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public virtual void LaunchBullet(Vector3 direction, float force)
    {
        AddLaunchForce(direction, force);
    }

    public virtual void LaunchBullet(Vector3 direction, float force, Vector3 initialVelocity)
    {
        AddLaunchForce(direction, force);

        // Inertia: the shooter's velocity is added on top of the launch force.
        rb.AddForce(initialVelocity, ForceMode.VelocityChange);
    }

    protected virtual IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);

        gameObject.SetActive(false);
    }

    protected void SetRigidbody()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = centerOfMass;
        }
    }

    protected void AddLaunchForce(Vector3 direction, float force)
    {
        SetRigidbody();

        rb.AddForce(direction * force);
    }
}
