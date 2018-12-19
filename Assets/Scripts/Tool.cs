using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

[RequireComponent(typeof(Interactable))]
public abstract class Tool : MonoBehaviour
{
    protected List<Ingredient> ingredients = new List<Ingredient>();
    public enum Type { continuous, pressable, toggle };
    public Type toolType;

    public bool hasBeenEnabled = false;
    protected Interactable interactable;
    public Vector3 addRot;

    protected virtual void Start()
    {
        interactable = GetComponent<Interactable>();

        switch (toolType)
        {
            case Type.continuous:
                hasBeenEnabled = true;
                break;
        }
    }

    protected virtual void Update()
    {
        //Handle input to switch activation
        switch (toolType)
        {
            case Type.continuous:
                //is always true
                break;

            case Type.pressable:
                if (interactable.attachedToHand != null)
                {
                    Hand hand = interactable.attachedToHand;
                    if (SteamVR_Input._default.inActions.Teleport.GetStateDown(hand.handType))
                    {
                        hasBeenEnabled = true;
                        OnActivate();
                    }
                    else if (SteamVR_Input._default.inActions.Teleport.GetStateUp(hand.handType))
                    {
                        hasBeenEnabled = false;
                        OnDesactivate();
                    }
                }
                else
                {
                    if (hasBeenEnabled)
                    {
                        hasBeenEnabled = false;
                        OnDesactivate();
                    }
                }
                break;

            case Type.toggle:
                if (interactable.attachedToHand != null)
                {
                    Hand hand = interactable.attachedToHand;
                    if (SteamVR_Input._default.inActions.Teleport.GetStateDown(hand.handType))
                    {
                        hasBeenEnabled = !hasBeenEnabled;
                        if (hasBeenEnabled)
                        {
                            OnActivate();
                        }
                        else
                        {
                            OnDesactivate();
                        }
                    }
                }
                break;
        }

        if (hasBeenEnabled)
        {
            ActiveAction();
        }

    }

    protected abstract void ActiveAction();

    protected virtual void OnActivate()
    {
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            anim.SetBool("active", true);
        }
    }

    protected virtual void OnDesactivate()
    {
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            anim.SetBool("active", false);
        }
    }

    public virtual void OnPickUp()
    {
        transform.position = interactable.attachedToHand.transform.position;
        transform.rotation = interactable.attachedToHand.transform.rotation * Quaternion.Euler(addRot);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            ingredients.Add(other.gameObject.GetComponent<Ingredient>());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            ingredients.Remove(other.gameObject.GetComponent<Ingredient>());
        }
    }
}
