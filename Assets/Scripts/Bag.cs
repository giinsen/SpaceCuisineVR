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

    private Interactable interactable;
    private float timer = 0.0f;

    protected override void Start()
    {
        base.Start();
        interactable = GetComponent<Interactable>();
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
        GameObject go = Instantiate(ChooseObjectToSpawn(), spawnPosition.position, Quaternion.identity);
        Vector3 originalScale = go.transform.localScale;
        go.transform.localScale = Vector3.zero;
        Vector3 pushForce = spawnPosition.forward * spawnForce;
        go.GetComponent<Rigidbody>().AddForce(pushForce);
        StartCoroutine(ScaleAnim(go, originalScale));
    }

    private IEnumerator ScaleAnim(GameObject target, Vector3 scaleTarget)
    {
        float timer = 0.0f;
        float timerDuration = 0.8f;
        while (timer < timerDuration)
        {
            timer += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(Vector3.zero, scaleTarget, timer / timerDuration);
            yield return new WaitForEndOfFrame();
        }

    }

    private GameObject ChooseObjectToSpawn()
    {
        return objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
    }
}
