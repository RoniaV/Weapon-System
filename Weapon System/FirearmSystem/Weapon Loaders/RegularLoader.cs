using UnityEngine;

/// <summary>Loader with a fixed ammo pool.</summary>
public class RegularLoader : WeaponLoaderBase
{
    public override bool TryConsumeAmmo(int amount)
    {
        if (currentAmmo <= 0 || currentAmmo < amount)
            return false;

        currentAmmo -= amount;

        return true;
    }

    public override int AddAmmo(int amount)
    {
        int remainingAmmo = Mathf.Max(0, currentAmmo + amount - MaxAmmo);

        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, MaxAmmo);

        return remainingAmmo;
    }
}
