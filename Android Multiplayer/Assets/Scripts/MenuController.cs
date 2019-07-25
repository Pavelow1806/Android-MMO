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
    public SpellControl SC;

    private void Awake()
    {
        if (SC == null) SC = GameObject.Find("SpellControl").GetComponent<SpellControl>();

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
    private void Start()
    {
        LoadTalents();
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
        int pos = 0;
        if (row < 6) { pos = GetPos(button.name.Substring(7, 1)); }
        TreeType type = GetType(button);
        if (SC.ActivateTalent(type, row, pos))
        {
            SpriteParticleEmitter.UIParticleRenderer ps;
            for (int i = 0; i < 3; i++)
            {
                ps = button.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<SpriteParticleEmitter.UIParticleRenderer>();
                ps.enabled = !ps.enabled;
            }
        }
        else
        {
            Debug.Log("Talent choice declined");
        }
    }
    private int GetPos(string pos)
    {
        if (pos == "R")
        {
            return 2;
        }
        else if (pos == "L")
        {
            return 1;
        }
        else
        {
            return 0;
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
    private void LoadTalents()
    {
        var v = Enum.GetValues(typeof(TreeType));
        foreach (var i in v)
        {
            Dictionary<int, Talent> t = SC.GetTalents((TreeType)i);
            foreach (KeyValuePair<int, Talent> talent in t)
            {
                if (talent.Value.Selected != 0) ToggleTalent(GameObject.Find(((TreeType)i).ToString() + "Tree").transform.Find("Talent" + talent.Value.Tier + (talent.Key == 6 ? "" : (talent.Value.Selected == 1 ? "L" : "R"))).gameObject);
            }
        }
    }
    private void ToggleTalent(GameObject go)
    {
        SpriteParticleEmitter.UIParticleRenderer ps;
        for (int i = 0; i < 3; i++)
        {
            ps = go.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<SpriteParticleEmitter.UIParticleRenderer>();
            ps.enabled = !ps.enabled;
        }
    }
    #endregion
}
