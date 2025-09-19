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
        [NonSerialized] public List<GameObject> pool = new List<GameObject>();
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

    //setters
    public void setTerrainWidth(float newWidth) {
        terrainWidth = newWidth;
    }

    //getters
    public float getTerrainWidth() {
        return terrainWidth;
    }

    void Start() {
        PrepopulatePools();
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
            if (!obj.activeSelf) continue; // skip inactive pooled objects
            obj.transform.position -= movement;
            WrapAround(obj.transform);
        }
    }

    // --- POOL SETUP ---
    void PrepopulatePools() {
        float renderDist = UIManager.Instance.getRenderDistanceValue();
        float area = Mathf.Pow(renderDist, 2);

        foreach (var type in foliageTypes) {
            int neededCount = Mathf.CeilToInt(type.foliageDensity * area);
            type.pool.Clear();

            for (int i = 0; i < neededCount; i++) {
                GameObject obj = Instantiate(type.prefab, transform);
                obj.SetActive(false);

                // --- Setup child mesh ---
                FoliageInstance instance = obj.GetComponent<FoliageInstance>();
                if (instance == null) {
                    Debug.LogError("Prefab missing FoliageInstance!");
                    continue;
                }

                instance.lockHeight = type.id == "grass";
                instance.lockTilt = type.id == "grass";
                instance.randomOffset = type.id == "grass" ? 0f : Random.Range(-1f, 1f);
                instance.meshTransform = obj.transform.GetChild(0); //child mesh

                instance.yaw = Random.Range(0f, 360f);
                instance.tiltDirection = Random.insideUnitCircle;
                if (instance.tiltDirection == Vector2.zero) instance.tiltDirection = Vector2.right;
                instance.tiltDirection.Normalize();

                //obj.SetActive(false); // hide until spawned
                type.pool.Add(obj);
            }
        }
    }

    // --- SPAWN / RESPAWN USING POOLS ---
    public void Spawn() {
        float height = UIManager.Instance.getFloralHeightValue();
        float heightVariance = UIManager.Instance.getFloralHeightVarianceValue();
        float tilt = UIManager.Instance.getFloralTiltValue();
        float renderDist = UIManager.Instance.getRenderDistanceValue();
        float area = Mathf.Pow(renderDist, 2);

        allFoliage.Clear();

        foreach (FoliageType type in foliageTypes) {
            // update density
            switch (type.id) {
                case "pink": type.foliageDensity = UIManager.Instance.getPinkDensityValue(); break;
                case "blue": type.foliageDensity = UIManager.Instance.getBlueDensityValue(); break;
                case "grass": type.foliageDensity = 1.0f; break;//0.6f; break;
            }

            int neededCount = Mathf.CeilToInt(type.foliageDensity * area);
            neededCount = Mathf.Min(neededCount, type.pool.Count);

            for (int i = 0; i < neededCount; i++) {
                //if (i >= type.pool.Count) break; // safety
                GameObject obj = type.pool[i];
                obj.SetActive(true);

                // --- ROOT POSITION ---
                obj.transform.localPosition = new Vector3(
                    Random.Range(-renderDist, renderDist),
                    0f,
                    Random.Range(-renderDist, renderDist)
                );

                allFoliage.Add(obj);
            }

            // deactivate unused objects
            for (int i = neededCount; i < type.pool.Count; i++) {
                type.pool[i].SetActive(false);
            }
        }

        // update child transforms immediately
        UpdateTransforms();
    }

    // public void Spawn() {
    //     float height = UIManager.Instance.getFloralHeightValue();
    //     float heightVariance = UIManager.Instance.getFloralHeightVarianceValue();
    //     float tilt = UIManager.Instance.getFloralTiltValue();
    //     float renderDist = UIManager.Instance.getRenderDistanceValue();
    //     float area = Mathf.Pow(renderDist, 2);

    //     ClearFoliage();

    //     foreach (FoliageType type in foliageTypes) {
    //         switch (type.id) {
    //             case "pink":
    //                 type.foliageDensity = UIManager.Instance.getPinkDensityValue();
    //                 break;
    //             case "blue":
    //                 type.foliageDensity = UIManager.Instance.getBlueDensityValue();
    //                 break;
    //             case "grass":
    //                 type.foliageDensity = 0.6f; // hardcoded, or inspector value
    //                 break;
    //             default:
    //                 break;
    //         }

    //         type.count = Mathf.CeilToInt(type.foliageDensity * area);

    //         for (int i = 0; i < type.count; i++) {
    //             // --- ROOT POSITION ---
    //             Vector3 pos = new Vector3(
    //                 Random.Range(-renderDist, renderDist),
    //                 0f,
    //                 Random.Range(-renderDist, renderDist)
    //             );

    //             GameObject obj = Instantiate(type.prefab, pos, Quaternion.identity, transform);

    //             // --- CHILD MESH ---
    //             // The mesh child handles height & tilt
    //             FoliageInstance instance = obj.GetComponent<FoliageInstance>();
    //             if (instance == null) {
    //                 Debug.LogError("Prefab missing FoliageInstance!");
    //                 continue;
    //             }

    //             instance.lockHeight = type.id == "grass"; //grounded
    //             instance.lockTilt = type.id == "grass"; //not tilting grass
    //             instance.randomOffset = type.id == "grass" ? 0f : Random.Range(-1f, 1f);
    //             instance.meshTransform = obj.transform.GetChild(0); // child mesh

    //             //random tilt
    //             instance.yaw = Random.Range(0f, 360f);
    //             instance.tiltDirection = Random.insideUnitCircle;
    //             if (instance.tiltDirection == Vector2.zero) instance.tiltDirection = Vector2.right;
    //             instance.tiltDirection.Normalize();

    //             // --- INITIAL MESH ROTATION & HEIGHT ---
    //             instance.meshTransform.localRotation = instance.lockTilt
    //                 ? Quaternion.Euler(0f, instance.yaw, 0f)
    //                 : Quaternion.Euler(instance.tiltDirection.x * tilt, instance.yaw, instance.tiltDirection.y * tilt);

    //             allFoliage.Add(obj);
    //         }
    //     }
    // }

    // public void UpdateTransforms() {
    //     tilt = UIManager.Instance.getFloralTiltValue();
    //     height = UIManager.Instance.getFloralHeightValue();
    //     heightVariance = UIManager.Instance.getFloralHeightVarianceValue();

    //     foreach (GameObject obj in allFoliage) {
    //         if (obj == null) continue;
    //         FoliageInstance instance = obj.GetComponent<FoliageInstance>();
    //         if (instance == null || instance.meshTransform == null) continue;

    //         // --- CHILD MESH HEIGHT ---
    //         Vector3 localPos = instance.meshTransform.localPosition;
    //         localPos.y = instance.lockHeight
    //             ? 0f
    //             : height + (instance.randomOffset * heightVariance);
    //         instance.meshTransform.localPosition = localPos;

    //         // --- CHILD MESH TILT ---
    //         instance.meshTransform.localRotation = instance.lockTilt
    //             ? Quaternion.Euler(0f, instance.yaw, 0f)
    //             : Quaternion.Euler(instance.tiltDirection.x * tilt, instance.yaw, instance.tiltDirection.y * tilt);

    //         // --- ROOT TRANSFORM ---
    //         // Do NOT modify root Y or rotation here; root is strictly for treadmill movement
    //     }
    // }

    // --- UPDATE CHILD HEIGHT/TILT ---
    public void UpdateTransforms() {
        tilt = UIManager.Instance.getFloralTiltValue();
        height = UIManager.Instance.getFloralHeightValue();
        heightVariance = UIManager.Instance.getFloralHeightVarianceValue();

        foreach (GameObject obj in allFoliage) {
            if (obj == null || !obj.activeSelf) continue;

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
        }
    }

    public void ClearFoliage() {
        foreach (var obj in allFoliage) {
            if (obj != null) obj.SetActive(false); // return to pool
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