using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitOnCollisionIngredient : Ingredient
{
    [Header("Split On Collision options")]
    public float triggerVelocity;
    public int numberOfSplitResult = 15;
    public GameObject splitResult;
    public bool useCutableOption = false;
    public CutableOption[] splitResults;


    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        if (col.relativeVelocity.magnitude > triggerVelocity)
        {
            if (!useCutableOption)
                Split(numberOfSplitResult, splitResult);
            else
                Split(splitResults);
            Destroy(this.gameObject);
        }
    }
}
