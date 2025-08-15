using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FollowMouseUI))]
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Image))]
public class DragPiece : MonoBehaviour {
  private Image image;
  private Outline outline;

  public Color Outline {
    get => outline.effectColor;
    set => outline.effectColor = value;
  }

  public Sprite Sprite {
    get => image.sprite;
    set => image.sprite = value;
  }

  public bool IsVisible {
    get => image.enabled;
    set => image.enabled = value;
  }

  private void Start() {
    IsVisible = false;
  }

  private void Awake() {
    image = GetComponent<Image>();
    outline = GetComponent<Outline>();
  }
}
