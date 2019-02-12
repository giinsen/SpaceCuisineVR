using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using Debug = UnityEngine.Debug;

public class Vacuum : Tool
{
	public ParticleSystem particleVacuum;
	public GameObject attractPoint;
    public Renderer coneRenderer;

    [Header("FMOD")]
    [EventRef]
    public string vacuumOn = "event:/TOOLS/VACUUM/VACUUM ON";

    private EventInstance onInst;

    protected override void Start()
    {
        base.Start();
        throwable.onDetachFromHand.AddListener(OnDrop);
        coneRenderer.enabled = false;
        onInst = RuntimeManager.CreateInstance(vacuumOn);
        onInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    protected override void OnPickup()
    {
        base.OnPickUp();
        coneRenderer.enabled = true;
    }

    private void OnDrop()
    {
        coneRenderer.enabled = false;
    }

    protected override void Update()
	{
		base.Update();
		if (hasBeenEnabled)
			particleVacuum.Play();	
		else {
			particleVacuum.Pause();
			particleVacuum.Clear();
		}			
	}

    protected void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            ingredients.Add(other.gameObject.GetComponent<Ingredient>());
        }

        if (other.gameObject.tag == "Tool")
        {
            otherItems.Add(other.gameObject.GetComponent<Item>());
        }
    }


	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            if (ing == null) continue;
            ing.Attract(attractPoint);
        }

        foreach (Item item in otherItems)
        {
            if (item == null) continue;
            item.Attract(attractPoint);
        }
        ingredients.Clear();
        otherItems.Clear();
	}


    protected override void OnActivate()
    {
        base.OnActivate();
        onInst.start();
    }

    protected override void OnDesactivate()
    {
        base.OnDesactivate();
        onInst.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
