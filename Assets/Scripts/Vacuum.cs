using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : Tool
{
	public ParticleSystem particleVacuum;

	public GameObject attractPoint;
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
    }
	protected override void ActiveAction()
	{
		foreach(Ingredient ing in ingredients)
        {
            ing.Attract(attractPoint);
        }
		ingredients.Clear();
	}
}
