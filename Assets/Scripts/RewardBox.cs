using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBox : Ingredient 
{
    [Header("Box parameters")]
	public CutableOption[] easyRewards;
	public CutableOption[] mediumRewards;
	public CutableOption[] hardRewards;
    public GameObject dollar;
    
    public Recipe.RecipeComplexity rewardTier = Recipe.RecipeComplexity.Easy;
    public float triggerVelocity = 3.0f;

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        if (col.relativeVelocity.magnitude >= triggerVelocity)
        {
            CutableOption[] rewards = new CutableOption[0];
            switch(rewardTier)
            {
                case Recipe.RecipeComplexity.Easy:
                    rewards = easyRewards;
                    break;
                
                case Recipe.RecipeComplexity.Medium:
                    rewards = mediumRewards;
                    break;

                case Recipe.RecipeComplexity.Hard:
                    rewards = hardRewards;
                    break;
            }
            rewards = ComputeReward(rewards);
            Split(rewards);
        }
    }

    private CutableOption[] ComputeReward(CutableOption[] possibleRewards)
    {
        int numberOfIngredients = Random.Range(1, 2);
        CutableOption[] result = new CutableOption[numberOfIngredients + 1];
        for (int i = 0; i < numberOfIngredients; ++i)
        {
            int idx = Random.Range(0, possibleRewards.Length);
            result[i] = possibleRewards[idx];
        }
        result[numberOfIngredients +1] = new CutableOption(1, dollar);
        return result;
    }
}
