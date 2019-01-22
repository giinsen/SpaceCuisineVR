using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBox : Item
{
    [Header("Explosion parameters")]
    public float explosionTimer = 5.0f;
    public float explosionRadius = 5.0f;
    public float explosionForce = 15.0f;
    public ForceMode explosionForceMode = ForceMode.Impulse;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explosionTimer);
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider col in cols)
        {
            if (col.gameObject != this.gameObject && col.gameObject.GetComponent<Item>() != null)
            {
                col.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, explosionForceMode);
            }
        }
        Destroy(this.gameObject);
    }
}
