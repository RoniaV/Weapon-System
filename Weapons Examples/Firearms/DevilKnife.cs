using UnityEngine;

/// <summary>Firearm that aims at the viewport point; the primary attack fails while the secondary barrel is not ready.</summary>
public class DevilKnife : FirearmBase
{
    [Header("Devil Knife")]
    [SerializeField] LookAtViewportPoint lookAtViewport;


    void FixedUpdate()
    {
        lookAtViewport.RotateToPoint();
        Barrels[0].ShotPoint.rotation =
            Quaternion.LookRotation((lookAtViewport.GetPointPosition() - Barrels[0].ShotPoint.position).normalized);
    }

    public override void Attack()
    {
        if (!Barrels[1].CanShoot)
            FailAttack();
        else if (Barrels[0].CanShoot)
            base.Attack();
    }

    public override void SecondaryAttack()
    {
        if (Barrels[1].CanShoot)
            base.SecondaryAttack();
        else
            FailSecondaryAttack();
    }

    protected override void DoAttack()
    {
        Barrels[0].Shoot();
    }

    protected override void DoReleaseAttack()
    {
        Barrels[0].ReleaseShoot();
    }

    protected override void DoSecondaryAttack()
    {
        Barrels[1].Shoot();
    }

    protected override void DoReleaseSecondaryAttack()
    {
        Barrels[1].ReleaseShoot();
    }
}
