using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool hasJustSpawned = true;

    private void Start()
    {
        StartCoroutine(HasJustSpawnedTimer());
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
            for (int i = 0; i < numberOfCutResult; ++i)
            {
                Instantiate(cutResult, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
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
}
