using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class Ingredient : Item
{
    [Header("Global")]
    public string ingredientName = "Ingredient";

    [Header("Cutting options")]
	public bool isCutable = true;
    public int numberOfCutResult = 2;
    public GameObject cutResult;

    [Header("Polishing options")]
    public bool isPolishable = false;
    public GameObject polishResult;

    [Header("Bubble options")]
    public bool isBubbling = true;
    public GameObject myBubble;
    public bool inBubble = false;

    protected bool hasJustSpawned = true;
    protected Collider col;
    protected Interactable interactable;

    protected override void Start()
    {
        base.Start();
        //GetComponent<Throwable>().onDetachFromHand += OnDetachFromHand();
        StartCoroutine(HasJustSpawnedTimer());
        col = GetComponent<Collider>();
        interactable = GetComponent<Interactable>();
    }

    private void Update()
    {
        if (inBubble)
        {
            transform.position = myBubble.transform.position;
        }
    }

    private IEnumerator HasJustSpawnedTimer()
    {
        yield return new WaitForSeconds(0.5f);
        hasJustSpawned = false;
    }

	public void Cut()
    {
		if (isCutable && !hasJustSpawned)
        {
            Split(numberOfCutResult, cutResult);
        }
	}

    public void OnPickUp()
    {
        if (isBubbling && myBubble != null)
        {
            myBubble.GetComponent<Bubble>().ingredientsInBubble.Remove(GetComponent<Ingredient>());
            inBubble = false;
            gameObject.layer = 0;
        }
    }

    public void OnDetachFromHand()
    {
        if (isBubbling && myBubble != null)
        {
            transform.position = myBubble.transform.position;
            //transform.parent = myBubble.transform;
            myBubble.GetComponent<Bubble>().ingredientsInBubble.Add(GetComponent<Ingredient>());
            inBubble = true;
            gameObject.layer = 9; //IngredientInBubble
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ingredient" && other.relativeVelocity.magnitude > GameManager.instance.minVelocity)
        {
            Ingredient otherIng = other.gameObject.GetComponent<Ingredient>();
            Recipe recipeToTest = new Recipe(ingredientName, otherIng.ingredientName);
            Recipe result;
            if (GameManager.instance.recipeList.Exist(recipeToTest, out result))
            {
                GameManager.instance.SendRecipeRequest(result, gameObject, otherIng.gameObject, other.contacts[0].point, other.relativeVelocity);
            }
        }
    }

    public void Polish()
    {
        if (isPolishable)
        {
            Instantiate(polishResult, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    protected void Split(int numberOfSplit, GameObject prefabSplitted)
    {
        for (int i = 0; i < numberOfSplit; ++i)
        {
            if (col == null) Debug.Log("here");
            Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, col.bounds.extents) + transform.position;
            Instantiate(prefabSplitted, spawnPosition, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}
