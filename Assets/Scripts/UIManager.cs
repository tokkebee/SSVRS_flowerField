using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This script handles all UI interactions.
//Sliders have Listeners
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private FoliageManager FoliageManager;

    [Header("UI Elements")]
    [SerializeField] private Button panelButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider floralDensitySlider;
    [SerializeField] private float floralDensitySliderValue;

    [SerializeField] private Slider renderDistanceSlider;
    [SerializeField] private float renderDistanceSliderValue;

    [SerializeField] private Button applySettingsButton;

    void Awake() {
        // if (Instance != null && Instance != this) {
        //     Destroy(gameObject);
        //     return;
        // }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        panel.SetActive(false);

        //default settings
        setFloralDensityValue(0.2f);
        setRenderDistanceValue(100f);

        //floral density listener
        floralDensitySlider.onValueChanged.AddListener((value) => {
            setFloralDensityValue(value);
            FoliageManager.Spawn();
        });

        //render distance listener
        renderDistanceSlider.onValueChanged.AddListener((value) => {
            //FoliageManager.ClearFoliage();
            setRenderDistanceValue(value);
            GameManager.Instance.fogDistance(value);
            FoliageManager.setTerrainWidth(value);
            FoliageManager.Spawn();
        });
    }

    //setters
    public void setFloralDensityValue(float newValue) {
        floralDensitySliderValue = newValue;
    }
    public void setRenderDistanceValue(float newValue) {
        renderDistanceSliderValue = newValue;
    }

    //getters
    public float getFloralDensityValue() {
        return floralDensitySliderValue;
    }
    public float getRenderDistanceValue() {
        return renderDistanceSliderValue;
    }

    public void panelToggle() {
        if (panel != null) {
            panel.SetActive(!panel.activeSelf);
        }
    }

    public void panelExit() {
        if (panel != null) {
            panel.SetActive(false);
            //GameManager.Instance.Player.GetComponent<PlayerLook>().isDebugMode = true;
        }
    }

    //applies settings by calling all related Spawn() functions
    public void applySettings() {
        FoliageManager.Spawn();
    }
}