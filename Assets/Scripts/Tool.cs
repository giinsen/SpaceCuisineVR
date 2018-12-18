using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour {

	protected List<Ingredient> ingredients;
	public enum Type {continuous, pressable_enter, pressable_stay}; 
	public Type toolType; 

	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Ingredient")
		{
			ingredients.Add(other.gameObject.GetComponent<Ingredient>());
		}
    }

	private void OnTriggerExit(Collider other)
    {
		if (other.gameObject.tag == "Ingredient")
		{
			ingredients.Remove(other.gameObject.GetComponent<Ingredient>());
		}
    }
}
