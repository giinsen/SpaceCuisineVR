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
    public Order[] possibleOrders;
    public GameObject[] rewards;
    public GameObject[] punishments;

    [Header("Integration parameters")]
    public Renderer orderVisualizer;
    public Transform alienPivot;
    public Transform hublotPivot;
    public Animator alienAnimator;

    [Header("Sound parameters")]
    public string orderBad = "event:/ENVIRONMENT/ORDER/ORDER BAD";
    public string orderGood = "event:/ENVIRONMENT/ORDER/ORDER GOOD";
    public string orderStarting = "event:/ENVIRONMENT/ORDER/ORDER STARTING";

    private EventInstance orderBadInst;
    private EventInstance orderGoodInst;
    private EventInstance orderStartingInst;

    private int choosenOrderIdx;
    private bool waitForOrder = false;
    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(SelectOrder());

        orderBadInst = RuntimeManager.CreateInstance(orderBad);
        orderGoodInst = RuntimeManager.CreateInstance(orderGood);
        orderStartingInst = RuntimeManager.CreateInstance(orderStarting);
    }

    private IEnumerator SelectOrder()
    {
        animator.SetBool("order", false);
        yield return new WaitForSeconds(5.0f);
        choosenOrderIdx = Random.Range(0, possibleOrders.Length);
        //orderVisualizer.material.SetTexture(0, possibleOrders[choosenOrderIdx].orderVisual);
        orderVisualizer.material.SetTexture("_MainTex", possibleOrders[choosenOrderIdx].orderVisual);
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
                if (otherIng.ingredientName == possibleOrders[choosenOrderIdx].orderName)
                {
                    StartCoroutine(Success(other.gameObject));
                }
                else
                {
                    StartCoroutine(Failure(other.gameObject));
                }
                waitForOrder = false;
            }
        }
    }

    private IEnumerator Success(GameObject objectToDestroy)
    {
        orderGoodInst.start();
        alienAnimator.SetTrigger("Hit");
        Destroy(objectToDestroy);
        alienAnimator.SetTrigger("SendBack");
        yield return new WaitForSeconds(3.0f);
        int idx = Random.Range(0, rewards.Length);
        SendBack(rewards[idx]);
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(SelectOrder());
    }

    private IEnumerator Failure(GameObject objectToDestroy)
    {
        orderBadInst.start();
        alienAnimator.SetTrigger("Hit");
        Destroy(objectToDestroy);
        alienAnimator.SetTrigger("SendBack");
        yield return new WaitForSeconds(3.0f);
        int idx = Random.Range(0, punishments.Length);
        SendBack(punishments[idx]);
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(SelectOrder());
    }

    private void SendBack(GameObject objectToSend)
    {
        GameObject go = Instantiate(objectToSend);
        go.transform.position = alienPivot.position;
        Vector3 direction = hublotPivot.position - go.transform.position;
        go.GetComponent<Rigidbody>().AddForce(direction * 15.0f);
    }
}

[System.Serializable]
public struct Order
{
    public string orderName;
    public UnityEngine.Texture2D orderVisual;
}
