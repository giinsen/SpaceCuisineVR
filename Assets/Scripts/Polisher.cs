using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using Debug = UnityEngine.Debug;

public class Polisher : MonoBehaviour {

    public GameObject phase1;
    public GameObject phase2;

    public GameObject particleCircle;
    public GameObject particleBase;
    public GameObject particleFinish;

    [Header("FMOD bullshit")]
    [EventRef]
    public string objectEnter = "event:/TOOLS/POLISHER/OBJECT ENTER";
    [EventRef]
    public string objectExit = "event:/TOOLS/POLISHER/OBJECT EJECT";
    [EventRef]
    public string polishing = "event:/TOOLS/POLISHER/POLISHING";

    private EventInstance enterInst;
    private EventInstance exitInst;
    private EventInstance polishingInst;

    private Ingredient ingredient;

    private bool isPolishing = false;


    private Vector3[] expulsionArray = new[] { new Vector3(0f, 0f, 1f), new Vector3(-0.5f, 0f, -0.4f), new Vector3(0.5f, 0f, -0.4f) };

    private void Start()
    {
        enterInst = RuntimeManager.CreateInstance(objectEnter);
        enterInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        exitInst = RuntimeManager.CreateInstance(objectExit);
        exitInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        polishingInst = RuntimeManager.CreateInstance(polishing);
        polishingInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
    }

    private IEnumerator PolishingSound()
    {
        polishingInst.start();
        polishingInst.setParameterValueByIndex(0, 1.0f);
        float timer = 0.0f;
        float t;
        float timerDuration = 5.0f;
        while (timer < timerDuration)
        {
            t = Mathf.Lerp(1.0f, 0f, timer / timerDuration);
            polishingInst.setParameterValueByIndex(0, t);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer = 0.0f;
        yield return new WaitForSeconds(1.0f);
        while (timer < timerDuration)
        {
            t = Mathf.Lerp(0f, 1.0f, timer / timerDuration);
            polishingInst.setParameterValueByIndex(0, t);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        polishingInst.stop(STOP_MODE.ALLOWFADEOUT);
    }


    private void OnTriggerStay(Collider other)
    {
        if (!isPolishing)
        {
            if (other.gameObject.tag == "Ingredient")
            {
                ingredient = other.gameObject.GetComponent<Ingredient>();
                if (ingredient.isPolishable && ingredient.hasJustBeenThrown)
                {
                    isPolishing = true;
                    StartCoroutine(StartPolishing());
                }
            }
        }
    }

    private IEnumerator StartPolishing()
    {
        enterInst.start();
        StartCoroutine(PolishingSound());
        ingredient.GetComponent<Collider>().enabled = false;
        StartCoroutine(ScaleAnim(ingredient.gameObject, ingredient.polishScale));
        yield return MoveToPhase(ingredient.transform, ingredient.transform.position, phase1.transform.position, 1f);
        yield return MoveToPhase(ingredient.transform, ingredient.transform.position, phase2.transform.position, 1f);
        ingredient.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ingredient.GetComponent<Rigidbody>().isKinematic = true;

        GameObject go1 = Instantiate(particleCircle,phase2.transform.position,Quaternion.identity, this.gameObject.transform);
        GameObject go2 = Instantiate(particleCircle,phase2.transform.position,Quaternion.identity, this.gameObject.transform);
        GameObject go3 = Instantiate(particleBase, phase2.transform.position,Quaternion.identity, this.gameObject.transform);

        yield return RotatePrefabQuickly();
        Destroy(go1); Destroy(go2); Destroy(go3);
        GameObject polishResult = Instantiate(ingredient.polishResult, phase2.transform.position, Quaternion.identity);
        Vector3 polishResultNormalScale = polishResult.transform.localScale;
        polishResult.transform.localScale = ingredient.polishResultScale;            
        Instantiate(particleFinish, phase2.transform.position, Quaternion.identity);

        Destroy(ingredient.gameObject);

        polishResult.GetComponent<Rigidbody>().isKinematic = true;
        polishResult.GetComponent<Collider>().enabled = false;
        StartCoroutine(RotatePrefabPolishQuickly(polishResult));

        yield return new WaitForSeconds(3f);

        StartCoroutine(ScaleAnim(polishResult.gameObject, polishResultNormalScale));
        polishResult.GetComponent<Rigidbody>().isKinematic = false;
        polishResult.GetComponent<Collider>().enabled = true;

        polishResult.GetComponent<Rigidbody>().AddForce(expulsionArray[Random.Range(0,2)] * 3f, ForceMode.Impulse);       
        isPolishing = false;

        exitInst.start();

    }

    private IEnumerator ScaleAnim(GameObject target, Vector3 scaleTarget)
    {
        float timer = 0.0f;
        float timerDuration = 0.5f;
        while (timer < timerDuration)
        {
            timer += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(target.transform.localScale, scaleTarget, timer / timerDuration);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator MoveToPhase(Transform objectToMove, Vector3 a, Vector3 b, float speed)
    {
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; 
            objectToMove.position = Vector3.Lerp(a, b, t); 
            yield return new WaitForFixedUpdate();          
        }
        objectToMove.position = b;
    }

    IEnumerator RotatePrefabQuickly()
    {       
        float t = 0;

        while (t <= 2.5f)
        {
            t += Time.fixedDeltaTime;
            ingredient.transform.Rotate(Vector3.up, 80f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator RotatePrefabPolishQuickly(GameObject go)
    {
        float t = 0;

        while (t <= 3f)
        {
            t += Time.fixedDeltaTime;
            go.transform.Rotate(Vector3.up, 15f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
