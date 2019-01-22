using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staser : Tool
{
	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Stase();
        }
        foreach (Tool tool in tools)
        {
            tool.Stase();
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
}
