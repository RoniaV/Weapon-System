using UnityEngine;

/// <summary>Charges a bullet while the trigger is held and launches it on release.</summary>
public class LoadShotBarrel : BarrelBase
{
    public bool IsLoading { get; private set; }

    [SerializeField] WaterLoadBullet bulletPrefab;
    [SerializeField] float loadSpeed = 1f;
    [SerializeField] float shotForce = 500f;
    [Header("Object Pool Settings")]
    [SerializeField] Transform poolParent;

    private WaterLoadBullet currentBullet;
    private ObjectPool<WaterLoadBullet> bulletPool;


    void Start()
    {
        bulletPool = new ObjectPool<WaterLoadBullet>(bulletPrefab, poolParent);
        poolParent.parent = null;
    }

    protected override void Update()
    {
        Debug.DrawRay(ShotPoint.position, ShotPoint.forward, Color.blue);

        if (!IsLoading && cooldownTimer > 0f && cooldownTimer <= cooldown)
            cooldownTimer -= Time.deltaTime;

        if (IsLoading && currentBullet != null)
            currentBullet.transform.position = ShotPoint.position;
    }

    public override void ReleaseShoot()
    {
        IsLoading = false;

        if (currentBullet != null)
        {
            currentBullet.LaunchBullet(ShotPoint.forward, shotForce);
            currentBullet = null;
        }

        base.ReleaseShoot();
    }

    protected override void DoShoot()
    {
        IsLoading = true;

        if (currentBullet == null)
        {
            currentBullet = bulletPool.GetObject();

            currentBullet.transform.position = ShotPoint.position;
            currentBullet.transform.rotation = ShotPoint.rotation;
        }
        else
            currentBullet.Load(loadSpeed);
    }

    protected override bool GetCanShoot()
    {
        return IsLoading || (isTriggerReady && cooldownTimer <= 0f);
    }
}
