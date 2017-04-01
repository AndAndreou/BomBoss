using UnityEngine;

/// <summary>
/// Routes control from player input to hover engines
/// </summary>
public class HoverControl : MonoBehaviour
{
    /// <summary>
    /// Turning script
    /// </summary>
    public HoverOrientation Orientation;
    /// <summary>
    /// Movement script
    /// </summary>
    public MovementEngine Movement;

    // To tell which player this is
    public MyPlayer myPlayer;

    void Update()
    {
        Movement.Thrust = Input.GetAxis("Vertical" + myPlayer.ToString());
        Orientation.Turn = Input.GetAxis("Horizontal" + myPlayer.ToString());
    }
}
