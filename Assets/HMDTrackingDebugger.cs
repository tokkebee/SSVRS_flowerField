using UnityEngine;

public class HMDTrackingDebugger : MonoBehaviour {
    [SerializeField] private Camera xrCamera; // Assign your XR Rig's Main Camera here

    void Update() {
        if (xrCamera != null) {
            Debug.Log("HMD Rotation: " + xrCamera.transform.rotation.eulerAngles);
        }
        else {
            Debug.LogWarning("No XR Camera assigned.");
        }
    }
}
