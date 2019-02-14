using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using FMOD;
using FMODUnity;
using FMOD.Studio;


public class Item : MonoBehaviour
{
    [Header("Sound parameters")]
    [EventRef]
    public string collisionEvent = "event:/ENVIRONMENT/CONTACT/CONTACT WALL";
    [EventRef]
    public string staseSound = "event:/TOOLS/STASER/OBJECT STASING";

    [Header("Attractable options")]
    public bool isAttractable = true;
    private float attractableSpeed = 3f;
    

    protected Rigidbody rb;
    protected Throwable throwable;

    private Coroutine staseAnim;
    [HideInInspector]
    public Vector3 baseScale;

    private EventInstance collisionSoundInstance;
    private EventInstance staseInst;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        throwable = GetComponent<Throwable>();
        throwable.onPickUp.AddListener(OnPickup);
        baseScale = transform.localScale;
        collisionSoundInstance = RuntimeManager.CreateInstance(collisionEvent);
        collisionSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        staseInst = RuntimeManager.CreateInstance(staseSound);
        staseInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    protected virtual void OnPickup()
    {
        throwable.interactable.attachedToHand.TriggerHapticPulse(0.15f,1/(0.15f/2),5f);
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

    public virtual void Stase()
    {
        staseInst.start();
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        if (staseAnim != null)
        {
            StopCoroutine(staseAnim);
            transform.localScale = baseScale;
        }
        staseAnim = StartCoroutine(StaseAnimation());
    }

    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Ingredient" && col.gameObject.tag != "Tool")
        {
            collisionSoundInstance.start();
        }
    }


    private IEnumerator StaseAnimation()
    {
        float timer = 0;
        float timerDuration = 0.1f;
        Vector3 targetScale = transform.localScale * 0.7f;
        while (timer < timerDuration)
        {
            transform.localScale = Vector3.Lerp(baseScale, targetScale, timer / timerDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer = 0.0f;
        while (timer < timerDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, baseScale, timer / timerDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = baseScale;
    }
}
