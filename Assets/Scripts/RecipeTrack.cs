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
    public Order[] possibleOrders;
    public GameObject[] rewards;
    public GameObject[] punishments;

    [Header("Integration parameters")]
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

    private int choosenOrderIdx;
    private Recipe orderedRecipe;
    private GameObject renderedObject;
    [HideInInspector]
    public bool waitForOrder = false;
    private Animator animator;
    private Coroutine failureTimer;

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
    }

    private IEnumerator FailureTimer()
    {
        yield return new WaitForSeconds(120.0f);
        if (waitForOrder == true)
        {
            waitForOrder = false;
            StartCoroutine(Failure(null));
        }
    }

    public void RequestOrder()
    {
        StartCoroutine(FailureTimer());
        orderedRecipe = ChooseOrder();
        //Spawn on render texture
        renderedObject = Instantiate(orderedRecipe.result, renderPivot.position, Quaternion.identity, renderPivot);
        renderedObject.GetComponent<Rigidbody>().isKinematic = true;
        waitForOrder = true;
        orderStartingInst.start();
        animator.SetBool("order", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (waitForOrder)
        {
            if (other.tag == "Ingredient")
            {
                Debug.Log("Ingredient");
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
                Destroy(renderedObject);
                waitForOrder = false;
            }
        }
    }

    private IEnumerator Success(GameObject objectToDestroy)
    {
        DifficultyManager.RegisterSuccess();
        orderGoodInst.start();
        alienAnimator.SetTrigger("Hit");
        Destroy(objectToDestroy);
        alienAnimator.SetTrigger("SendBack");
        yield return new WaitForSeconds(3.0f);
        int idx = Random.Range(0, rewards.Length);
        SendBack(rewards[idx]);
    }

    private IEnumerator Failure(GameObject objectToDestroy)
    {
        DifficultyManager.RegisterFailure();
        orderBadInst.start();
        alienAnimator.SetTrigger("Hit");
        Destroy(objectToDestroy);
        alienAnimator.SetTrigger("SendBack");
        yield return new WaitForSeconds(3.0f);
        int idx = Random.Range(0, punishments.Length);
        SendBack(punishments[idx]);
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
        else
        {
            value = Random.value;
            if (value <= mediumProba)
            {
                return recipeList.GetRandom(Recipe.RecipeComplexity.Medium);
            }
            else
            {
                return recipeList.GetRandom(Recipe.RecipeComplexity.Easy);
            }
        }
        Debug.LogWarning("Unable to choose order!");
        return null;
    }

}




[System.Serializable]
public struct Order
{
    public string orderName;
    public UnityEngine.Texture2D orderVisual;
}
