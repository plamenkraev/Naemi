using System.Text.RegularExpressions;
using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Validation;

/// <summary>
/// Reproduces the exact validation rules found in the legacy EFDAL entity classes
/// (Landlord / Property <c>IDataErrorInfo</c> implementations), so that business
/// behavior is preserved 1:1. Returns field name -> Bulgarian error message.
/// </summary>
public static class BulgarianValidation
{
    private static readonly Regex CyrillicNameRegex = new("^([а-яА-Я '-]+)$", RegexOptions.Compiled);
    private static readonly Regex StreetRegex = new("^([0-9а-яА-Я '-.]+)$", RegexOptions.Compiled);
    private static readonly Regex DigitsOnlyRegex = new("^[0-9]+$", RegexOptions.Compiled);

    public static IReadOnlyDictionary<string, string> ValidateLandlord(Landlord n)
    {
        var errors = new Dictionary<string, string>();
        if (n.IsCompany)
        {
            if (string.IsNullOrWhiteSpace(n.CompanyName))
                errors["CompanyName"] = "Въведете наименование на фирмата.";
            if (string.IsNullOrWhiteSpace(n.EIK))
                errors["EIK"] = "Въведете ЕИК / БУЛСТАТ.";
            else if (!Regex.IsMatch(n.EIK, @"^\d{9}(\d{4})?$|^\d{10}$"))
                errors["EIK"] = "ЕИК / БУЛСТАТ трябва да съдържа 9, 10 или 13 цифри.";
            return errors;
        }


       /* if (!string.IsNullOrWhiteSpace(n.NAME) && n.NAME == "[нов]")
        {
            errors["NAME"] = "Въведете име.";
        }

        if (!string.IsNullOrWhiteSpace(n.SURNAME) && !CyrillicNameRegex.IsMatch(n.SURNAME))
        {
            errors["SURNAME"] = "Позволени символи - А-Я, място и точка.";
        }

        if (!string.IsNullOrWhiteSpace(n.LASTNAME) && !CyrillicNameRegex.IsMatch(n.LASTNAME))
        {
            errors["LASTNAME"] = "Позволени символи - А-Я, място и точка.";
        }

        if (!string.IsNullOrWhiteSpace(n.STREET) && !StreetRegex.IsMatch(n.STREET))
        {
            errors["STREET"] = "Позволени символи - А-Я,1-9, място и точка.";
        }*/

        if (!string.IsNullOrWhiteSpace(n.EGN) && !DigitsOnlyRegex.IsMatch(n.EGN))
        {
            errors["EGN"] = "Позволени символи - 1-9.";
        }

        if (!string.IsNullOrWhiteSpace(n.LK) && !DigitsOnlyRegex.IsMatch(n.LK))
        {
            errors["LK"] = "Позволени символи - 1-9.";
        }

       /* if (!string.IsNullOrWhiteSpace(n.MVR) && !CyrillicNameRegex.IsMatch(n.MVR))
        {
            errors["MVR"] = "Позволени символи - А-Я, място и точка.";
        }*/

        return errors;
    }

    public static IReadOnlyDictionary<string, string> ValidateProperty(Property m)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(m.NOMER))
        {
            errors["NOMER"] = "Попълнете полето 'Номер'.";
        }
        else if (m.NOMER.Length > 6)
        {
            errors["NOMER"] = "Максимален брой 6.";
        }
        else if (!DigitsOnlyRegex.IsMatch(m.NOMER))
        {
            errors["NOMER"] = "Позволени символи - 1-9";
        }

        return errors;
    }
}
