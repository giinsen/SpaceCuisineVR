using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staser : Tool
{
	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
	}
}
