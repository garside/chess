using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Piece : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private PieceType pieceType;
  [SerializeField] private bool isWhite;
  [SerializeField] private Image outline;

  private RectTransform rectTransform;

  private Image image;

  private Color originalOutlineColor;

  private Square square;

  #endregion

  #region Events

  #endregion

  #region Properties

  public PieceType PieceType => pieceType;

  public bool IsWhite => isWhite;

  public Sprite Sprite => image.sprite;

  public Color OutlineColor {
    get => outline.color;
    set => outline.color = value;
  }

  public Square Square {
    get => square;
    set {
      square = value;
      ReparentToSquare();
    }
  }

  #endregion

  #region Methods

  public void ReparentToSquare() {
    ReparentTo(square.transform);
  }

  public void ReparentTo(Transform transform) {
    transform.parent = transform;
    transform.localPosition = Vector3.zero;
    transform.localScale = Vector3.one;
    rectTransform.offsetMin = Vector2.zero;
    rectTransform.offsetMax = Vector2.zero;
  }

  public void ResetOutlineColor() {
    OutlineColor = originalOutlineColor;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    originalOutlineColor = outline.color;
    name = $"{PieceType.ToChar()}";
  }

  private void Awake() {
    rectTransform = transform as RectTransform;
    image = GetComponent<Image>();
  }

  #endregion
}
