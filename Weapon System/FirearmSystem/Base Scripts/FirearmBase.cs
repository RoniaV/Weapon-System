using UnityEngine;

/// <summary>Base for weapons built from Barrels and Loaders.</summary>
public abstract class FirearmBase : WeaponBase
{
    [field: SerializeField] public BarrelBase[] Barrels { get; protected set; }
    [field: SerializeField] public WeaponLoaderBase[] Loaders { get; protected set; }
    [SerializeField] protected int ammoForAttack = 1;
    [SerializeField] protected int ammoForSecondaryAttack = 1;
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] protected bool debugAttack;
#endif


    protected virtual void Update()
    {
#if UNITY_EDITOR
        if (debugAttack)
        {
            debugAttack = false;
            DoAttack();
        }
#endif
    }
}
