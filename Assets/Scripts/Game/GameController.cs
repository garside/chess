using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(PieceManager))]
public class GameController : MonoBehaviour {
  [System.Serializable] public class MoveEvent : UnityEvent<PieceManager.Move> { }

  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public UnityEvent OnReady;
  public UnityEvent OnEnded;
  public MoveEvent OnPieceMoved;

  public bool IsReady { get; private set; }
  public bool IsOver { get; private set; }
  public bool IsRunning => IsReady && !IsOver;
  public bool WhiteToMove { get; private set; }

  public Board Board => board;
  public PieceManager PieceManager => pieceManager;

  [Header("Game Settings")]
  [SerializeField] private Board board;

  [Header("Audio Settings")]
  [SerializeField] private AudioMixer audioMixer;
  [SerializeField] private string soundVolumeParam = "SoundVolume";
  [SerializeField] private string musicVolumeParam = "MusicVolume";

  private PieceManager pieceManager;

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

  private void HandleBoardReady() {
    pieceManager.Generate(board);
    ReadyUp();
  }

  private void HandlePieceMoved(PieceManager.Move move) {
    WhiteToMove = !WhiteToMove;
    OnPieceMoved.Invoke(move);
  }

  private void Start() {
    InitVolumePrefs(soundVolumeParam);
    InitVolumePrefs(musicVolumeParam);

    pieceManager.OnPieceMoved.AddListener(HandlePieceMoved);

    if (board.IsReady) HandleBoardReady();
    else board.OnReady.AddListener(HandleBoardReady);
  }

  private void Awake() {
    WhiteToMove = true;
    pieceManager = GetComponent<PieceManager>();
  }
}
