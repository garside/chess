using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Player : MonoBehaviour {
  #region Constants

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private bool isWhite;

  [Header("Audio")]
  [SerializeField] private AudioSource moveBad;
  [SerializeField] private AudioSource moveAbort;

  [Header("References")]
  [SerializeField] private DragHarness dragHarness;

  private GameController gameController;

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool IsWhite => isWhite;

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleGameReady() {
    Debug.Log("[Player] HandleGameReady");
  }

  private void HandleSquareClicked(Square square) {
    Debug.LogFormat("[Player] HandleSquareClicked {0}", square.name);
  }

  private void HandleSquareDragBegan(Square square) {
    Debug.LogFormat("[Player] HandleSquareDragBegan {0}", square.name);
  }

  private void HandleSquareDragEnded(Square square) {
    Debug.LogFormat("[Player] HandleSquareDragEnded {0}", square.name);
  }

  private void HandleSquareDropped(Square square) {
    Debug.LogFormat("[Player] HandleSquareDropped {0}", square.name);
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
