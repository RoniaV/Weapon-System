using UnityEngine;

/// <summary>Regenerates ammo on its own once the cooldown since the last use has passed.</summary>
public class AutoLoader : WeaponLoaderBase
{
    [SerializeField] float cooldown = 1f;
    [SerializeField] int ammoPerFrame = 1;

    private float cooldownTimer;


    void Update()
    {
        if (cooldownTimer < cooldown)
            cooldownTimer += Time.deltaTime;
        else if (currentAmmo < MaxAmmo)
            AddAmmo(ammoPerFrame);
    }

    public override bool TryConsumeAmmo(int amount)
    {
        if (currentAmmo <= 0 || currentAmmo < amount)
            return false;

        currentAmmo -= amount;
        cooldownTimer = 0f;

        return true;
    }

    public override int AddAmmo(int amount)
    {
        int remainingAmmo = Mathf.Max(0, currentAmmo + amount - MaxAmmo);

        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, MaxAmmo);

        return remainingAmmo;
    }
}
