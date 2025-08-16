public sealed class Move {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  #endregion

  #region Properties

  public MoveFlags Flags { get; private set; }

  public Piece Piece { get; private set; }

  public Square From { get; private set; }

  public Square To { get; private set; }

  public Square Captures { get; private set; }

  public bool IsCapture => (Flags & MoveFlags.Capture) != 0;

  public bool IsEnPassant => (Flags & MoveFlags.EnPassant) != 0;

  public bool IsDoublePush => (Flags & MoveFlags.DoublePush) != 0;

  public bool IsCastleKS => (Flags & MoveFlags.CastleKingSide) != 0;

  public bool IsCastleQS => (Flags & MoveFlags.CastleQueenSide) != 0;

  public bool IsPromotion => (Flags & MoveFlags.Promotion) != 0;

  public bool GivesCheck => (Flags & MoveFlags.Check) != 0;

  #endregion

  #region Methods

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public Move(Piece piece, Square to, MoveFlags flags = MoveFlags.None, Square captures = null) {
    Piece = piece;
    From = piece.Square;
    To = to;
    Flags = flags;
    Captures = captures;
  }

  #endregion
}