using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoliageManager : MonoBehaviour {
    [System.Serializable]
    public class FoliageType {
        public GameObject prefab;
        public int count = 1000;
        public float foliageDensity = .4f; //TODO
    }

    [Header("Terrain Settings")]
    [SerializeField] private float terrainWidth = 100f;

    [Header("Foliage Settings")]
    public List<FoliageType> foliageTypes;
    [SerializeField] private float height = 1f;
    [SerializeField] private float heightVariance = 0.1f;
    //private float foliageDensity;

    [Header("Movement Settings")]
    //public float scrollSpeed = 5f;

    private List<GameObject> allFoliage = new List<GameObject>();

    void Start() {
        Spawn();
        //UIManager.Instance.getFloralDensityValue();
        //getFoliageDensity();
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
        float heightVariance = UIManager.Instance.getFloralHeightVarianceValue();
        float renderDist = UIManager.Instance.getRenderDistanceValue();
        float area = Mathf.Pow(renderDist, 2);
        ClearFoliage();

        foreach (FoliageType type in foliageTypes) {
            // type.count = Mathf.CeilToInt(type.foliageDensity * area);
            // type.foliageDensity = type.count / area;
            height = UIManager.Instance.getFloralHeightValue();
            type.foliageDensity = UIManager.Instance.getFloralDensityValue();
            type.count = Mathf.CeilToInt(type.foliageDensity * area);

            for (int i = 0; i < type.count; i++) {
                //ensures no flowers are underground or z-fighting
                float ranHeight = heightVariance == 0 ? height : height - Random.Range(-heightVariance, heightVariance);

                Vector3 pos = new Vector3(
                    Random.Range(-renderDist, renderDist),
                    ranHeight, //height
                    Random.Range(-renderDist, renderDist)
                );

                GameObject obj = Instantiate(type.prefab, pos, Quaternion.identity, transform);
                obj.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
                allFoliage.Add(obj);
            }
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