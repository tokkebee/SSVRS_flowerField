using UnityEngine;

//This script moves player vertically according to the mesh below them.
public class PlayerHeightFollower : MonoBehaviour {
    public float heightOffset = 1.8f;
    public float raycastDistance = 100f;
    public float smoothTime = 0.1f;
    public LayerMask terrainLayer; // ← assign in Inspector to only hit terrain

    private float currentVelocity = 0f;

    void Update() {
        Vector3 origin = transform.position + Vector3.up * 10f; // raycast from above to avoid clipping
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastDistance, terrainLayer)) {
            float targetY = hit.point.y + heightOffset;
            Vector3 pos = transform.position;
            pos.y = Mathf.SmoothDamp(pos.y, targetY, ref currentVelocity, smoothTime);
            transform.position = pos;
        }
    }
}
