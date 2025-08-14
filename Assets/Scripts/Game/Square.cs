using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour {
  [System.Serializable]
  public class CoverageOverlay {
    public Image player;
    public Image enemy;
  }

  public static float[] CoverageOpacity = new[] { 0, 0.2f, 0.4f, 0.6f };
  private static readonly int CoverageLimit = CoverageOpacity.Length - 1;

  public static char ColumnChar(int columnIndex) => (char)('a' + columnIndex);
  public static int RowValue(int rowIndex) => rowIndex + 1;
  public static string NameFor(int columnIndex, int rowIndex) => $"{ColumnChar(columnIndex)}{RowValue(rowIndex)}";

  public Board Board { get; private set; }
  public bool IsWhite { get; private set; }
  public int Column { get; private set; }
  public int Row { get; private set; }

  public bool ShowDetails {
    get => showDetails;
    set {
      if (showDetails == value) return;
      showDetails = value;
      textName.enabled = value;
      coverageOverlay.player.enabled = value;
      coverageOverlay.enemy.enabled = value;
    }
  }

  public int PlayerCoverage {
    get => playerCoverage;
    set {
      playerCoverage = value;
      playerCoverageColor.a = CoverageOpacity[Mathf.Min(playerCoverage, CoverageLimit)];
      coverageOverlay.player.color = playerCoverageColor;
    }
  }

  public int EnemyCoverage {
    get => enemyCoverage;
    set {
      enemyCoverage = value;
      enemyCoverageColor.a = CoverageOpacity[Mathf.Min(enemyCoverage, CoverageLimit)];
      coverageOverlay.enemy.color = enemyCoverageColor;
    }
  }

  private Image image;

  [Header("References")]
  [SerializeField] private TextMeshProUGUI textName;
  [SerializeField] private CoverageOverlay coverageOverlay;

  private Color playerCoverageColor;
  private Color enemyCoverageColor;

  private int playerCoverage;
  private int enemyCoverage;
  private bool showDetails;

  public Square UpLeft(int spaces = int.MaxValue) {
    int row = Row + spaces;
    int column = Column + spaces;
    if (row > Board.MaxRow) return null;
    if (column > Board.MaxColumn) return null;
    return Board[NameFor(column, row)];
  }

  public Square UpRight(int spaces = int.MaxValue) {
    int row = Row + spaces;
    int column = Column - spaces;
    if (row > Board.MaxRow) return null;
    if (column < 0) return null;
    return Board[NameFor(column, row)];
  }

  public Square DownLeft(int spaces = int.MaxValue) {
    int row = Row - spaces;
    int column = Column + spaces;
    if (row < 0) return null;
    if (column > Board.MaxColumn) return null;
    return Board[NameFor(column, row)];
  }

  public Square DownRight(int spaces = int.MaxValue) {
    int row = Row - spaces;
    int column = Column - spaces;
    if (row < 0) return null;
    if (column < 0) return null;
    return Board[NameFor(column, row)];
  }

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

  public Square Left(int spaces = int.MaxValue) {
    int column = Column + spaces;
    if (column > Board.MaxColumn) return null;
    return Board[NameFor(column, Row)];
  }

  public Square Right(int spaces = int.MaxValue) {
    int column = Column - spaces;
    if (column < 0) return null;
    return Board[NameFor(column, Row)];
  }

  public void Configure(Board board, int column, int row, bool isWhite) {
    Board = board;
    IsWhite = isWhite;
    Column = column;
    Row = row;

    name = NameFor(Column, Row);
    image.color = board.Palette.For(isWhite);

    textName.text = name;
    textName.enabled = board.ShowSquareDetail;
    //textName.color = board.Palette.For(!isWhite);

    board.OnShowSquareDetailChanged.AddListener((show) => ShowDetails = show);

    PlayerCoverage = 0;
    EnemyCoverage = 0;
  }

  private void Awake() {
    image = GetComponent<Image>();
    playerCoverageColor = coverageOverlay.player.color;
    enemyCoverageColor = coverageOverlay.enemy.color;
  }
}
