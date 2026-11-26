using UnityEngine;

/// <summary>Semi-automatic firearm: one shot per button press.</summary>
public class Gun : FirearmBase
{
    [Header("Gun")]
    [SerializeField] LookAtViewportPoint lookAtViewport;

    private bool canAttack = true;


    void FixedUpdate()
    {
        lookAtViewport.RotateToPoint();
        Barrels[0].ShotPoint.rotation =
            Quaternion.LookRotation((lookAtViewport.GetPointPosition() - Barrels[0].ShotPoint.position).normalized);
    }

    public override void Attack()
    {
        if (!canAttack)
            return;

        if (Barrels[0].CanShoot && Loaders[0].TryConsumeAmmo(Barrels.Length * ammoForAttack))
            base.Attack();
        else if (Loaders[0].CurrentAmmo <= 0)
            FailAttack();

        canAttack = false;
    }

    protected override void DoAttack()
    {
        Barrels[0].Shoot();
    }

    protected override void DoReleaseAttack()
    {
        Barrels[0].ReleaseShoot();
        canAttack = true;
    }
}
