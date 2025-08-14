using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PieceManager : MonoBehaviour {
  [System.Serializable]
  public class PiecePrefabs {
    public GameObject pawn;
    public GameObject rook;
    public GameObject knight;
    public GameObject bishop;
    public GameObject queen;
    public GameObject king;
  }

  [System.Serializable]
  private class Move {
    public Piece Piece;
    public Square Square;
    public MoveType Type;

    public Move(Piece piece, Square square, MoveType type) {
      Piece = piece;
      Square = square;
      Type = type;
    }
  }

  private enum MoveType {
    Invalid,
    Allowed,
    Opposed,
    Guarded,
  }

  public bool PlayerIsWhite { get; private set; }

  [SerializeField] private PiecePrefabs piecePrefabs;
  [SerializeField] private Palette palette;

  private readonly List<Piece> pieces = new();
  private readonly HashSet<Piece> moved = new();
  private readonly List<Move> moves = new();

  private Board board;

  public Piece this[Square square] => pieces.FirstOrDefault(piece => piece.Square == square);

  public void Generate(Board board, bool playerIsWhite) {
    PlayerIsWhite = playerIsWhite;
    this.board = board;

    CreateWhitePieces();
    CreateBlackPieces();

    foreach (var piece in pieces) Calculate(piece);
    foreach (var square in board.Squares) Summarize(square);
  }

  private GameObject PrefabFor(Piece.PieceType pieceType) {
    return pieceType switch {
      Piece.PieceType.Pawn => piecePrefabs.pawn,
      Piece.PieceType.Rook => piecePrefabs.rook,
      Piece.PieceType.Knight => piecePrefabs.knight,
      Piece.PieceType.Bishop => piecePrefabs.bishop,
      Piece.PieceType.Queen => piecePrefabs.queen,
      Piece.PieceType.King => piecePrefabs.king,
      _ => null,
    };
  }

  private void CreatePiece(Piece.PieceType pieceType, bool isWhite, Square square) {
    var piece = Instantiate(PrefabFor(pieceType), Vector2.zero, Quaternion.identity, square.transform).GetComponent<Piece>();
    piece.Square = square;
    piece.IsWhite = isWhite;
    piece.Color = isWhite ? palette.White : palette.Black;
    piece.Outline = isWhite ? palette.Black : palette.White;
    pieces.Add(piece);
  }

  private void CreateWhitePieces() {
    CreatePiece(Piece.PieceType.Pawn, true, board["a2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["b2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["c2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["d2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["e2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["f2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["g2"]);
    CreatePiece(Piece.PieceType.Pawn, true, board["h2"]);
    CreatePiece(Piece.PieceType.Rook, true, board["a1"]);
    CreatePiece(Piece.PieceType.Rook, true, board["h1"]);
    CreatePiece(Piece.PieceType.Knight, true, board["b1"]);
    CreatePiece(Piece.PieceType.Knight, true, board["g1"]);
    CreatePiece(Piece.PieceType.Bishop, true, board["c1"]);
    CreatePiece(Piece.PieceType.Bishop, true, board["f1"]);
    CreatePiece(Piece.PieceType.Queen, true, board["d1"]);
    CreatePiece(Piece.PieceType.King, true, board["e1"]);
  }

  private void CreateBlackPieces() {
    CreatePiece(Piece.PieceType.Pawn, false, board["a7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["b7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["c7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["d7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["e7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["f7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["g7"]);
    CreatePiece(Piece.PieceType.Pawn, false, board["h7"]);
    CreatePiece(Piece.PieceType.Rook, false, board["a8"]);
    CreatePiece(Piece.PieceType.Rook, false, board["h8"]);
    CreatePiece(Piece.PieceType.Knight, false, board["b8"]);
    CreatePiece(Piece.PieceType.Knight, false, board["g8"]);
    CreatePiece(Piece.PieceType.Bishop, false, board["c8"]);
    CreatePiece(Piece.PieceType.Bishop, false, board["f8"]);
    CreatePiece(Piece.PieceType.Queen, false, board["e8"]);
    CreatePiece(Piece.PieceType.King, false, board["d8"]);
  }

  public void Calculate(Piece piece) {
    switch (piece.Type) {
      case Piece.PieceType.Pawn:
        CalculatePawnMoves(piece);
        break;
      case Piece.PieceType.Rook:
        CalculateRookMoves(piece);
        break;
      case Piece.PieceType.Knight:
        CalculateKnightMoves(piece);
        break;
      case Piece.PieceType.Bishop:
        CalculateBishopMoves(piece);
        break;
      case Piece.PieceType.Queen:
        CalculateQueenMoves(piece);
        break;
      case Piece.PieceType.King:
        CalculateKingMoves(piece);
        break;
    }
  }

  private MoveType AddMove(Piece piece, Square square) {
    if (square == null) return MoveType.Invalid;

    var moveType = MoveType.Allowed;

    var pieceOnSquare = this[square];
    if (pieceOnSquare != null) {
      moveType = pieceOnSquare.IsWhite == piece.IsWhite ? MoveType.Guarded : MoveType.Opposed;
    }

    Move move = new(piece, square, moveType);
    Debug.LogFormat("{0} {1} => {2} {3}", piece.Type, piece.Square.name, square.name, moveType);
    moves.Add(move);
    return moveType;
  }

  private void CalculatePawnMoves(Piece piece) {
    if (AddMove(piece, piece.IsWhite ? piece.Square.Up(1) : piece.Square.Down(1)) == MoveType.Allowed) {
      if (!moved.Contains(piece)) AddMove(piece, piece.IsWhite ? piece.Square.Up(2) : piece.Square.Down(2));
    }
  }

  private void CalculateRookMoves(Piece piece) {
    for (int up = 1; up < Board.Size; up++) {
      if (AddMove(piece, piece.Square.Up(up)) != MoveType.Allowed) break;
    }

    for (int down = 1; down < Board.Size; down++) {
      if (AddMove(piece, piece.Square.Down(down)) != MoveType.Allowed) break;
    }

    for (int left = 1; left < Board.Size; left++) {
      if (AddMove(piece, piece.Square.Left(left)) != MoveType.Allowed) break;
    }

    for (int right = 1; right < Board.Size; right++) {
      if (AddMove(piece, piece.Square.Right(right)) != MoveType.Allowed) break;
    }
  }

  private void CalculateKnightMoves(Piece piece) {
  }

  private void CalculateBishopMoves(Piece piece) {
    for (int ul = 1; ul < Board.Size; ul++) {
      if (AddMove(piece, piece.Square.UpLeft(ul)) != MoveType.Allowed) break;
    }

    for (int ur = 1; ur < Board.Size; ur++) {
      if (AddMove(piece, piece.Square.UpRight(ur)) != MoveType.Allowed) break;
    }

    for (int dl = 1; dl < Board.Size; dl++) {
      if (AddMove(piece, piece.Square.DownLeft(dl)) != MoveType.Allowed) break;
    }

    for (int dr = 1; dr < Board.Size; dr++) {
      if (AddMove(piece, piece.Square.DownRight(dr)) != MoveType.Allowed) break;
    }
  }

  private void CalculateQueenMoves(Piece piece) {
    CalculateRookMoves(piece);
    CalculateBishopMoves(piece);
  }

  private void CalculateKingMoves(Piece piece) {
    AddMove(piece, piece.Square.Up(1));
    AddMove(piece, piece.Square.Down(1));
    AddMove(piece, piece.Square.Left(1));
    AddMove(piece, piece.Square.Right(1));
    AddMove(piece, piece.Square.UpLeft(1));
    AddMove(piece, piece.Square.UpRight(1));
    AddMove(piece, piece.Square.DownLeft(1));
    AddMove(piece, piece.Square.DownRight(1));
  }

  private void Summarize(Square square) {
    var allowed = moves.Where(move => move.Square == square && move.Type == MoveType.Allowed);
    var claimCount = allowed.Count();

    square.ClaimCount = claimCount;

    if (claimCount == 0) square.Claim = Square.ClaimType.Undefined;
    else if (allowed.All(move => move.Piece.IsWhite)) {
      square.Claim = PlayerIsWhite ? Square.ClaimType.Player : Square.ClaimType.Opponent;
    } else if (allowed.All(move => !move.Piece.IsWhite)) {
      square.Claim = PlayerIsWhite ? Square.ClaimType.Opponent : Square.ClaimType.Player;
    } else square.Claim = Square.ClaimType.Contested;

    square.GuardedCount = moves.Count(move => move.Square == square && move.Type == MoveType.Guarded);
    square.OpposedCount = moves.Count(move => move.Square == square && move.Type == MoveType.Opposed);
  }
}
