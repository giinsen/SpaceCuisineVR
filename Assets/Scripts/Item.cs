using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [Header("Attractable options")]
    public bool isAttractable = true;
    private float attractableSpeed = 3f;

    protected Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Attract(GameObject attractPoint)
    {
        if (isAttractable)
        {
            Vector3 targetDirection = Vector3.Normalize(attractPoint.transform.position - transform.position);
            Vector3 velocity = rb.velocity;
            float actualSpeed = velocity.magnitude;
            Vector3 actualDirection = velocity.normalized;
            velocity += targetDirection * attractableSpeed * Time.deltaTime;
            rb.velocity = velocity;
        }
    }

    public virtual void Stase()
    {
        rb.velocity = Vector3.zero;
    }
}
