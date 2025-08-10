using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public UnityEvent OnReady;
  public UnityEvent OnEnded;

  public bool IsReady { get; private set; }

  [SerializeField] private Board board;

  private Player player;

  private void ReadyUp() {
    if (IsReady) return;
    IsReady = true;
    OnReady.Invoke();
    Debug.Log("Game Ready");
  }

  private void CreatePieces() {
    ReadyUp();
  }

  private void HandleBoardReady() {
    CreatePieces();
  }

  private void Start() {
    if (board.IsReady) HandleBoardReady();
    else board.OnReady.AddListener(HandleBoardReady);
  }

  private void Awake() {
    player = Player.Instance;
  }
}
