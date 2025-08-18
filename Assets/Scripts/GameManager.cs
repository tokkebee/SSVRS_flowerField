using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is the Game Manager. Currently is used to reference Mesh Generator and Player, and to updates Mesh Generator position

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    //public GameObject MeshGenerator;
    public GameObject FoliageManager;
    public GameObject Player;
    public GameObject mainCamera;
    public GameObject debugCamera;


    void Awake() {
        // if (Instance != null && Instance != this) {
        //     Destroy(gameObject);
        //     return;
        // }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        mainCamera.SetActive(true);
        debugCamera.SetActive(false);
    }

    // public void UpdateMeshPosition(Vector3 newPosition) {
    //     MeshGenerator.transform.position = newPosition;
    // }

}