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


        static void Main(string[] args)
        {
            var tree = XElement.Load("../../../tree.xml");
        
        
            ChildrenTree treeObjects=new ChildrenTree(tree);
           
            treeObjects.ReCreate(tree);
            
            //Append(tree, paCell);
           

            var children = treeObjects.GetChildren("tree");
            children = treeObjects.GetChildren("treespace1");


            var parents = treeObjects.GetParents(children[0]).ToArray();
            var testConnection = treeObjects.TestConnection(children[children.Length - 1], children[children.Length - 2]);
            testConnection = treeObjects.TestConnection(parents[0], children[children.Length - 2]);
            testConnection = treeObjects.TestConnection(parents[1], children[children.Length - 2]);
            testConnection = treeObjects.TestConnection("tree", children[children.Length - 2]);

        }

        #region Create

    

   

        #endregion
        


    }
}
