using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
  #region Constants

  public static Color Black => new(0.88f, 0.88f, 0.88f);
  public static Color White => new(1.00f, 1.00f, 1.00f);

  #endregion

  #region Internal

  [System.Serializable]
  public class Overlays {
    public Image opponentCoverage;
    public Image playerCoverage;
    public Image screen;
  }

  #endregion

  #region Fields

  [SerializeField] private Overlays overlays;

  private Image image;

  private int? index;

  #endregion

  #region Events

  #endregion

  #region Properties

  public int Index {
    get {
      if (!index.HasValue) index = transform.GetSiblingIndex();
      return index.Value;
    }
  }

  public bool IsWhite => ((File + Rank) % 2) == 0;

  public int Rank => Index / 8;

  public int File => Index % 8;

  public float OpponentCoverageOpacity {
    get => overlays.opponentCoverage.color.a;
    set {
      var color = overlays.opponentCoverage.color;
      color.a = Mathf.Clamp01(value);
      overlays.opponentCoverage.color = color;
    }
  }

  public float PlayerCoverageOpacity {
    get => overlays.playerCoverage.color.a;
    set {
      var color = overlays.playerCoverage.color;
      color.a = Mathf.Clamp01(value);
      overlays.playerCoverage.color = color;
    }
  }

  public bool ScreenVisible {
    get => overlays.screen.enabled;
    set => overlays.screen.enabled = value;
  }

  #endregion

  #region Methods

  public override string ToString() => $"{(char)('a' + File)}{Rank + 1}";

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
    throw new System.NotImplementedException();
  }

  void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
    throw new System.NotImplementedException();
  }

  void IDragHandler.OnDrag(PointerEventData eventData) {
    throw new System.NotImplementedException();
  }

  void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
    throw new System.NotImplementedException();
  }

  #endregion

  #region Lifecycle

  private void Start() {
    name = ToString();
    image.color = IsWhite ? White : Black;
    OpponentCoverageOpacity = 0f;
    PlayerCoverageOpacity = 0f;
    ScreenVisible = false;
  }

  private void Awake() {
    image = GetComponent<Image>();
  }

  #endregion
}
