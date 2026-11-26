using System;
using UnityEngine;

/// <summary>Base for every weapon: raises the attack events and delegates the logic to subclasses.</summary>
public abstract class WeaponBase : MonoBehaviour
{
    public event Action Attacked;
    public event Action AttackReleased;
    public event Action AttackFailed;

    public event Action SecondaryAttacked;
    public event Action SecondaryAttackReleased;
    public event Action SecondaryAttackFailed;

    [field: SerializeField] public GameObject Feedback { get; protected set; }


    public virtual void Attack()
    {
        Attacked?.Invoke();
        DoAttack();
    }

    public void ReleaseAttack()
    {
        AttackReleased?.Invoke();
        DoReleaseAttack();
    }

    public virtual void SecondaryAttack()
    {
        SecondaryAttacked?.Invoke();
        DoSecondaryAttack();
    }

    public void ReleaseSecondaryAttack()
    {
        SecondaryAttackReleased?.Invoke();
        DoReleaseSecondaryAttack();
    }

    protected abstract void DoAttack();

    protected abstract void DoReleaseAttack();

    protected virtual void DoSecondaryAttack() { }

    protected virtual void DoReleaseSecondaryAttack() { }

    protected virtual void FailAttack()
    {
        AttackFailed?.Invoke();
    }

    protected virtual void FailSecondaryAttack()
    {
        SecondaryAttackFailed?.Invoke();
    }
}
