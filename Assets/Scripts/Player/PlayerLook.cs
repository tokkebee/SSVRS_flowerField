using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script allows for the player to look around via mouse movement and camera.

public class PlayerLook : MonoBehaviour {
    public int mouseSensitivity = 500;
    public Transform playerBody;
    private float xRotation = 0f;

    void Update() {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * UIManager.Instance.getMouseSensitivityValue() * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * UIManager.Instance.getMouseSensitivityValue() * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
