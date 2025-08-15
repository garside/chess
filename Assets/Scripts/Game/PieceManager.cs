using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class PieceManager : MonoBehaviour {
  [System.Serializable] public class PieceEvent : UnityEvent<Piece> { }
  [System.Serializable] public class MoveEvent : UnityEvent<Move> { }

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
  public class Move {
    public enum MoveType {
      Invalid,
      Allowed,
      Opposed,
      Guarded,
    }

    public Piece Piece;
    public Square Square;
    public MoveType Type;

    public Move(Piece piece, Square square, MoveType type) {
      Piece = piece;
      Square = square;
      Type = type;
    }
  }

  public PieceEvent OnClick;
  public PieceEvent OnDragStarted;
  public PieceEvent OnDragEnded;
  public MoveEvent OnPieceMoved;

  public Move[] PlayerMoves => player.IsWhite ? WhiteMoves : BlackMoves;
  public Move[] OpponentMoves => player.IsWhite ? BlackMoves : WhiteMoves;
  public Move[] WhiteMoves => moves.Where(move => move.Piece.IsWhite).ToArray();
  public Move[] BlackMoves => moves.Where(move => !move.Piece.IsWhite).ToArray();

  public Piece this[Square square] => pieces.FirstOrDefault(piece => piece.Square == square);
  public Move[] this[Piece piece] => moves.Where(move => move.Piece == piece).ToArray();

  [SerializeField] private PiecePrefabs piecePrefabs;
  [SerializeField] private Palette palette;

  private readonly List<Piece> pieces = new();
  private readonly HashSet<Piece> moved = new();
  private readonly List<Move> moves = new();

  private Board board;
  private Player player;

  public bool HasAnyMoves(Piece piece) => moves.Any(move => move.Piece == piece && move.Type != Move.MoveType.Guarded);
  public bool IsValidMove(Piece piece, Square square) => moves.Any(move => move.Piece == piece && move.Square == square);
  public void MakeMove(Piece piece, Square square) => Make(moves.First(move => move.Piece == piece && move.Square == square));
  public void Make(Move move) {
    Debug.Log("MOVE!");
    OnPieceMoved.Invoke(move);
  }

  public void Generate(Board board) {
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

    piece.OnClick.AddListener(HandlePieceClicked);
    piece.OnDragStart.AddListener(HandlePieceDragStarted);
    piece.OnDragEnd.AddListener(HandlePieceDragEnded);
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

  private Move.MoveType AddMove(Piece piece, Square square) {
    if (square == null) return Move.MoveType.Invalid;

    var moveType = Move.MoveType.Allowed;

    var pieceOnSquare = this[square];
    if (pieceOnSquare != null) {
      moveType = pieceOnSquare.IsWhite == piece.IsWhite ? Move.MoveType.Guarded : Move.MoveType.Opposed;
    }

    Move move = new(piece, square, moveType);
    Debug.LogFormat("{0} {1} => {2} {3}", piece.Type, piece.Square.name, square.name, moveType);
    moves.Add(move);
    return moveType;
  }

  private void CalculatePawnMoves(Piece piece) {
    if (AddMove(piece, piece.IsWhite ? piece.Square.Up(1) : piece.Square.Down(1)) == Move.MoveType.Allowed) {
      if (!moved.Contains(piece)) AddMove(piece, piece.IsWhite ? piece.Square.Up(2) : piece.Square.Down(2));
    }
    // todo: capturing
    // todo: en passant (fuck you)
    // todo: promotions
  }

  private void CalculateRookMoves(Piece piece) {
    for (int up = 1; up < Board.Size; up++) {
      if (AddMove(piece, piece.Square.Up(up)) != Move.MoveType.Allowed) break;
    }

    for (int down = 1; down < Board.Size; down++) {
      if (AddMove(piece, piece.Square.Down(down)) != Move.MoveType.Allowed) break;
    }

    for (int left = 1; left < Board.Size; left++) {
      if (AddMove(piece, piece.Square.Left(left)) != Move.MoveType.Allowed) break;
    }

    for (int right = 1; right < Board.Size; right++) {
      if (AddMove(piece, piece.Square.Right(right)) != Move.MoveType.Allowed) break;
    }
  }

  private void CalculateKnightMoves(Piece piece) {
    AddMove(piece, piece.Square.Knight(2, 1));
    AddMove(piece, piece.Square.Knight(-2, 1));
    AddMove(piece, piece.Square.Knight(2, -1));
    AddMove(piece, piece.Square.Knight(-2, -1));
    AddMove(piece, piece.Square.Knight(1, 2));
    AddMove(piece, piece.Square.Knight(1, -2));
    AddMove(piece, piece.Square.Knight(-1, 2));
    AddMove(piece, piece.Square.Knight(-1, -2));
  }

  private void CalculateBishopMoves(Piece piece) {
    for (int ul = 1; ul < Board.Size; ul++) {
      if (AddMove(piece, piece.Square.UpLeft(ul)) != Move.MoveType.Allowed) break;
    }

    for (int ur = 1; ur < Board.Size; ur++) {
      if (AddMove(piece, piece.Square.UpRight(ur)) != Move.MoveType.Allowed) break;
    }

    for (int dl = 1; dl < Board.Size; dl++) {
      if (AddMove(piece, piece.Square.DownLeft(dl)) != Move.MoveType.Allowed) break;
    }

    for (int dr = 1; dr < Board.Size; dr++) {
      if (AddMove(piece, piece.Square.DownRight(dr)) != Move.MoveType.Allowed) break;
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
    var coverage = moves.Where(move => move.Square == square);
    var white = coverage.Count(move => move.Piece.IsWhite);
    var black = coverage.Count(move => !move.Piece.IsWhite);

    square.PlayerCoverage = player.IsWhite ? white : black;
    square.EnemyCoverage = player.IsWhite ? black : white;
  }

  private void HandlePieceDragEnded(Piece piece) {
    OnDragEnded.Invoke(piece);
    player.DragEnd(piece);
  }

  private void HandlePieceDragStarted(Piece piece) {
    OnDragStarted.Invoke(piece);
    player.DragStart(piece);
  }

  private void HandlePieceClicked(Piece piece) {
    OnClick.Invoke(piece);
    player.Click(piece);
  }

  private void Awake() {
    player = GameObject.FindWithTag("Player").GetComponent<Player>();
  }
}
