using UnityEngine;
using UnityEngine.EventSystems;

public class FollowMouseUI : MonoBehaviour {
  private RectTransform rectTransform;
  private Canvas canvas;

  private void Awake() {
    rectTransform = GetComponent<RectTransform>();
    canvas = GetComponentInParent<Canvas>();

    if (canvas == null) {
      Debug.LogError("FollowMouseUI: No parent Canvas found!");
    }
  }

  private void Update() {
    if (canvas == null) return;

    // Convert mouse position to local position inside the canvas
    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        Input.mousePosition,
        canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
        out localPoint
    );

    rectTransform.localPosition = localPoint;
  }
}
