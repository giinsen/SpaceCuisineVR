using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class Ingredient : MonoBehaviour
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

    [Header("Attractable options")]
    public bool isAttractable = true;
    public float attractableSpeed = 40f;

    [Header("Bubble options")]
    public bool isBubbling = true;
    public GameObject myBubble;
    public bool inBubble = false;

    protected bool hasJustSpawned = true;
    protected Collider col;
    protected Rigidbody rb;
    protected Interactable interactable;

    protected virtual void Start()
    {
        //Add listener ne marche po;
        GetComponent<Throwable>().onDetachFromHand.AddListener(OnDetachFromHand);
        StartCoroutine(HasJustSpawnedTimer());
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
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

    public virtual void Stase()
    {
        rb.velocity = Vector3.zero;
    }

    public void Attract(GameObject attractPoint)
    {
        if (isAttractable)
        {
            Vector3 targetDirection = Vector3.Normalize(attractPoint.transform.position - transform.position);
            Vector3 velocity = rb.velocity;
            float actualSpeed = velocity.magnitude;
            Vector3 actualDirection = velocity.normalized;
            velocity += targetDirection * attractableSpeed * Time.deltaTime;
            rb.velocity = velocity;
        }
    }

	public void Cut()
    {
		if (isCutable && !hasJustSpawned)
        {
            Split(numberOfCutResult, cutResult);
        }
	}

    public void OnDetachFromHand()
    {
        if (isBubbling && myBubble != null)
        {
            //GetComponent<Collider>().enabled = false;
            transform.position = myBubble.transform.position;
            transform.parent = myBubble.transform;
            myBubble.GetComponent<Bubble>().ingredients.Add(GetComponent<Ingredient>());
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
                GameManager.instance.RecipeSpawn(result, gameObject, otherIng.gameObject, other.contacts[0].point);
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
