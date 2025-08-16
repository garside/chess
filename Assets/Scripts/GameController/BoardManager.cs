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

  [HideInInspector] public SquareEvent OnSquareClicked;
  [HideInInspector] public SquareEvent OnSquareDragBegan;
  [HideInInspector] public SquareEvent OnSquareDragEnded;
  [HideInInspector] public SquareEvent OnSquareDropped;
  [HideInInspector] public SquareEvent OnSquareEntered;
  [HideInInspector] public SquareEvent OnSquareExited;

  #endregion

  #region Properties

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    board.OnSquareClicked.AddListener(square => OnSquareClicked.Invoke(square));
    board.OnSquareDragBegan.AddListener(square => OnSquareDragBegan.Invoke(square));
    board.OnSquareDragEnded.AddListener(square => OnSquareDragEnded.Invoke(square));
    board.OnSquareDropped.AddListener(square => OnSquareDropped.Invoke(square));
    board.OnSquareEntered.AddListener(square => OnSquareEntered.Invoke(square));
    board.OnSquareExited.AddListener(square => OnSquareExited.Invoke(square));
  }

  #endregion
}
