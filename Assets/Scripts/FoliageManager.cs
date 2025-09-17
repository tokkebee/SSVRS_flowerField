using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoliageManager : MonoBehaviour {
    [System.Serializable]
    public class FoliageType {
        public string id; //IMPORTANT: "pink", "blue", "grass"
        public GameObject prefab;
        public float foliageDensity = 0.4f;
        public int count;
    }

    [Header("Terrain Settings")]
    [SerializeField] private float terrainWidth = 100f;

    [Header("Foliage Settings")]
    public List<FoliageType> foliageTypes;
    [SerializeField] private float height = 1f;
    [SerializeField] private float heightVariance = 0.1f;
    [SerializeField] private float tilt = 0f;

    [Header("Movement Settings")]
    //public float scrollSpeed = 5f;

    private List<GameObject> allFoliage = new List<GameObject>();

    void Start() {
        Spawn();
    }

    //setters
    public void setTerrainWidth(float newWidth) {
        terrainWidth = newWidth;
    }

    //getters
    public float getTerrainWidth() {
        return terrainWidth;
    }

    public void Spawn() {
        //heightVariance = UIManager.Instance.getFloralHeightVarianceValue();
        float renderDist = UIManager.Instance.getRenderDistanceValue();
        float area = Mathf.Pow(renderDist, 2);
        ClearFoliage();

        foreach (FoliageType type in foliageTypes) {
            // tilt = UIManager.Instance.getFloralTiltValue();
            // height = UIManager.Instance.getFloralHeightValue();
            type.count = Mathf.CeilToInt(type.foliageDensity * area);

            switch (type.id) { //NAMES ARE HARDCODED
                case "pink":
                    type.foliageDensity = UIManager.Instance.getPinkDensityValue();
                    break;
                case "blue":
                    type.foliageDensity = UIManager.Instance.getBlueDensityValue();
                    break;
                case "grass":
                    type.foliageDensity = 0.6f; // hardcoded, or inspector value
                    break;
                default:
                    break;
            }
            type.count = Mathf.CeilToInt(type.foliageDensity * area);


            for (int i = 0; i < type.count; i++) {
                // //ensures no flowers are underground or z-fighting; grass is strictly on the ground
                float yPos;
                if (type.prefab.name == "IL3DN_Plant_Grass_01")
                    yPos = 0f;
                else if (heightVariance == 0)
                    yPos = height;
                else
                    yPos = height - Random.Range(-heightVariance, heightVariance);


                //position
                Vector3 pos = new Vector3(
                    Random.Range(-renderDist, renderDist),
                    yPos, //height
                    Random.Range(-renderDist, renderDist)
                );

                // //vector3 for new rotation parameter
                // //xyz rotation
                // Vector3 rot = new Vector3(
                //     Random.Range(-tilt, tilt),
                //     Random.Range(0f, 360f),
                //     Random.Range(-tilt, tilt)
                // );

                GameObject obj = Instantiate(type.prefab, pos, Quaternion.identity, transform);
                //obj.transform.Rotate(rot);
                allFoliage.Add(obj);
            }
        }
    }

    public void UpdateTransforms() {
        float tilt = UIManager.Instance.getFloralTiltValue();
        float height = UIManager.Instance.getFloralHeightValue();
        float variance = UIManager.Instance.getFloralHeightVarianceValue();

        foreach (GameObject obj in allFoliage) {
            Vector3 pos = obj.transform.position;

            // grass stays grounded
            if (obj.name.Contains("Grass")) {
                pos.y = 0f;
            }
            else {
                pos.y = variance == 0 ? height : height - Random.Range(-variance, variance);
            }
            obj.transform.position = pos;

            // new tilt
            Vector3 rot = new Vector3(
                Random.Range(-tilt, tilt),
                obj.transform.rotation.eulerAngles.y, // keep yaw
                Random.Range(-tilt, tilt)
            );
            obj.transform.rotation = Quaternion.Euler(rot);
        }
    }

    public void ClearFoliage() {
        for (int i = allFoliage.Count - 1; i >= 0; i--) {
            Destroy(allFoliage[i]);
        }
        allFoliage.Clear();
    }

    void Update() {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDirection = (camForward * inputZ + camRight * inputX).normalized;
        Vector3 movement = inputDirection * UIManager.Instance.getMovementSpeedValue() * Time.deltaTime;

        foreach (GameObject obj in allFoliage) {
            obj.transform.position -= movement;
            WrapAround(obj.transform);
        }
    }

    void WrapAround(Transform obj) {
        Vector3 pos = obj.position;
        float halfWidth = terrainWidth / 2f;

        if (pos.x < -halfWidth) pos.x += terrainWidth;
        else if (pos.x > halfWidth) pos.x -= terrainWidth;

        if (pos.z < -halfWidth) pos.z += terrainWidth;
        else if (pos.z > halfWidth) pos.z -= terrainWidth;

        obj.position = pos;
    }
}