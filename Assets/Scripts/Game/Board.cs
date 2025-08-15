using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(GridLayoutGroup))]
public class Board : MonoBehaviour {
  [System.Serializable] public class SquareEvent : UnityEvent<Square> { }
  [System.Serializable] public class BoolEvent : UnityEvent<bool> { }

  public const int Size = 8;
  public const int Columns = 8;
  public const int Rows = 8;
  public const int MaxColumn = Columns - 1;
  public const int MaxRow = Rows - 1;

  public SquareEvent OnClicked;
  public SquareEvent OnDropped;
  public BoolEvent OnShowSquareDetailChanged;
  public UnityEvent OnReady;

  public Palette Palette => palette;

  public bool IsReady { get; private set; }

  public bool ShowSquareDetail {
    get => showSquareDetail;
    set {
      if (showSquareDetail == value) return;
      showSquareDetail = value;
      OnShowSquareDetailChanged.Invoke(value);
    }
  }

  public Square[] Squares => squares.ToArray();
  public Square this[string squareName] => squares.FirstOrDefault(square => square.name == squareName);

  [Header("Tile Settings")]
  [SerializeField] private Palette palette;
  [SerializeField] private GameObject tilePrefab;

  private bool showSquareDetail = true;

  private GridLayoutGroup gridLayoutGroup;

  private readonly List<Square> squares = new();

  private void HandleSquareClicked(Square square) {
    OnClicked.Invoke(square);
  }

  private void HandleSquareDropped(Square square) {
    OnDropped.Invoke(square);
  }

  private void Start() {
    bool isWhite = true;
    for (int row = 0; row < Rows; row++) {
      for (int column = 0; column < Columns; column++) {
        var square = Instantiate(tilePrefab, transform).GetComponent<Square>();
        square.Configure(this, column, row, isWhite);
        squares.Add(square);
        square.OnClicked.AddListener(HandleSquareClicked);
        square.OnDropped.AddListener(HandleSquareDropped);
        isWhite = !isWhite;
      }
      isWhite = !isWhite;
    }

    IsReady = true;
    OnReady.Invoke();
  }

  private void Awake() {
    gridLayoutGroup = GetComponent<GridLayoutGroup>();
    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    gridLayoutGroup.constraintCount = Columns;
  }
}
