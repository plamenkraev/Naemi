namespace NaemZZ.Desktop.Printing;

/// <summary>
/// Converts numbers to Bulgarian words for the printed rental contracts (area in дка and the
/// cadastral "Номер"). Ported unchanged from the legacy NaemZZ application (original:
/// NUM2WORD.cs) to preserve the exact printed wording.
/// </summary>
public class NUM2WORD
{
    private static readonly string[] warray = new string[10];

    private static readonly string[] wordray = new string[10];

    private static string sto = "";

    private static string dwesta = "";

    private static string trista = "";

    private static string stotin = "";

    private static string deset = "";

    private static string mil = "";

    private static string mila = "";

    private static string hil = "";

    private static string hili = "";

    private static string lw = "";

    private static string st = "";

    private static string i = "";

    private static string jn = "";

    private static string na = "";

    private static int C1;

    private static void Init()
    {
        warray[1] = "ед";
        warray[2] = "дв";
        warray[3] = "три";
        warray[4] = "четири";
        warray[5] = "пет";
        warray[6] = "шест";
        warray[7] = "седем";
        warray[8] = "осем";
        warray[9] = "девет";
        jn = "ин";
        na = "на";
        sto = "сто";
        dwesta = "двеста";
        trista = "триста";
        stotin = "стотин";
        deset = "десет";
        mil = "милион";
        mila = " милиона";
        hil = "хиляда";
        hili = " хиляди";
        lw = " дка";
        st = " квадрата";
        i = " и ";
    }

    private static void initWord()
    {
        wordray[0] = "нула";
        wordray[1] = "едно";
        wordray[2] = "две";
        wordray[3] = "три";
        wordray[4] = "четири";
        wordray[5] = "пет";
        wordray[6] = "шест";
        wordray[7] = "седем";
        wordray[8] = "осем";
        wordray[9] = "девет";
    }

    private static string Conv(string sstr)
    {
        var astrc = "";
        var rstr = "";
        var bstrc = "";
        var cstrc = "";
        var asc = '0';
        int apos;
        int bpos;
        int cpos;
        if (sstr.Length == 3)
        {
            apos = Convert.ToChar(sstr.Substring(2, 1)) - asc;
            bpos = Convert.ToChar(sstr.Substring(1, 1)) - asc;
            cpos = Convert.ToChar(sstr.Substring(0, 1)) - asc;
        }
        else
        {
            apos = Convert.ToChar(sstr.Substring(1, 1)) - asc;
            bpos = Convert.ToChar(sstr.Substring(0, 1)) - asc;
            cpos = 0;
        }
        switch (apos)
        {
            case 1:
                if (sstr.Length != 3)
                {
                    astrc = warray[1] + na;
                }
                else
                {
                    astrc = (C1 != 1) ? (warray[1] + jn) : (warray[1] + na);
                }
                break;
            case 2:
                if (sstr.Length != 3)
                {
                    astrc = warray[2] + "e";
                }
                else
                {
                    astrc = (!((C1 == 1) & (bpos != 1))) ? (warray[2] + "а") : (warray[2] + "е");
                }
                break;
            default:
                if ((apos >= 3) & (apos <= 9))
                {
                    astrc = warray[apos];
                }
                break;
        }
        switch (bpos)
        {
            case 1:
                switch (apos)
                {
                    case 1:
                        bstrc = astrc + "а" + deset;
                        astrc = "";
                        break;
                    case 0:
                        bstrc = deset;
                        astrc = "";
                        break;
                    default:
                        if ((apos >= 2) & (apos <= 9))
                        {
                            bstrc = astrc + na + deset;
                        }
                        astrc = "";
                        break;
                }
                break;
            case 2:
                bstrc = warray[bpos] + "а" + deset;
                break;
            default:
                if ((bpos >= 3) & (bpos <= 9))
                {
                    bstrc = warray[bpos] + deset;
                }
                break;
        }
        switch (cpos)
        {
            case 1:
                cstrc = sto;
                break;
            case 2:
                cstrc = dwesta;
                break;
            case 3:
                cstrc = trista;
                break;
            default:
                if ((cpos >= 4) & (cpos <= 9))
                {
                    cstrc = warray[cpos] + stotin;
                }
                break;
        }
        rstr = astrc;
        if (cstrc.Length > 0)
        {
            if (astrc.Length > 0)
            {
                rstr = (bstrc.Length <= 0) ? (cstrc + i + rstr) : (cstrc + " " + bstrc + i + rstr);
            }
            else
            {
                rstr = (bstrc.Length <= 0) ? cstrc : (cstrc + i + bstrc);
            }
        }
        else if (bstrc.Length > 0)
        {
            rstr = (astrc.Length <= 0) ? bstrc : (bstrc + i + rstr);
        }
        return rstr;
    }

    public static string Num2Txt(string instring)
    {
        var fstr = "";
        Init();
        var wstr = instring;
        var lastDelimiter = wstr.IndexOf(',', 0);
        if (lastDelimiter <= 0)
        {
            lastDelimiter = wstr.IndexOf('.', 0);
        }
        if (lastDelimiter > 0)
        {
            fstr = wstr.Substring(lastDelimiter + 1, 3);
            wstr = wstr[..lastDelimiter];
        }
        if (fstr.Length < 3)
        {
            fstr += "000".Substring(1, 3 - fstr.Length);
        }
        var ostring = "";
        C1 = 0;
        while (!string.IsNullOrEmpty(wstr))
        {
            if (wstr.Length < 3)
            {
                wstr = "000".Substring(1, 3 - wstr.Length) + wstr;
            }
            var tstr = wstr.Substring(wstr.Length - 3, 3);
            var cstrc = Conv(tstr);
            switch (C1)
            {
                case 1:
                    if (tstr == "001")
                    {
                        cstrc = hil;
                    }
                    else if (!string.IsNullOrEmpty(cstrc))
                    {
                        cstrc += hili;
                    }
                    break;
                case 2:
                    if (tstr == "001")
                    {
                        cstrc = warray[1] + jn + " " + mil;
                    }
                    else if (!string.IsNullOrEmpty(cstrc))
                    {
                        cstrc += mila;
                    }
                    break;
            }
            wstr = (wstr.Length < 3) ? "" : wstr.Substring(1, wstr.Length - 3);
            if (!string.IsNullOrEmpty(cstrc))
            {
                if (C1 != 0)
                {
                    ostring = cstrc + " " + ostring;
                }
                else
                {
                    ostring = (!(!string.IsNullOrEmpty(wstr) & (cstrc.IndexOf(i, 1) == 0))) ? cstrc : (i + cstrc);
                }
            }
            C1++;
        }
        if (fstr.Length > 0)
        {
            if (!string.IsNullOrEmpty(ostring))
            {
                return ostring + lw + " и " + fstr + st;
            }
            return fstr + st;
        }
        return ostring + lw;
    }

    public static string word(string instring)
    {
        var returnWord = "";
        initWord();
        for (var z = 0; z < instring.Length; z++)
        {
            returnWord = returnWord + " " + wordray[Convert.ToInt32(instring.Substring(z, 1))];
        }
        return returnWord;
    }
}
