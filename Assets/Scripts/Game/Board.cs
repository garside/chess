using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(GridLayoutGroup))]
public class Board : MonoBehaviour {
  #region Constants

  public const int Dimension = 8;

  public const int SquareCount = Dimension * Dimension;

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private GameObject squarePrefab;

  private readonly List<Square> squares = new();

  private GridLayoutGroup gridLayoutGroup;

  #endregion

  #region Events

  #endregion

  #region Properties

  public Square[] Squares => squares.ToArray();

  public Square this[int index] => squares[index];

  public Square this[int rank, int file] => this[rank * 8 + file];

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    for (int i = 0; i < SquareCount; i++) squares.Add(Instantiate(squarePrefab, transform).GetComponent<Square>());
  }

  private void Awake() {
    gridLayoutGroup = GetComponent<GridLayoutGroup>();
    gridLayoutGroup.spacing = Vector2.zero;
    gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
    gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    gridLayoutGroup.constraintCount = Dimension;
  }

  #endregion
}
