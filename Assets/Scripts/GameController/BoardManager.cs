using UnityEngine;
using UnityEngine.Events;

public class BoardManager : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class SquareEvent : UnityEvent<Square> { }

  #endregion

  #region Fields

  [SerializeField] private Board board;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;
  [HideInInspector] public SquareEvent OnSquareClicked;
  [HideInInspector] public SquareEvent OnSquareDragBegan;
  [HideInInspector] public SquareEvent OnSquareDragEnded;
  [HideInInspector] public SquareEvent OnSquareDropped;
  [HideInInspector] public SquareEvent OnSquareEntered;
  [HideInInspector] public SquareEvent OnSquareExited;

  #endregion

  #region Properties

  public bool IsReady => board.IsReady;

  public Square[] Squares => board.Squares;

  public Square this[int index] => board[index];

  public Square this[int rank, int file] => board[rank, file];

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    if (board.IsReady) OnReady.Invoke();
    else board.OnReady.AddListener(OnReady.Invoke);

    board.OnSquareClicked.AddListener(OnSquareClicked.Invoke);
    board.OnSquareDragBegan.AddListener(OnSquareDragBegan.Invoke);
    board.OnSquareDragEnded.AddListener(OnSquareDragEnded.Invoke);
    board.OnSquareDropped.AddListener(OnSquareDropped.Invoke);
    board.OnSquareEntered.AddListener(OnSquareEntered.Invoke);
    board.OnSquareExited.AddListener(OnSquareExited.Invoke);
  }

  #endregion
}
