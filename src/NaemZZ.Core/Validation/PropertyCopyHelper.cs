using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Validation;

/// <summary>
/// Copies the cascading-lookup and descriptive fields from one property to another,
/// mirroring the legacy "Копирай"/"Постави" (copy/paste) grid buttons.
/// </summary>
public static class PropertyCopyHelper
{
    public static void CopyFields(Property source, Property target)
    {
        target.Region = source.Region;
        target.EKATTE_MUNICIPALITY = source.EKATTE_MUNICIPALITY;
        target.EKATTE_CITY = source.EKATTE_CITY;
        target.TERRAIN = source.TERRAIN;
        target.LIMITS = source.LIMITS;
        target.NOMER = source.NOMER;
        target.PropertyCategory = source.PropertyCategory;
        target.PLOST = source.PLOST;
    }
}
