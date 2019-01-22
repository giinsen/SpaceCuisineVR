using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int choosenOrderIdx;
    private bool waitForOrder = false;
    private Animator animator;


    private void Start()
    {
        StartCoroutine(SelectOrder());
        animator = GetComponent<Animator>();
    }

    private IEnumerator SelectOrder()
    {
        yield return new WaitForSeconds(30.0f);
        choosenOrderIdx = Random.Range(0, possibleOrders.Length);
        orderVisualizer.material.SetTexture(0, possibleOrders[choosenOrderIdx].orderVisual);
        waitForOrder = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (waitForOrder)
        {
            if (other.tag == "Ingredient")
            {
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
