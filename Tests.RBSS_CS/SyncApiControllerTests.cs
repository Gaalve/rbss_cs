using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL1.RBSS_CS;
using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using DAL1.RBSS_CS.Datastructures;
using DAL1.RBSS_CS.Hashfunction;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using RBSS_CS;
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
            _sync = new SyncApi(new ServerSettings(), _persistenceLayer);
        }

        public void Dispose()
        {
            _persistenceLayer.Clear();
        }
        private void AddAllToLayer(List<SimpleDataObject> set)
        {
            foreach (var e in set)
            {
                _persistenceLayer.Insert(e);
            }
        }

        private RangeSet createRangeSet(List<SimpleDataObject> set)
        {
            AddAllToLayer(set);
            var range = _persistenceLayer.CreateRangeSet();
            _persistenceLayer.Clear();
            return range;
        }

        private RangeSet createRangeSet(List<SimpleDataObject> set, string idFrom, string idTo)
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
            var set = new List<SimpleDataObject>()
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
            var set = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
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
            var setRemote = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("bee", "")),
            };
            var setHost = new List<SimpleDataObject>()
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
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep);
            Assert.Empty(((InsertStep)state.Steps[0].CurrentStep).DataToInsert);
        }
        [Fact]
        public void PostRequestSetHostOneElementSet()
        {
            var setRemote = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("bee", "")),
            };
            var setHost = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("cat", "")),
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
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep);
            Assert.Single(((InsertStep)state.Steps[0].CurrentStep).DataToInsert);
        }

        private bool checkValidateStep(Step step, string idFrom, string idTo)
        {
            if (step.CurrentStep.GetType() != typeof(ValidateStep)) return false;
            var vs = (ValidateStep)step.CurrentStep;
            return vs.IdFrom.Equals(idFrom) && vs.IdTo.Equals(idTo);
        }

        private bool checkInsertStep(Step step, string idFrom, string idTo, ICollection<string> candidates, bool handled)
        {
            if (step.CurrentStep.GetType() != typeof(InsertStep)) return false;
            var ins = (InsertStep)step.CurrentStep;
            if (!(ins.IdFrom.Equals(idFrom) && ins.IdTo.Equals(idTo))) return false;
            if (ins.DataToInsert.Count != candidates.Count) return false;
            if (ins.Handled != handled) return false;
            return ins.DataToInsert.TrueForAll((o => candidates.Contains(o.Id)));
        }

        [Fact]
        public void PostRequestFullRangeNotInSet()
        {
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("bee", "")),
                (new SimpleDataObject("cat", "")),
                (new SimpleDataObject("doe", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("hog", "")),
            };
            var setInitiator = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("gnu", "")),
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
            result = _sync.SyncPut(new InlineResponse(state));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<InlineResponse>(((OkObjectResult)result).Value);
            state = ((InlineResponse)((OkObjectResult)result).Value!).Syncstate;
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
            var setRemote = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("cat", "")),
            };
            var setHost = new List<SimpleDataObject>()
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
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep);
            Assert.Empty(((InsertStep)state.Steps[0].CurrentStep).DataToInsert);


            Dispose();
            AddAllToLayer(setRemote);
            result = _sync.SyncPut(new InlineResponse(state));

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<InlineResponse>(((OkObjectResult)result).Value);
            state = ((InlineResponse)((OkObjectResult)result).Value!).Syncstate;

            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep);
            Assert.Single(((InsertStep)state.Steps[0].CurrentStep).DataToInsert);
        }

        [Fact]
        public void PostRequestInsertStepOverbounds()
        {
            var setRemote = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("appreciated", "")),
                (new SimpleDataObject("connectivity", "")),
                (new SimpleDataObject("grad", "")),
                (new SimpleDataObject("greetings", "")),
                (new SimpleDataObject("industry", "")),
                (new SimpleDataObject("sense", "")),
            };
            var setHost = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ace", "")),
                (new SimpleDataObject("appreciated", "")),
                (new SimpleDataObject("connectivity", "")),
                (new SimpleDataObject("grad", "")),
                (new SimpleDataObject("industry", "")),
                (new SimpleDataObject("theme", "")),
            };
            AddAllToLayer(setHost);
            var result = _sync.SyncPut(new InlineResponse(new SyncState(0, 
                new List<Step>()
                {
                    new Step(0, new ValidateStep("industry", "appreciated", "fp"))
                })));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<InlineResponse>(((OkObjectResult)result).Value);
            var state = ((InlineResponse)((OkObjectResult)result).Value!).Syncstate;
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
            var setHost = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("peace", "")),
                (new SimpleDataObject("poverty", "")),
                (new SimpleDataObject("scotland", "")),
                (new SimpleDataObject("spa", "")),
                (new SimpleDataObject("te", "")),
            };
            AddAllToLayer(setHost);
            _persistenceLayer.Insert(new SimpleDataObject("te", ""));
            _persistenceLayer.Insert(new SimpleDataObject("failure", ""));
            _persistenceLayer.Insert(new SimpleDataObject("full", ""));
            // _persistenceLayer.Insert(new SimpleDataObject("failure", ""));
            var result = _sync.SyncPut(new InlineResponse(new SyncState(0,
                new List<Step>()
                {
                    new Step(0, (new InsertStep("observations", new List<string>(), "scotland", new List<SimpleDataObject>(){new SimpleDataObject("observations", "")}, false)))
                })));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<InlineResponse>(((OkObjectResult)result).Value);
            var state = ((InlineResponse)((OkObjectResult)result).Value!).Syncstate;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);
            Assert.Equal(1, state.Steps.Count);
            Assert.Contains(state.Steps, (step =>
                checkInsertStep(step, "observations", "scotland", new List<string>(){"peace", "poverty"}, true)));
        }

        [Fact]
        public void PutRequestElemInsertStep()
        {
            var setRemote = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("bee", "")),
                (new SimpleDataObject("cat", "")),
                (new SimpleDataObject("doe", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("hog", "")),
            };
            var setHost = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("gnu", "")),
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
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep);
            var insert = (InsertStep)state.Steps[0].CurrentStep;
            Assert.Contains("ape", insert.DataToInsert.Select(s => s.Id));


            Dispose();
            AddAllToLayer(setRemote);
            result = _sync.SyncPut(new InlineResponse(state));

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<InlineResponse>(((OkObjectResult)result).Value);
            state = ((InlineResponse)((OkObjectResult)result).Value!).Syncstate;

            Assert.NotEmpty(state.Steps);
            Assert.Single(state.Steps);
            Assert.IsType<InsertStep>(state.Steps[0].CurrentStep);
            insert = (InsertStep)state.Steps[0].CurrentStep;
            Assert.True(insert.Handled);
            
            var data = insert.DataToInsert.Select(s => s.Id).ToArray();
            Assert.Contains("bee", data);
            Assert.Contains("cat", data);
            Assert.Contains("doe", data);

            Assert.Equal(3, insert.DataToInsert.Count);
        }

        [Fact]
        public void PutUpdateInsert()
        {
            var setRemote = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 500,"")),
                (new SimpleDataObject("doe", 1,"")),
            };
            var setHost = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1,"")),
                (new SimpleDataObject("doe", 1,"")),
            };
            AddAllToLayer(setRemote);
            var validate = _persistenceLayer.CreateRangeSet();
            Dispose();
            AddAllToLayer(setHost);

            var result = _sync.SyncPost(new ValidateStep(validate.IdFrom, validate.IdTo, validate.Fingerprint));
            
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            var state = (SyncState)((OkObjectResult)result).Value!;
            Assert.False(equalFP(state));
            Assert.NotEmpty(state.Steps);

            result = _sync.SyncPut(new InlineResponse(new SyncState(0, new List<Step>()
            {
                new Step(0, (new InsertStep(
                    "cat", new List<string>(), "doe",
                    new List<SimpleDataObject>() { new SimpleDataObject("cat", 500, "") }, false
                )))
            })));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<InlineResponse>(((OkObjectResult)result).Value);
            var state2 = ((InlineResponse)((OkObjectResult)result).Value!).Syncstate;
            var json = state2.ToJson();

            result = _sync.SyncPost(new ValidateStep(validate.IdFrom, validate.IdTo, validate.Fingerprint));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            state = (SyncState)((OkObjectResult)result).Value!;
            Assert.True(equalFP(state));
            _sync.SyncPut(new InlineResponse(new SyncState(0, new List<Step>()
            {
                new Step(0, (new InsertStep(
                    "cat", new List<string>(), "doe",
                    new List<SimpleDataObject>() { new SimpleDataObject("cat", 1, "") }, false
                )))
            })));
            result = _sync.SyncPost(new ValidateStep(validate.IdFrom, validate.IdTo, validate.Fingerprint));
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsType<SyncState>(((OkObjectResult)result).Value);
            state = (SyncState)((OkObjectResult)result).Value!;
            Assert.True(equalFP(state));
        }
    }



    public class SortedSetSyncControllerTestsXor : SyncApiControllerTests
    {
        public SortedSetSyncControllerTestsXor(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub(), new XorBifunctor(), new StableHash(), 2))
        {
        }
    }

    public class SortedSetSyncControllerTestsAdd : SyncApiControllerTests
    {
        public SortedSetSyncControllerTestsAdd(): base(new PersistenceLayer<SortedSetPersistence>(new DatabaseStub(), new AddBifunctor(), new SHA256Hash(), 2))
        {
        }
    }

    public class RedBlackTreeSyncControllerTestsXor : SyncApiControllerTests
    {
        public RedBlackTreeSyncControllerTestsXor(): base(new PersistenceLayer<RedBlackTreePersistence>(new DatabaseStub(), new XorBifunctor(), new SHA256Hash(), 2))
        {
        }
    }

    public class RedBlackTreeSyncControllerTestsAdd : SyncApiControllerTests
    {
        public RedBlackTreeSyncControllerTestsAdd(): base(new PersistenceLayer<RedBlackTreePersistence>(new DatabaseStub(), new AddBifunctor(), new StableHash(), 2))
        {
        }
    }
}
