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
    public bool alternativeCut = true;
    public CutableOption[] cutResults;

    [Header("Polishing options")]
    public bool isPolishable = false;
    public Vector3 polishScale;
    public GameObject polishResult;
    public Vector3 polishResultScale;
    

    [Header("Bubble options")]
    public bool isBubblable = true;
    public GameObject myBubble;
    public bool inBubble = false;

    protected bool hasJustSpawned = true;
    protected Collider col;
    protected Interactable interactable;

    public bool hasJustBeenThrown = false;
    

    [System.Serializable]
    public struct CutableOption
    {
        public int numberOfResult;
        public GameObject result;
    }

    protected override void Start()
    {
        base.Start();
        GetComponent<Throwable>().onDetachFromHand.AddListener(OnDetachFromHand);
        GetComponent<Throwable>().onPickUp.AddListener(OnPickUp);
        StartCoroutine(HasJustSpawnedTimer());
        col = GetComponentInChildren<Collider>();
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
        //Debug.Log("enter function");
		if (isCutable && !hasJustSpawned)
        {
            if (alternativeCut)
            {
                //Debug.Log("enter function alter :" + gameObject.name + "  " + cutResults.Length);

                Split(cutResults);
            }
            else
            {
                Split(numberOfCutResult, cutResult);
            }
        }
	}

    public void OnPickUp()
    {
        if (hasJustBeenThrown)
        {
            StopAllCoroutines();
            hasJustBeenThrown = false;
            gameObject.layer = 0;
        }
        if (isBubblable && myBubble != null)
        {
            myBubble.GetComponent<Bubble>().ingredientsInBubble.Remove(GetComponent<Ingredient>());
            inBubble = false;
            gameObject.layer = 0;
        }
    }

    public void OnDetachFromHand()
    {
        if (isBubblable && myBubble != null)
        {
            transform.position = myBubble.transform.position;
            //transform.parent = myBubble.transform;
            myBubble.GetComponent<Bubble>().ingredientsInBubble.Add(GetComponent<Ingredient>());
            inBubble = true;
            gameObject.layer = 9; //IngredientInBubble
        }
        
        if (myBubble == null)
        {
            StartCoroutine(HolowallLayer());
        }
    }

    private IEnumerator HolowallLayer()
    {
        hasJustBeenThrown = true;
        gameObject.layer = LayerMask.NameToLayer("HoloWall");
        float timer = 0;
        while (timer <= 3.0f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gameObject.layer = 0;
        hasJustBeenThrown = false;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ingredient" && ((other.relativeVelocity.magnitude > GameManager.instance.minVelocityToFusion)
            || (hasJustBeenThrown && other.relativeVelocity.magnitude > GameManager.instance.minVelocityFusionNice)))
        {
            Ingredient otherIng = other.gameObject.GetComponent<Ingredient>();
            Recipe recipeToTest = new Recipe(ingredientName, otherIng.ingredientName);
            Recipe result;
            if (GameManager.instance.recipeList.Exist(recipeToTest, out result))
            {
                GameManager.instance.SendRecipeRequest(result, gameObject, otherIng.gameObject, other.contacts[0].point, other.relativeVelocity);
            }
        }
        if (hasJustBeenThrown)
        {
            StopAllCoroutines();
            hasJustBeenThrown = false;
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
        for (int i = 0; i < numberOfSplit; ++i)
        {
            if (col == null) Debug.Log("here");
            Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, col.bounds.extents) + transform.position;
            Instantiate(prefabSplitted, spawnPosition, Quaternion.identity);
        }
        StartCoroutine(CutterExplosionForce());      
    }

    protected void Split(CutableOption[] options)
    {
        foreach(CutableOption option in options)
        {
            for (int i = 0; i < option.numberOfResult; ++i)
            {
                if (col == null) Debug.Log("here");
                Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, col.bounds.extents) + transform.position;
                Instantiate(option.result, spawnPosition, Quaternion.identity);
            }
        }
        StartCoroutine(CutterExplosionForce());
    }

    private IEnumerator CutterExplosionForce()
    {
        GameManager.instance.LaunchCutterParticle(transform.position);
        rb.AddExplosionForce(10f, transform.position, 50f);
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
        yield break;
    }
}
