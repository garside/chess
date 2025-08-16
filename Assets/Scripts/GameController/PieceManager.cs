using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoardManager))]
public class PieceManager : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class PiecePrefabs {
    public GameObject Pawn;
    public GameObject Knight;
    public GameObject Bishop;
    public GameObject Rook;
    public GameObject Queen;
    public GameObject King;
  }

  #endregion

  #region Fields

  [Header("Prefabs")]
  [SerializeField] private PiecePrefabs whitePrefabs;
  [SerializeField] private PiecePrefabs blackPrefabs;

  private BoardManager boardManager;

  private readonly List<Piece> pieces = new();

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public bool IsReady { get; private set; }

  public Piece[] Pieces => pieces.ToArray();

  public Piece this[Square square] => pieces.FirstOrDefault(piece => piece.Square == square);

  #endregion

  #region Methods

  public bool AnyOn(Square square) => this[square] != null;

  private void Spawn(GameObject prefab, Square square) {
    var piece = Instantiate(prefab).GetComponent<Piece>();
    piece.Square = square;
    pieces.Add(piece);
  }

  private void SpawnPawns(GameObject prefab, int rank) {
    for (int file = 0; file < 8; file++) Spawn(prefab, boardManager[rank, file]);
  }

  private void SpawnWhite() {
    SpawnPawns(whitePrefabs.Pawn, 1);
    Spawn(whitePrefabs.Knight, boardManager[0, 1]);
    Spawn(whitePrefabs.Knight, boardManager[0, 6]);
  }

  private void SpawnBlack() {
    SpawnPawns(blackPrefabs.Pawn, 6);
    Spawn(blackPrefabs.Knight, boardManager[7, 1]);
    Spawn(blackPrefabs.Knight, boardManager[7, 6]);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleBoardReady() {
    if (IsReady) return;

    SpawnWhite();
    SpawnBlack();
    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Lifecycle

  private void Start() {
    if (boardManager.IsReady) HandleBoardReady();
    else boardManager.OnReady.AddListener(HandleBoardReady);
  }

  private void Awake() {
    boardManager = GetComponent<BoardManager>();
  }

  #endregion
}
