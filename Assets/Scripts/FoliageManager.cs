using System.Collections.Generic;
using UnityEngine;

public class FoliageManager : MonoBehaviour {
    [System.Serializable]
    public class FoliageType {
        public GameObject prefab;
        public int count = 50;
        public float foliageDensity = 0.4f; //TODO
    }

    [Header("Terrain Settings")]
    [SerializeField] private float terrainWidth = 50f;

    [Header("Foliage Settings")]
    public List<FoliageType> foliageTypes;
    private float foliageDensity;

    [Header("Movement Settings")]
    public float scrollSpeed = 5f;

    private List<GameObject> allFoliage = new List<GameObject>();

    void Start() {
        Spawn();
        getFoliageDensity();
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
        //float area = terrainWidth * 2f * 2f;
        float area = UIManager.Instance.getRenderDistanceValue() * 2f * 2f;
        ClearFoliage();

        foreach (FoliageType type in foliageTypes) {
            // if (type.density >= 0f && type.density <= .01f) {
            //     type.density = type.count / area;
            // }

            int adjustedCount = Mathf.CeilToInt(type.foliageDensity * area);
            type.count = adjustedCount;

            for (int i = 0; i < adjustedCount; i++) {
                Vector3 pos = new Vector3(
                    Random.Range(-terrainWidth, terrainWidth),
                    0f,
                    Random.Range(-terrainWidth, terrainWidth)
                );

                GameObject obj = Instantiate(type.prefab, pos, Quaternion.identity, transform);
                obj.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
                allFoliage.Add(obj);
            }
        }

        foliageDensity = getFoliageDensity();
    }

    public void ClearFoliage() {
        for (int i = allFoliage.Count - 1; i >= 0; i--) {
            Destroy(allFoliage[i]);
        }
        allFoliage.Clear();
    }

    public float getFoliageDensity() {
        foliageDensity = allFoliage.Count / (terrainWidth * 2);
        return foliageDensity;
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
        Vector3 movement = inputDirection * scrollSpeed * Time.deltaTime;

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