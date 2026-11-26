using UnityEngine;

/// <summary>Base for the ammo sources used by firearms.</summary>
public abstract class WeaponLoaderBase : MonoBehaviour
{
    public float NormalizedAmmo { get { return Mathf.InverseLerp(0f, MaxAmmo, currentAmmo); } }
    public int CurrentAmmo { get { return currentAmmo; } }

    [field: SerializeField] public int MaxAmmo { get; protected set; } = 10;
    [SerializeField] protected int initialAmmo = 10;

    protected int currentAmmo;


    protected virtual void Start()
    {
        currentAmmo = initialAmmo;
    }

    public abstract bool TryConsumeAmmo(int amount);

    public abstract int AddAmmo(int amount);
}
