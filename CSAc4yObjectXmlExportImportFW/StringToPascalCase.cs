using System;
using System.Collections.Generic;

namespace CSAc4yObjectXmlExportImportFW
{
    class StringToPascalCase
    {

        private Dictionary<string, string> foreign_characters = new Dictionary<string, string>
        {

            { "äæǽ", "ae" },
            { "öœ", "o" },
            { "ü", "u" },
            { "Ä", "Ae" },
            { "Ü", "U" },
            { "Ö", "O" },
            { "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "Б", "B" },
            { "б", "b" },
            { "ÇĆĈĊČ", "C" },
            { "çćĉċč", "c" },
            { "Д", "D" },
            { "д", "d" },
            { "ÐĎĐΔ", "Dj" },
            { "ðďđδ", "dj" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "Ф", "F" },
            { "ф", "f" },
            { "ĜĞĠĢΓГҐ", "G" },
            { "ĝğġģγгґ", "g" },
            { "ĤĦ", "H" },
            { "ĥħ", "h" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "Ĵ", "J" },
            { "ĵ", "j" },
            { "ĶΚК", "K" },
            { "ķκк", "k" },
            { "ĹĻĽĿŁΛЛ", "L" },
            { "ĺļľŀłλл", "l" },
            { "М", "M" },
            { "м", "m" },
            { "ÑŃŅŇΝН", "N" },
            { "ñńņňŉνн", "n" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "П", "P" },
            { "п", "p" },
            { "ŔŖŘΡР", "R" },
            { "ŕŗřρр", "r" },
            { "ŚŜŞȘŠΣС", "S" },
            { "śŝşșšſσςс", "s" },
            { "ȚŢŤŦτТ", "T" },
            { "țţťŧт", "t" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "В", "V" },
            { "в", "v" },
            { "Ŵ", "W" },
            { "ŵ", "w" },
            { "ŹŻŽΖЗ", "Z" },
            { "źżžζз", "z" },
            { "ÆǼ", "AE" },
            { "ß", "ss" },
            { "Ĳ", "IJ" },
            { "ĳ", "ij" },
            { "Œ", "OE" },
            { "ƒ", "f" },
            { "ξ", "ks" },
            { "π", "p" },
            { "β", "v" },
            { "μ", "m" },
            { "ψ", "ps" },
            { "Ё", "Yo" },
            { "ё", "yo" },
            { "Є", "Ye" },
            { "є", "ye" },
            { "Ї", "Yi" },
            { "Ж", "Zh" },
            { "ж", "zh" },
            { "Х", "Kh" },
            { "х", "kh" },
            { "Ц", "Ts" },
            { "ц", "ts" },
            { "Ч", "Ch" },
            { "ч", "ch" },
            { "Ш", "Sh" },
            { "ш", "sh" },
            { "Щ", "Shch" },
            { "щ", "shch" },
            { "ЪъЬь", "" },
            { "Ю", "Yu" },
            { "ю", "yu" },
            { "Я", "Ya" },
            { "я", "ya" },
        };

        public string DeleteAccents(string defaultText)
        {
            //StringBuilder sb = new StringBuilder ();
            string text = "";

            if (defaultText.Length > 0)
            {


                foreach (char c in defaultText)
                {
                    int len = text.Length;

                    foreach (KeyValuePair<string, string> entry in foreign_characters)
                    {
                        if (entry.Key.IndexOf(c) != -1)
                        {
                            text += entry.Value;
                            break;
                        }
                    }

                    if (len == text.Length)
                    {
                        text += c;
                    }
                }
            }

            return text;

        } // DeleteAccents

        public string ConvertToPascalCase(string text)
        {
            if (text.Length > 0)
            {
                string[] words = text.Split(
                    new char[] { },
                    StringSplitOptions.RemoveEmptyEntries);

                string result = words[0].ToLower();
                for (int i = 1; i < words.Length; i++)
                {
                    result +=
                        words[i].Substring(0, 1).ToUpper() +
                        words[i].Substring(1);
                }

                return result;
            }
            else
            {
                return "";
            }

        } // ConvertToPascalCase

        public string Convert(string text)
        {
            if (text.Length > 0)
            {
                string pascalCase = ConvertToPascalCase(DeleteAccents(text));
                string result = pascalCase.Substring(0, 1).ToUpper() + pascalCase.Substring(1);
                return result;
            }
            else
            {
                return "";
            }

        } // Convert

    } // StringToPascalCase

} // CSAc4yObjectXmlExportImport