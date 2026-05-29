using Godot;

namespace Pact.Client;

public enum FacingDirection : byte
{
    N, NE, E, SE, S, SW, W, NW
}

public static class FacingDirectionExtensions
{
    public static FacingDirection ToFacingDirection(this Vector2 vector)
    {
        var deg = Mathf.RadToDeg(vector.Angle());
        if (deg < 0) deg += 360f;
        return ((int)((deg + 22.5f) / 45f) % 8) switch
        {
            0 => FacingDirection.E,
            1 => FacingDirection.SE,
            2 => FacingDirection.S,
            3 => FacingDirection.SW,
            4 => FacingDirection.W,
            5 => FacingDirection.NW,
            6 => FacingDirection.N,
            7 => FacingDirection.NE,
            _ => FacingDirection.S
        };
    }
    
    public static string ToString(this FacingDirection facing) => facing switch
    {
        FacingDirection.N  => "n",
        FacingDirection.NE => "ne",
        FacingDirection.E  => "e",
        FacingDirection.SE => "se",
        FacingDirection.S  => "s",
        FacingDirection.SW => "sw",
        FacingDirection.W  => "w",
        FacingDirection.NW => "nw",
        _      => "s"
    }; 
}