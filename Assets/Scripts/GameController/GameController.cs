using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(PieceManager))]
[RequireComponent(typeof(MoveManager))]
public class GameController : MonoBehaviour {
  #region Constants

  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public AudioManager AudioManager { get; private set; }

  public BoardManager BoardManager { get; private set; }

  public PieceManager PieceManager { get; private set; }

  public MoveManager MoveManager { get; private set; }

  public bool IsReady { get; private set; }

  #endregion

  #region Methods

  private void ReadyUp() {
    if (IsReady) return;
    if (!BoardManager.IsReady) return;
    if (!PieceManager.IsReady) return;
    if (!MoveManager.IsReady) return;

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    if (!BoardManager.IsReady) BoardManager.OnReady.AddListener(ReadyUp);
    if (!PieceManager.IsReady) PieceManager.OnReady.AddListener(ReadyUp);
    if (!MoveManager.IsReady) MoveManager.OnReady.AddListener(ReadyUp);
    ReadyUp();
  }

  private void Awake() {
    AudioManager = GetComponent<AudioManager>();
    BoardManager = GetComponent<BoardManager>();
    PieceManager = GetComponent<PieceManager>();
    MoveManager = GetComponent<MoveManager>();
  }

  #endregion
}
