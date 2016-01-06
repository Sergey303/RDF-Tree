using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BinaryTable
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = XElement.Load("../../../tree.xml");
            Dictionary<string, List<string>> linksUp = tree.Descendants().ToDictionary(descendant => descendant.Name.ToString(), descendant => descendant.Ancestors().Select(x => x.Name.ToString()).ToList());
            Dictionary<string, List<string>> linksDown = tree.DescendantsAndSelf().Where(element => element.HasElements).ToDictionary(descendant => descendant.Name.ToString(), descendant => descendant.Ancestors().Select(x => x.Name.ToString()).ToList()); 
        }
    }
}
