using UnityEngine;

public class Player : MonoBehaviour {
  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  private GameController gameController;

  private void Awake() {
    gameController = GameController.Instance;
  }
}
