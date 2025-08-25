using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script allows for the player to look around via mouse movement and camera.

public class PlayerLook : MonoBehaviour {
    public float mouseSensitivity = 1.0f;
    public Transform playerBody;
    private float xRotation = 0f;

    public bool isDebugMode = false; // Toggle UI interaction mode

    void Start() {
        LockCursor();
    }

    void Update() {
        // Toggle debug mode with Escape or another key
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isDebugMode = !isDebugMode;
            if (isDebugMode) {
                UnlockCursor();
            } else {
                LockCursor();
            }
        }

        if (!isDebugMode) {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
