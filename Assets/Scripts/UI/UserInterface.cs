using UnityEngine;

public class UserInterface : MonoBehaviour {
  public static UserInterface Instance => GameObject.FindWithTag("UserInterface").GetComponent<UserInterface>();

  [Header("References")]
  [SerializeField] private GameObject loadingScreen;
  [SerializeField] private GameObject opponentTurn;
  [SerializeField] private GameObject playerTurn;

  private Player player;
  private GameController gameController;

  private void SyncTurn() {
    bool isPlayerTurn = player.IsWhite ? gameController.WhiteToMove : !gameController.WhiteToMove;
    opponentTurn.SetActive(!isPlayerTurn);
    playerTurn.SetActive(isPlayerTurn);
  }

  private void HandleGameReady() {
    SyncTurn();
    loadingScreen.SetActive(false);
  }

  private void HandlePieceMoved(PieceManager.Move move) {
    SyncTurn();
  }

  private void Start() {
    gameController.OnPieceMoved.AddListener(HandlePieceMoved);

    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);
  }

  private void Awake() {
    player = Player.Instance;
    gameController = GameController.Instance;
  }
}
