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

        private bool checkInsertStep(Step step, string idFrom, string idTo, ICollection<string> candidates, bool handled)
        {
            if (step.CurrentStep.Step.GetType() != typeof(InsertStep)) return false;
            var ins = (InsertStep)step.CurrentStep.Step;
            if (!(ins.IdFrom.Equals(idFrom) && ins.IdTo.Equals(idTo))) return false;
            if (ins.DataToInsert.Count != candidates.Count) return false;
            if (ins.Handled != handled) return false;
            return ins.DataToInsert.TrueForAll((o => candidates.Contains(o.Id)));
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
            var setInitiator = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("fox", "")),
                new(new SimpleDataObject("gnu", "")),
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

            Dispose();
            AddAllToLayer(setInitiator);
            result = _sync.SyncPut(state);
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            state = (SyncState)((OkObjectResult)result).Value!;
            Assert.NotNull(state);
            Assert.NotEmpty(state!.Steps);
            Assert.Equal(3, state.Steps.Count);
            Assert.Contains(state.Steps, (step =>
                checkInsertStep(step, "ape", "eel", new List<string>(){"ape"}, false)));
            Assert.Contains(state.Steps, (step =>
                checkValidateStep(step, "eel", "gnu")));
            Assert.Contains(state.Steps, (step =>
                checkInsertStep(step, "gnu", "ape", new List<string>(){"gnu"}, false)));

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
        public void PostRequestInsertStepOverbounds()
        {
            var setRemote = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("appreciated", "")),
                new(new SimpleDataObject("connectivity", "")),
                new(new SimpleDataObject("grad", "")),
                new(new SimpleDataObject("greetings", "")),
                new(new SimpleDataObject("industry", "")),
                new(new SimpleDataObject("sense", "")),
            };
            var setHost = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ace", "")),
                new(new SimpleDataObject("appreciated", "")),
                new(new SimpleDataObject("connectivity", "")),
                new(new SimpleDataObject("grad", "")),
                new(new SimpleDataObject("industry", "")),
                new(new SimpleDataObject("theme", "")),
            };
            AddAllToLayer(setHost);
            var result = _sync.SyncPut(new SyncState(0,
                new List<Step>()
                {
                    new Step(0, new OneOfValidateStepInsertStep(new ValidateStep("industry", "appreciated", "fp")))
                }));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Equal(2, state.Steps.Count);
            Assert.Contains(state.Steps, (step =>
                checkValidateStep(step, "industry", "ace")));
            Assert.Contains(state.Steps, (step =>
                checkInsertStep(step, "ace", "appreciated", new List<string>(){"ace"}, false)));
        }

        [Fact]
        public void PostRequestInsertStep()
        {
            var setHost = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("peace", "")),
                new(new SimpleDataObject("poverty", "")),
                new(new SimpleDataObject("scotland", "")),
                new(new SimpleDataObject("spa", "")),
                new(new SimpleDataObject("te", "")),
            };
            AddAllToLayer(setHost);
            _persistenceLayer.Insert(new SimpleDataObject("te", ""));
            _persistenceLayer.Insert(new SimpleDataObject("failure", ""));
            _persistenceLayer.Insert(new SimpleDataObject("full", ""));
            // _persistenceLayer.Insert(new SimpleDataObject("failure", ""));
            var result = _sync.SyncPut(new SyncState(0,
                new List<Step>()
                {
                    new Step(0, new OneOfValidateStepInsertStep(new InsertStep("observations", new List<string>(), "scotland", new List<SimpleDataObject>(){new SimpleDataObject("observations", "")}, false)))
                }));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Equal(1, state.Steps.Count);
            Assert.Contains(state.Steps, (step =>
                checkInsertStep(step, "observations", "scotland", new List<string>(){"peace", "poverty"}, true)));
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
        public SortedSetSyncControllerTests(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub()))
        {
        }
    }

    public class RedBlackTreeSyncControllerTests : SyncApiControllerTests
    {
        public RedBlackTreeSyncControllerTests(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub()))
        {
        }
    }
}
