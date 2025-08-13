using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public UnityEvent OnReady;
  public UnityEvent OnEnded;

  public bool IsReady { get; private set; }

  [SerializeField] private Board board;
  [SerializeField] private AudioMixer audioMixer;
  [SerializeField] private string soundVolumeParam = "SoundVolume";
  [SerializeField] private string musicVolumeParam = "MusicVolume";

  private Player player;

  private void InitVolumePrefs(string key) {
    float volume = 0.5f; // default volume (0..1)

    // Load from PlayerPrefs if available
    if (PlayerPrefs.HasKey(key))
      volume = Mathf.Clamp01(PlayerPrefs.GetFloat(key));
    else
      PlayerPrefs.SetFloat(key, volume);

    // Apply to mixer
    float dB = (volume <= 0.0001f) ? VolumeControl.minDb : Mathf.Log10(volume) * 20f;
    audioMixer.SetFloat(key, dB);
  }

  private void ReadyUp() {
    if (IsReady) return;
    IsReady = true;
    OnReady.Invoke();
    Debug.Log("Game Ready");
  }

  private void CreatePieces() {
    ReadyUp();
  }

  private void HandleBoardReady() {
    CreatePieces();
  }

  private void Start() {
    InitVolumePrefs(soundVolumeParam);
    InitVolumePrefs(musicVolumeParam);

    if (board.IsReady) HandleBoardReady();
    else board.OnReady.AddListener(HandleBoardReady);
  }

  private void Awake() {
    player = Player.Instance;
  }
}
