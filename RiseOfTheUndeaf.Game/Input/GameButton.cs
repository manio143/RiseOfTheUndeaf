namespace RiseOfTheUndeaf.Input
{
    /// <summary>
    /// Virtual button definitions for the game.
    /// </summary>
    public enum GameButton
    {
        /// <summary>
        /// Movement left (-1) to right (+1).
        /// </summary>
        MovementHorizontal = 1,
        /// <summary>
        /// Movement back (-1) to front (+1).
        /// </summary>
        MovementVertical = 2,

        /// <summary>
        /// Camera rotation left (-1) to right (+1).
        /// </summary>
        CameraHorizontal = 3,
        /// <summary>
        /// Camera rotation down (negative) and up (positive).
        /// </summary>
        CameraVertical = 4,

        /// <summary>
        /// Character jump.
        /// </summary>
        Jump = 5,

        /// <summary>
        /// Execute primary attack for the held weapon.
        /// </summary>
        PrimaryAttack = 6,

        /// <summary>
        /// Execute secondary attack for the held weapon.
        /// </summary>
        SecondaryAttack = 7,
    }
}
