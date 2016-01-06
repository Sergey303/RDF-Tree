using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tree_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            int k = 10; // максимальное число потомков
            int h = 10; // высота
            var random = new Random();
            XElement tree =new XElement("tree");
            
            List<XElement> newlists= new List<XElement>() { tree };
            for (int level = 0; level < h; level++)
            {
                List<XElement> lists = newlists;
                newlists = new List<XElement>();
                foreach (var node in lists)
                {
                    for (int i = 0; i < random.Next(k); i++)
                    {
                        var newList = new XElement(node.Name+"space"+i);
                        newlists.Add(newList);
                        node.Add(newList);
                    }
                }
            }
            tree.Save("../../../tree.xml");
        }
    }
}
