using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
  public const float minDb = -80f;

  [Header("UI")]
  [SerializeField] private Button mute;
  [SerializeField] private Button unmute;
  [SerializeField] private Slider slider;

  [Header("Audio")]
  [SerializeField] private AudioMixer audioMixer;
  [SerializeField] private string mixerVolumeParam = "MasterVolume"; // Set this to your exposed mixer param
  [SerializeField] private AudioSource audioSource;

  [Header("Settings")]
  [SerializeField] private bool saveToPlayerPrefs = true;

  private bool changing;                // prevents feedback loops when syncing UI
  private float lastNonZeroVolume = 1f; // 0..1 linear remembered when muting

  private void Start() {
    mute.onClick.AddListener(Mute);
    unmute.onClick.AddListener(Unmute);
    slider.onValueChanged.AddListener(HandleSliderChanged);

    // Initial restore / default
    float startVol = 1f;
    if (saveToPlayerPrefs && PlayerPrefs.HasKey(mixerVolumeParam))
      startVol = Mathf.Clamp01(PlayerPrefs.GetFloat(mixerVolumeParam));

    // Apply to mixer + UI
    changing = true;
    slider.value = startVol;
    changing = false;

    ApplySliderToMixer(startVol, playIfSilent: false);
    Sync();
  }

  private void OnDestroy() {
    // good hygiene
    mute.onClick.RemoveListener(Mute);
    unmute.onClick.RemoveListener(Unmute);
    slider.onValueChanged.RemoveListener(HandleSliderChanged);
  }

  // --- UI Actions ---

  private void Mute() {
    // Remember last audible value so unmute can restore
    if (slider.value > 0.0001f) lastNonZeroVolume = slider.value;

    SetVolumeLinear(0f);
  }

  private void Unmute() {
    float restore = (lastNonZeroVolume <= 0.0001f) ? 1f : lastNonZeroVolume;
    SetVolumeLinear(restore);
  }

  private void HandleSliderChanged(float _) {
    if (changing) return;

    // Start the source if user interacts while silent
    if (audioSource && !audioSource.isPlaying) audioSource.Play();

    ApplySliderToMixer(slider.value, playIfSilent: false);
    Sync();
  }

  // --- Core ---

  private void SetVolumeLinear(float linear01) {
    linear01 = Mathf.Clamp01(linear01);

    changing = true;
    slider.value = linear01; // update UI first (prevents bounce)
    changing = false;

    ApplySliderToMixer(linear01, playIfSilent: true);
    Sync();
  }

  private void ApplySliderToMixer(float linear01, bool playIfSilent) {
    if (!audioMixer) return;

    // Convert 0..1 linear to decibels for the mixer
    float dB = (linear01 <= 0.0001f) ? minDb : Mathf.Log10(linear01) * 20f;
    audioMixer.SetFloat(mixerVolumeParam, dB);

    if (saveToPlayerPrefs) PlayerPrefs.SetFloat(mixerVolumeParam, linear01);

    // Manage AudioSource audibility
    if (audioSource) {
      audioSource.mute = linear01 <= 0.0001f;

      if (playIfSilent && !audioSource.isPlaying && linear01 > 0.0001f)
        audioSource.Play();
    }

    // Track the last audible setting for Unmute()
    if (linear01 > 0.0001f)
      lastNonZeroVolume = linear01;
  }

  private void Sync() {
    // Read current mixer dB and reflect in UI (in case external code changed it)
    if (audioMixer && audioMixer.GetFloat(mixerVolumeParam, out float dB)) {
      float lin = dB <= minDb ? 0f : Mathf.Clamp01(Mathf.Pow(10f, dB / 20f));

      // keep slider consistent with mixer
      if (!Mathf.Approximately(slider.value, lin)) {
        changing = true;
        slider.value = lin;
        changing = false;
      }

      // toggle button visibility/state
      bool isMuted = lin <= 0.0001f;
      if (mute) mute.gameObject.SetActive(!isMuted);
      if (unmute) unmute.gameObject.SetActive(isMuted);
    } else {
      // Fallback if no mixer: use slider value
      bool isMuted = slider.value <= 0.0001f;
      if (mute) mute.gameObject.SetActive(!isMuted);
      if (unmute) unmute.gameObject.SetActive(isMuted);
    }
  }
}
