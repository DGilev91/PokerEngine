namespace PokerEngine.Enums;

public enum HandState
{
    /// <summary>
    /// The hand has not been initialized.
    /// </summary>
    None,

    /// <summary>
    /// The hand has been initialized and is ready for forced posts and setup.
    /// </summary>
    Initialized,

    /// <summary>
    /// The hand has started and gameplay is in progress.
    /// </summary>
    Started,

    /// <summary>
    /// The hand has finished and all pots have been awarded.
    /// </summary>
    Completed
}