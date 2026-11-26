using UnityEngine;

/// <summary>Shoots pooled bullets from the shot point.</summary>
public class InstantiateBarrel : BarrelBase
{
    [SerializeField] BulletBase bulletPrefab;
    [SerializeField] float shotForce;
    [SerializeField] bool addInertia;
    [Header("Object Pool Settings")]
    [SerializeField] Transform poolParent;

    private ObjectPool<BulletBase> bulletPool;
    private CharacterController character;


    void Start()
    {
        bulletPool = new ObjectPool<BulletBase>(bulletPrefab, poolParent);
        poolParent.parent = null;
    }

    protected override void DoShoot()
    {
        BulletBase bullet = bulletPool.GetObject();

        bullet.transform.position = ShotPoint.position;
        bullet.transform.rotation = ShotPoint.rotation;

        if (addInertia)
        {
            SetCharacter();

            bullet.LaunchBullet(ShotPoint.forward, shotForce, character.velocity);
        }
        else
            bullet.LaunchBullet(ShotPoint.forward, shotForce);
    }

    private void SetCharacter()
    {
        if (character == null)
            character = transform.root.GetComponent<CharacterController>();
    }
}
