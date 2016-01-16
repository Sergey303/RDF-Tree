using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PolarDB;

namespace Polar
{
    class Program
    {

        static List<string> names=new List<string>();

        static void Main(string[] args)
        {
            var tree = XElement.Load("../../../tree.xml");
            int height = GetTreeHeight(tree);
        
            ChildrenTree tree4SeaChildren=new ChildrenTree(Coding, i => names[i], height);
           
            
            //Append(tree, paCell);
           

            var children = tree4SeaChildren.GetChildren("tree", i=> names[i]);
            children = tree4SeaChildren.GetChildren("treespace0", i => names[i]);



            PType parentsSearchType=new PTypeSequence(new PType(PTypeEnumeration.integer));
            PaCell parentSequence=new PaCell(parentsSearchType, "../../parentsSeq.pa", false);
            parentSequence.Clear();
            parentSequence.Fill(new object[0]);
            foreach (var node in tree.Elements())
            {
                foreach (var rows in parentSequence.Root.Elements())
                {
                    
                }
            }
        }

        #region Create

        private static int GetTreeHeight(XElement x) => x.Elements().Any()
            ? x.Elements().Max(x1 => Program.GetTreeHeight(x1) + 1)
            : 1;

        public static int Coding(string s)
        {
            names.Add(s);
            return names.Count - 1;
        }

        #endregion
        


    }
}
