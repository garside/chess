using UnityEngine;

[CreateAssetMenu(fileName = "Palette", menuName = "Resources/Palette")]
public class Palette : ScriptableObject {
  public Color White => white;
  public Color Black => black;

  [Header("Colors")]
  [SerializeField] private Color white;
  [SerializeField] private Color black;

  public Color For(bool isWhite) => isWhite ? White : Black;
}
