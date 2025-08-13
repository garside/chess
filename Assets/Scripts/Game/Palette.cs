using UnityEngine;

[CreateAssetMenu(fileName = "Palette", menuName = "Resources/Palette")]
public class Palette : ScriptableObject {
  public Color Black => black;
  public Color White => white;

  [Header("Colors")]
  [SerializeField] private Color black;
  [SerializeField] private Color white;

  public Color For(bool black) => black ? Black : White;
}
