using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Player : MonoBehaviour {
  [System.Serializable] public class PieceEvent : UnityEvent<Piece> { }

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public bool IsWhite => isWhite;
  public Piece Selected { get; private set; }

  public PieceEvent OnSelected;
  public UnityEvent OnCleared;

  [Header("Settings")]
  [SerializeField] private bool isWhite;
  [SerializeField] private Color selected;

  [Header("Audio")]
  [SerializeField] private AudioSource moveBad;
  [SerializeField] private AudioSource moveAbort;

  [Header("References")]
  [SerializeField] private DragPiece dragPiece;

  private Color? original;
  private GameController gameController;

  public bool IsPlayerTurn => IsWhite ? gameController.WhiteToMove : !gameController.WhiteToMove;

  public void DragEnd(Piece piece) {
    if (Selected != piece) return;
    piece.IsVisible = true;
    dragPiece.IsVisible = false;
    Clear();
    moveAbort.Play();
  }

  public void DragStart(Piece piece) {
    if (!IsPlayerTurn) {
      moveBad.Play();
      return;
    }

    bool isOwn = piece.IsWhite == IsWhite;
    if (isOwn && gameController.PieceManager.HasAnyMoves(piece)) {
      piece.IsVisible = false;
      dragPiece.Sprite = piece.Sprite;
      dragPiece.Outline = piece.Outline;
      dragPiece.IsVisible = true;
      Select(piece);
    } else moveBad.Play();
  }

  public void Click(Piece piece) {
    if (!IsPlayerTurn) {
      moveBad.Play();
      return;
    }

    Select(piece);
  }

  private void Select(Piece piece) {
    bool clearOnly = Selected == piece;
    Clear();
    if (clearOnly) return;

    bool isOwn = piece.IsWhite == IsWhite;
    var squares = gameController.PieceManager[piece].Where(move => move.Type != PieceManager.Move.MoveType.Guarded).Select(move => move.Square);
    foreach (var square in gameController.Board.Squares) {
      if (isOwn) square.ShowScreen = !squares.Contains(square);
      else square.DangerZone = squares.Contains(square);
    }

    original = piece.Outline;
    piece.Outline = isOwn ? selected : Color.red;
    Selected = piece;
    OnSelected.Invoke(piece);
  }

  private void Clear() {
    if (Selected == null) return;
    Selected.Outline = original.Value;
    Reset();
    original = null;
    Selected = null;
    OnCleared.Invoke();
  }

  private void Reset() {
    foreach (var square in gameController.Board.Squares) {
      square.ShowScreen = false;
      square.DangerZone = false;
    }
  }

  private void MoveSelectedTo(Square square) {
    if (gameController.PieceManager.IsValidMove(Selected, square)) {
      var piece = Selected;
      Clear();
      gameController.PieceManager.MakeMove(piece, square);
    }
  }

  private void HandleSquareClicked(Square square) {
    MoveSelectedTo(square);
  }

  private void HandleSquareDropped(Square square) {
    if (Selected != null) Selected.IsVisible = true;
    dragPiece.IsVisible = false;
    MoveSelectedTo(square);
  }

  private void Start() {
    gameController.Board.OnClicked.AddListener(HandleSquareClicked);
    gameController.Board.OnDropped.AddListener(HandleSquareDropped);
  }

  private void Awake() {
    gameController = GameController.Instance;
  }
}
