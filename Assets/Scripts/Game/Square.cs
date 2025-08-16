using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IDropHandler {
  #region Constants

  public static Color Black => new(0.88f, 0.88f, 0.88f);
  public static Color White => new(1.00f, 1.00f, 1.00f);

  #endregion

  #region Internal

  [System.Serializable] public class SquareEvent : UnityEvent<Square> { }

  [System.Serializable]
  public class Overlays {
    public Image opponentCoverage;
    public Image playerCoverage;
    public Image border;
    public Image screen;
  }

  #endregion

  #region Fields

  [SerializeField] private Overlays overlays;

  private Image image;

  private int? index;

  #endregion

  #region Events

  [HideInInspector] public SquareEvent OnClicked;
  [HideInInspector] public SquareEvent OnDragBegan;
  [HideInInspector] public SquareEvent OnDragEnded;
  [HideInInspector] public SquareEvent OnDropped;
  [HideInInspector] public SquareEvent OnEntered;
  [HideInInspector] public SquareEvent OnExited;

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

  public bool BorderVisible {
    get => overlays.border.enabled;
    set => overlays.border.enabled = value;
  }

  public bool ScreenVisible {
    get => overlays.screen.enabled;
    set => overlays.screen.enabled = value;
  }

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

  #endregion

  #region Methods

  public override string ToString() => $"{(char)('a' + File)}{Rank + 1}";

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => OnClicked.Invoke(this);

  void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) => OnDragBegan.Invoke(this);

  public void OnDrag(PointerEventData eventData) { }

  void IEndDragHandler.OnEndDrag(PointerEventData eventData) => OnDragEnded.Invoke(this);

  void IDropHandler.OnDrop(PointerEventData eventData) => OnDropped.Invoke(this);

  void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnEntered.Invoke(this);

  void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => OnExited.Invoke(this);

  #endregion

  #region Lifecycle

  private void Start() {
    name = ToString();
    image.color = IsWhite ? White : Black;
    BorderVisible = false;
    ScreenVisible = false;
    OpponentCoverageOpacity = 0f;
    PlayerCoverageOpacity = 0f;
  }

  private void Awake() {
    image = GetComponent<Image>();
  }

  #endregion
}
