using System.Collections;
using UnityEngine;

/// <summary>Flattens against the surface it hits and shrinks away once its lifetime ends.</summary>
public class WaterBullet : BulletBase
{
    [SerializeField] Transform visuals;
    [SerializeField] float fadeTime = 1f;


    protected override void OnCollisionEnter(Collision coll)
    {
        base.OnCollisionEnter(coll);

        if (coll.collider != null)
        {
            visuals.localScale = new Vector3(visuals.localScale.x, visuals.localScale.y, 0.01f);
            visuals.rotation = Quaternion.LookRotation(coll.GetContact(0).normal);

            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        rb.isKinematic = false;
        visuals.localScale = Vector3.one;
        visuals.rotation = Quaternion.identity;
    }

    protected override IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);

        float t = 0f;
        Vector3 startScale = visuals.localScale;

        while (t < fadeTime)
        {
            visuals.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.InverseLerp(0f, fadeTime, t));

            t += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
