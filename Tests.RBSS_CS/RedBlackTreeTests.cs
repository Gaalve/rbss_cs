using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL1.RBSS_CS;
using Models.RBSS_CS;
using Xunit;

namespace Tests.RBSS_CS
{
    public class RedBlackTreeTests : IDisposable
    {
        public void Dispose()
        {
        }
        private void CheckArrayEquality(RedBlackTree<SimpleObjectWrapper> rbt, SimpleObjectWrapper[] rbtArr, SimpleObjectWrapper[] sortedSetArr)
        {
            
            Assert.Equal(rbtArr.Length, sortedSetArr.Length);
            for (int i = 0; i < rbtArr.Length; i++)
            {
                Assert.NotNull(rbt.Search(rbtArr[i]));
                Assert.Equal(rbtArr[i], sortedSetArr[i]);
            }
        }
        [Fact]
        public void TestSorting()
        {
            var list = new List<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("doe", "")),
                new(new SimpleDataObject("gnu", "")),
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("hog", "")),
                new(new SimpleDataObject("bee", "")),
                new(new SimpleDataObject("cat", "")),
            };
            
            RedBlackTree<SimpleObjectWrapper> rbt = new();
            SortedSet<SimpleObjectWrapper> sortedSet = new();
            int n = 0;
            foreach (var v in list)
            {
                Assert.True(rbt.GetHeight() <= 2 * Math.Log2(n + 1));
                rbt.Insert(v);
                sortedSet.Add(v);
                ++n;
                CheckArrayEquality(rbt, rbt.GetSortedList().ToArray(), sortedSet.ToArray());
            }
            Assert.True(rbt.GetHeight() <= 2 * Math.Log2(n + 1));
        }


        

        [Fact]
        public void TestSorting2()
        {
            var list = new List<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("bee", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("doe", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("gnu", "")),
                new(new SimpleDataObject("hog", "")),
            };
            
            RedBlackTree<SimpleObjectWrapper> rbt = new();
            SortedSet<SimpleObjectWrapper> sortedSet = new();
            int n = 0;
            foreach (var v in list)
            {
                Assert.True(rbt.GetHeight() <= 2 * Math.Log2(n + 1));
                rbt.Insert(v);
                sortedSet.Add(v);
                ++n;
                CheckArrayEquality(rbt, rbt.GetSortedList().ToArray(), sortedSet.ToArray());
            }
            Assert.True(rbt.GetHeight() <= 2 * Math.Log2(n + 1));
        }

        [Fact]
        public void TestSorting3()
        {
            var list = new List<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("bee", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("doe", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("hog", "")),
            };
            
            RedBlackTree<SimpleObjectWrapper> rbt = new();
            SortedSet<SimpleObjectWrapper> sortedSet = new();
            int n = 0;
            foreach (var v in list)
            {
                Assert.True(rbt.GetHeight() <= 2 * Math.Log2(n + 1));
                rbt.Insert(v);
                sortedSet.Add(v);
                ++n;
                CheckArrayEquality(rbt, rbt.GetSortedList().ToArray(), sortedSet.ToArray());
            }
            Assert.True(rbt.GetHeight() <= 2 * Math.Log2(n + 1));
        }
    }
}
