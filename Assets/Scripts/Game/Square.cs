using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour {
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

  public void Configure(Board board, int column, int row, bool black) {
    Board = board;
    Black = black;
    Column = column;
    Row = row;

    name = $"{(char)('a' + column)}{row + 1}";
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
