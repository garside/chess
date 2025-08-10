using UnityEngine;

public class UserInterface : MonoBehaviour {
  public static UserInterface Instance => GameObject.FindWithTag("UserInterface").GetComponent<UserInterface>();

  [Header("References")]
  [SerializeField] private GameObject loadingScreen;

  private Player player;
  private GameController gameController;

  private void HandleGameReady() {
    loadingScreen.SetActive(false);
  }

  private void Start() {
    gameController.OnReady.AddListener(HandleGameReady);
  }

  private void Awake() {
    player = Player.Instance;
    gameController = GameController.Instance;
  }
}
