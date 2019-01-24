using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polisher : MonoBehaviour {

    public GameObject phase1;
    public GameObject phase2;

    public GameObject particleCircle;
    public GameObject particleBase;
    public GameObject particleFinish;

    private Ingredient ingredient;

    private bool isPolishing = false;



    private void OnTriggerEnter(Collider other)
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
        ingredient.GetComponent<Collider>().enabled = false;
        StartCoroutine(ScaleAnim(ingredient.gameObject, ingredient.polishScale));
        yield return MoveToPhase(ingredient.transform, ingredient.transform.position, phase1.transform.position, 1f);
        yield return MoveToPhase(ingredient.transform, ingredient.transform.position, phase2.transform.position, 1f);
        ingredient.Stase();

        GameObject go;
        go = Instantiate(particleCircle, this.gameObject.transform);
        go.transform.position = new Vector3(0, 1.6f, 0);
        go = Instantiate(particleCircle, this.gameObject.transform);
        go.transform.position = new Vector3(0, 1.6f, 0);
        go = Instantiate(particleBase, this.gameObject.transform);
        go.transform.position = new Vector3(0, 1.2f, 0);

        yield return RotatePrefabQuickly();

        go = ingredient.polishResult;
        Destroy(ingredient.gameObject);
        Instantiate(go);
        Instantiate(particleFinish);
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
            Debug.Log("ok");
            t += Time.fixedDeltaTime;
            ingredient.transform.Rotate(Vector3.up, 40f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
