using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchforkScraping
{
    static class Extensions
    {
        //public static IEnumerable<HtmlNode> GetNodesWithClassDescendantSearch(this HtmlDocument doc, string className)
        //{
        //    return doc.DocumentNode.Descendants().Where(n => n.HasClass(className));
        //}
        //public static IEnumerable<HtmlNode> GetNodesWithClass(this HtmlDocument doc, string className, string divOrSpan = "div")
        //{
        //    var nodes = doc.DocumentNode.SelectNodes($"//{divOrSpan}[@class='{className}']");//TODO--why does this work only some of the time
        //    if (nodes == null)
        //    {
        //        return doc.GetNodesWithClassDescendantSearch(className);
        //    }
        //    else
        //    {
        //        return nodes.ToList();
        //    }
        //}
        //public static bool TryGetTextOfFirstNodeWithClass(this HtmlDocument doc, string className, string divOrSpan = "div", out string text)
        //{
        //    try
        //    {
        //        text = doc.GetNodesWithClass(className, divOrSpan).FirstOrDefault().InnerText;
        //        return true;
        //    }catch (NullReferenceException){
        //        text = "";
        //        return false;
        //    }
        //}
        public static bool TryGetTextOfFirstNodeWithClass(this HtmlDocument doc, string searchString, out string text)
        {
            try
            {
                text = doc.DocumentNode.SelectNodes(searchString).FirstOrDefault().InnerText;
                return true;
            }
            catch (NullReferenceException)
            {
                text = "";
                return false;
            }
        }
    }
}
