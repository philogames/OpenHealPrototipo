using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetPrefab : MonoBehaviour
{
    [MMReadOnly]
    public string presetName;
    [MMReadOnly]
    public string presetDescription;

    public Text textName;
    public Text textDescription;
    public Text currentLevel;
    public Button playPreset;
    GerenteTelas gerenteTelas;
    Image borda;
   
    private void OnEnable()
    {
        if(gerenteTelas == null)
            gerenteTelas = FindObjectOfType<GerenteTelas>();

        playPreset.onClick.AddListener(OnPlayPreset);

        if(borda==null)
            borda = GetComponent<Image>();

     

        currentLevel.text = "";
        borda.color = Color.gray;
    }

    private void OnDisable()
    {
        playPreset.onClick.RemoveListener(OnPlayPreset);
    }

    public void SetPresetName(string name)
    {
        presetName = name;
        textName.text = presetName;
    }

    public void SetPresetDescription(string description)
    {
        presetDescription = description;
        textDescription.text = presetDescription;
    }

    public void SetPresetCurrentLevel()
    {
        borda.color = Color.green;
        currentLevel.text = "Current Level";
    }

    void OnPlayPreset()
    {
        gerenteTelas.BotaoBubblesJogar(presetName);
    }

}
