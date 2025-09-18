using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UIElements;
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

    [SerializeField] private List<GameObject> allFoliage = new List<GameObject>();

    void Start() {
        Spawn();
        UpdateTransforms();
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

        // if (Input.GetKey(KeyCode.H)) {
        //     Debug.Log("yay h key pressed");
        //     foreach (var obj in allFoliage) {
        //         if (obj == null) continue;
        //         FoliageInstance inst = obj.GetComponent<FoliageInstance>();
        //         if (inst == null || inst.meshTransform == null) continue;

        //         Vector3 pos = inst.meshTransform.localPosition;
        //         pos.y = 5f; // arbitrary test
        //         inst.meshTransform.localPosition = pos;

        //         inst.meshTransform.localRotation = Quaternion.Euler(30f, 45f, 60f);
        //     }
        // }

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
        float height = UIManager.Instance.getFloralHeightValue();
        float heightVariance = UIManager.Instance.getFloralHeightVarianceValue();
        float tilt = UIManager.Instance.getFloralTiltValue();
        float renderDist = UIManager.Instance.getRenderDistanceValue();
        float area = Mathf.Pow(renderDist, 2);

        ClearFoliage();

        foreach (FoliageType type in foliageTypes) {
            switch (type.id) {
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
                // --- ROOT POSITION ---
                Vector3 pos = new Vector3(
                    Random.Range(-renderDist, renderDist),
                    0f,
                    Random.Range(-renderDist, renderDist)
                );

                GameObject obj = Instantiate(type.prefab, pos, Quaternion.identity, transform);

                // --- CHILD MESH ---
                // The mesh child handles height & tilt
                FoliageInstance instance = obj.GetComponent<FoliageInstance>();
                if (instance == null) {
                    Debug.LogError("Prefab missing FoliageInstance!");
                    continue;
                }

                instance.lockHeight = type.id == "grass"; //grounded
                instance.lockTilt = type.id == "grass"; //not tilting grass
                instance.randomOffset = type.id == "grass" ? 0f : Random.Range(-1f, 1f);
                instance.meshTransform = obj.transform.GetChild(0); // child mesh

                //random tilt
                instance.yaw = Random.Range(0f, 360f);
                instance.tiltDirection = Random.insideUnitCircle;
                if (instance.tiltDirection == Vector2.zero) instance.tiltDirection = Vector2.right;
                instance.tiltDirection.Normalize();

                // --- INITIAL MESH ROTATION & HEIGHT ---
                instance.meshTransform.localRotation = instance.lockTilt
                    ? Quaternion.Euler(0f, instance.yaw, 0f)
                    : Quaternion.Euler(instance.tiltDirection.x * tilt, instance.yaw, instance.tiltDirection.y * tilt);

                allFoliage.Add(obj);
            }
        }
    }

    public void UpdateTransforms() {
        tilt = UIManager.Instance.getFloralTiltValue();
        height = UIManager.Instance.getFloralHeightValue();
        heightVariance = UIManager.Instance.getFloralHeightVarianceValue();

        foreach (GameObject obj in allFoliage) {
            if (obj == null) continue;
            FoliageInstance instance = obj.GetComponent<FoliageInstance>();
            if (instance == null || instance.meshTransform == null) continue;

            // --- CHILD MESH HEIGHT ---
            Vector3 localPos = instance.meshTransform.localPosition;
            localPos.y = instance.lockHeight
                ? 0f
                : height + (instance.randomOffset * heightVariance);
            instance.meshTransform.localPosition = localPos;

            // --- CHILD MESH TILT ---
            instance.meshTransform.localRotation = instance.lockTilt
                ? Quaternion.Euler(0f, instance.yaw, 0f)
                : Quaternion.Euler(instance.tiltDirection.x * tilt, instance.yaw, instance.tiltDirection.y * tilt);

            // --- ROOT TRANSFORM ---
            // Do NOT modify root Y or rotation here; root is strictly for treadmill movement
        }
    }

    public void ClearFoliage() {
        for (int i = allFoliage.Count - 1; i >= 0; i--) {
            Destroy(allFoliage[i]);
        }
        allFoliage.Clear();
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