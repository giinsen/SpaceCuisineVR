﻿using System.Collections;
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
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        StartCoroutine(StaseAnimation());
    }

    private IEnumerator StaseAnimation()
    {
        float timer = 0;
        float timerDuration = 0.1f;
        Vector3 baseScale = transform.localScale;
        Vector3 targetScale = transform.localScale * 0.7f;
        while (timer < timerDuration)
        {
            transform.localScale = Vector3.Lerp(baseScale, targetScale, timer / timerDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer = 0.0f;
        while (timer < timerDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, baseScale, timer / timerDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = baseScale;
    }
}
