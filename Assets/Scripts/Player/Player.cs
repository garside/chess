using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Player : MonoBehaviour {
  #region Constants

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public static Color OpponentHighlight => Color.red;

  public static Color PlayerHighlight => Color.yellow;

  #endregion

  #region Internal

  [System.Serializable]
  public class MoveAudio {
    public AudioSource bad;
    public AudioSource click;
    public AudioSource highlight;
    public AudioSource none;
    public AudioSource pickup;
    public AudioSource place;
    public AudioSource reset;
    public AudioSource swoosh;
    public AudioSource tap;
  }

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private MoveAudio moveAudio;
  [SerializeField] private bool isWhite;

  [Header("References")]
  [SerializeField] private DragHarness dragHarness;

  private GameController gameController;

  #endregion

  #region Events

  #endregion

  #region Properties

  public Piece Clicked { get; private set; }

  public Piece Dragging { get; private set; }

  public bool IsWhite {
    get => isWhite;
    set => isWhite = value;
  }

  #endregion

  #region Methods

  private void ClearClicked() {
    if (Clicked == null) return;
    Clicked.ResetOutlineColor();
    Clicked = null;
    foreach (var square in gameController.BoardManager.Squares) square.BorderVisible = false;
  }

  private void ClearDragging() {
    if (Dragging == null) return;
    Dragging.ResetOutlineColor();
    Dragging.ReparentToSquare();
    Dragging = null;
    foreach (var square in gameController.BoardManager.Squares) {
      square.ScreenVisible = false;
      square.BorderVisible = false;
    }
  }

  private void Click(Piece piece) {
    bool onlyClear = piece == null || piece == Clicked;
    ClearClicked();
    if (onlyClear) {
      moveAudio.tap.Play();
    } else {
      if (!gameController.MoveManager.HasAny(piece)) {
        moveAudio.none.Play();
        return;
      }

      moveAudio.click.Play();
      Outline(piece);
      Clicked = piece;

      foreach (var square in gameController.BoardManager.Squares) {
        square.BorderVisible = gameController.MoveManager.IsValid(Clicked, square);
        square.BorderColor = piece.IsWhite == IsWhite ? PlayerHighlight : OpponentHighlight;
      }
    }
  }

  private void BeginDrag(Piece piece) {
    ClearClicked();
    if (piece == null) moveAudio.tap.Play();
    else {
      if (piece.IsWhite != IsWhite) {
        moveAudio.bad.Play();
        return;
      }

      if (!gameController.MoveManager.HasAny(piece)) {
        moveAudio.none.Play();
        return;
      }

      moveAudio.pickup.Play();
      piece.ReparentTo(dragHarness.transform);
      Outline(piece);
      Dragging = piece;

      foreach (var square in gameController.BoardManager.Squares) square.ScreenVisible = !gameController.MoveManager.IsValid(Dragging, square);
    }
  }

  private void EndDrag(Piece piece) {
    if (Dragging != piece) return;
    moveAudio.reset.Play();
    ClearDragging();
  }

  private void DropOnto(Square square) {
    if (Dragging == null) return;
    if (!gameController.MoveManager.IsValid(Dragging, square)) return;

    var piece = Dragging;
    ClearDragging();
    gameController.MoveManager.Move(piece, square);
    moveAudio.place.Play();
  }

  private void ClickToMove(Square square) {
    if (Clicked == null) return;
    if (Clicked.IsWhite != IsWhite || !gameController.MoveManager.IsValid(Clicked, square)) {
      var currentPiece = gameController.PieceManager[square];
      if (currentPiece == null) {
        ClearClicked();
        moveAudio.bad.Play();
      } else Click(currentPiece);
      return;
    }

    var piece = Clicked;
    ClearClicked();
    gameController.MoveManager.Move(piece, square);
    moveAudio.swoosh.Play();
  }

  private void Outline(Piece piece) {
    piece.OutlineColor = piece.IsWhite == IsWhite ? PlayerHighlight : OpponentHighlight;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleGameReady() {
    Debug.Log("[Player] HandleGameReady");
  }

  private void HandleSquareClicked(Square square) {
    if (Clicked == null) Click(gameController.PieceManager[square]);
    else ClickToMove(square);
  }

  private void HandleSquareDragBegan(Square square) {
    BeginDrag(gameController.PieceManager[square]);
  }

  private void HandleSquareDragEnded(Square square) {
    EndDrag(gameController.PieceManager[square]);
  }

  private void HandleSquareDropped(Square square) {
    DropOnto(square);
  }

  private void HandleSquareEntered(Square square) {
    if (Dragging == null) return;

    square.BorderVisible = gameController.MoveManager.IsValid(Dragging, square);
    square.BorderColor = PlayerHighlight;
    if (square.BorderVisible) moveAudio.highlight.Play();
  }

  private void HandleSquareExited(Square square) {
    if (Dragging == null) return;
    square.BorderVisible = false;
  }

  #endregion

  #region Lifecycle

  private void Start() {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);

    gameController.BoardManager.OnSquareClicked.AddListener(HandleSquareClicked);
    gameController.BoardManager.OnSquareDragBegan.AddListener(HandleSquareDragBegan);
    gameController.BoardManager.OnSquareDragEnded.AddListener(HandleSquareDragEnded);
    gameController.BoardManager.OnSquareDropped.AddListener(HandleSquareDropped);
    gameController.BoardManager.OnSquareEntered.AddListener(HandleSquareEntered);
    gameController.BoardManager.OnSquareExited.AddListener(HandleSquareExited);
  }

  private void Awake() {
    gameController = GameController.Instance;
  }

  #endregion
}
