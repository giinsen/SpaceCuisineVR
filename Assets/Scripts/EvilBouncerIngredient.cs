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
            if (rb.velocity.magnitude < maximumVelocity) rb.velocity += rb.velocity.normalized * acceleration;
        }
        else
        {
            //how to restore rush state
            if (Utils.AlmostEqual(rb.velocity, Vector3.zero))
            {
                rush = true;
            }
        }

        if (interactable.attachedToHand != null && rush)
        {
            rush = false;
        }
    }
}
