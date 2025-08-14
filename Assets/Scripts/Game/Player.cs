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

  private Color? original;
  private GameController gameController;

  public void Click(Piece piece) {
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

  private void HandleSquareClicked(Square square) {
    Debug.Log(square);
  }

  private void Start() {
    gameController.Board.OnClicked.AddListener(HandleSquareClicked);
  }

  private void Awake() {
    gameController = GameController.Instance;
  }
}
