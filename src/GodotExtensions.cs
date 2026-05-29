namespace Pact;

public static class GodotExtensions
{
    public const float PixelsToUnitsRatio = 64f; 
    
    public static float PixelsToUnits(this float pixels) => pixels * PixelsToUnitsRatio;
}
