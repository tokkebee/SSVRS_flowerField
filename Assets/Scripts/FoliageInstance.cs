using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoliageInstance : MonoBehaviour {
    public string typeId;        // e.g. "pink", "blue", "grass"
    public float yaw;            // stable yaw
    public bool lockHeight;      // ground lock
    public bool lockTilt;        // tilt lock
    public Vector2 tiltDirection;      // normalized direction for tilt (x,z)

    public float baseHeight;      // the central slider height when spawned
    public float randomOffset;    // per-instance random offset

    public Transform meshTransform;
}
