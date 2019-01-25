using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class ChaoticFollowerIngredient : Ingredient
{
    [Header("Follower parameters")]
    public float maxSpeed = 15.0f;
    public float pitchAcceleration;
    public float speedAcceleration;
    public float radius;
    public float pushForce = 15.0f;

    [EventRef]
    public string octopusCut = "event:/INGREDIENTS/OCTOPUS/OCTO CUT";
    private EventInstance cutInst;
    [EventRef]
    public string octopusMove = "event:/INGREDIENTS/OCTOPUS/OCTO MOVING";
    private EventInstance moveInst;
    [EventRef]
    public string octopusStase = "event:/INGREDIENTS/OCTOPUS/OCTO STASTE";
    private EventInstance staseInst;

    private Transform target;
    private bool stased = false;

    protected override void Start()
    {
        base.Start();
        if (FindTarget(out target) == false)
        {
            StartCoroutine(Idle());
        }
        cutInst = RuntimeManager.CreateInstance(octopusCut);
        moveInst = RuntimeManager.CreateInstance(octopusMove);
        staseInst = RuntimeManager.CreateInstance(octopusStase);

        moveInst.start();
    }


    private void OnDestroy()
    {
        moveInst.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void MoveTowardTarget()
    {
        Vector3 targetDirection = Vector3.Normalize(target.position - transform.position);
        Vector3 velocity = rb.velocity;
        float actualSpeed = velocity.magnitude;
        Vector3 actualDirection = velocity.normalized;
        velocity = Vector3.MoveTowards(actualDirection, targetDirection, pitchAcceleration*Time.deltaTime) * Mathf.MoveTowards(actualSpeed, maxSpeed, speedAcceleration * Time.deltaTime);
        rb.velocity = velocity;
    }

    private void Update()
    {
        if (stased == false && target != null)
            MoveTowardTarget();
    }

    private bool FindTarget(out Transform result)
    {
        result = null;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        List<Transform> surroundings = new List<Transform>();
        foreach(Collider col in cols)
        {
            if (col.tag == "Ingredient" && col.GetComponent<Ingredient>() != this)
            {
                surroundings.Add(col.transform);
            }
        }
        if (surroundings.Count == 0) return false;
        int idx = Random.Range(0, surroundings.Count);
        result = surroundings[idx];
        return true;
    }

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        if (stased)
        {
            stased = false;
            if (FindTarget(out target) == false)
            {
                StartCoroutine(Idle());
            }
        }
        if (col.transform == target)
        {
            Vector3 direction = Vector3.Normalize(col.rigidbody.transform.position - transform.position);
            col.rigidbody.AddForce(direction * pushForce);
            if (FindTarget(out target) == false)
            {
                StartCoroutine(Idle());
            }
        }
    }

    public override void Stase()
    {
        base.Stase();
        stased = true;
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(5.0f);
        while (FindTarget(out target) == false)
        {
            yield return new WaitForSeconds(5.0f);
        }
    }

    public override void Cut()
    {
        base.Cut();

    }
}
