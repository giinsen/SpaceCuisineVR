using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshDistortLite;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class Staser : Tool
{
    public GameObject distortObject;

    [Header("FMOD")]
    [EventRef]
    public string staserOn = "event:/TOOLS/STASER/STASER ON";
    [EventRef]
    public string staserOff = "event:/TOOLS/STASER/SATSER OFF";

    private EventInstance onInst;
    private EventInstance offInst;

    protected override void Start()
    {
        base.Start();
        onInst = RuntimeManager.CreateInstance(staserOn);
        offInst = RuntimeManager.CreateInstance(staserOff);
    }

    protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Stase();
            StartCoroutine(AddDistortion(ing.gameObject));            
        }
        foreach (Item item in otherItems)
        {
            item.Stase();
            StartCoroutine(AddDistortion(item.gameObject));
        }
        ingredients.Clear();
        otherItems.Clear();
	}

    protected override void OnTriggerEnter(Collider other)
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

    private IEnumerator AddDistortion(GameObject go)
    {
        //UnityEditorInternal.ComponentUtility.CopyComponent(distortObject.GetComponent<Distort>());
        //UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
        //go.AddComponent<AnimatedDistort>();
        //yield return new WaitForSeconds(1f);
        //go.GetComponent<Distort>().distort.ToArray()[0].enabled = false;
        //yield return new WaitForEndOfFrame();
        //Destroy(go.GetComponent<AnimatedDistort>());
        //Destroy(go.GetComponent<Distort>());
        yield break;
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
}
