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

    private bool canPassHoloWalls = false;

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
        if (canPassHoloWalls)
        {
            StopAllCoroutines();
            canPassHoloWalls = false;
            gameObject.layer = 0;
        }
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
        else
        {
            StartCoroutine(HolowallLayer());
        }
    }

    private IEnumerator HolowallLayer()
    {
        canPassHoloWalls = true;
        gameObject.layer = LayerMask.NameToLayer("HoloWall");
        float timer = 0;
        while (timer <= 3.0f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gameObject.layer = 0;
        canPassHoloWalls = false;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ingredient" && other.relativeVelocity.magnitude > GameManager.instance.minVelocityToFusion)
        {
            Ingredient otherIng = other.gameObject.GetComponent<Ingredient>();
            Recipe recipeToTest = new Recipe(ingredientName, otherIng.ingredientName);
            Recipe result;
            if (GameManager.instance.recipeList.Exist(recipeToTest, out result))
            {
                GameManager.instance.SendRecipeRequest(result, gameObject, otherIng.gameObject, other.contacts[0].point, other.relativeVelocity);
            }
        }
        if (canPassHoloWalls)
        {
            StopAllCoroutines();
            canPassHoloWalls = false;
            gameObject.layer = 0;
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
        Debug.Log("boom");
        for (int i = 0; i < numberOfSplit; ++i)
        {
            if (col == null) Debug.Log("here");
            Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, col.bounds.extents) + transform.position;
            Instantiate(prefabSplitted, spawnPosition, Quaternion.identity);
        }
        StartCoroutine(CutterExplosionForce());      
    }

    private IEnumerator CutterExplosionForce()
    {
        rb.AddExplosionForce(30f, transform.position, 50f);
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
        yield break;
    }
}
