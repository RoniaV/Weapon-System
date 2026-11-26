using UnityEngine;

/// <summary>Base for Barrels: handles the shot cooldown and the trigger press/release cycle.</summary>
public abstract class BarrelBase : MonoBehaviour
{
    /// <summary> 1 right after a shot, 0 once the cooldown has finished.</summary>
    public float NormalizedCooldown { get { return Mathf.InverseLerp(0f, cooldown, cooldownTimer); } }
    public bool CanShoot { get { return GetCanShoot(); } }

    [field: SerializeField] public Transform ShotPoint { get; protected set; }
    [SerializeField] protected float cooldown = 1f;
    [Tooltip("If false, the trigger must be released before the next shot")]
    [SerializeField] protected bool continuousShooting;

    protected bool isTriggerReady = true;
    protected float cooldownTimer;


    protected virtual void Update()
    {
        Debug.DrawRay(ShotPoint.position, ShotPoint.forward, Color.blue);
        if (cooldownTimer > 0f && cooldownTimer <= cooldown)
            cooldownTimer -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (CanShoot)
        {
            cooldownTimer = cooldown;
            isTriggerReady = continuousShooting;

            DoShoot();
        }
    }

    public virtual void ReleaseShoot()
    {
        isTriggerReady = true;
    }

    protected abstract void DoShoot();

    protected virtual bool GetCanShoot()
    {
        return isTriggerReady && cooldownTimer <= 0f;
    }
}
