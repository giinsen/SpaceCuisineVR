using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBouncerBox : Item
{
    public float triggerVelocity = 5.0f;
    public GameObject bouncingBallPrefab;
    public Vector2 rangeTimer = new Vector2(0.5f, 35.0f);

    private float timer = 0.0f;
    private float timerDuration;

    protected override void Start()
    {
        base.Start();

        timerDuration = Random.Range(rangeTimer.x, rangeTimer.y);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerDuration)
        {
            timer = 0.0f;
            timerDuration = Random.Range(rangeTimer.x, rangeTimer.y);
            Impulse();
        }
    }

    private void Impulse()
    {
        Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        rb.AddForce(direction * 5.0f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.relativeVelocity.magnitude > triggerVelocity)
        {
            Instantiate(bouncingBallPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
