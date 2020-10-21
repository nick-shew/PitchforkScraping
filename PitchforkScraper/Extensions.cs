using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchforkScraping
{
    static class Extensions
    {
        public static bool TryGetTextOfFirstNodeWithClass(this HtmlDocument doc, string searchString, out string text)
        {
            try
            {
                text = doc.DocumentNode.SelectNodes(searchString).FirstOrDefault().InnerText;
                return true;
            }
            catch (Exception e)
            {
                if(e is NullReferenceException || e is ArgumentNullException)
                {
                    text = "";
                    return false;
                }
                throw;
            }
        }
    }
}
