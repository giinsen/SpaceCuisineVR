using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : Tool {

	void Start () 
	{
		
	}
	
	void Update () 
	{
		foreach (Ingredient ingredient in ingredients)
		{
			ingredient.cut();
		}
	}
}
