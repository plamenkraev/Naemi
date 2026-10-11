using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Validation;

/// <summary>
/// Copies the cascading-lookup and descriptive fields from one property to another,
/// mirroring the legacy "Копирай"/"Постави" (copy/paste) grid buttons.
/// </summary>
public static class PropertyCopyHelper
{
    public static void CopyFields(IMOTI source, IMOTI target)
    {
        target.EK_OBL = source.EK_OBL;
        target.EKATTE_MUNICIPALITY = source.EKATTE_MUNICIPALITY;
        target.EKATTE_CITY = source.EKATTE_CITY;
        target.TERRAIN = source.TERRAIN;
        target.LIMITS = source.LIMITS;
        target.NOMER = source.NOMER;
        target.CATEGORI = source.CATEGORI;
        target.PLOST = source.PLOST;
    }
}
