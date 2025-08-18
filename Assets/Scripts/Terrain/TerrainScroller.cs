using UnityEngine;

//This script moves the terrain according to player

public class TerrainScroller : MonoBehaviour {
    public MeshGenerator meshGenerator;
    public Transform playerCamera;
    public float scrollSpeed = 2f;

    void Update() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = (right * input.x + forward * input.z);

        meshGenerator.noiseOffset += new Vector2(direction.x, direction.z) * scrollSpeed * Time.deltaTime;
    }
}