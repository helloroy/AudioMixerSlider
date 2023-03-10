using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[HelpURL("https://github.com/helloroy/AudioMixerSlider")]
[RequireComponent(typeof(Slider))]
public class AudioMixerSlider : MonoBehaviour
{
    [SerializeField, Tooltip("AudioMixer, not AudioMixerGroup.")]
    private AudioMixer audioMixer;

    [SerializeField, Tooltip("Exposed parameter name in AudioMixer. To add Exposed parameter, select your mixer, select your audio group, right click on the attenuation level in the inspector, click expose.")]
    private string exposedParameter;

    [SerializeField, Tooltip("Value type used in silder.")]
    private Unit sliderUnit = Unit.Voltage;

    [SerializeField, Tooltip("Auto load & save by PlayerPrefs.")]
    private bool enablePlayerPrefs = true;

    [SerializeField, HideInInspector]
    private Slider slider;

    [System.Serializable]
    private enum Unit
    {
        Decibel, Voltage
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateMixer);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateMixer);
    }

    private void Start()
    {
        var db = Slider2Db(slider.value);
        audioMixer.SetFloat(exposedParameter, Slider2Db(slider.value));
        if (enablePlayerPrefs)
        {
            db = PlayerPrefs.GetFloat("AudioMixerSlider" + exposedParameter, db);
            slider.value = Db2Slider(db);
        }
    }

    private void Save(float db)
    {
        PlayerPrefs.SetFloat("AudioMixerSlider" + exposedParameter, db);
        //Debug.Log($"Saved to PlayerPrefs: AudioMixerSlider{exposedParameter} = {db}", gameObject);
    }

    private void UpdateMixer(float value)
    {
        var db = Slider2Db(value);
        audioMixer.SetFloat(exposedParameter, db);
        if (enablePlayerPrefs) Save(db);
    }

    private float Db2Slider(float value)
    {
        return sliderUnit == Unit.Voltage ? Db2V(value) : value;
    }
    private float Slider2Db(float value)
    {
        return sliderUnit == Unit.Voltage ? V2Db(value) : value;
    }

    #region Decibel conversion (http://www.mogami.com/e/cad/db.html)

    private float Db2V(float db)
    {
        return Mathf.Pow(10, Mathf.Clamp(db, -80f, 20f) / 20);
    }
    private float V2Db(float v)
    {
        return 20 * Mathf.Log10(Mathf.Clamp(v, 0.000001f, 1f));
    }
    #endregion

    private void OnValidate()
    {
        slider ??= GetComponent<Slider>();

        // preset slider range
        if (sliderUnit != Unit.Decibel) { slider.minValue = 0; slider.maxValue = 1; }
        else { slider.minValue = -80; slider.maxValue = 20; }
    }
}
