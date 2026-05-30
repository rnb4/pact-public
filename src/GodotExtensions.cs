namespace Pact;

public static class GodotExtensions
{
    public const float PixelsToUnitsRatio = 64f / 1f;

    public static float UnitsToPixels(this float pixels)
    {
        return pixels * PixelsToUnitsRatio;
    }

    public static float PixelsToUnits(this float pixels)
    {
        return pixels / PixelsToUnitsRatio;
    }
}