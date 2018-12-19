using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

[RequireComponent(typeof(Interactable))]
public abstract class Tool : MonoBehaviour
{
    protected List<Ingredient> ingredients;
    public enum Type { continuous, pressable, toggle };
    public Type toolType;

    protected bool active = false;
    protected Interactable interactable;

    protected virtual void Start()
    {
        interactable = GetComponent<Interactable>();

        switch (toolType)
        {
            case Type.continuous:
                active = true;
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
                        active = true;
                        OnActivate();
                    }
                    else if (SteamVR_Input._default.inActions.Teleport.GetStateUp(hand.handType))
                    {
                        active = false;
                        OnDesactivate();
                    }
                }
                else
                {
                    if (active)
                    {
                        active = false;
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
                        active = !active;
                        if (active)
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

        if (active)
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
