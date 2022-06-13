using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using RBSS_CS.Controllers;
using Xunit;

namespace Tests.RBSS_CS
{
    public class SyncApiControllerTests : IDisposable
    {
        private readonly SyncApi _sync;
        public SyncApiControllerTests()
        {
            _sync = new SyncApi();
        }

        public void Dispose()
        {
            PersistenceLayer.Instance.Clear();
        }
        private void AddAllToLayer(SortedSet<SimpleObjectWrapper> set)
        {
            foreach (var e in set)
            {
                PersistenceLayer.Instance.Insert(e.Data);
            }
        }

        private RangeSet createRangeSet(SortedSet<SimpleObjectWrapper> set)
        {
            AddAllToLayer(set);
            var range = PersistenceLayer.Instance.CreateRangeSet();
            PersistenceLayer.Instance.Clear();
            return range;
        }

        private RangeSet createRangeSet(SortedSet<SimpleObjectWrapper> set, string idFrom, string idTo)
        {
            AddAllToLayer(set);
            var range = PersistenceLayer.Instance.CreateRangeSet(idFrom, idTo);
            PersistenceLayer.Instance.Clear();
            return range;
        }

        private bool equalFP(SyncState state)
        {
            return state.Steps.Count == 0;
        }

        [Fact]
        public void PostRequestEmptySetEqualFp()
        {
            var set = new SortedSet<SimpleObjectWrapper>()
            {
            };
            var range = createRangeSet(set);

            var result = _sync.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.True(equalFP(state));
        }
        [Fact]
        public void PostRequestOneElementSetEqualFp()
        {
            var set = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
            };
            var range = createRangeSet(set);
            AddAllToLayer(set); // simulated host and remote have the same set

            var result = _sync.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.True(equalFP(state));
        }
        [Fact]
        public void PostRequestSetEqualFp()
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
            var range = createRangeSet(set);
            AddAllToLayer(set); // simulated host and remote have the same set

            var result = _sync.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.True(equalFP(state));
        }
        [Fact]
        public void PostRequestSetHostEmptySet()
        {
            var setRemote = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("bee", "")),
            };
            var setHost = new SortedSet<SimpleObjectWrapper>()
            {
            };
            var range = createRangeSet(setRemote);
            AddAllToLayer(setHost);

            var result = _sync.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));
            
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep.Step);
            Assert.Empty(((InsertStep)state.Steps[0].CurrentStep.Step).DataToInsert);
        }
        [Fact]
        public void PostRequestSetHostOneElementSet()
        {
            var setRemote = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("bee", "")),
            };
            var setHost = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("cat", "")),
            };
            var range = createRangeSet(setRemote);
            AddAllToLayer(setHost);

            var result = _sync.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));
            
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep.Step);
            Assert.Single(((InsertStep)state.Steps[0].CurrentStep.Step).DataToInsert);
        }


        [Fact]
        public void PutRequest0ElemInsertStep()
        {
            var setRemote = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("cat", "")),
            };
            var setHost = new SortedSet<SimpleObjectWrapper>()
            {

            };
            var range = createRangeSet(setRemote);
            AddAllToLayer(setHost);

            var result = _sync.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));
            
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep.Step);
            Assert.Empty(((InsertStep)state.Steps[0].CurrentStep.Step).DataToInsert);


            Dispose();
            AddAllToLayer(setRemote);
            result = _sync.SyncPut(state);

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            state = (SyncState)((OkObjectResult)result).Value!;

            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep.Step);
            Assert.Single(((InsertStep)state.Steps[0].CurrentStep.Step).DataToInsert);
        }
    }
}
