using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeList", menuName = "Custom/RecipeList", order = 1)]
public class RecipeList : ScriptableObject
{
    public Recipe[] recipeList;

    public bool Exist(Recipe input, out Recipe result)
    {
        result = null;
        foreach(Recipe r in recipeList)
        {
            if (Recipe.SameIngredients(r, input))
            {
                result = r;
                return true;
            } 
        }
        return false;
    }
}

[System.Serializable]
public class Recipe
{
    public string ingredientA;
    public string ingredientB;
    public GameObject result;
    public bool velocityOnSpawn = true;

    public Recipe(string ingredientA, string ingredientB)
    {
        result = null;
        this.ingredientA = ingredientA;
        this.ingredientB = ingredientB;
    }

    public static bool SameIngredients(Recipe a, Recipe b)
    {
        bool b1 = a.ingredientA == b.ingredientA;
        bool b2 = a.ingredientB == b.ingredientB;
        bool b3 = a.ingredientA == b.ingredientB;
        bool b4 = a.ingredientB == b.ingredientA;
        return (b1 && b2) ^ (b3 && b4);
    }
}
