using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Player : MonoBehaviour {
  #region Constants

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public static Color OpponentOutline => Color.red;

  public static Color PlayerOutline => Color.green;

  #endregion

  #region Internal

  [System.Serializable]
  public class MoveAudio {
    public AudioSource bad;
    public AudioSource click;
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
  }

  private void ClearDragging() {
    if (Dragging == null) return;
    Dragging.ResetOutlineColor();
    Dragging.ReparentToSquare();
    Dragging = null;
  }

  private void Click(Piece piece) {
    bool onlyClear = piece == null || piece == Clicked;
    ClearClicked();
    if (onlyClear) {
      moveAudio.tap.Play();
    } else {
      moveAudio.click.Play();
      Outline(piece);
      Clicked = piece;
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

      moveAudio.pickup.Play();
      piece.ReparentTo(dragHarness.transform);
      Outline(piece);
      Dragging = piece;
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
    if (!gameController.MoveManager.IsValid(Clicked, square)) {
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
    piece.OutlineColor = piece.IsWhite == IsWhite ? PlayerOutline : OpponentOutline;
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
    Debug.LogFormat("[Player] HandleSquareEntered {0}", square.name);
  }

  private void HandleSquareExited(Square square) {
    Debug.LogFormat("[Player] HandleSquareExited {0}", square.name);
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
