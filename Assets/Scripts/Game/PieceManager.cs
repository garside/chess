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

  private enum MoveType {
    Invalid,
    Allowed,
    Blocked,
    Capture,
  }

  [SerializeField] private PiecePrefabs piecePrefabs;
  [SerializeField] private Palette palette;

  private readonly List<Piece> pieces = new();
  private readonly HashSet<Piece> moved = new();
  private readonly Dictionary<Piece, List<Square>> moves = new();

  private Board board;

  public Piece this[Square square] => pieces.FirstOrDefault(piece => piece.Square == square);

  public void Generate(Board board) {
    this.board = board;

    CreateWhitePieces();
    CreateBlackPieces();

    foreach (var piece in pieces) CalculateMoves(piece);
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

  public void CalculateMoves(Piece piece) {
    if (moves.ContainsKey(piece)) moves[piece].Clear();
    else moves[piece] = new();

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

    Debug.LogFormat("{0} {1} @{2} moves: {3}", piece.IsWhite ? "White" : "Black", piece.Type, piece.Square.name, string.Join(", ", moves[piece].Select(move => move.name)));
  }

  private MoveType AddMove(Piece piece, Square square) {
    if (square == null) return MoveType.Invalid;

    var pieceOnSquare = this[square];
    if (pieceOnSquare != null) {
      if (pieceOnSquare.IsWhite == piece.IsWhite) return MoveType.Blocked;
      return MoveType.Capture;
    }

    moves[piece].Add(square);
    return MoveType.Allowed;
  }

  private void CalculatePawnMoves(Piece piece) {
    if (AddMove(piece, piece.IsWhite ? piece.Square.Up(1) : piece.Square.Down(1)) == MoveType.Allowed) {
      if (!moved.Contains(piece)) AddMove(piece, piece.IsWhite ? piece.Square.Up(2) : piece.Square.Down(2));
    }
  }

  private void CalculateRookMoves(Piece piece) {
    for (int up = 1; up < Board.Rows; up++) {
      if (AddMove(piece, piece.Square.Up(up)) != MoveType.Allowed) break;
    }

    for (int down = 1; down < Board.Rows; down++) {
      if (AddMove(piece, piece.Square.Down(down)) != MoveType.Allowed) break;
    }
  }

  private void CalculateKnightMoves(Piece piece) {

  }

  private void CalculateBishopMoves(Piece piece) {

  }

  private void CalculateQueenMoves(Piece piece) {

  }

  private void CalculateKingMoves(Piece piece) {

  }
}
