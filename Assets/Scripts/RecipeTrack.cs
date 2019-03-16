using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using Debug = UnityEngine.Debug;

public class RecipeTrack : MonoBehaviour
{
    [Header("Gameplay parameters")]
    public RecipeList recipeList;
    public GameObject reward;
    public GameObject[] punishments;

    [Header("Integration parameters")]
    public int trackIndex = 0;
    public Renderer orderVisualizer;
    public Transform alienPivot;
    public Transform hublotPivot;
    public Transform renderPivot;
    public Animator alienAnimator;

    [Header("Sound parameters")]
    public string orderBad = "event:/ENVIRONMENT/ORDER/ORDER BAD";
    public string orderGood = "event:/ENVIRONMENT/ORDER/ORDER GOOD";
    public string orderStarting = "event:/ENVIRONMENT/ORDER/ORDER STARTING";

    private EventInstance orderBadInst;
    private EventInstance orderGoodInst;
    private EventInstance orderStartingInst;

    private Recipe orderedRecipe;
    private GameObject renderedObject;
    [HideInInspector]
    public bool waitForOrder = false;
    private Animator animator;
    private Coroutine failureTimer;

    private bool tutorial = false;
    private int tutorialProgress = 0;

    public GameObject[] tutorialRewards;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("order", false);

        orderBadInst = RuntimeManager.CreateInstance(orderBad);
        orderBadInst.set3DAttributes(RuntimeUtils.To3DAttributes(hublotPivot));
        orderGoodInst = RuntimeManager.CreateInstance(orderGood);
        orderGoodInst.set3DAttributes(RuntimeUtils.To3DAttributes(hublotPivot));
        orderStartingInst = RuntimeManager.CreateInstance(orderStarting);
        orderStartingInst.set3DAttributes(RuntimeUtils.To3DAttributes(hublotPivot));

        if (trackIndex == 0)
        {
            tutorial = true;
            StartCoroutine(Tutorial());
        }

    }

    private IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(5.0f);

        RequestOrderTutorial("HeadSandwich");

        while (IngredientExists("HeadSandwich") == false)
        {
            yield return new WaitForEndOfFrame();
        }

        OpenGate();

        while (waitForOrder)
        {
            yield return new WaitForEndOfFrame();
        }

        tutorialProgress ++;

        RequestOrderTutorial("TentaclesSandwich");

        while (waitForOrder)
        {
            yield return new WaitForEndOfFrame();
        }

        tutorialProgress ++;

        RequestOrderTutorial("HeadSandwich");

        while (waitForOrder)
        {
            yield return new WaitForEndOfFrame();
        }

        FindObjectOfType<DifficultyManager>().BeginGame();
        OpenAllGates();
    }

    private bool IngredientExists(string name)
    {
        Ingredient[] all = FindObjectsOfType<Ingredient>();
        foreach(Ingredient ing in all)
        {
            if (ing.name == name)
            {
                return true;
            }
        }
        return false;
    }

    private void OpenGate()
    {

    }

    private void OpenAllGates()
    {

    }

    private IEnumerator FailureTimer()
    {
        yield return new WaitForSeconds(120.0f);
        //if (waitForOrder == true)
        if (0 > 1)
        {
            waitForOrder = false;
            StartCoroutine(Failure(null));
        }
    }

    public void RequestOrder()
    {
        if (waitForOrder)
        {
            Debug.LogWarning("Unsuspected behavior");
            return;
        }
        StartCoroutine(FailureTimer());
        //orderedRecipe = ChooseOrder();
        orderedRecipe = GetOrderFromParser();

        //Spawn on render texture
        renderedObject = Instantiate(orderedRecipe.result, renderPivot.position, Quaternion.identity, renderPivot);
        renderedObject.GetComponent<Rigidbody>().isKinematic = true;
        waitForOrder = true;
        orderStartingInst.start();
        animator.SetTrigger("order");
    }

    private void RequestOrderTutorial(string order)
    {
        if (waitForOrder)
        {
            Debug.LogWarning("Unsuspected behavior");
            return;
        }
        StartCoroutine(FailureTimer());
        //orderedRecipe = ChooseOrder();
        orderedRecipe = recipeList.GetFromResultName(order);

        //Spawn on render texture
        renderedObject = Instantiate(orderedRecipe.result, renderPivot.position, Quaternion.identity, renderPivot);
        renderedObject.GetComponent<Rigidbody>().isKinematic = true;
        waitForOrder = true;
        orderStartingInst.start();
        animator.SetTrigger("order");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (waitForOrder && !tutorial)
        {
            if (other.tag == "Ingredient")
            {
                //Debug.Log("Ingredient");
                //Animation ? Check ?
                Ingredient otherIng = other.GetComponent<Ingredient>();
                if (otherIng.ingredientName == orderedRecipe.GetResultName())
                {
                    StartCoroutine(Success(other.gameObject));
                }
                else
                {
                    StartCoroutine(Failure(other.gameObject));
                }

            }
        }
        else if (waitForOrder && tutorial)
        {
            if (other.tag == "Ingredient")
            {
                StartCoroutine(TutorialSuccess(other.gameObject, tutorialRewards[tutorialProgress]));
            }
        }
    }

    private IEnumerator Success(GameObject objectToDestroy)
    {
        animator.SetTrigger("success");
        Destroy(renderedObject);
        DifficultyManager.RegisterSuccess();
        orderGoodInst.start();
        alienAnimator.SetTrigger("success");
        Destroy(objectToDestroy);
        yield return new WaitForSeconds(2.0f);
        alienAnimator.SetTrigger("order");
        yield return new WaitForSeconds(3.0f);
        reward.GetComponent<RewardBox>().rewardTier = orderedRecipe.complexity;
        SendBack(reward);
        waitForOrder = false;
    }

    private IEnumerator TutorialSuccess(GameObject objectToDestroy, GameObject rewardToSend)
    {
        animator.SetTrigger("success");
        Destroy(renderedObject);
        orderGoodInst.start();
        alienAnimator.SetTrigger("success");
        Destroy(objectToDestroy);
        yield return new WaitForSeconds(2.0f);
        alienAnimator.SetTrigger("order");
        yield return new WaitForSeconds(3.0f);
        SendBack(rewardToSend);
        waitForOrder = false;
    }

    private IEnumerator Failure(GameObject objectToDestroy)
    {
        animator.SetTrigger("failure");
        Destroy(renderedObject);
        DifficultyManager.RegisterFailure();
        orderBadInst.start();
        alienAnimator.SetTrigger("failure");
        Destroy(objectToDestroy);
        yield return new WaitForSeconds(2.0f);
        alienAnimator.SetTrigger("order");
        yield return new WaitForSeconds(3.0f);
        int idx = Random.Range(0, punishments.Length);
        SendBack(punishments[idx]);
        waitForOrder = false;
    }

    private void SendBack(GameObject objectToSend)
    {
        GameObject go = Instantiate(objectToSend);
        go.transform.position = alienPivot.position;
        Vector3 direction = hublotPivot.position - go.transform.position;
        go.GetComponent<Rigidbody>().AddForce(direction * 15.0f);
    }

    private Recipe ChooseOrder()
    {
        float mediumProba = DifficultyManager.difficultyScore + 25;
        mediumProba *= 0.01f;
        float hardProba = DifficultyManager.difficultyScore - 15;
        hardProba *= 0.01f;
        float value = 0.0f;
        if (hardProba > 0.0f)
        {
            value = Random.value;
            if (value <= hardProba)
            {
                return recipeList.GetRandom(Recipe.RecipeComplexity.Hard);
            }
        }

        value = Random.value;
        if (value <= mediumProba)
        {
            return recipeList.GetRandom(Recipe.RecipeComplexity.Medium);
        }
        else
        {
            return recipeList.GetRandom(Recipe.RecipeComplexity.Easy);
        }

        Debug.LogWarning("Unable to choose order!");
        return null;
    }

    private Recipe GetOrderFromParser()
    {
        string order = RecipeParser.instance.orderLists[trackIndex].GetNext();
        return recipeList.GetFromResultName(order);
    }

}
