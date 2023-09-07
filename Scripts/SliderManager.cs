using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider multiplayerSlider;
    
    public TMP_Text musicValueText;
    public TMP_Text sfxValueText;
    public TMP_Text multiplayerValueText;

    private void Start()
    {
        UpdateMusicValueText(musicSlider.value);
        UpdateSFXValueText(sfxSlider.value);
        UpdateMuliplayerValueText(multiplayerSlider.value);
    }

    public void OnMusicSliderValueChanged(float value)
    {
        UpdateMusicValueText(value);
    }

    public void OnMuliplayerSliderValueChanged(float value)
    {
        UpdateMuliplayerValueText(value);
    }

    public void OnSFXSliderValueChanged(float value)
    {
        UpdateSFXValueText(value);
    }

    private void UpdateMusicValueText(float value)
    {
        int scaledValue = Mathf.RoundToInt(value * 100f);
        musicValueText.text = "Music:" + scaledValue.ToString();
    }

    private void UpdateSFXValueText(float value)
    {
        int scaledValue = Mathf.RoundToInt(value * 100f);
        sfxValueText.text = "SFX:" + scaledValue.ToString();
    }
        private void UpdateMuliplayerValueText(float value)
    {
        int scaledValue = Mathf.RoundToInt(value * 100f);
        multiplayerValueText.text = scaledValue.ToString()+ "$";
    }
}
