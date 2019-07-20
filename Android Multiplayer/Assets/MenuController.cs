using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TreeType
{
    Fire,
    Arcane,
    Poison
}

public class MenuController : MonoBehaviour
{
    [Header("Visibility")]
    public GameObject SettingsMenu;
    public GameObject TalentMenu;
    public GameObject AbilityMenu;

    [Header("Settings")]
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
    // Camera Distance
    public Text CameraNumber;
    public Slider CameraSlider;

    [Header("Talents")]
    public int RemainingPoints = 15;
    public int FireTalentMax = 0;
    public int ArcaneTalentMax = 0;
    public int PoisonTalentMax = 0;

    private void Awake()
    {
        #region Settings Player Prefs
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
        if (PlayerPrefs.HasKey("CameraDistance"))
            CameraSlider.value = PlayerPrefs.GetFloat("CameraDistance");
        else
            PlayerPrefs.SetFloat("CameraDistance", 2.0f);
        #endregion
    }

    #region General
    public void LoadGame()
    {
        SceneManager.LoadScene("Arena");
    }
    public void Settings()
    {
        SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
    }
    public void Talents()
    {
        TalentMenu.SetActive(!TalentMenu.activeInHierarchy);
    }
    public void Abilities()
    {
        AbilityMenu.SetActive(!AbilityMenu.activeInHierarchy);
    }
    #endregion

    #region Settings
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
    public void CameraDistanceChange(Slider slider)
    {
        PlayerPrefs.SetFloat("CameraDistance", slider.value);
        CameraNumber.text = ((int)(slider.value * 100)).ToString();
        Camera.main.orthographicSize = (5.0f * slider.value) + 5.0f;
    }
    public void CloseSettings()
    {
        SettingsMenu.SetActive(false);
    }
    #endregion

    #region Talents
    public void ToggleTalent(Button button)
    {
        int row = Convert.ToInt32(button.name.Substring(6, 1));
        if (row < 6)
        {
            string pos = button.name.Substring(7, 1);
        }
        if (RemainingPoints > 0)
        {
            TreeType type = GetType(button);
            switch (type)
            {
                case TreeType.Fire:
                    if (FireTalentMax + 1 <= row) // TBC
                    {
                        return;
                    }
                    if (button.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteParticleEmitter.UIParticleRenderer>().enabled)
                    {
                        --FireTalentMax;
                        ++RemainingPoints;
                    }
                    else
                    {
                        ++FireTalentMax;
                        --RemainingPoints;
                    }
                    break;
                case TreeType.Arcane:
                    if (ArcaneTalentMax + 1 != row)
                    {
                        return;
                    }
                    if (button.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteParticleEmitter.UIParticleRenderer>().enabled)
                    {
                        --ArcaneTalentMax;
                        ++RemainingPoints;
                    }
                    else
                    {
                        ++ArcaneTalentMax;
                        --RemainingPoints;
                    }
                    break;
                case TreeType.Poison:
                    if (PoisonTalentMax + 1 != row)
                    {
                        return;
                    }
                    if (button.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteParticleEmitter.UIParticleRenderer>().enabled)
                    {
                        --PoisonTalentMax;
                        ++RemainingPoints;
                    }
                    else
                    {
                        ++PoisonTalentMax;
                        --RemainingPoints;
                    }
                    break;
                default:
                    break;
            }
            SpriteParticleEmitter.UIParticleRenderer ps;
            for (int i = 0; i < 3; i++)
            {
                ps = button.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<SpriteParticleEmitter.UIParticleRenderer>();
                ps.enabled = !ps.enabled;
            }
        }
    }
    public void CloseTalents()
    {
        TalentMenu.SetActive(false);
    }

    private TreeType GetType(Button button)
    {
        string ParentName = button.transform.parent.name.Substring(0, 1);
        switch (ParentName)
        {
            case "F":
                return TreeType.Fire;
            case "A":
                return TreeType.Arcane;
            case "P":
                return TreeType.Poison;
            default:
                return TreeType.Fire;
        }
    }
    #endregion
}
