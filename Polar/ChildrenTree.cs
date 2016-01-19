using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PolarDB;

namespace Polar
{
    internal class ChildrenTree
    {
        private Dictionary<string, long> searchIndex;
        private readonly PaCell paCell;
        private int maxChildrenCount;
        private readonly Dictionary<string, int> coding;
        private readonly Dictionary<int, string> decoding = new Dictionary<int, string>();
        private readonly int height;

        public ChildrenTree(XElement tree)
        {
            var rootCode = tree.Name.ToString();
            height = GetTreeHeight(tree);
            coding = new Dictionary<string, int> ();
            GetMaxChiledrenCount(tree);
            PType treePType = Enumerable.Range(0, height).
                Aggregate(new PType(PTypeEnumeration.integer),
                    (PType res, int level) =>
                        new PTypeRecord(new NamedType("value", new PType(PTypeEnumeration.integer)),
                            new NamedType("children", new PTypeSequence(res))));
            paCell = new PaCell(treePType, "../../tree.pa", false);
            CreateCoding(tree, 0);
        }

        private void GetMaxChiledrenCount(XElement tree)
        {
            var count = tree.Elements().Count();
            if (count >= maxChildrenCount)
                maxChildrenCount = count + 1;
            foreach (var child in tree.Elements())
            {
                GetMaxChiledrenCount(child);
            }
        }

        private void CreateCoding(XElement tree, int rootCode)
        {
            var key = tree.Name.ToString();
            coding.Add(key, rootCode);
            decoding.Add(rootCode, key);

            int i = 1;
            foreach (var child in tree.Elements())
            {
                CreateCoding(child, rootCode*maxChildrenCount + (i++));
            }
        }


        public void ReCreate(XElement tree)
        {
            paCell.Clear();
            var valu = CreateTree(tree, 0);
            paCell.Fill(valu);
            searchIndex = new Dictionary<string, long>();
            AddInIndex(paCell.Root);
        }

        private void AddInIndex(PaEntry entry)
        {
            var index = (int) entry.Field(0).Get();
            searchIndex.Add(decoding[index], entry.offset);
            foreach (var chiledEntry in entry.Field(1).Elements())
            {
                AddInIndex(chiledEntry);
            }
        }

        private void Append(XElement tree)
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

        public object CreateTree(XElement x, int level)
        {

            return level != height
                ? (object)
                    new object[]
                    {coding[x.Name.ToString()], x.Elements().Select(c => CreateTree(c, level + 1)).ToArray()}
                : coding[x.Name.ToString()];
        }

        private void CreateTree(XElement x, PaEntry paTreeNodeCollection)
        {
            if (x.Elements().Any())
            {
                paTreeNodeCollection.AppendElement(new object[] {coding[x.Name.ToString()], new object[0]});
                var childNodeCollection = paTreeNodeCollection.Field(1);
                foreach (var child in x.Elements())
                {
                    CreateTree(child, childNodeCollection);
                }
            }
            else
            {
                paTreeNodeCollection.AppendElement((object) coding[x.Name.ToString()]);
            }
        }
        private static int GetTreeHeight(XElement x) => x.Elements().Any()
           ? x.Elements().Max(x1 => GetTreeHeight(x1) + 1)
           : 1;

        public string[] GetChildren(string name)
        {
            if (paCell.Root.Type.Vid == PTypeEnumeration.integer)
                return new string[0];
            var paEntry = paCell.Root;
            paEntry.offset = searchIndex[name];
            return paEntry.Field(1).Elements().Select(entry => (int) entry.Field(0).Get())
                .Select(c=>decoding[c]).ToArray();
        }

       


        public bool TestConnection(string node1, string node2)
        {
            int code1, code2, div;
            if (!coding.TryGetValue(node1, out code1)) return false;
            if (!coding.TryGetValue(node2, out code2)) return false;
            if (code2 > code1)
            {
                div = code2;
                return
                    Enumerable.Range(1, height)
                        .Any(i =>
                        {
                            div = div / maxChildrenCount;
                            return code1 == div;
                        });
            }
            else
                div = code1;
            return
                Enumerable.Range(1, height)
                    .Any(i =>
                    {
                        div = div / maxChildrenCount;
                        return code2 == div;
                    });
        }

        public IEnumerable<string> GetParents(string node)
        {
            int code;
            if (!coding.TryGetValue(node, out code))
                yield break;
            while (code > 0)
            {
                code = code / maxChildrenCount;
                yield return decoding[code];
            }
        }
    }
}