using System.Reflection;
using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using RBSS_CS.Controllers;
using Xunit;
using Xunit.Sdk;

namespace RBSS_CS
{

    public class IntegrationTest
    {

        [AttributeUsage(AttributeTargets.Method)]
        private class IntegrationTestMethodAttribute : Attribute{}

        private readonly DebugApi _debugApi;
        private readonly SyncApi _syncApi;
        private readonly ModifyApi _modifyApi;
        private readonly Client _remoteClient;
        private readonly IPersitenceLayerSingleton _persitenceLayer;

        public IntegrationTest(DebugApi dbgApi, SyncApi syncApi, ModifyApi modifyApi, IPersitenceLayerSingleton persitenceLayer)
        {
            _debugApi = dbgApi;
            _syncApi = syncApi;
            _modifyApi = modifyApi;
            _remoteClient = new Client("http://host.docker.internal:5634");
            _persitenceLayer = persitenceLayer;
        }

        private void Cleanup()
        {
            _modifyApi.DeletePost(new SimpleDataObject("", ""));
            _remoteClient.ModifyApi.DeletePost(new SimpleDataObject("", ""));
        }

        private void AddToRemote(SortedSet<SimpleObjectWrapper> set)
        {
            foreach (var e in set)
            {
                _remoteClient.ModifyApi.InsertPost(e.Data);
            }
        }

        private void AddToHost(SortedSet<SimpleObjectWrapper> set)
        {
            foreach (var e in set)
            {
                _modifyApi.InsertPost(e.Data);
            }
        }

        private SyncState InitiateSync()
        {
            var range = _persitenceLayer.CreateRangeSet();
            return _remoteClient.SyncApi.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));
        }

        private bool SetsSynchronized()
        {
            return InitiateSync().Steps.Count == 0;
        }


        private SyncState? GetLocalResult(SyncState remoteResult)
        {
            var localAction = _syncApi.SyncPut(remoteResult);

            if (localAction.GetType() != typeof(OkObjectResult))
            {
                Console.WriteLine("Wrong Result Type returned.");
                return null;
            }

            var localResult = ((OkObjectResult)localAction).Value;

            if (localResult == null)
            {
                Console.WriteLine("No object attached.");
                return null;
            }

            if (localResult.GetType() != typeof(SyncState))
            {
                Console.WriteLine("Wrong object attached.");
                return null;
            }

            return (SyncState)localResult;
        }

        private void Synchronize()
        {
            var remoteResult = InitiateSync();
            while (remoteResult.Steps is { Count: > 0 })
            {
                var localSyncState = GetLocalResult(remoteResult);
                Assert.NotNull(localSyncState);
                if (localSyncState!.Steps.Count == 0) return;
                remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            }
        }

        public void Run()
        {
            var members = typeof(IntegrationTest).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            Console.WriteLine("Running Integration Test");
            foreach (var m in members)
            {
                if (m.GetCustomAttributes()
                    .Count(attribute => attribute.GetType() == typeof(IntegrationTestMethodAttribute)) > 0)
                {
                    try
                    {
                        Cleanup();
                        m.Invoke(this, null);
                        Console.WriteLine("############################");
                        Console.WriteLine("## " + m.Name + ": Success");
                        Console.WriteLine("############################");
                    }
                    catch (TargetInvocationException e)
                    {
                        if (e.InnerException is XunitException xe)
                        {
                            Console.WriteLine("############################");
                            Console.Error.WriteLine("## " + m.Name + ": Failed");
                            Console.Error.WriteLine(xe.Message);
                            Console.Error.WriteLine(xe.StackTrace);
                            Console.WriteLine("############################");
                        }
                        else throw;
                    }
                }
            }
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

        /// <summary>
        /// Runs the rbss protocol for the example given on pg. 37
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestExampleRun1()
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
            AddToRemote(setParticipant);
            AddToHost(setInitiator);
            
            var remoteResult = InitiateSync();
            Assert.Equal(2, remoteResult.Steps.Count);
            Assert.Contains(remoteResult.Steps, (step =>
                checkValidateStep(step, "ape", "eel")));
            Assert.Contains(remoteResult.Steps, (step =>
                checkValidateStep(step, "eel", "ape")));

            var localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.NotEmpty(localSyncState!.Steps);
            Assert.Equal(3, localSyncState.Steps.Count);
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "ape", "eel", new List<string>(){"ape"}, false)));
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "eel", "gnu")));
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "gnu", "ape", new List<string>(){"gnu"}, false)));

            remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            Assert.Equal(2, remoteResult.Steps.Count);
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "ape", "eel", new List<string>(){"bee", "cat", "doe"}, true)));
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "gnu", "ape", new List<string>(){"hog"}, true)));

            localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.Empty(localSyncState!.Steps);

            Assert.True(SetsSynchronized());
        }


        /// <summary>
        /// Runs the rbss protocol for the example given on pg. 40, fig. 3.2
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestExampleRun2()
        {
            var setParticipant = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("bee", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("doe", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("gnu", "")),
                new(new SimpleDataObject("hog", "")),
            };
            var setInitiator = new SortedSet<SimpleObjectWrapper>()
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
            AddToRemote(setParticipant);
            AddToHost(setInitiator);
            
            var remoteResult = InitiateSync();
            Assert.Equal(2, remoteResult.Steps.Count);
            Assert.Contains(remoteResult.Steps, (step =>
                checkValidateStep(step, "ape", "eel")));
            Assert.Contains(remoteResult.Steps, (step =>
                checkValidateStep(step, "eel", "ape")));

            var localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.NotEmpty(localSyncState!.Steps);
            Assert.Equal(2, localSyncState.Steps.Count);
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "eel", "gnu")));
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "gnu", "ape")));

            remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            Assert.Single(remoteResult.Steps);
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "eel", "gnu", new List<string>(){"eel"}, false)));

            localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.NotEmpty(localSyncState!.Steps);
            Assert.Single(localSyncState.Steps);
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "eel", "gnu", new List<string>(){"fox"}, true)));

            remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            Assert.NotNull(remoteResult);
            Assert.Empty(remoteResult!.Steps);

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Runs the rbss protocol for the example given on pg. 40, fig. 3.3
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestExampleRun3()
        {
            var setParticipant = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
                new(new SimpleDataObject("cat", "")),
                new(new SimpleDataObject("eel", "")),
                new(new SimpleDataObject("gnu", "")),
            };
            var setInitiator = new SortedSet<SimpleObjectWrapper>()
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
            AddToRemote(setParticipant);
            AddToHost(setInitiator);
            
            var remoteResult = InitiateSync();
            Assert.Equal(2, remoteResult.Steps.Count);
            Assert.Contains(remoteResult.Steps, (step =>
                checkValidateStep(step, "ape", "eel")));
            Assert.Contains(remoteResult.Steps, (step =>
                checkValidateStep(step, "eel", "ape")));

            var localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.NotEmpty(localSyncState!.Steps);
            Assert.Equal(4, localSyncState.Steps.Count);
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "ape", "cat")));
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "cat", "eel")));
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "eel", "gnu")));
            Assert.Contains(localSyncState.Steps, (step =>
                checkValidateStep(step, "gnu", "ape")));

            remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            Assert.Equal(4, remoteResult.Steps.Count);
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "ape", "cat", new List<string>(){"ape"}, false)));
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "cat", "eel", new List<string>(){"cat"}, false)));
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "eel", "gnu", new List<string>(){"eel"}, false)));
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "gnu", "ape", new List<string>(){"gnu"}, false)));

            localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.NotEmpty(localSyncState!.Steps);
            Assert.Equal(4, localSyncState.Steps.Count);
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "ape", "cat", new List<string>(){"bee"}, true)));
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "cat", "eel", new List<string>(){"doe"}, true)));
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "eel", "gnu", new List<string>(){"fox"}, true)));
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "gnu", "ape", new List<string>(){"hog"}, true)));

            remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            Assert.NotNull(remoteResult);
            Assert.Empty(remoteResult!.Steps);

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp when both peers have no elements in their respective sets
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void Test0To0()
        {
            var setParticipant = new SortedSet<SimpleObjectWrapper>()
            {
            };
            var setInitiator = new SortedSet<SimpleObjectWrapper>()
            {
            };
            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the participant has exactly one element in the set and the initiator has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void Test0To1()
        {
            var setParticipant = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
            };
            var setInitiator = new SortedSet<SimpleObjectWrapper>()
            {
            };
            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();


            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the initiator has exactly one element in the set and the participant has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void Test1To0()
        {
            var setParticipant = new SortedSet<SimpleObjectWrapper>()
            {
            };
            var setInitiator = new SortedSet<SimpleObjectWrapper>()
            {
                new(new SimpleDataObject("ape", "")),
            };
            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();

            Assert.True(SetsSynchronized());
        }
    }
}
