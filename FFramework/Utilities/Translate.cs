using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace FFramework.Utilities
{
    public class Translate
    {
        ///<summary>Translates the given text</summary>
        ///<param name="input">Text you want to translate</param>
        ///<param name="language_pair">Pair of languages that you want to translate to/from, example: "en|ru"</param>
        ///<returns>Translated text</returns>
        public static string TranslateText(string input, string language_pair)
        {
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, language_pair);
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            string result = webClient.DownloadString(url);
            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            return result.Trim();
        }
    }
}
