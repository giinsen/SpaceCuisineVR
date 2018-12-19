using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class Ingredient : MonoBehaviour
{
    [Header("Global")]
    public string name = "Ingredient";

    [Header("Cutting options")]
	public bool isCutable = true;
    public int numberOfCutResult = 2;
    public GameObject cutResult;

    [Header("Polishing options")]
    public bool isPolishable = false;
    public GameObject polishResult;

    protected bool hasJustSpawned = true;
    protected Collider col;
    protected Rigidbody rb;
    protected Interactable interactable;

    protected virtual void Start()
    {
        StartCoroutine(HasJustSpawnedTimer());
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();

    }

    private IEnumerator HasJustSpawnedTimer()
    {
        yield return new WaitForSeconds(1.0f);
        hasJustSpawned = false;
    }

	public void Cut()
    {
		if (isCutable && !hasJustSpawned)
        {
            /*
            for (int i = 0; i < numberOfCutResult; ++i)
            {
                Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, col.bounds.extents) + transform.position;
                Instantiate(cutResult, spawnPosition, Quaternion.identity);
            }
            Destroy(this.gameObject);
            */
            Split(numberOfCutResult, cutResult);
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
        // if (gameObject == null) return;
        for (int i = 0; i < numberOfSplit; ++i)
        {
            if (col == null) Debug.Log("here");
            Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, col.bounds.extents) + transform.position;
            Instantiate(prefabSplitted, spawnPosition, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}
