using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : Tool
{
	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Cut();
        }
		ingredients.Clear();
	}
}
