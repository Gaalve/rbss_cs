using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Text;
using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Newtonsoft.Json;
using Org.OpenAPITools.Client;
using RBSS_CS.Controllers;
using Serilog;
using Serilog.Core;
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
        private readonly ServerSettings _settings;

        private readonly Logger _testLogger;
        private readonly Logger _memLogger;

        private SimpleDataObject[] _wordData;
        private SimpleDataObject[] _wordBigData;
        public IntegrationTest(DebugApi dbgApi, SyncApi syncApi, ModifyApi modifyApi, IPersistenceLayerSingleton persistenceLayer, ServerSettings settings)
        {
            _debugApi = dbgApi;
            _syncApi = syncApi;
            _modifyApi = modifyApi;
            _remoteClient = ClientMap.Instance.SuccessorClient!;
            _persistenceLayer = persistenceLayer;
            _settings = settings;
            using var client = new HttpClient();
            var downloadedString =
                client.GetStringAsync(
                    "https://www.mit.edu/~ecprice/wordlist.100000").Result;
            string[] randomWords = downloadedString.Split('\n');
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            _wordData = randomWords.Select(s => new SimpleDataObject(s, 1, "")).ToArray();
            _wordBigData = randomWords.Select(s => new SimpleDataObject(s, 1, new string(Enumerable.Repeat(
                chars, 1024).Select(s => s[Random.Shared.Next(s.Length)]).ToArray()))).ToArray();

            _testLogger = new LoggerConfiguration().WriteTo.File(
                path: "testResults.log", outputTemplate: "{Message}{NewLine}").CreateLogger();
            _memLogger = new LoggerConfiguration().WriteTo.File(
                path: "memResults.log", outputTemplate: "{Message}{NewLine}"
            ).CreateLogger();

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();
            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            _remoteClient.ModifyApi.Client.Post<IActionResult>("/debugSyncStop", localVarRequestOptions);
        }

        private void Cleanup()
        {
            Console.WriteLine("Cleanup ");
            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();
            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            Console.WriteLine("SendingRequest ");
            _remoteClient.ModifyApi.Client.Post<IActionResult>("/debugReset", localVarRequestOptions);
            Console.WriteLine("RequestComplete ");
            _debugApi.DebugReset();
            Console.WriteLine("Local Reset Complete ");
        }
        private void AddToRemote(SimpleDataObject sdo)
        {
            try
            {
                _remoteClient.ModifyApi.InsertPost(sdo);
            }
            catch (Org.OpenAPITools.Client.ApiException ex)
            {
                if (ex.ErrorCode != 409) throw; // Error Code 409 is expected when duplicate items are added
            }
        }

        private void AddToHost(SimpleDataObject sdo)
        {
            _modifyApi.InsertPost((SimpleDataObject) JsonConvert.DeserializeObject(sdo.ToJson(), typeof(SimpleDataObject))!);
        }

        private void AddToHostBatch(IEnumerable<SimpleDataObject> set)
        {
            _debugApi.DebugBatchInsert(new DebugInsert(set.Select(s => 
                (SimpleDataObject) JsonConvert.DeserializeObject(s.ToJson(), typeof(SimpleDataObject))! ).ToArray()));
        }

        private void AddToRemoteBatch(IEnumerable<SimpleDataObject> set)
        {
            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();
            string[] _contentTypes = new string[] {
                "application/json"
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            localVarRequestOptions.Data = new DebugInsert(set.ToArray());

            _remoteClient.ModifyApi.Client.Post<IActionResult>("/batchInsert", localVarRequestOptions);
        }

        private SyncState InitiateSync()
        {
            var range = _persistenceLayer.CreateRangeSet();
            return _remoteClient.SyncApi.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint)).Syncstate;
        }

        private ApiResponse<InlineResponse> InitiateSyncWithInfo()
        {
            var range = _persistenceLayer.CreateRangeSet();
            return _remoteClient.SyncApi.SyncPostWithHttpInfo(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));
        }

        private bool SetsSynchronized()
        {
            return InitiateSync().Steps.Count == 0;
        }


        private SyncState? GetLocalResult(SyncState remoteResult)
        {
            var localAction = _syncApi.SyncPut(new InlineResponse(remoteResult));

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

            if (localResult.GetType() != typeof(InlineResponse))
            {
                Console.WriteLine("Wrong object attached.");
                return null;
            }

            return ((InlineResponse)localResult).Syncstate;
        }

        private (int, int) Synchronize()
        {
            var comRounds = 2; // Verification Step + Response
            var comTraffic = 0;
            var remoteResult = InitiateSync();
            while (remoteResult.Steps is { Count: > 0 })
            {
                comRounds += 1;
                var localSyncState = GetLocalResult(remoteResult);
                Assert.NotNull(localSyncState);
                comTraffic += remoteResult.Steps.Count + localSyncState!.Steps.Count;
                if (localSyncState!.Steps.Count == 0) break;
                comRounds += 1;
                remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
            }
            return (comRounds, comTraffic);
        }

        private (int, int) SynchronizeMeasureBytes()
        {
            var bytesSent = 0;
            var bytesReceived = 0;
            var range = _persistenceLayer.CreateRangeSet();
            var step = new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint);
            bytesSent += step.ToJson().Length;
            var remoteResult = _remoteClient.SyncApi.SyncPost(step).Syncstate;
            while (remoteResult.Steps is { Count: > 0 })
            {
                bytesReceived += remoteResult.ToJson().Length;
                var localSyncState = GetLocalResult(remoteResult);
                Assert.NotNull(localSyncState);
                if (localSyncState!.Steps.Count == 0) break;
                bytesSent += localSyncState.ToJson().Length;
                remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
            }
            bytesReceived += remoteResult.ToJson().Length;
            return (bytesSent, bytesReceived);
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
                        Console.WriteLine("Starting next Test: "+m.Name);
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

        /// <summary>
        /// Runs the rbss protocol for the example given on pg. 37
        /// </summary>
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestExampleRun1()
        {
            if (_settings.BranchingFactor != 2 || _settings.ItemSize != 1) return;
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1, "")),
                (new SimpleDataObject("doe", 1, "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("fox", 1, "")),
                (new SimpleDataObject("hog", 1, "")),
            };
            var setInitiator = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", 1, "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("fox", 1, "")),
                (new SimpleDataObject("gnu", 1, "")),
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

            var remoteResultInfo = InitiateSyncWithInfo();
            Console.WriteLine(remoteResultInfo.RawContent);


            Assert.NotNull(remoteResultInfo.Data);
            
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

            remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
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
            if (_settings.BranchingFactor != 2 || _settings.ItemSize != 1) return;
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", 1, "")),
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1, "")),
                (new SimpleDataObject("doe", 1, "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("gnu", 1, "")),
                (new SimpleDataObject("hog", 1, "")),
            };
            var setInitiator = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", 1, "")),
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1, "")),
                (new SimpleDataObject("doe", 1, "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("fox", 1, "")),
                (new SimpleDataObject("gnu", 1, "")),
                (new SimpleDataObject("hog", 1, "")),
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);
            
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

            remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
            Assert.Single(remoteResult.Steps);
            Assert.Contains(remoteResult.Steps, (step =>
                checkInsertStep(step, "eel", "gnu", new List<string>(){"eel"}, false)));

            localSyncState = GetLocalResult(remoteResult);
            Assert.NotNull(localSyncState);
            Assert.NotEmpty(localSyncState!.Steps);
            Assert.Single(localSyncState.Steps);
            Assert.Contains(localSyncState.Steps, (step =>
                checkInsertStep(step, "eel", "gnu", new List<string>(){"fox"}, true)));

            remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
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
            if (_settings.BranchingFactor != 2 || _settings.ItemSize != 1) return;
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", 1, "")),
                (new SimpleDataObject("cat", 1, "")),
                (new SimpleDataObject("eel", 1,  "")),
                (new SimpleDataObject("gnu", 1,  "")),
            };
            var setInitiator = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("ape", 1,  "")),
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1,  "")),
                (new SimpleDataObject("doe", 1,  "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("fox", "")),
                (new SimpleDataObject("gnu", 1, "")),
                (new SimpleDataObject("hog", 1, "")),
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);
            
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

            remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
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

            remoteResult = _remoteClient.SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
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
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

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
                (new SimpleDataObject("ape", 1, "")),
            };
            var setInitiator = new List<SimpleDataObject>()
            {
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

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
                (new SimpleDataObject("ape", 1, "")),
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

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
                (new SimpleDataObject("ape", 1, "")),
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1, "")),
                (new SimpleDataObject("doe", 1, "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("fox", 1, "")),
                (new SimpleDataObject("gnu", 1, "")),
                (new SimpleDataObject("hog", 1, "")),
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

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
                (new SimpleDataObject("ape", 1, "")),
                (new SimpleDataObject("bee", 1, "")),
                (new SimpleDataObject("cat", 1, "")),
                (new SimpleDataObject("doe", 1, "")),
                (new SimpleDataObject("eel", 1, "")),
                (new SimpleDataObject("fox", 1, "")),
                (new SimpleDataObject("gnu", 1, "")),
                (new SimpleDataObject("hog", 1, "")),
            };
            var setInitiator = new List<SimpleDataObject>()
            {
            };
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

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
                (new SimpleDataObject("failure", 1, "")),
                (new SimpleDataObject("full", 1, "")),
                (new SimpleDataObject("observations", 1, "")),
                (new SimpleDataObject("scotland", 1, "")),
                (new SimpleDataObject("te", 1, "")),
            };
            var setParticipant = new List<SimpleDataObject>()
            {
                (new SimpleDataObject("peace", 1, "")),
                (new SimpleDataObject("poverty", 1, "")),
                (new SimpleDataObject("scotland", 1, "")),
                (new SimpleDataObject("spa", 1, "")),
                (new SimpleDataObject("te", 1, "")),
            };

            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

            Synchronize();

            Assert.True(SetsSynchronized());
        }

        private static readonly string[] ByteSuffix = { "B", "KB", "MB", "GB" };
        private static string GetByteString(long value)
        {
            if (value < 0) return $"{value} B";
            var magnitute = (int)Math.Max(0, Math.Log(value, 1024));
            var adjSize = Math.Round(value / Math.Pow(1024, magnitute), 2);
            return $"{adjSize} {ByteSuffix[magnitute]}";
        }

        private void TestRandom(int length, float intersection, bool big, bool fixedSeed = false,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            Cleanup();

            var random = fixedSeed ? new Random(5663334) : Random.Shared;

            var setParticipant = new HashSet<SimpleDataObject>();
            var setInitiator = new HashSet<SimpleDataObject>();

            int l2 = length / 2;
            SimpleDataObject[] arr = big ? _wordBigData : _wordData;
            int al = arr.Length;
            for (int i = 0; i < l2; i++)
            {
                int r1 = random.Next(al);
                int r2 = random.Next(al);
                setInitiator.Add(arr[r1]);
                setParticipant.Add(arr[r2]);
                if (random.NextSingle() < intersection) setInitiator.Add(arr[r2]);
                if (random.NextSingle() < intersection) setParticipant.Add(arr[r1]);
            }
            var nA = setInitiator.Union(setParticipant).Count();

            var n0 = setInitiator.Count;
            var n1 = setParticipant.Count;

            var n = n0 + n1;

            var nDelta = nA - setInitiator.Intersect(setParticipant).Count();
            var nMin = Math.Min(setInitiator.Count, setParticipant.Count);

            // int comRoundsUpper = (int)Math.Ceiling(Math.Log(n, _settings.BranchingFactor));
            // int comComplexUpper = (int)Math.Min(nDelta * Math.Log(n, _settings.BranchingFactor), 2 * n - 1);

            var b = _settings.BranchingFactor;
            var t = _settings.ItemSize;
            int comRoundsUpper = (int)Math.Ceiling(2 + 2 * Math.Log(nMin, b) - Math.Log(t, b));
            int comComplexUpper = (int)Math.Min(Math.Ceiling(nDelta * (2 + 2 * Math.Log(nMin, b) - Math.Log(t, b))), 2 * n);

            GC.Collect();
            var memoryBefore = GC.GetTotalMemory(true);
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

            var co = Console.Out;
            Console.SetOut(StreamWriter.Null);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var (r, c) = Synchronize();
            watch.Stop();
            GC.Collect();
            var memoryNow = GC.GetTotalMemory(true);
            Assert.True(SetsSynchronized());

            Cleanup();
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);
            var (bytesSent, bytesReceived) = SynchronizeMeasureBytes();
            Console.SetOut(co);

            Console.WriteLine("### Time (ms) needed for synchronization: " + watch.ElapsedMilliseconds);
            Console.WriteLine("### Communication rounds needed " + r + " <= " + comRoundsUpper);
            Console.WriteLine("### Communication complexity needed " + c + " <= " + comComplexUpper);
            Console.WriteLine("### Elements in Union: " + n);
            Console.WriteLine("### Elements missing: " + nDelta);

            Console.WriteLine("### Bytes Sent: " + GetByteString(bytesSent));
            Console.WriteLine("### Bytes Received: " + GetByteString(bytesReceived));
            Console.WriteLine("### Bytes Total: " + GetByteString(bytesSent + bytesReceived));

            Console.WriteLine("### Memory Before: " + GetByteString(memoryBefore));
            Console.WriteLine("### Memory Now: " + GetByteString(memoryNow));
            Console.WriteLine("### Memory Usage: " + GetByteString(memoryNow - memoryBefore));

            Assert.True(SetsSynchronized());
            //Test succeeded, write to File
            StringBuilder sb = new StringBuilder();
            sb.Append(memberName).Append(",").Append(_settings.AuxillaryDS).Append(",").Append("b: ")
                .Append(_settings.BranchingFactor).Append("t: ").Append(_settings.ItemSize).Append(",").Append(n0)
                .Append(",").Append(n1).Append(",").Append(bytesReceived).Append(",").Append(bytesSent).Append(",")
                .Append(c).Append(",").Append(r).Append(",").Append(watch.ElapsedMilliseconds).Append(",").Append(nDelta)
                .Append(",").Append(nA);

            _testLogger.Information(sb.ToString());
        }


        private void TestRandomUpdate(int length, float intersection, float updateAmount)
        {
            Cleanup();
            var setParticipant = new HashSet<SimpleDataObject>();
            var setInitiator = new HashSet<SimpleDataObject>();

            int l2 = length / 2;
            SimpleDataObject[] arr = _wordData;
            int al = arr.Length;
            for (int i = 0; i < l2; i++)
            {
                int r1 = Random.Shared.Next(al);
                int r2 = Random.Shared.Next(al);
                setInitiator.Add(arr[r1]);
                setParticipant.Add(arr[r2]);
                if (Random.Shared.NextSingle() < intersection) setInitiator.Add(arr[r2]);
                if (Random.Shared.NextSingle() < intersection) setParticipant.Add(arr[r1]);
            }
            AddToRemoteBatch(setParticipant);
            AddToHostBatch(setInitiator);

            var co = Console.Out;
            Console.SetOut(StreamWriter.Null);
            Synchronize();
            Console.SetOut(co);
            Assert.True(SetsSynchronized());
            for (int i = 0; i < length * updateAmount; i++)
            {
                var e_in = setInitiator.ElementAt(Random.Shared.Next(setInitiator.Count));
                e_in.AdditionalProperties = "updated";
                AddToHost(e_in);
                var e_pa = setParticipant.ElementAt(Random.Shared.Next(setParticipant.Count));
                e_pa.AdditionalProperties = "updated";
                AddToRemote(e_pa);
            }
            Assert.False(SetsSynchronized());
            Console.SetOut(StreamWriter.Null);
            Synchronize();
            Console.SetOut(co);
            Assert.True(SetsSynchronized());
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10()
        {
            TestRandom(10, 0.5f, false);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom100()
        {
            TestRandom(100, 0.5f, false);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000()
        {
            TestRandom(1000, 0.5f, false);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000BigIntersection()
        {
            TestRandom(1000, 0.9f, false);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10000BigIntersection()
        {
            TestRandom(10000, 0.9f, false);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10000BigData()
        {
            TestRandom(10000, 0.5f, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10000BigIntersectionBigData()
        {
            TestRandom(10000, 0.9f, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10FixedSeed()
        {
            TestRandom(10, 0.5f, false, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom100FixedSeed()
        {
            TestRandom(100, 0.5f, false, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000FixedSeed()
        {
            TestRandom(1000, 0.5f, false, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000BigIntersectionFixedSeed()
        {
            TestRandom(1000, 0.9f, false, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10000BigIntersectionFixedSeed()
        {
            TestRandom(10000, 0.9f, false, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10000BigDataFixedSeed()
        {
            TestRandom(10000, 0.5f, true, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom10000BigIntersectionBigDataFixedSeed()
        {
            TestRandom(10000, 0.9f, true, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom20000BigDataFixedSeed()
        {
            TestRandom(20000, 0.5f, true, true);
        }

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom20000BigIntersectionBigDataFixedSeed()
        {
            TestRandom(20000, 0.9f, true, true);
        }
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom30000BigIntersectionBigDataFixedSeed()
        {
            TestRandom(30000, 0.95f, true, true);
        }
        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom30000BigDataFixedSeed()
        {
            TestRandom(30000, 0.7f, true, true);
        }

        

        [IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestRandom1000Update()
        {
            TestRandomUpdate(1000, 0.5f, 0.2f);
        }

        //[IntegrationTestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via Reflection")]
        // ReSharper disable once UnusedMember.Local
        private void TestMemory()
        {
            Cleanup();
            GC.Collect();
            var memBefore = GC.GetTotalMemory(true);
            for (int i = 0; i < 1000; i++)
            {
                AddToHost(_wordData[i]);
                GC.Collect();
                var memAfter = GC.GetTotalMemory(true);
                var sb = new StringBuilder();
                sb.Append(_settings.AuxillaryDS).Append(',').Append((memAfter - memBefore)).Append(',').Append(i);
                _memLogger.Information(sb.ToString());
            }
        }
    }
}
