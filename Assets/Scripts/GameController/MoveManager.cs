using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(PieceManager))]
public class MoveManager : MonoBehaviour {
  #region Constants

  private static Vector2Int[] knightMovements = new Vector2Int[] {
    new(1, 2), new(1, -2), new(-1, 2), new(-1, -2),
    new(2, 1), new(2, -1), new(-2, 1), new(-2, -1),
  };

  #endregion

  #region Internal

  [System.Serializable] public class PieceEvent : UnityEvent<Piece> { }

  #endregion

  #region Fields

  private BoardManager boardManager;
  private PieceManager pieceManager;

  private readonly List<Move> moves = new();
  private readonly HashSet<Piece> moved = new();

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;
  [HideInInspector] public PieceEvent OnMoved;

  #endregion

  #region Properties

  public bool IsReady { get; private set; }

  #endregion

  #region Methods

  public bool HasAny(Piece piece) => moves.Any(move => move.Piece == piece);

  public bool IsValid(Piece piece, Square square) => moves.Any(move => move.Piece == piece && move.To == square);

  public bool Move(Piece piece, Square square) {
    if (!IsValid(piece, square)) return false;

    // TODO: Apply move
    // TODO: Cleanup move

    moved.Add(piece);
    OnMoved.Invoke(piece);
    return true;
  }

  private void AddMoves(Piece piece) {
    switch (piece.PieceType) {
      case PieceType.Pawn:
        AddPawnMoves(piece);
        break;
      case PieceType.Knight:
        AddKnightMoves(piece);
        break;
      case PieceType.Bishop:
        AddBishopMoves(piece);
        break;
      case PieceType.Rook:
        AddRookMoves(piece);
        break;
      case PieceType.Queen:
        AddQueenMoves(piece);
        break;
      case PieceType.King:
        AddKingMoves(piece);
        break;
    }
  }

  private void AddPawnMoves(Piece pawn) {
    Assert.IsTrue(pawn.PieceType == PieceType.Pawn);

    var from = pawn.Square;
    var to = boardManager[from.Rank + (pawn.IsWhite ? 1 : -1), from.File];
    if (to == null || pieceManager.AnyOn(to)) return;

    moves.Add(new(pawn, to));

    if (moved.Contains(pawn)) return;

    var push = boardManager[from.Rank + (pawn.IsWhite ? 2 : -2), from.File];
    if (push == null || pieceManager.AnyOn(push)) return;
    moves.Add(new(pawn, push, MoveFlags.DoublePush));
  }

  private void AddKnightMoves(Piece knight) {
    Assert.IsTrue(knight.PieceType == PieceType.Knight);

    var from = knight.Square;
    foreach (var movement in knightMovements) {
      var to = boardManager[from.Rank + movement.x, from.File + movement.y];
      if (to == null) continue;

      var piece = pieceManager[to];
      if (piece != null && piece.IsWhite == knight.IsWhite) continue;

      moves.Add(new(knight, to));
    }
  }

  private void AddBishopMoves(Piece pawn) {
    Assert.IsTrue(pawn.PieceType == PieceType.Bishop);
  }

  private void AddRookMoves(Piece pawn) {
    Assert.IsTrue(pawn.PieceType == PieceType.Rook);
  }

  private void AddQueenMoves(Piece pawn) {
    Assert.IsTrue(pawn.PieceType == PieceType.Queen);
  }

  private void AddKingMoves(Piece pawn) {
    Assert.IsTrue(pawn.PieceType == PieceType.King);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandlePiecesReady() {
    foreach (var piece in pieceManager.Pieces) AddMoves(piece);
    Debug.LogFormat("[MoveManager] Ready with {0} opening moves", moves.Count);
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
