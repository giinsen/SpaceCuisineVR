using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static bool AlmostEqual(Vector3 a, Vector3 b, float tolerance = 0.1f)
    {
        bool x = Mathf.Abs(a.x - b.x) <= tolerance;
        bool y = Mathf.Abs(a.y - b.y) <= tolerance;
        bool z = Mathf.Abs(a.z - b.z) <= tolerance;
        return x && y && z;
    }

    public static bool AlmostEqual(float a, float b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }
}
