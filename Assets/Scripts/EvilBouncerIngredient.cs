using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBouncerIngredient : Ingredient
{
    [Header("Evil Bouncer Parameters")]
    public float acceleration = 0.5f;
    public float maximumVelocity = 15.0f;

    private bool rush = true;


    void Update()
    {
        if (rush)
        {
            if (rb.velocity.magnitude < maximumVelocity)
            {
                Vector3 velocity = rb.velocity;
                velocity += rb.velocity.normalized * acceleration;
                rb.velocity = velocity;
            }
        }
        rush = interactable == null;
    }
}
