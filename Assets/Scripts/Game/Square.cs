using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour {
  public static char ColumnChar(int columnIndex) => (char)('a' + columnIndex);
  public static int RowValue(int rowIndex) => rowIndex + 1;
  public static string NameFor(int columnIndex, int rowIndex) => $"{ColumnChar(columnIndex)}{RowValue(rowIndex)}";

  public Board Board { get; private set; }
  public bool Black { get; private set; }
  public int Column { get; private set; }
  public int Row { get; private set; }

  public bool ShowName {
    get => textName.enabled;
    set => textName.enabled = value;
  }

  private Image image;

  [Header("References")]
  [SerializeField] private TextMeshProUGUI textName;

  public Square Up(int spaces = int.MaxValue) {
    int row = Row + spaces;
    if (row > Board.MaxRow) return null;
    return Board[NameFor(Column, row)];
  }

  public Square Down(int spaces = int.MaxValue) {
    int row = Row - spaces;
    if (row < 0) return null;
    return Board[NameFor(Column, row)];
  }

  public void Configure(Board board, int column, int row, bool black) {
    Board = board;
    Black = black;
    Column = column;
    Row = row;

    name = NameFor(Column, Row);
    image.color = board.Palette.For(black);

    textName.text = name;
    textName.enabled = board.ShowSquareName;
    textName.color = board.Palette.For(!black);

    board.OnShowSquareNameChanged.AddListener((show) => ShowName = show);
  }

  private void Awake() {
    image = GetComponent<Image>();
  }
}
