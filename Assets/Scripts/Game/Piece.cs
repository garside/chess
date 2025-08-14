using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Outline))]
public class Piece : MonoBehaviour {
  [System.Serializable] public class PieceEvent : UnityEvent<Piece> { }

  public enum PieceType {
    Undefined,
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King,
  }

  public PieceEvent OnClick;

  public PieceType Type => type;

  public Color Color {
    get => image.color;
    set => image.color = value;
  }

  public Color Outline {
    get => outline.effectColor;
    set => outline.effectColor = value;
  }

  public Square Square {
    get => square;
    set {
      square = value;
      transform.parent = square.transform;
      transform.localPosition = Vector2.zero;
    }
  }

  public bool IsWhite {
    get => isWhite.Value;
    set {
      if (isWhite.HasValue) return;
      isWhite = value;
    }
  }

  [SerializeField] private PieceType type;

  private Square square;
  private bool? isWhite;
  private Image image;
  private Outline outline;

  public void Click() {
    OnClick.Invoke(this);
  }

  private void Awake() {
    image = GetComponent<Image>();
    outline = GetComponent<Outline>();
  }
}
