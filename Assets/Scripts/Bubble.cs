using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class Bubble : Item
{
	public List<Ingredient> ingredientsInBubble;
	private Ingredient ingredient;

	private Interactable interactable;

	protected override void Start()
	{
        base.Start();
		interactable = GetComponent<Interactable>();
	}

	public void OnPickUp()
    {
        if (interactable.attachedToHand == null)
        {
            Debug.Log("null ref of doom ! mission abort !");
            return;
        }
        transform.position = interactable.attachedToHand.transform.position;
    }

	private void OnTriggerEnter(Collider other)
	{		
		if (other.gameObject.tag == "Ingredient")
        {
			ingredient = other.gameObject.GetComponent<Ingredient>();
            ingredient.myBubble = this.gameObject;
        }
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Ingredient" && other.gameObject.GetComponent<Ingredient>().inBubble == false)
        {
            ingredient.myBubble = null;
			ingredient = null;
        }
	}
}
