using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Player : MonoBehaviour {
  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public bool IsWhite => isWhite;

  [Header("Settings")]
  [SerializeField] private bool isWhite;

  [Header("Audio")]
  [SerializeField] private AudioSource moveBad;
  [SerializeField] private AudioSource moveAbort;

  [Header("References")]
  [SerializeField] private DragPiece dragPiece;

  private GameController gameController;

  private void Start() {
  }

  private void Awake() {
    gameController = GameController.Instance;
  }
}
