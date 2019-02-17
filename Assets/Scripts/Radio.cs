using FMOD;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Radio : Tool {

    Animator anim;

    private enum RadioState { off, one, two };
    private RadioState state;

    [EventRef]
    public string music1 = "event:/MUSIC/CASSETTE/MUSIC SHABING MIRROR";
    [EventRef]
    public string music2 = "event:/MUSIC/CASSETTE/MUSIC ZIGZAG";
    [EventRef]
    public string switchMusic = "event:/MUSIC/CASSETTE/SWICTH";

    private EventInstance music1Inst;
    private EventInstance music2Inst;
    private EventInstance switchInst;

    protected override void Start ()
    {
        base.Start();
        anim = GetComponent<Animator>();
        
        music1Inst = RuntimeManager.CreateInstance(music1);
        music1Inst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        music2Inst = RuntimeManager.CreateInstance(music2);
        music2Inst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        switchInst = RuntimeManager.CreateInstance(switchMusic);
        switchInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        //anim.SetTrigger("OffTo1");
        //state = RadioState.one;

        music1Inst.start();
        music1Inst.setPaused(true);
        music2Inst.start();
        music2Inst.setPaused(true);
    }

    protected override void Update ()
    {
        if (interactable.attachedToHand != null)
        {
            Hand hand = interactable.attachedToHand;
            if (SteamVR_Input._default.inActions.Teleport.GetStateDown(hand.handType))
            {
                switchInst.start();
                music1Inst.setPaused(true);
                music2Inst.setPaused(true);
                //music1Inst.stop(STOP_MODE.IMMEDIATE);
                //music2Inst.stop(STOP_MODE.IMMEDIATE);

                if (state == RadioState.off)
                {
                    anim.SetTrigger("OffTo1");
                    state = RadioState.one;
                    //music1Inst.start();
                    music1Inst.setPaused(false);
                    UnityEngine.Debug.Log("OffTo1");
                }
                else if (state == RadioState.one)
                {
                    anim.SetTrigger("1To2");
                    state = RadioState.two;
                    //music2Inst.start();
                    music2Inst.setPaused(false);
                    UnityEngine.Debug.Log("1To2");
                }
                else if (state == RadioState.two)
                {
                    anim.SetTrigger("2ToOff");
                    state = RadioState.off;
                    UnityEngine.Debug.Log("2ToOff");
                }
            }
        }
    }

    protected override void ActiveAction()
    {
    }
}
