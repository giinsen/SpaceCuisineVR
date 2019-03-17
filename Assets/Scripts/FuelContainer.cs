using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class FuelContainer : MonoBehaviour {

    public TextMesh t;
    public int nbFuelCharge = 5;
    private int actualFuelCharge = 0;
    [EventRef]
    public string fuelSound;

    private EventInstance soundInst;

	void Start ()
    {
        UpdateRenderTexture();

        soundInst = RuntimeManager.CreateInstance(fuelSound);
        soundInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Fuel_Charge"))
        {
            soundInst.start();
            GameManager.instance.LaunchFusionParticle(other.transform.position);
            Destroy(other.gameObject);
            actualFuelCharge++;
            UpdateRenderTexture();
        }
    }

    private void UpdateRenderTexture()
    {
        t.text = actualFuelCharge.ToString() + " / " + nbFuelCharge.ToString();
    }
}
