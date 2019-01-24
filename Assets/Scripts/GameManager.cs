using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class GameManager : MonoBehaviour
{
    [Header("Global Parameters")]
    public RecipeList recipeList;
    public float minVelocityToFusion = 10.0f;
    public static GameManager instance;
    public float minSpeedToGetThroughHolowalls = 3.0f;

    private List<GameObject> blacklist = new List<GameObject>();

    private List<Request> requests = new List<Request>();

    public GameObject cutParticle;
    public GameObject fusionParticle;


    private struct Request
    {
        public GameObject other;
        public GameObject me;
        public Recipe recipe;
        public Vector3 velocity;
        public Vector3 position;

        public Request(GameObject me, GameObject other, Recipe recipe, Vector3 velocity, Vector3 position)
        {
            this.me = me;
            this.other = other;
            this.recipe = recipe;
            this.velocity = velocity;
            this.position = position;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        //SteamVR_Utils.Event.Send("hide_render_models", !visible);
    }


    private void Update()
    {
        if (requests.Count > 1)
        {
            if (requests[0].me == requests[1].other && requests[1].me == requests[0].other)
            {
                Vector3 greaterVelocity = requests[0].velocity.magnitude > requests[1].velocity.magnitude ? requests[0].velocity : requests[1].velocity;
                RecipeSpawn(requests[0], greaterVelocity);
            }
            else
            {
                Debug.LogWarning("Conflicting requests!!!");
            }

            requests.RemoveAt(0);
            requests.RemoveAt(0);
        }
    }

    private void RecipeSpawn(Request r, Vector3 velocity)
    {
        GameObject go = Instantiate(r.recipe.result, r.position, Quaternion.identity);
        Destroy(GameObject.Find("Highlighter"));
        if (r.recipe.velocityOnSpawn)
        {
            
            go.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
        }
        LaunchFusionParticle(r.position);
        Destroy(r.me);
        Destroy(r.other);
    }

    public void SendRecipeRequest(Recipe recipe, GameObject me, GameObject other, Vector3 position, Vector3 velocity)
    {
        Request request = new Request(me, other, recipe, velocity, position);
        requests.Add(request);
    }

    public void LaunchCutterParticle(Vector3 t)
    {
        //cutterParticle.Play();
        Instantiate(cutParticle, t, Quaternion.identity);
    }

    public void LaunchFusionParticle(Vector3 t)
    {
        //cutterParticle.Play();
        Instantiate(fusionParticle, t, Quaternion.identity);
    }
}
