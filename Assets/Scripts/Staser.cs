using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshDistortLite;

public class Staser : Tool
{
    public GameObject distortObject; 

	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Stase();
            Debug.Log("oui");
            StartCoroutine(AddDistortion(ing.gameObject));            
        }
        foreach (Tool tool in tools)
        {
            tool.Stase();
            StartCoroutine(AddDistortion(tool.gameObject));
        }
        ingredients.Clear();
        tools.Clear();
	}

    public void randomf()
    {
        foreach (Ingredient ing in ingredients)
        {
            //ing.Stase();
            Debug.Log("oui");
            StartCoroutine(AddDistortion(ing.gameObject));
        }
        foreach (Tool tool in tools)
        {
            tool.Stase();
            StartCoroutine(AddDistortion(tool.gameObject));
        }
        ingredients.Clear();
        tools.Clear();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            ingredients.Add(other.gameObject.GetComponent<Ingredient>());
        }
        if (other.gameObject.tag == "Tool")
        {
            tools.Add(other.gameObject.GetComponent<Tool>());
        }
    }

    private IEnumerator AddDistortion(GameObject go)
    {
        UnityEditorInternal.ComponentUtility.CopyComponent(distortObject.GetComponent<Distort>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
        go.AddComponent<AnimatedDistort>();
        yield return new WaitForSeconds(1f);
        go.GetComponent<Distort>().distort.ToArray()[0].enabled = false;
        yield return new WaitForEndOfFrame();
        Destroy(go.GetComponent<AnimatedDistort>());
        Destroy(go.GetComponent<Distort>());
        yield break;
    }
}
