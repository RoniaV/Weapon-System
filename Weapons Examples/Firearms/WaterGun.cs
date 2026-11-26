using UnityEngine;

/// <summary>Firearm with a primary attack and a chargeable secondary attack.</summary>
public class WaterGun : FirearmBase
{
    [Header("Water Gun")]
    [SerializeField] LookAtViewportPoint lookAtViewport;

    private bool isLoading;
    // A failed attempt is not retried until the button is released.
    private bool failedAttack;
    private bool failedSecondary;


    void FixedUpdate()
    {
        lookAtViewport.RotateToPoint();
        Barrels[0].ShotPoint.rotation =
            Quaternion.LookRotation((lookAtViewport.GetPointPosition() - Barrels[0].ShotPoint.position).normalized);
    }

    public override void Attack()
    {
        if (failedAttack)
            return;

        if (!isLoading && Loaders[0].TryConsumeAmmo(ammoForAttack))
            base.Attack();
        else
            FailAttack();
    }

    public override void SecondaryAttack()
    {
        if (failedSecondary)
            return;

        if (Barrels[1].CanShoot && Loaders[0].TryConsumeAmmo(ammoForSecondaryAttack))
        {
            isLoading = true;
            base.SecondaryAttack();
        }
        else
            FailSecondaryAttack();
    }

    protected override void DoAttack()
    {
        Barrels[0].Shoot();
    }

    protected override void DoReleaseAttack()
    {
        failedAttack = false;

        Barrels[0].ReleaseShoot();
    }

    protected override void DoSecondaryAttack()
    {
        Barrels[1].Shoot();
    }

    protected override void DoReleaseSecondaryAttack()
    {
        isLoading = false;
        failedSecondary = false;

        Barrels[1].ReleaseShoot();
    }

    protected override void FailAttack()
    {
        failedAttack = true;
        base.FailAttack();
    }

    protected override void FailSecondaryAttack()
    {
        failedSecondary = true;
        base.FailSecondaryAttack();
    }
}
