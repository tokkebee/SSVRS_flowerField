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
    [SerializeField] private Slider movementSpeedSlider;
    [SerializeField] private float movementSpeedSliderValue;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private int mouseSensitivitySliderValue;
    [SerializeField] private Slider floralDensitySlider;
    [SerializeField] private float floralDensitySliderValue;

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
        setMovementSpeedValue(5);
        setMouseSensitivityValue(1000);
        setFloralDensityValue(0.2f);
        setRenderDistanceValue(100f);

        //movement speed listener
        movementSpeedSlider.onValueChanged.AddListener((value) => {
            setMovementSpeedValue(value);
        });

        //mouse sensitivity listener
        mouseSensitivitySlider.onValueChanged.AddListener((value) => {
            int intValue = Mathf.RoundToInt(value);
            setMouseSensitivityValue(intValue);
        });

        //floral density listener
        floralDensitySlider.onValueChanged.AddListener((value) => {
            setFloralDensityValue(value);
            FoliageManager.Spawn();
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
    public void setMovementSpeedValue(float newValue) {
        movementSpeedSliderValue = newValue;
    }
    public void setMouseSensitivityValue(int newValue) {
        mouseSensitivitySliderValue = newValue;
    }
    public void setFloralDensityValue(float newValue) {
        floralDensitySliderValue = newValue;
    }
    public void setRenderDistanceValue(float newValue) {
        renderDistanceSliderValue = newValue;
    }

    //getters
    public float getMovementSpeedValue() {
        return movementSpeedSliderValue;
    }
    public int getMouseSensitivityValue() {
        return mouseSensitivitySliderValue;
    }
    public float getFloralDensityValue() {
        return floralDensitySliderValue;
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