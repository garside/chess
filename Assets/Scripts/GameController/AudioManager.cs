using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour {
  [Header("Mixer")]
  [SerializeField] private AudioMixer mixer;

  [Header("Exposed Mixer Params")]
  [SerializeField] private string soundVolumeParam = "SoundVolume";
  [SerializeField] private string musicVolumeParam = "MusicVolume";

  [Header("Defaults")]
  [Range(0f, 1f)]
  [SerializeField] private float defaultVolume = 0.5f;

  public const float MinDb = -80f; // typical floor

  public void InitializeFromPrefs() {
    float soundVol = LoadOrDefault(soundVolumeParam, defaultVolume);
    float musicVol = LoadOrDefault(musicVolumeParam, defaultVolume);
    ApplyLinear(soundVolumeParam, soundVol);
    ApplyLinear(musicVolumeParam, musicVol);
  }

  public void SetSoundVolume(float linear01) {
    SetVolume(soundVolumeParam, linear01);
  }

  public void SetMusicVolume(float linear01) {
    SetVolume(musicVolumeParam, linear01);
  }

  public float GetSoundVolume() => PlayerPrefs.GetFloat(soundVolumeParam, defaultVolume);
  public float GetMusicVolume() => PlayerPrefs.GetFloat(musicVolumeParam, defaultVolume);

  // --- Internals ---
  private float LoadOrDefault(string key, float fallback) {
    if (!PlayerPrefs.HasKey(key))
      PlayerPrefs.SetFloat(key, fallback);
    return Mathf.Clamp01(PlayerPrefs.GetFloat(key));
  }

  private void SetVolume(string param, float linear01) {
    linear01 = Mathf.Clamp01(linear01);
    ApplyLinear(param, linear01);
    PlayerPrefs.SetFloat(param, linear01);
  }

  private void ApplyLinear(string param, float linear01) {
    if (!mixer) return;
    float dB = (linear01 <= 0.0001f) ? MinDb : Mathf.Log10(linear01) * 20f;
    mixer.SetFloat(param, dB);
  }
}
