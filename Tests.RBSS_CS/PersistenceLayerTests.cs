using System;
using System.Collections.Generic;
using DAL1.RBSS_CS;
using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using DAL1.RBSS_CS.Datastructures;
using DAL1.RBSS_CS.Hashfunction;
using Models.RBSS_CS;
using Xunit;

namespace Tests.RBSS_CS
{
    public abstract class PersistenceLayerTests : IDisposable 
    {
        private readonly IPersistenceLayerSingleton _persistenceLayer;
        public PersistenceLayerTests(IPersistenceLayerSingleton persistenceLayer)
        {
            _persistenceLayer = persistenceLayer;
        }

        private void AddAllToLayer(List<SimpleDataObject> set)
        {
            foreach (var e in set)
            {
                _persistenceLayer.Insert(e);
            }
        }

        public void Dispose()
        {
            _persistenceLayer.Clear();
        }

        /// <summary>
        /// Tests case where a subset contains the last element of the range specified
        /// </summary>
        [Fact]
        public void SplitRangesTest1()
        {
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("bee", "")),
                (new SimpleDataObject("cat", "")),
                (new SimpleDataObject("doe", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("gnu", "")),
                (new SimpleDataObject("hog", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "eel");
            Assert.Equal(2, ranges.Length);
            Assert.NotNull(ranges[0].Data);
            Assert.Equal(2, ranges[0].Data!.Length);
            Assert.Contains(ranges[0].Data!, o => o.Id.Equals("ape"));
            Assert.Contains(ranges[0].Data!, o => o.Id.Equals("bee"));

            Assert.NotNull(ranges[1].Data);
            Assert.Equal(2, ranges[1].Data!.Length);
            Assert.Contains(ranges[1].Data!, o => o.Id.Equals("cat"));
            Assert.Contains(ranges[1].Data!, o => o.Id.Equals("doe"));

        }

        /// <summary>
        /// Tests case where a range goes over the last element
        /// </summary>
        [Fact]
        public void SplitRangesTest2()
        {
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("gnu", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("eel", "ape");
            Assert.Equal(2, ranges.Length);
            Assert.NotNull(ranges[0].Data);
            Assert.Equal(2, ranges[0].Data!.Length);
            Assert.Contains(ranges[0].Data!, o => o.Id.Equals("eel"));
            Assert.Contains(ranges[0].Data!, o => o.Id.Equals("fox"));

            Assert.NotNull(ranges[1].Data);
            Assert.Equal(1, ranges[1].Data!.Length);
            Assert.Contains(ranges[1].Data!, o => o.Id.Equals("gnu"));

        }

        /// <summary>
        /// Tests case where ranges consist only of an insert step
        /// </summary>
        [Fact]
        public void SplitRangesTest3()
        {
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("cat", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("gnu", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "cat");

            ranges = _persistenceLayer.SplitRange("cat", "eel");
            ranges = _persistenceLayer.SplitRange("eel", "gnu");
            ranges = _persistenceLayer.SplitRange("gnu", "ape");



        }

        [Fact]
        public void SplitRangesTestEmptySetFullRange()
        {
            var set = new List<SimpleDataObject>()
            {
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "ape");
        }

        [Fact]
        public void SplitRangesTestEmptySetSpecific()
        {
            var set = new List<SimpleDataObject>()
            {
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "eel");
        }

        [Fact]
        public void SplitRangesTestOneElementSetFullRange()
        {
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("cat", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "ape");
        }

        [Fact]
        public void SplitRangesTestOneElementSetSpecificRangeIn()
        {
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("cat", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "eel");
        }

        [Fact]
        public void SplitRangesTestOneElementSetSpecificRangeOut()
        {
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("cat", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("eel", "gnu");
        }
    }

    public class SortedSetPersitenceTestsXor : PersistenceLayerTests
    {
        public SortedSetPersitenceTestsXor(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub(), new XorBifunctor(), new StableHash(), 2))
        {
        }
    }

    public class RedBlackTreePersistenceTestsXor : PersistenceLayerTests
    {
        public RedBlackTreePersistenceTestsXor(): base(new PersistenceLayer<RedBlackTreePersistence>(new DatabaseStub(), new XorBifunctor(), new SHA256Hash(), 2))
        {
        }
    }

    public class RedBlackTreePersistenceTestsAdd : PersistenceLayerTests
    {
        public RedBlackTreePersistenceTestsAdd(): base(new PersistenceLayer<RedBlackTreePersistence>(new DatabaseStub(), new AddBifunctor(), new SHA256Hash(), 2))
        {
        }
    }
}