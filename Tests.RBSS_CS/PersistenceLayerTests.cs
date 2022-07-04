using System;
using System.Collections.Generic;
using DAL1.RBSS_CS;
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

        private void AddAllToLayer(SortedSet<SimpleObjectWrapper> set)
        {
            foreach (var e in set)
            {
                _persistenceLayer.Insert(e.Data);
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
            var set = new SortedSet<SimpleObjectWrapper>()
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
            var set = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("gnu", "")),
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
            var set = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("gnu", "")),
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
            var set = new SortedSet<SimpleObjectWrapper>()
            {
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "ape");
        }

        [Fact]
        public void SplitRangesTestEmptySetSpecific()
        {
            var set = new SortedSet<SimpleObjectWrapper>()
            {
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "eel");
        }

        [Fact]
        public void SplitRangesTestOneElementSetFullRange()
        {
            var set = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("cat", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "ape");
        }

        [Fact]
        public void SplitRangesTestOneElementSetSpecificRangeIn()
        {
            var set = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("cat", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("ape", "eel");
        }

        [Fact]
        public void SplitRangesTestOneElementSetSpecificRangeOut()
        {
            var set = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("cat", "")),
            };
            AddAllToLayer(set);

            var ranges = _persistenceLayer.SplitRange("eel", "gnu");
        }
    }

    public class SortedSetPersitenceTests : PersistenceLayerTests
    {
        public SortedSetPersitenceTests(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub()))
        {
        }
    }

    public class RedBlackTreePersistenceTests : PersistenceLayerTests
    {
        public RedBlackTreePersistenceTests(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub()))
        {
        }
    }
}