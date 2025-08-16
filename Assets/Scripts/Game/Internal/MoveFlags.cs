using System;

[Flags]
public enum MoveFlags : ushort {
  None = 0,

  // Mechanics
  Capture = 1 << 0,           // captures a piece (usually on To)
  EnPassant = 1 << 1,         // special capture: capturedSquare != To
  DoublePush = 1 << 2,        // pawn two-step; may create EP square
  CastleKingSide = 1 << 3,    // also moves rook h-file -> f-file
  CastleQueenSide = 1 << 4,   // also moves rook a-file -> d-file
  Promotion = 1 << 5,         // promotes pawn; actual piece stored elsewhere

  // Convenience
  Check = 1 << 6,             // move gives check
}
