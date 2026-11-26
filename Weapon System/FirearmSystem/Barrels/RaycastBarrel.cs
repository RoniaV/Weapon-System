using System;
using UnityEngine;

/// <summary>Hitscan barrel: damages whatever the ray hits.</summary>
public class RaycastBarrel : BarrelBase
{
    public event Action<RaycastHit> OnShotHit;

    [SerializeField] float maxDistance = 100f;
    [SerializeField] float baseDamage = 1f;


    protected override void DoShoot()
    {
        if (Physics.Raycast(ShotPoint.position, ShotPoint.forward, out RaycastHit hit, maxDistance))
        {
            OnShotHit?.Invoke(hit);

            if (hit.collider.TryGetComponent(out IDamageable damageable))
                damageable.ReceiveDamage(baseDamage);
        }
    }
}
