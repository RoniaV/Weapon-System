using UnityEngine;

/// <summary>Sticks into surfaces it hits head-on; otherwise it starts falling.</summary>
public class KnifeBullet : BulletBase
{
    private Vector3 launchDirection;


    protected override void OnCollisionEnter(Collision coll)
    {
        base.OnCollisionEnter(coll);

        if (coll.collider != null)
        {
            // Dot < -0.5: the impact is within 60 degrees of head-on.
            if (Vector3.Dot(coll.GetContact(0).normal, launchDirection) < -0.5f)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            else
                rb.useGravity = true;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        rb.isKinematic = false;
        rb.useGravity = false;
        launchDirection = Vector3.zero;
    }

    public override void LaunchBullet(Vector3 direction, float force)
    {
        base.LaunchBullet(direction, force);

        launchDirection = direction;
    }

    public override void LaunchBullet(Vector3 direction, float force, Vector3 initialVelocity)
    {
        base.LaunchBullet(direction, force, initialVelocity);

        launchDirection = direction;
    }
}
