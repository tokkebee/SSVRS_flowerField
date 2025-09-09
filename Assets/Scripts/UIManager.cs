using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//This script handles all UI interactions.
//Sliders have Listeners
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private FoliageManager FoliageManager;

    [Header("Scripts")]
    [SerializeField] private PlayerLook PlayerLook;

    [Header("UI Elements")]
    [SerializeField] private Button panelButton;
    [SerializeField] public GameObject panel;
    //foliage
    [SerializeField] private Slider floralTiltSlider;
    [SerializeField] private float floralTiltSliderValue;
    [SerializeField] private Slider floralHeightSlider;
    [SerializeField] private float floralHeightSliderValue;
    [SerializeField] private Slider floralHeightVarianceSlider;
    [SerializeField] private float floralHeightVarianceSliderValue;
    [SerializeField] private Slider floralDensitySlider;
    [SerializeField] private float floralDensitySliderValue;
    //mechanical
    [SerializeField] private Slider movementSpeedSlider;
    [SerializeField] private float movementSpeedSliderValue;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private int mouseSensitivitySliderValue;

    [SerializeField] private Slider renderDistanceSlider;
    [SerializeField] private float renderDistanceSliderValue;

    [SerializeField] private Button applySettingsButton;

    void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        lockCursor();
        panel.SetActive(false);

        //default settings
        setFloralTiltValue(.5f);
        setFloralHeightValue(1f);
        setFloralHeightVarianceValue(0f);
        setFloralDensityValue(0.2f);
        setMovementSpeedValue(5);
        setMouseSensitivityValue(1000);
        setRenderDistanceValue(100f);

        //floral tilt listener
        floralTiltSlider.onValueChanged.AddListener((value) => {
            setFloralTiltValue(value);
            FoliageManager.Spawn();
        });

        //floral height listener
        floralHeightSlider.onValueChanged.AddListener((value) => {
            setFloralHeightValue(value);
            FoliageManager.Spawn();
        });

        //floral height variance listener
        floralHeightVarianceSlider.onValueChanged.AddListener((value) => {
            setFloralHeightVarianceValue(value);
            FoliageManager.Spawn();
        });

        //floral density listener
        floralDensitySlider.onValueChanged.AddListener((value) => {
            setFloralDensityValue(value);
            FoliageManager.Spawn();
        });

        //movement speed listener
        movementSpeedSlider.onValueChanged.AddListener((value) => {
            setMovementSpeedValue(value);
        });

        //mouse sensitivity listener
        mouseSensitivitySlider.onValueChanged.AddListener((value) => {
            int intValue = Mathf.RoundToInt(value);
            setMouseSensitivityValue(intValue);
        });

        //render distance listener
        renderDistanceSlider.onValueChanged.AddListener((value) => {
            setRenderDistanceValue(value);
            GameManager.Instance.fogDistance(value);
            FoliageManager.setTerrainWidth(value);
            FoliageManager.Spawn();
        });
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!panel.activeSelf) {
                panelEnter();
            } else {
                panelExit();
            }
        }
    }

    //setters
    public void setFloralTiltValue(float newValue) {
        floralTiltSliderValue = newValue;
    }
    public void setFloralHeightValue(float newValue) {
        floralHeightSliderValue = newValue;
    }
    public void setFloralHeightVarianceValue(float newValue) {
        floralHeightVarianceSliderValue = newValue;
    }
    public void setFloralDensityValue(float newValue) {
        floralDensitySliderValue = newValue;
    }
    public void setMovementSpeedValue(float newValue) {
        movementSpeedSliderValue = newValue;
    }
    public void setMouseSensitivityValue(int newValue) {
        mouseSensitivitySliderValue = newValue;
    }
    public void setRenderDistanceValue(float newValue) {
        renderDistanceSliderValue = newValue;
    }

    //getters
    public float getFloralTiltValue() {
        return floralTiltSliderValue;
    }
    public float getFloralHeightValue() {
        return floralHeightSliderValue;
    }
    public float getFloralHeightVarianceValue() {
        return floralHeightVarianceSliderValue;
    }
    public float getFloralDensityValue() {
        return floralDensitySliderValue;
    }
    public float getMovementSpeedValue() {
        return movementSpeedSliderValue;
    }
    public int getMouseSensitivityValue() {
        return mouseSensitivitySliderValue;
    }
    public float getRenderDistanceValue() {
        return renderDistanceSliderValue;
    }

    public void panelEnter() {
        if (panel != null) {
            panel.SetActive(true);
            unlockCursor();
        }
    }

    public void panelExit() {
        if (panel != null) {
            panel.SetActive(false);
            lockCursor();
        }
    }

    //applies settings by calling all related Spawn() functions
    public void applySettings() {
        FoliageManager.Spawn();
    }

    //cursor lock/unlock
    void lockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void unlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}