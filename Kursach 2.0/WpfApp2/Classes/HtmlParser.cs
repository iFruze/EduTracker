using AngleSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp2.Classes
{
    internal static class HtmlParser
    {
        public async static Task<List<string>> GetDayOfWeek(string url, string subjectsTag, string groupsTag)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var doc = await context.OpenAsync(url);
            //if(doc.StatusCode == System.Net.HttpStatusCode.OK)
            if(doc != null)
                {
                var day = new List<string>();
                var links1 = doc.QuerySelectorAll(subjectsTag);
                var links2 = doc.QuerySelectorAll(groupsTag);
                for (int j = 0; j < links1.Length; j++)
                {
                    day.Add($"{links2[j].TextContent} {links1[j].TextContent}");
                }
                return day;
            }
            else
            {
                return null;
            }
        }
    }
}
