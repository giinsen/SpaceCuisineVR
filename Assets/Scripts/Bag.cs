using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class Bag : Tool
{
    [Header("Bag parameters")]
    public Transform spawnPosition;
    public GameObject[] objectsToSpawn;
    public float spawnForce = 5.0f;
    public float spawnTimerDuration = 0.5f;

    private float timer = 0.0f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDesactivate()
    {
        base.OnDesactivate();
        timer = 0.0f;
    }

    protected override void ActiveAction()
    {
        interactable.attachedToHand.TriggerHapticPulse(1);
        timer += Time.deltaTime;
        if (timer >= spawnTimerDuration)
        {
            SpawnBag();
            timer = 0.0f;
        }
    }

    private void SpawnBag()
    {
        GameObject prefab = ChooseObjectToSpawn();
        GameObject go = Instantiate(prefab, spawnPosition.position, Quaternion.identity);
        Vector3 originalScale = prefab.transform.localScale;
        Debug.Log(originalScale);
        go.GetComponent<Item>().baseScale = originalScale;
        go.transform.localScale = Vector3.zero;
        Vector3 pushForce = spawnPosition.forward * spawnForce;
        go.GetComponent<Rigidbody>().AddForce(pushForce);
        StartCoroutine(ScaleAnim(go, originalScale, prefab));
    }

    private IEnumerator ScaleAnim(GameObject target, Vector3 scaleTarget, GameObject prefab)
    {
        target.GetComponent<Collider>().enabled = false;
        yield return new WaitForEndOfFrame();
        Vector3 originalScale = prefab.transform.localScale;
        target.GetComponent<Item>().baseScale = prefab.transform.localScale;
        float timer = 0.0f;
        float timerDuration = 0.5f;
        while (timer < timerDuration)
        {
            timer += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(Vector3.zero, scaleTarget, timer / timerDuration);
            yield return new WaitForEndOfFrame();
        }
        target.GetComponent<Collider>().enabled = true;

    }

    private GameObject ChooseObjectToSpawn()
    {
        return objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
    }
}
