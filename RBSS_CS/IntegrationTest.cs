﻿using System.Diagnostics;
using System.Net;
using System.Reflection;
using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPITools.Client;
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
        private readonly IPersistenceLayerSingleton _persistenceLayer;

        public IntegrationTest(DebugApi dbgApi, SyncApi syncApi, ModifyApi modifyApi, IPersistenceLayerSingleton persistenceLayer)
        {
            _debugApi = dbgApi;
            _syncApi = syncApi;
            _modifyApi = modifyApi;
            // _remoteClient = new Client("http://host.docker.internal:5634");
            _remoteClient = ClientMap.Instance.SuccessorClient!;
            _persistenceLayer = persistenceLayer;
        }

        private void Cleanup()
        {
            _modifyApi.DeletePost(new SimpleDataObject("", ""));
            _remoteClient.ModifyApi.DeletePost(new SimpleDataObject("", ""));
        }

        private void AddToRemote(IEnumerable<SimpleDataObject> set)
        {
            foreach (var e in set)
            {
                try
                {
                    _remoteClient.ModifyApi.InsertPost(e);
                }
                catch (Org.OpenAPITools.Client.ApiException ex)
                {
                    if (ex.ErrorCode != 409) throw; // Error Code 409 is expected when duplicate items are added
                }
            }
        }

        private void AddToHost(IEnumerable<SimpleDataObject> set)
        {
            foreach (var e in set)
            {
                _modifyApi.InsertPost(e);
            }
        }

        private SyncState InitiateSync()
        {
            var range = _persistenceLayer.CreateRangeSet();
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

        private (int, int) Synchronize()
        {
            var comRounds = 1;
            var comTraffic = 0;
            var remoteResult = InitiateSync();
            while (remoteResult.Steps is { Count: > 0 })
            {
                var localSyncState = GetLocalResult(remoteResult);
                Assert.NotNull(localSyncState);
                comRounds += 1;
                comTraffic += remoteResult.Steps.Count + localSyncState.Steps.Count;
                if (localSyncState!.Steps.Count == 0) break;
                remoteResult = _remoteClient.SyncApi.SyncPut(localSyncState);
            }
            return (comRounds, comTraffic);
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
                        GC.Collect();
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
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("bee", "")),
                (new SimpleDataObject("cat", "")),
                (new SimpleDataObject("doe", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("gnu", "")),
                (new SimpleDataObject("hog", "")),
            };
            var setInitiator = new List<SimpleDataObject>()
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
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
                (new SimpleDataObject("cat", "")),
                (new SimpleDataObject("eel", "")),
                (new SimpleDataObject("gnu", "")),
            };
            var setInitiator = new List<SimpleDataObject>()
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
            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
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
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
            };
            var setInitiator = new List<SimpleDataObject>()
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
            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", "")),
            };
            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the initiator has some elements in the set and the participant has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestNTo0()
        {
            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
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
            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the participant has some elements in the set and the initiator has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void Test0ToN()
        {
            var setParticipant = new List<SimpleDataObject>()
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
            var setInitiator = new List<SimpleDataObject>()
            {
            };
            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();


            Assert.True(SetsSynchronized());
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestNToN()
        {
            var setInitiator = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("failure", "")),
                (new SimpleDataObject("full", "")),
                (new SimpleDataObject("observations", "")),
                (new SimpleDataObject("scotland", "")),
                (new SimpleDataObject("te", "")),
            };
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("peace", "")),
                (new SimpleDataObject("poverty", "")),
                (new SimpleDataObject("scotland", "")),
                (new SimpleDataObject("spa", "")),
                (new SimpleDataObject("te", "")),
            };

            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            Synchronize();

            var union = setInitiator.Union(setParticipant);
            // var controlSetInitiatorAction = _debugApi.DebugGetSet();
            // Assert.IsType<OkObjectResult>(controlSetInitiatorAction.GetType());
            // var controlSetInitiatorResult = ((OkObjectResult)controlSetInitiatorAction).Value;
            // Assert.NotNull(controlSetInitiatorResult);
            // Assert.IsType<SimpleDataObject[]>(controlSetInitiatorResult!.GetType());
            // var controlSetInitiator = (SimpleDataObject[])controlSetInitiatorResult!;

            var controlSetInitiator = _persistenceLayer.GetDataObjects();

            var controlSetParticipant =
                _remoteClient.SyncApi.Client.Get<SimpleDataObject[]>("/getset", new RequestOptions()).Data;

            Console.WriteLine("Initiator: ");
            foreach (var v in controlSetInitiator)
            {
                Console.WriteLine("\t" + v.Id);
            }
            Console.WriteLine("Participant: ");
            foreach (var v in controlSetParticipant)
            {
                Console.WriteLine("\t" + v.Id);
            }


            Assert.True(SetsSynchronized());
        }

                /// <summary>
        /// Tests for equal Fp after synchronization when the participant has exactly one element in the set and the initiator has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10()
        {
            using var client = new HttpClient();
            var downloadedString =
                client.GetStringAsync(
                    "https://www.mit.edu/~ecprice/wordlist.10000").Result;
            string[] randomWords = downloadedString.Split('\n');

            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
            {
            };

            for (int i = 0; i < 4; i++)
            {
                setParticipant.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
                setInitiator.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
            }
            for (int i = 0; i < Random.Shared.Next(4); i++)
            {
                setParticipant.Add(setInitiator.ElementAt(Random.Shared.Next(setInitiator.Count)));
                setInitiator.Add(setParticipant.ElementAt(Random.Shared.Next(setParticipant.Count)));
            }

            Console.WriteLine("### Elements in Initiator: ");
            foreach (var swo in setInitiator)
            {
                Console.WriteLine(swo.Id);
            }

            Console.WriteLine("### Elements in Participant: ");
            foreach (var swo in setParticipant)
            {
                Console.WriteLine(swo.Id);
            }

            var n = setInitiator.Union(setParticipant).Count();
            var nDelta = n - setInitiator.Intersect(setParticipant).Count();
        


            int comRoundsUpper = (int)Math.Log2(n);
            int comComplexUpper = (int)Math.Min(nDelta * Math.Log2(n), 2 * n - 1);

            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            //var co = Console.Out;
            //Console.SetOut(new System.IO.StreamWriter(System.IO.Stream.Null));
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var (r, c) = Synchronize();
            watch.Stop();
            //Console.SetOut(co);

            Console.WriteLine("### Time (ms) needed for synchronization: " + watch.ElapsedMilliseconds);
            Console.WriteLine("### Communication rounds needed " + r + " <= " + comRoundsUpper);
            Console.WriteLine("### Communication complexity needed " + c + " <= " + comComplexUpper);
            Console.WriteLine("### Elements in Union: " + n);
            Console.WriteLine("### Elements missing: " + nDelta);

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the participant has exactly one element in the set and the initiator has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom100()
        {
            using var client = new HttpClient();
            var downloadedString =
                client.GetStringAsync(
                    "https://www.mit.edu/~ecprice/wordlist.10000").Result;
            string[] randomWords = downloadedString.Split('\n');

            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
            {
            };

            for (int i = 0; i < 50; i++)
            {
                setParticipant.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
                setInitiator.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
            }
            for (int i = 0; i < 10; i++)
            {
                setParticipant.Add(setInitiator.ElementAt(Random.Shared.Next(setInitiator.Count)));
                setInitiator.Add(setParticipant.ElementAt(Random.Shared.Next(setParticipant.Count)));
            }

            var n = setInitiator.Union(setParticipant).Count();
            var nDelta = n - setInitiator.Intersect(setParticipant).Count();
            


            int comRoundsUpper = (int)Math.Log2(n);
            int comComplexUpper = (int)Math.Min(nDelta * Math.Log2(n), 2 * n - 1);

            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            var co = Console.Out;
            Console.SetOut(new System.IO.StreamWriter(System.IO.Stream.Null));
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var (r, c) = Synchronize();
            watch.Stop();
            Console.SetOut(co);

            Console.WriteLine("### Time (ms) needed for synchronization: " + watch.ElapsedMilliseconds);
            Console.WriteLine("### Communication rounds needed " + r + " <= " + comRoundsUpper);
            Console.WriteLine("### Communication complexity needed " + c + " <= " + comComplexUpper);
            Console.WriteLine("### Elements in Union: " + n);
            Console.WriteLine("### Elements missing: " + nDelta);

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the participant has exactly one element in the set and the initiator has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000()
        {
            Cleanup();
            using var client = new HttpClient();
            var downloadedString =
                client.GetStringAsync(
                    "https://www.mit.edu/~ecprice/wordlist.10000").Result;
            string[] randomWords = downloadedString.Split('\n');

            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
            {
            };

            for (int i = 0; i < 500; i++)
            {
                setParticipant.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
                setInitiator.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
            }
            for (int i = 0; i < 100; i++)
            {
                setParticipant.Add(setInitiator.ElementAt(Random.Shared.Next(setInitiator.Count)));
                setInitiator.Add(setParticipant.ElementAt(Random.Shared.Next(setParticipant.Count)));
            }

            var n = setInitiator.Union(setParticipant).Count();
            var nDelta = n - setInitiator.Intersect(setParticipant).Count();
            

            int comRoundsUpper = (int)Math.Log2(n);
            int comComplexUpper = (int)Math.Min(nDelta * Math.Log2(n), 2 * n - 1);

            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            var co = Console.Out;
            Console.SetOut(new System.IO.StreamWriter(System.IO.Stream.Null));
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var (r, c) = Synchronize();
            watch.Stop();
            Console.SetOut(co);

            Console.WriteLine("### Time (ms) needed for synchronization: " + watch.ElapsedMilliseconds);
            Console.WriteLine("### Communication rounds needed " + r + " <= " + comRoundsUpper);
            Console.WriteLine("### Communication complexity needed " + c + " <= " + comComplexUpper);
            Console.WriteLine("### Elements in Union: " + n);
            Console.WriteLine("### Elements missing: " + nDelta);

            Assert.True(SetsSynchronized());
        }

        /// <summary>
        /// Tests for equal Fp after synchronization when the participant has exactly one element in the set and the initiator has none
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000BigIntersection()
        {
            Cleanup();
            using var client = new HttpClient();
            var downloadedString =
                client.GetStringAsync(
                    "https://www.mit.edu/~ecprice/wordlist.10000").Result;
            string[] randomWords = downloadedString.Split('\n');

            var setParticipant = new List<SimpleDataObject>()
            {
            };
            var setInitiator = new List<SimpleDataObject>()
            {
            };

            for (int i = 0; i < 500; i++)
            {
                setParticipant.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
                setInitiator.Add(
                    (new SimpleDataObject(randomWords[Random.Shared.Next(randomWords.Length)], "")));
            }
            for (int i = 0; i < 750; i++)
            {
                setParticipant.Add(setInitiator.ElementAt(Random.Shared.Next(setInitiator.Count)));
                setInitiator.Add(setParticipant.ElementAt(Random.Shared.Next(setParticipant.Count)));
            }

            var n = setInitiator.Union(setParticipant).Count();
            var nDelta = n - setInitiator.Intersect(setParticipant).Count();
            

            int comRoundsUpper = (int)Math.Log2(n);
            int comComplexUpper = (int)Math.Min(nDelta * Math.Log2(n), 2 * n - 1);

            AddToRemote(setParticipant);
            AddToHost(setInitiator);

            var co = Console.Out;
            Console.SetOut(new System.IO.StreamWriter(System.IO.Stream.Null));
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var (r, c) = Synchronize();
            watch.Stop();
            Console.SetOut(co);

            Console.WriteLine("### Time (ms) needed for synchronization: " + watch.ElapsedMilliseconds);
            Console.WriteLine("### Communication rounds needed " + r + " <= " + comRoundsUpper);
            Console.WriteLine("### Communication complexity needed " + c + " <= " + comComplexUpper);
            Console.WriteLine("### Elements in Union: " + n);
            Console.WriteLine("### Elements missing: " + nDelta);

            Assert.True(SetsSynchronized());
        }
    }
}
