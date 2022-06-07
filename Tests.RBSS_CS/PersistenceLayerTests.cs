using System;
using System.Collections.Generic;
using DAL1.RBB_CS;
using DAL1.RBSS_CS;
using Models.RBB_CS;
using Xunit;

namespace Tests.RBSS_CS
{
    public class PersistenceLayerTests : IDisposable
    {

        public PersistenceLayerTests()
        {

        }

        private void AddAllToLayer(SortedSet<SimpleObjectWrapper> set)
        {
            foreach (var e in set)
            {
                PersistenceLayer.Instance.Insert(e.Data);
            }
        }

        public void Dispose()
        {
            PersistenceLayer.Instance.Clear();
        }

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

            var ranges = PersistenceLayer.Instance.SplitRange("ape", "eel");
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

            var ranges = PersistenceLayer.Instance.SplitRange("eel", "ape");
            Assert.Equal(2, ranges.Length);
            Assert.NotNull(ranges[0].Data);
            Assert.Equal(2, ranges[0].Data!.Length);
            Assert.Contains(ranges[0].Data!, o => o.Id.Equals("eel"));
            Assert.Contains(ranges[0].Data!, o => o.Id.Equals("fox"));

            Assert.NotNull(ranges[1].Data);
            Assert.Equal(1, ranges[1].Data!.Length);
            Assert.Contains(ranges[1].Data!, o => o.Id.Equals("gnu"));

        }

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

            var ranges = PersistenceLayer.Instance.SplitRange("ape", "cat");

            ranges = PersistenceLayer.Instance.SplitRange("cat", "eel");
            ranges = PersistenceLayer.Instance.SplitRange("eel", "gnu");
            ranges = PersistenceLayer.Instance.SplitRange("gnu", "ape");



        }
    }
}