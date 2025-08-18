using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This script handles all UI interactions.
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private FoliageManager FoliageManager;

    [Header("UI Elements")]
    [SerializeField] private Button panelButton;
    [SerializeField] private GameObject panel;

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
        //render distance slider

        setRenderDistanceValue(50f);

        renderDistanceSlider.onValueChanged.AddListener((value) => {
            setRenderDistanceValue(value);
            FoliageManager.setTerrainWidth(value);
        });
    }

    // //setters
    public void setRenderDistanceValue(float newValue) {
        renderDistanceSliderValue = newValue;
    }

    // //getters
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
        }
    }

    //applies settings by calling all related Spawn() functions
    //applies settings by reloading the scene (stupid fucking idea)
    public void applySettings() {
        FoliageManager.Spawn();
        // Scene currentScene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(currentScene.name);
    }
}