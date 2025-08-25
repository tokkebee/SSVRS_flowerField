using System.Collections;
using System.Collections.Generic;
using Kino;
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

    public void fogDistance(float dist) {
        int min = 100;
        int max = 200;
        float normalizedDist = Mathf.InverseLerp(min, max, dist);

        mainCamera.GetComponent<Fog>().startDistance = (normalizedDist * 50) + 25;

        //min terrain width is 100, and corresponding fog distance is 25
        //medium is 150 and fog distance is 50
        //max width is 200 and corresponding fog distance is 75
    }

    // public void UpdateMeshPosition(Vector3 newPosition) {
    //     MeshGenerator.transform.position = newPosition;
    // }

}