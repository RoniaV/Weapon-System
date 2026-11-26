using UnityEngine;

/// <summary>Shoots pooled bullets from a random point and direction of a ShapeSampler.</summary>
public class ShapeInstantiateBarrel : BarrelBase
{
    [SerializeField] BulletBase bulletPrefab;
    [SerializeField] float shotForce;
    [SerializeField] bool addInertia;
    [SerializeField] ShapeSampler shapeSampler;
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

        shapeSampler.Sample(out Vector3 position, out Vector3 direction);

        bullet.transform.position = position;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        if (addInertia)
        {
            SetCharacter();

            bullet.LaunchBullet(direction, shotForce, character.velocity);
        }
        else
            bullet.LaunchBullet(direction, shotForce);
    }

    private void SetCharacter()
    {
        if (character == null)
            character = transform.root.GetComponent<CharacterController>();
    }
}
