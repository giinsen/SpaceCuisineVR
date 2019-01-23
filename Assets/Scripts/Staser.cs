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
