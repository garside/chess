using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(PieceManager))]
public class MoveManager : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class PieceEvent : UnityEvent<Piece> { }

  #endregion

  #region Fields

  private BoardManager boardManager;
  private PieceManager pieceManager;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;
  [HideInInspector] public PieceEvent OnMoved;

  #endregion

  #region Properties

  public bool IsReady { get; private set; }

  #endregion

  #region Methods

  public bool Move(Piece piece, Square square) {
    if (!IsValid(piece, square)) return false;
    OnMoved.Invoke(piece);
    return true;
  }

  public bool IsValid(Piece piece, Square square) {
    return false;
  }

  private void CalculateMoves() {
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandlePiecesReady() {
    CalculateMoves();
    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Lifecycle

  private void Start() {
    if (pieceManager.IsReady) HandlePiecesReady();
    else pieceManager.OnReady.AddListener(HandlePiecesReady);
  }

  private void Awake() {
    boardManager = GetComponent<BoardManager>();
    pieceManager = GetComponent<PieceManager>();
  }

  #endregion
}
