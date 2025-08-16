using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(PieceManager))]
public class MoveManager : MonoBehaviour {
  #region Constants

  private static Vector2Int[] kingMovements = new Vector2Int[] {
    new(-1, 1),  new(0, 1),  new(1, 1),
    new(-1, 0),              new(1, 0),
    new(-1, -1), new(0, -1), new(1, -1),
  };

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

    // todo: captures

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

      var on = pieceManager[to];
      if (on != null && on.IsWhite == knight.IsWhite) continue;

      moves.Add(new(knight, to, on == null ? MoveFlags.None : MoveFlags.Capture));
    }
  }

  private void AddBishopMoves(Piece bishop) {
    Assert.IsTrue(bishop.PieceType == PieceType.Bishop);

    AddMovementRay(bishop, 1, 1);
    AddMovementRay(bishop, 1, -1);
    AddMovementRay(bishop, -1, 1);
    AddMovementRay(bishop, -1, -1);
  }

  private void AddRookMoves(Piece rook) {
    Assert.IsTrue(rook.PieceType == PieceType.Rook);

    AddMovementRay(rook, 1, 0);
    AddMovementRay(rook, -1, 0);
    AddMovementRay(rook, 0, 1);
    AddMovementRay(rook, 0, -1);
  }

  private void AddQueenMoves(Piece queen) {
    Assert.IsTrue(queen.PieceType == PieceType.Queen);

    AddMovementRay(queen, 1, 1);
    AddMovementRay(queen, 1, -1);
    AddMovementRay(queen, -1, 1);
    AddMovementRay(queen, -1, -1);
    AddMovementRay(queen, 1, 0);
    AddMovementRay(queen, -1, 0);
    AddMovementRay(queen, 0, 1);
    AddMovementRay(queen, 0, -1);
  }

  private void AddMovementRay(Piece piece, int rankDirection, int fileDirection) {
    var from = piece.Square;
    for (int i = 1; i < Board.Dimension; i++) {
      var to = boardManager[from.Rank + i * rankDirection, from.File + i * fileDirection];
      if (to == null) break;

      var on = pieceManager[to];
      if (on == null) moves.Add(new(piece, to));
      else {
        if (on.IsWhite != piece.IsWhite) moves.Add(new(piece, to, MoveFlags.Capture));
        break;
      }
    }
  }

  private void AddKingMoves(Piece king) {
    Assert.IsTrue(king.PieceType == PieceType.King);

    var from = king.Square;
    foreach (var movement in kingMovements) {
      var to = boardManager[from.Rank + movement.x, from.File + movement.y];
      if (to == null) continue;

      var on = pieceManager[to];
      if (on != null && on.IsWhite == king.IsWhite) continue;

      moves.Add(new(king, to, on == null ? MoveFlags.None : MoveFlags.Capture));
    }
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
