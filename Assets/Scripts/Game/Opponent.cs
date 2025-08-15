using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Opponent : MonoBehaviour {
  public static Opponent Instance => GameObject.FindWithTag("Opponent").GetComponent<Opponent>();

  private Player player;
  private GameController gameController;

  private void DecideMove() {
    List<Square> squares = new();

    var movesBySquare = gameController.PieceManager.OpponentMoves.GroupBy(move => move.Square);
    int maxGuardedCount = -int.MaxValue;

    foreach (var entry in movesBySquare) {
      var guardedCount = entry.Count(move => move.Type == PieceManager.Move.MoveType.Guarded);
      if (guardedCount > maxGuardedCount) {
        squares.Clear();
        squares.Add(entry.Key);
        maxGuardedCount = guardedCount;
        Debug.LogFormat("[AI] {0} guardedCount {1} is highest", entry.Key, guardedCount);
      } else if (guardedCount == maxGuardedCount) {
        squares.Add(entry.Key);
        Debug.LogFormat("[AI] {0} guardedCount is equally high", entry.Key);
      }
    }

    Debug.LogFormat("[AI] Possible squares: ", string.Join(",", squares));
  }

  private void SyncTurn() {
    bool isOpponentTurn = player.IsWhite ? !gameController.WhiteToMove : gameController.WhiteToMove;
    if (!isOpponentTurn) return;

    Debug.LogFormat("AI Turn Begins @{0}", Time.time);
    //DecideMove();
  }

  private void HandleGameReady() {
    SyncTurn();
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
