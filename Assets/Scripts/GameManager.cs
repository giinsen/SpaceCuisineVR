using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RecipeList recipeList;

    public static GameManager instance;

    private List<GameObject> blacklist = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public void RecipeSpawn(Recipe recipe, GameObject a, GameObject b, Vector3 position)
    {
        if (a == null || b == null) return;
        if (blacklist.Contains(a) && blacklist.Contains(b)) return;

        blacklist.Add(a);
        blacklist.Add(b);
        Instantiate(recipe.result, position, Quaternion.identity);
        Destroy(a);
        Destroy(b);
        blacklist.Remove(a);
        blacklist.Remove(b);
    }

}
