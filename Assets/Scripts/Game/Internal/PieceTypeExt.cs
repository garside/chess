
public static class PieceTypeExt {
  // Common material values in centipawns
  public static int Value(this PieceType pt) => pt switch {
    PieceType.Pawn => 100,
    PieceType.Knight => 320,
    PieceType.Bishop => 330,
    PieceType.Rook => 500,
    PieceType.Queen => 900,
    _ => 0
  };

  // Uppercase letters for white by convention; lowercase for black later
  public static char ToChar(this PieceType pt) => pt switch {
    PieceType.Pawn => 'P',
    PieceType.Knight => 'N',
    PieceType.Bishop => 'B',
    PieceType.Rook => 'R',
    PieceType.Queen => 'Q',
    PieceType.King => 'K',
    _ => '.'
  };
}
