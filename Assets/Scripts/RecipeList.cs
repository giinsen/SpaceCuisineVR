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

    public Recipe GetRandom(Recipe.RecipeComplexity complexity)
    {
        List<Recipe> resultList = new List<Recipe>();
        foreach (Recipe r in recipeList)
        {
            if (r.complexity == complexity)
            {
                resultList.Add(r);
            }
        }
        if (resultList.Count == 0) 
            return null;

        int idx = Random.Range(0, resultList.Count);
        return resultList[idx];
    }

    public Recipe GetFromResultName(string resultName)
    {
        Debug.Log("Result name" + resultName);
        foreach(Recipe r in recipeList)
        {
            if (r.GetResultName() == resultName)
            {
                return r;
            }
        }
        Debug.LogError("argh");
        return null;
    }
}

[System.Serializable]
public class Recipe
{
    public string ingredientA;
    public string ingredientB;
    public GameObject result;
    public bool velocityOnSpawn = true;
    public enum RecipeComplexity
    {
        Easy, Medium, Hard, None
    }
    public RecipeComplexity complexity;

    public Recipe(string ingredientA, string ingredientB)
    {
        result = null;
        complexity = RecipeComplexity.None; //inactive by default
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

    public string GetResultName()
    {
        return result.GetComponent<Ingredient>().ingredientName;
    }
}
