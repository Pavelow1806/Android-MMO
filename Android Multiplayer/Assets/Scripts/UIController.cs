using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectedAbility
{
    Fire,
    Laser,
    Poison
}
public class UIController : MonoBehaviour
{
    [Header("Abilities")]
    public Image ability1;
    [HideInInspector]
    public Image ability1icon;
    public Image ability2;
    [HideInInspector]
    public Image ability2icon;
    public Image ability3;
    [HideInInspector]
    public Image ability3icon;
    public GameObject Highlight;
    // Special
    public float SpecialProgress = 0.0f;
    public Color SpecialGradualStart;
    public Color SpecialGradualEnd;
    public Image SpecialGradual;
    public Color SpecialStart;
    public Color SpecialEnd;
    public Image Special;
    private float SpecialCatchupSpeed = 0.25f;
    private float SpecialDelay = 0.5f;
    private DateTime SpecialDelayTime = default;

    [Header("Settings")]
    public GameObject SettingsPanel;
        [Header("Volume")]
        // Master
        public Text MasterVolumeNumber;
        public Slider MasterVolumeSlider;
        // Music
        public Text MusicVolumeNumber;
        public Slider MusicVolumeSlider;
        // SFX
        public Text SFXVolumeNumber;
        public Slider SFXVolumeSlider;
        [Header("Gameplay")]
        // Movement Speed
        public Text SpeedNumber;
        public Slider SpeedSlider;
        // Ability Auto-Fire
        public Toggle AutoFire;

    public SelectedAbility Ability = SelectedAbility.Fire;

    public float MoveSpeed = 5.0f;

    private void Awake()
    {
        // Abilities
        if (ability1 != null)
            ability1icon = ability1.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (ability2 != null)
            ability2icon = ability2.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (ability3 != null)
            ability3icon = ability3.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        // Settings PlayerPrefs
        //      Volume
        if (PlayerPrefs.HasKey("MasterVolume"))
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        else
            PlayerPrefs.SetFloat("MasterVolume", 1.0f);
        if (PlayerPrefs.HasKey("MusicVolume"))
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        else
            PlayerPrefs.SetFloat("MusicVolume", 1.0f);
        if (PlayerPrefs.HasKey("SFXVolume"))
            SFXVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        else
            PlayerPrefs.SetFloat("SFXVolume", 1.0f);
        //      Gameplay
        if (PlayerPrefs.HasKey("Speed"))
            SpeedSlider.value = (PlayerPrefs.GetFloat("Speed") / 2.0f);
        else
            PlayerPrefs.SetFloat("Speed", 1.0f);
        if (PlayerPrefs.HasKey("AutoFire"))
            AutoFire.isOn = (PlayerPrefs.GetInt("AutoFire") == 1 ? true : false);
        else
            PlayerPrefs.SetInt("AutoFire", 0);
    }

    private void Update()
    {
        if (SpecialGradual.fillAmount < Special.fillAmount && DateTime.Now >= SpecialDelayTime)
        {
            SpecialGradual.fillAmount += (SpecialCatchupSpeed * ((Special.fillAmount - SpecialGradual.fillAmount) * 100.0f)) * Time.deltaTime;
        }
    }

    public void Ability1()
    {
        if (SelectedAbility.Fire != Ability) ResetSpecial();
        SetAbility(SelectedAbility.Fire);
    }
    public void Ability2()
    {
        if (SelectedAbility.Laser != Ability) ResetSpecial();
        SetAbility(SelectedAbility.Laser);
    }
    public void Ability3()
    {
        if (SelectedAbility.Poison != Ability) ResetSpecial();
        SetAbility(SelectedAbility.Poison);
    }
    private void SetAbility(SelectedAbility selectedAbility)
    {
        Ability = selectedAbility;
        switch (selectedAbility)
        {
            case SelectedAbility.Fire:
                Highlight.transform.position = ability1.transform.position;
                break;
            case SelectedAbility.Laser:
                Highlight.transform.position = ability2.transform.position;
                break;
            case SelectedAbility.Poison:
                Highlight.transform.position = ability3.transform.position;
                break;
            default:
                break;
        }
    }
    public void Settings()
    {
        SettingsPanel.SetActive(!SettingsPanel.activeInHierarchy);
    }
    public void AutoFireChange(Toggle toggle)
    {
        PlayerPrefs.SetInt("AutoFire", (toggle.isOn ? 1 : 0));
    }
    public void SpeedChange(Slider slider)
    {
        PlayerPrefs.SetFloat("Speed", (slider.value * 2.0f));
        SpeedNumber.text = ((int)(slider.value * 100)).ToString();
    }
    public void MasterVolumeChange(Slider slider)
    {
        PlayerPrefs.SetFloat("MasterVolume", slider.value);
        MusicVolumeSlider.value = slider.value;
        SFXVolumeSlider.value = slider.value;
        MasterVolumeNumber.text = ((int)(slider.value * 100)).ToString();
    }
    public void MusicVolumeChange(Slider slider)
    {
        PlayerPrefs.SetFloat("MusicVolume", slider.value);
        // If the master slider is lower than the proposed new value, set the master to be equal to that of the new value
        if (MasterVolumeSlider.value < slider.value)
        {
            MasterVolumeSlider.value = slider.value;
        }
        MusicVolumeNumber.text = ((int)(slider.value * 100)).ToString();
    }
    public void SFXVolumeChange(Slider slider)
    {
        PlayerPrefs.SetFloat("SFXVolume", slider.value);
        // If the master slider is lower than the proposed new value, set the master to be equal to that of the new value
        if (MasterVolumeSlider.value < slider.value)
        {
            MasterVolumeSlider.value = slider.value;
        }
        SFXVolumeNumber.text = ((int)(slider.value * 100)).ToString();
    }
    public bool IncreaseSpecial(float amount, bool DisableCatcup = false)
    {
        bool SpecialReady = false;

        // Increase the special progress by the amount passed in, not breaching the 100.0f cap
        if (SpecialProgress + amount > 100.0f)
        { 
            SpecialProgress = 100.0f;
        }
        else
            SpecialProgress += amount;
        
        // Quit if the progress is already max
        if (SpecialProgress == 100.0f) SpecialReady = true; // Return true to signify special is ready
        if (SpecialProgress == 100.0f && Special.fillAmount == 1.0f) return true;

        // Change the visual UI representation
        ChangeSpecialColor();

        // Set the time when the gradual bar catches up
        if (!DisableCatcup)
            SpecialDelayTime = DateTime.Now.AddSeconds(SpecialDelay);
        else
            SpecialDelayTime = DateTime.Now;

        return SpecialReady;
    }
    private void ChangeSpecialColor()
    {
        float Perc = SpecialProgress / 100.0f;
        Special.fillAmount = Perc;
        // Change the Special bar to be the new color based on percentage towards completion
        Color temp = new Color((SpecialEnd.r - SpecialStart.r) * Perc, (SpecialEnd.g - SpecialStart.g) * Perc, (SpecialEnd.b - SpecialStart.b) * Perc);
        Special.color = new Color(SpecialStart.r + temp.r, SpecialStart.g + temp.g, SpecialStart.b + temp.b);
        // Change the special gradual bar to be the new color based on percentage towards completion
        temp = new Color((SpecialGradualEnd.r - SpecialGradualStart.r) * Perc, (SpecialGradualEnd.g - SpecialGradualStart.g) * Perc, (SpecialGradualEnd.b - SpecialGradualStart.b) * Perc);
        SpecialGradual.color = new Color(SpecialGradualStart.r + temp.r, SpecialGradualStart.g + temp.g, SpecialGradualStart.b + temp.b);
    }
    public void ResetSpecial()
    {
        SpecialProgress = 0.0f;
        Special.fillAmount = 0.0f;
        SpecialGradual.fillAmount = 0.0f;
    }
    public bool DrainSpecial(float amount)
    {
        if (SpecialProgress - (amount * Time.deltaTime) < 0.0f)
            SpecialProgress = 0.0f;
        else
            SpecialProgress -= (amount * Time.deltaTime);

        ChangeSpecialColor();
        SpecialGradual.fillAmount = SpecialProgress / 100.0f;

        // Let the player class know that the special has finished
        return (SpecialProgress == 0.0f ? true : false);
    }
}
