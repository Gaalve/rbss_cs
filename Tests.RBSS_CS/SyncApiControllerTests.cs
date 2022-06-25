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
    public abstract class SyncApiControllerTests : IDisposable
    {
        private readonly IPersistenceLayerSingleton _persistenceLayer;
        private readonly SyncApi _sync;
        
        public SyncApiControllerTests(IPersistenceLayerSingleton persistenceLayer)
        {
            _persistenceLayer = persistenceLayer;
            _sync = new SyncApi(_persistenceLayer);
        }

        public void Dispose()
        {
            _persistenceLayer.Clear();
        }
        private void AddAllToLayer(SortedSet<SimpleObjectWrapper> set)
        {
            foreach (var e in set)
            {
                _persistenceLayer.Insert(e.Data);
            }
        }

        private RangeSet createRangeSet(SortedSet<SimpleObjectWrapper> set)
        {
            AddAllToLayer(set);
            var range = _persistenceLayer.CreateRangeSet();
            _persistenceLayer.Clear();
            return range;
        }

        private RangeSet createRangeSet(SortedSet<SimpleObjectWrapper> set, string idFrom, string idTo)
        {
            AddAllToLayer(set);
            var range = _persistenceLayer.CreateRangeSet(idFrom, idTo);
            _persistenceLayer.Clear();
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

        private bool checkValidateStep(Step step, string idFrom, string idTo)
        {
            if (step.CurrentStep.Step.GetType() != typeof(ValidateStep)) return false;
            var vs = (ValidateStep)step.CurrentStep.Step;
            return vs.IdFrom.Equals(idFrom) && vs.IdTo.Equals(idTo);
        }

        [Fact]
        public void PostRequestFullRangeNotInSet()
        {
            var setParticipant = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("bee", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("doe", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("hog", "")),
            };
            AddAllToLayer(setParticipant);

            var result = _sync.SyncPost(new ValidateStep("ape", "ape", "fp"));
            
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.Equal(2, state.Steps.Count);
            Assert.Contains(state.Steps, (step =>
                checkValidateStep(step, "ape", "eel")));
            Assert.Contains(state.Steps, (step =>
                checkValidateStep(step, "eel", "ape")));
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

        [Fact]
        public void PutRequestElemInsertStep()
        {
            var setRemote = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("bee", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("doe", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("hog", "")),
            };
            var setHost = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("gnu", "")),
            };
            AddAllToLayer(setHost);

            var result = _sync.SyncPost(new ValidateStep("ape", "eel", "fp"));
            
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep.Step);
            var insert = (InsertStep)state.Steps[0].CurrentStep.Step;
            Assert.Contains("ape", insert.DataToInsert.Select(s => s.Id));


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
            insert = (InsertStep)state.Steps[0].CurrentStep.Step;
            Assert.True(insert.Handled);
            
            var data = insert.DataToInsert.Select(s => s.Id).ToArray();
            Assert.Contains("bee", data);
            Assert.Contains("cat", data);
            Assert.Contains("doe", data);

            Assert.Equal(3, insert.DataToInsert.Count);
        }
    }


    public class SortedSetSyncControllerTests : SyncApiControllerTests
    {
        public SortedSetSyncControllerTests(): base(PersistenceLayer<SortedSetPersistence>.Instance)
        {
        }
    }

    public class RedBlackTreeSyncControllerTests : SyncApiControllerTests
    {
        public RedBlackTreeSyncControllerTests(): base(PersistenceLayer<RedBlackTreePersistence>.Instance)
        {
        }
    }
}
