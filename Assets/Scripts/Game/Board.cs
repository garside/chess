using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(GridLayoutGroup))]
public class Board : MonoBehaviour {
  [System.Serializable] public class BoolEvent : UnityEvent<bool> { }

  public const int Size = 8;
  public const int Columns = 8;
  public const int Rows = 8;
  public const int MaxColumn = Columns - 1;
  public const int MaxRow = Rows - 1;

  public BoolEvent OnShowSquareNameChanged;
  public BoolEvent OnShowSquareDetailChanged;
  public UnityEvent OnReady;

  public Palette Palette => palette;

  public bool IsReady { get; private set; }

  public bool ShowSquareName {
    get => showSquareName;
    set {
      if (showSquareName == value) return;
      showSquareName = value;
      OnShowSquareNameChanged.Invoke(value);
    }
  }

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

  private bool showSquareName = true;
  private bool showSquareDetail = true;

  private GridLayoutGroup gridLayoutGroup;

  private readonly List<Square> squares = new();

  private void Start() {
    bool black = false;
    for (int row = 0; row < Rows; row++) {
      for (int column = 0; column < Columns; column++) {
        var square = Instantiate(tilePrefab, transform).GetComponent<Square>();
        square.Configure(this, column, row, black);
        squares.Add(square);
        black = !black;
      }
      black = !black;
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
