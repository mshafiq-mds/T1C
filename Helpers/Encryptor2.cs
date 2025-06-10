using System;
using System.Collections.Generic;
using System.Linq;

public static class Encryptor2
{
    const string quote = "\"";
    public static string Encode(string vText)
    {
        int CurSpc=0;
        int varLen;
        string varChr;
        string varFin=string.Empty;

        varLen = vText.Length;

        while (CurSpc < varLen)
        {
            //CurSpc = CurSpc + 1;
            varChr = vText.Substring(CurSpc, 1); //Strings.Mid(vText, CurSpc, 1);
            switch (varChr)
            {
                case "a":
                    {
                        varChr = "coe";
                        break;
                    }

                case "b":
                    {
                        varChr = "wer";
                        break;
                    }

                case "c":
                    {
                        varChr = "ibq";
                        break;
                    }

                case "d":
                    {
                        varChr = "am7";
                        break;
                    }

                case "e":
                    {
                        varChr = "pm1";
                        break;
                    }

                case "f":
                    {
                        varChr = "mop";
                        break;
                    }

                case "g":
                    {
                        varChr = "9v4";
                        break;
                    }

                case "h":
                    {
                        varChr = "qu6";
                        break;
                    }

                case "i":
                    {
                        varChr = "zxc";
                        break;
                    }

                case "j":
                    {
                        varChr = "4mp";
                        break;
                    }

                case "k":
                    {
                        varChr = "f88";
                        break;
                    }

                case "l":
                    {
                        varChr = "qe2";
                        break;
                    }

                case "m":
                    {
                        varChr = "vbn";
                        break;
                    }

                case "n":
                    {
                        varChr = "qwt";
                        break;
                    }

                case "o":
                    {
                        varChr = "pl5";
                        break;
                    }

                case "p":
                    {
                        varChr = "13s";
                        break;
                    }

                case "q":
                    {
                        varChr = "c%l";
                        break;
                    }

                case "r":
                    {
                        varChr = "w$w";
                        break;
                    }

                case "s":
                    {
                        varChr = "6a@";
                        break;
                    }

                case "t":
                    {
                        varChr = "!2i";
                        break;
                    }

                case "u":
                    {
                        varChr = "xcg";
                        break;
                    }

                case "v":
                    {
                        varChr = "wvf";
                        break;
                    }

                case "w":
                    {
                        varChr = "dp0";
                        break;
                    }

                case "x":
                    {
                        varChr = "w$-";
                        break;
                    }

                case "y":
                    {
                        varChr = "vni";
                        break;
                    }

                case "z":
                    {
                        varChr = "c*4";
                        break;
                    }

                case "1":
                    {
                        varChr = "aq@";
                        break;
                    }

                case "2":
                    {
                        varChr = "902";
                        break;
                    }

                case "3":
                    {
                        varChr = "2qi";
                        break;
                    }

                case "4":
                    {
                        varChr = "/w!";
                        break;
                    }

                case "5":
                    {
                        varChr = "mpq";
                        break;
                    }

                case "6":
                    {
                        varChr = "mlm";
                        break;
                    }

                case "7":
                    {
                        varChr = "t'?";
                        break;
                    }

                case "8":
                    {
                        varChr = "q^s";
                        break;
                    }

                case "9":
                    {
                        varChr = "<s^";
                        break;
                    }

                case "0":
                    {
                        varChr = ";ic";
                        break;
                    }

                case "A":
                    {
                        varChr = "$)c";
                        break;
                    }

                case "B":
                    {
                        varChr = "-gt";
                        break;
                    }

                case "C":
                    {
                        varChr = "mp*";
                        break;
                    }

                case "D":
                    {
                        //varChr = "1" +  Strings.Chr(34) + "r";
                        varChr = "1" + quote + "r";
                        break;
                    }

                case "E":
                    {
                        varChr = "cq:";
                        break;
                    }

                case "F":
                    {
                        varChr = "@+x";
                        break;
                    }

                case "G":
                    {
                        varChr = "v^a";
                        break;
                    }

                case "H":
                    {
                        varChr = "]eE";
                        break;
                    }

                case "I":
                    {
                        varChr = "aP0";
                        break;
                    }

                case "J":
                    {
                        varChr = "{=1";
                        break;
                    }

                case "K":
                    {
                        varChr = "cWv";
                        break;
                    }

                case "L":
                    {
                        varChr = "cDc";
                        break;
                    }

                case "M":
                    {
                        varChr = "*,!";
                        break;
                    }

                case "N":
                    {
                        //varChr = "fW" + Strings.Chr(34);
                        varChr = "fW" + quote;
                        break;
                    }

                case "O":
                    {
                        varChr = ".?T";
                        break;
                    }

                case "P":
                    {
                        varChr = "%<8";
                        break;
                    }

                case "Q":
                    {
                        varChr = "@:a";
                        break;
                    }

                case "R":
                    {
                        varChr = "ic$";
                        break;
                    }

                case "S":
                    {
                        varChr = "WnY";
                        break;
                    }

                case "T":
                    {
                        varChr = "{Sh";
                        break;
                    }

                case "U":
                    {
                        varChr = "_%M";
                        break;
                    }

                case "V":
                    {
                        varChr = "}'$";
                        break;
                    }

                case "W":
                    {
                        varChr = "QlU";
                        break;
                    }

                case "X":
                    {
                        varChr = "Im^";
                        break;
                    }

                case "Y":
                    {
                        varChr = "lmP";
                        break;
                    }

                case "Z":
                    {
                        varChr = ".q#";
                        break;
                    }

                case "!":
                    {
                        //varChr = @"\" + Strings.Chr(34) + "]";
                        varChr = @"\" + quote + "]";
                        break;
                    }

                case "@":
                    {
                        varChr = "cY,";
                        break;
                    }

                case "#":
                    {
                        varChr = "x%B";
                        break;
                    }

                case "$":
                    {
                        varChr = "a*v";
                        break;
                    }

                case "%":
                    {
                        varChr = "'iT";
                        break;
                    }

                case "^":
                    {
                        varChr = ";%R";
                        break;
                    }

                case "&":
                    {
                        varChr = "eG_";
                        break;
                    }

                case "*":
                    {
                        varChr = "Z/e";
                        break;
                    }

                case "(":
                    {
                        varChr = @"rG\";
                        break;
                    }

                case ")":
                    {
                        varChr = "]*F";
                        break;
                    }

                case "_":
                    {
                        varChr = "@B*";
                        break;
                    }

                case "-":
                    {
                        varChr = "+Hc";
                        break;
                    }

                case "=":
                    {
                        varChr = "imD";
                        break;
                    }

                case "+":
                    {
                        varChr = "(:#";
                        break;
                    }

                case "[":
                    {
                        varChr = "SlW";
                        break;
                    }

                case "]":
                    {
                        varChr = "'QB";
                        break;
                    }

                case "{":
                    {
                        varChr = "{Dq";
                        break;
                    }

                case "}":
                    {
                        varChr = "+c%";
                        break;
                    }

                case ":":
                    {
                        varChr = "(s:";
                        break;
                    }

                case ";":
                    {
                        varChr = "^a(";
                        break;
                    }

                case "'":
                    {
                        varChr = "16.";
                        break;
                    }

                //case Strings.Chr(34):
                case quote:
                    {
                        varChr = "s.*";
                        break;
                    }

                case ",":
                    {
                        varChr = "i?W";
                        break;
                    }

                case ".":
                    {
                        varChr = "GPQ";
                        break;
                    }

                case "<":
                    {
                        varChr = "SK*";
                        break;
                    }

                case ">":
                    {
                        varChr = "RL^";
                        break;
                    }

                case "/":
                    {
                        varChr = "40C";
                        break;
                    }

                case "?":
                    {
                        varChr = "?#9";
                        break;
                    }

                case @"\":
                    {
                        varChr = "_?/";
                        break;
                    }

                case "|":
                    {
                        varChr = "(_@";
                        break;
                    }

                case " ":
                    {
                        varChr = "=#B";
                        break;
                    }
            }
            CurSpc = CurSpc + 1;    
            varFin = varFin + varChr;
        }

        //Encode = varFin;
        return varFin;
    }

    public static string DeCode(string vText)
    {
        int CurSpc=0;
        int varLen;
        string varChr;
        string varFin=string.Empty;
        CurSpc = CurSpc + 1;
        //varLen = strings.Len(vText);
        varLen = vText.Length;

        while (CurSpc <= varLen)
        {
            //DoEvents();
            //varChr = Strings.Mid(vText, CurSpc, 3);
            varChr = vText.Substring(CurSpc, 3);


            switch (varChr)
            {
                case "coe":
                    {
                        varChr = "a";
                        break;
                    }

                case "wer":
                    {
                        varChr = "b";
                        break;
                    }

                case "ibq":
                    {
                        varChr = "c";
                        break;
                    }

                case "am7":
                    {
                        varChr = "d";
                        break;
                    }

                case "pm1":
                    {
                        varChr = "e";
                        break;
                    }

                case "mop":
                    {
                        varChr = "f";
                        break;
                    }

                case "9v4":
                    {
                        varChr = "g";
                        break;
                    }

                case "qu6":
                    {
                        varChr = "h";
                        break;
                    }

                case "zxc":
                    {
                        varChr = "i";
                        break;
                    }

                case "4mp":
                    {
                        varChr = "j";
                        break;
                    }

                case "f88":
                    {
                        varChr = "k";
                        break;
                    }

                case "qe2":
                    {
                        varChr = "l";
                        break;
                    }

                case "vbn":
                    {
                        varChr = "m";
                        break;
                    }

                case "qwt":
                    {
                        varChr = "n";
                        break;
                    }

                case "pl5":
                    {
                        varChr = "o";
                        break;
                    }

                case "13s":
                    {
                        varChr = "p";
                        break;
                    }

                case "c%l":
                    {
                        varChr = "q";
                        break;
                    }

                case "w$w":
                    {
                        varChr = "r";
                        break;
                    }

                case "6a@":
                    {
                        varChr = "s";
                        break;
                    }

                case "!2i":
                    {
                        varChr = "t";
                        break;
                    }

                case "xcg":
                    {
                        varChr = "u";
                        break;
                    }

                case "wvf":
                    {
                        varChr = "v";
                        break;
                    }

                case "dp0":
                    {
                        varChr = "w";
                        break;
                    }

                case "w$-":
                    {
                        varChr = "x";
                        break;
                    }

                case "vni":
                    {
                        varChr = "y";
                        break;
                    }

                case "c*4":
                    {
                        varChr = "z";
                        break;
                    }

                case "aq@":
                    {
                        varChr = "1";
                        break;
                    }

                case "902":
                    {
                        varChr = "2";
                        break;
                    }

                case "2qi":
                    {
                        varChr = "3";
                        break;
                    }

                case "/w!":
                    {
                        varChr = "4";
                        break;
                    }

                case "mpq":
                    {
                        varChr = "5";
                        break;
                    }

                case "mlm":
                    {
                        varChr = "6";
                        break;
                    }

                case "t'?":
                    {
                        varChr = "7";
                        break;
                    }

                case "q^s":
                    {
                        varChr = "8";
                        break;
                    }

                case "<s^":
                    {
                        varChr = "9";
                        break;
                    }

                case ";ic":
                    {
                        varChr = "0";
                        break;
                    }

                case "$)c":
                    {
                        varChr = "A";
                        break;
                    }

                case "-gt":
                    {
                        varChr = "B";
                        break;
                    }

                case "mp*":
                    {
                        varChr = "C";
                        break;
                    }

                //case "1" + Strings.Chr(34) + "r":
                case "1" + quote + "r":
                    {
                        varChr = "D";
                        break;
                    }

                case "cq:":
                    {
                        varChr = "E";
                        break;
                    }

                case "@+x":
                    {
                        varChr = "F";
                        break;
                    }

                case "v^a":
                    {
                        varChr = "G";
                        break;
                    }

                case "]eE":
                    {
                        varChr = "H";
                        break;
                    }

                case "aP0":
                    {
                        varChr = "I";
                        break;
                    }

                case "{=1":
                    {
                        varChr = "J";
                        break;
                    }

                case "cWv":
                    {
                        varChr = "K";
                        break;
                    }

                case "cDc":
                    {
                        varChr = "L";
                        break;
                    }

                case "*,!":
                    {
                        varChr = "M";
                        break;
                    }

                //case "fW" + Strings.Chr(34):
                case "fW" + quote:
                    {
                        varChr = "N";
                        break;
                    }

                case ".?T":
                    {
                        varChr = "O";
                        break;
                    }

                case "%<8":
                    {
                        varChr = "P";
                        break;
                    }

                case "@:a":
                    {
                        varChr = "Q";
                        break;
                    }

                case "ic$":
                    {
                        varChr = "R";
                        break;
                    }

                case "WnY":
                    {
                        varChr = "S";
                        break;
                    }

                case "{Sh":
                    {
                        varChr = "T";
                        break;
                    }

                case "_%M":
                    {
                        varChr = "U";
                        break;
                    }

                case "}'$":
                    {
                        varChr = "V";
                        break;
                    }

                case "QlU":
                    {
                        varChr = "W";
                        break;
                    }

                case "Im^":
                    {
                        varChr = "X";
                        break;
                    }

                case "lmP":
                    {
                        varChr = "Y";
                        break;
                    }

                case ".q#":
                    {
                        varChr = "Z";
                        break;
                    }

                //case @"\" + Strings.Chr(34) + "]":
                case @"\" + quote + "]":
                    {
                        varChr = "!";
                        break;
                    }

                case "cY,":
                    {
                        varChr = "@";
                        break;
                    }

                case "x%B":
                    {
                        varChr = "#";
                        break;
                    }

                case "a*v":
                    {
                        varChr = "$";
                        break;
                    }

                case "'iT":
                    {
                        varChr = "%";
                        break;
                    }

                case ";%R":
                    {
                        varChr = "^";
                        break;
                    }

                case "eG_":
                    {
                        varChr = "&";
                        break;
                    }

                case "Z/e":
                    {
                        varChr = "*";
                        break;
                    }

                case @"rG\":
                    {
                        varChr = "(";
                        break;
                    }

                case "]*F":
                    {
                        varChr = ")";
                        break;
                    }

                case "@B*":
                    {
                        varChr = "_";
                        break;
                    }

                case "+Hc":
                    {
                        varChr = "-";
                        break;
                    }

                case "imD":
                    {
                        varChr = "=";
                        break;
                    }

                case "(:#":
                    {
                        varChr = "+";
                        break;
                    }

                case "SlW":
                    {
                        varChr = "[";
                        break;
                    }

                case "'QB":
                    {
                        varChr = "]";
                        break;
                    }

                case "{Dq":
                    {
                        varChr = "{";
                        break;
                    }

                case "+c%":
                    {
                        varChr = "}";
                        break;
                    }

                case "(s:":
                    {
                        varChr = ":";
                        break;
                    }

                case "^a(":
                    {
                        varChr = ";";
                        break;
                    }

                case "16.":
                    {
                        varChr = "'";
                        break;
                    }

                case "s.*":
                    {
                        //varChr = Strings.Chr(34);
                        varChr = quote;
                        break;
                    }

                case "i?W":
                    {
                        varChr = ",";
                        break;
                    }

                case "GPQ":
                    {
                        varChr = ".";
                        break;
                    }

                case "SK*":
                    {
                        varChr = "<";
                        break;
                    }

                case "RL^":
                    {
                        varChr = ">";
                        break;
                    }

                case "40C":
                    {
                        varChr = "/";
                        break;
                    }

                case "?#9":
                    {
                        varChr = "?";
                        break;
                    }

                case "_?/":
                    {
                        varChr = @"\";
                        break;
                    }

                case "(_@":
                    {
                        varChr = "|";
                        break;
                    }

                case "=#B":
                    {
                        varChr = " ";
                        break;
                    }
            }

            varFin = varFin + varChr;
            CurSpc = CurSpc + 3;
        }

        //DeCode = varFin;
        return varFin;
    }
}
