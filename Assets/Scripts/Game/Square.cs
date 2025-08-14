using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour {
  public enum ClaimType {
    Undefined,
    Player,
    Opponent,
    Contested,
  }

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

  public int GuardedCount {
    get => _guardedCount;
    set {
      _guardedCount = value;
      guardedCount.text = _guardedCount.ToString();
      guardedCount.enabled = _guardedCount > 1;
      guarded.enabled = _guardedCount > 0;
    }
  }

  public int OpposedCount {
    get => _opposedCount;
    set {
      _opposedCount = value;
      opposedCount.text = _opposedCount.ToString();
      opposedCount.enabled = _opposedCount > 1;
      opposed.enabled = _opposedCount > 0;
    }
  }

  public bool ShowDetails {
    get => showDetails;
    set {
      if (showDetails == value) return;
      showDetails = value;
      opposed.gameObject.SetActive(showDetails);
      guarded.gameObject.SetActive(showDetails);
      playerFlag.gameObject.SetActive(showDetails);
      opponentFlag.gameObject.SetActive(showDetails);
      contestedFlag.gameObject.SetActive(showDetails);
    }
  }

  public ClaimType Claim {
    get => _claim;
    set {
      if (_claim == value) return;
      _claim = value;

      playerFlag.enabled = _claim == ClaimType.Player;
      opponentFlag.enabled = _claim == ClaimType.Opponent;
      contestedFlag.enabled = _claim == ClaimType.Contested;
    }
  }

  public int ClaimCount {
    get => _claimCount;
    set {
      _claimCount = value;
      claimCount.enabled = _claimCount > 1;
    }
  }

  private int _claimCount;
  private ClaimType _claim;
  private Image image;

  [Header("References")]
  [SerializeField] private TextMeshProUGUI textName;
  [SerializeField] private Image guarded;
  [SerializeField] private TextMeshProUGUI guardedCount;
  [SerializeField] private Image opposed;
  [SerializeField] private TextMeshProUGUI opposedCount;
  [SerializeField] private Image playerFlag;
  [SerializeField] private Image contestedFlag;
  [SerializeField] private Image opponentFlag;
  [SerializeField] private TextMeshProUGUI claimCount;

  private int _guardedCount;
  private int _opposedCount;
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
    board.OnShowSquareDetailChanged.AddListener((show) => ShowDetails = show);

    GuardedCount = 0;
    OpposedCount = 0;

    playerFlag.enabled = false;
    contestedFlag.enabled = false;
    opponentFlag.enabled = false;
  }

  private void Awake() {
    image = GetComponent<Image>();
  }
}
