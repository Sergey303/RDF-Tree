using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PolarDB;

namespace Polar
{
    class ChildrenTree
    {
        public  Dictionary<string, long> searchIndex;
        public  PaCell paCell;
        private Func<string, int> coding;
        private Func<int, string> decoding;
        private int height;

        public ChildrenTree(Func<string, int> coding, Func<int, string> decoding, int height)
        {
            this.coding = coding;
            this.decoding = decoding;
            this.height = height;
            PType treePType = Enumerable.Range(0, height).
            Aggregate(new PType(PTypeEnumeration.integer),
                (PType res, int level) =>
                    new PTypeRecord(new NamedType("value", new PType(PTypeEnumeration.integer)),
                        new NamedType("children", new PTypeSequence(res))));
            paCell = new PaCell(treePType, "../../tree.pa", false);
        }

        public void ReCreate(XElement tree)
        {
            paCell.Clear();
            var valu = CreateTree(tree, 0, height);
            paCell.Fill(valu);
            searchIndex = new Dictionary<string, long>();
            AddInIndex(paCell.Root);
        }

        private void AddInIndex(PaEntry entry)
        {
            var index = (int) entry.Field(0).Get();
            searchIndex.Add(decoding(index), entry.offset);
            foreach (var chiledEntry in entry.Field(1).Elements())
            {
                AddInIndex(chiledEntry);
            }
        }

        private  void Append(XElement tree)
        {
            if (tree.Elements().Any())
            {
                paCell.Fill(new object[] {1, new object[0]});
                var childNodeCollection = paCell.Root.Field(1);
                foreach (var child in tree.Elements())
                {
                    CreateTree(child, childNodeCollection);
                }
            }
            else
            {
                paCell.Fill((object) 1);
            }
        }

        public  object CreateTree(XElement x, int level, int height)
        {

            return level != height
                ? (object)
                    new object[]
                    {Program.Coding(x.Name.ToString()), x.Elements().Select(c => CreateTree(c, level + 1, height)).ToArray()}
                : Program.Coding(x.Name.ToString());
        }

        private  void CreateTree(XElement x, PaEntry paTreeNodeCollection)
        {
            if (x.Elements().Any())
            {
                paTreeNodeCollection.AppendElement(new object[] { coding(x.Name.ToString()), new object[0]});
                var childNodeCollection = paTreeNodeCollection.Field(1);
                foreach (var child in x.Elements())
                {
                    CreateTree(child, childNodeCollection);
                }
            }
            else
            {
                paTreeNodeCollection.AppendElement((object)coding(x.Name.ToString()));
            }
        }

        
        public T[] GetChildren<T>(string name, Func<int, T> decode )
        {
            if (paCell.Root.Type.Vid == PTypeEnumeration.integer)
                return new T[0];
            var paEntry = paCell.Root;
            paEntry.offset = searchIndex[name];
            return paEntry.Field(1).Elements().Select(entry => (int)entry.Field(0).Get())
                .Select(decode).ToArray();
        }
    }
}