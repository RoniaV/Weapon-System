/// <summary>Loader that never runs out of ammo.</summary>
public class InfiniteLoader : WeaponLoaderBase
{
    public override bool TryConsumeAmmo(int amount)
    {
        return true;
    }

    public override int AddAmmo(int amount)
    {
        return 0;
    }
}
