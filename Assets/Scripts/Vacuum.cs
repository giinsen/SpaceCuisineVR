using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : Tool
{
	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Attract(interactable.attachedToHand);
        }
	}
}
