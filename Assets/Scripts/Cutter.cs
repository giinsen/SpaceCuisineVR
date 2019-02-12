using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class Cutter : Tool
{
    [Header("Sound parameters")]
    [EventRef]
    public string laserCut = "event:/TOOLS/LASER SABER/LASER SABER CUT";
    [EventRef]
    public string laserOff = "event:/TOOLS/LASER SABER/LASER SABER OFF";
    [EventRef]
    public string laserOn = "event:/TOOLS/LASER SABER/LASER SABER ON";

    private EventInstance cutInst;
    private EventInstance offInst;
    private EventInstance onInst;


    protected override void Start()
    {
        base.Start();
        cutInst = RuntimeManager.CreateInstance(laserCut);
        cutInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        offInst = RuntimeManager.CreateInstance(laserOff);
        offInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        onInst = RuntimeManager.CreateInstance(laserOn);
        onInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        onInst.start();
    }

    protected override void OnDesactivate()
    {
        base.OnDesactivate();
        offInst.start();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.tag == "Ingredient")
        {
            cutInst.start();
        }
    }

    protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Cut();
        }
		ingredients.Clear();
	}
}
