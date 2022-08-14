using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;
using Xunit;

namespace RBSS_CS.Controllers
{
    [ApiController]
    public class SyncApi : SyncApiController
    {
        private readonly ServerSettings _settings;
        private readonly IPersistenceLayerSingleton _persistenceLayer;
        

        public SyncApi(ServerSettings settings, IPersistenceLayerSingleton persistenceLayer)
        {
            _settings = settings;
            _persistenceLayer = persistenceLayer;
        }
        private OneOfValidateStepInsertStep? CreateStep(RangeSet? set)
        {
            if (set?.Data == null || set.Data.Length == 0) return null;
            else if (set.Data.Length <= _settings.ItemSize)
                return new InsertStep(set.IdFrom, new List<string>(), set.IdTo, new List<SimpleDataObject>(collection: set.Data), false);

            return new ValidateStep(set.IdFrom, set.IdTo, set.Fingerprint);
        }

        private void HandleValidateStep(ValidateStep validateStep, SyncState state)
        {
            // Console.WriteLine("\tHandleValidateStep: " + validateStep.ToString());
            var equalFp = _persistenceLayer.GetFingerprint(validateStep.IdFrom, validateStep.IdTo).ToString() ==
                          validateStep.FpOfData;
            if (equalFp) return;
            var ranges = _persistenceLayer.SplitRange(validateStep.IdFrom, validateStep.IdTo);

            bool notNull = false;

            foreach (var rangeSet in ranges)
            {
                var step = CreateStep(rangeSet);
                if (step != null)
                {
                    state.Steps.Add(new Step(0, step));
                    notNull = true;
                }
            }
            if (!notNull) state.Steps.Add(new Step(0, 
                new InsertStep(validateStep.IdFrom, new List<string>(), validateStep.IdTo, new List<SimpleDataObject>(), false)));
        }

        private void HandleInsertStep(InsertStep insertStep, SyncState state)
        {
            // Console.WriteLine("\tInsertStep: " + insertStep.ToString());


            if (insertStep.Handled == false)
            {
                RangeSet set = _persistenceLayer.CreateRangeSet(insertStep.IdFrom, insertStep.IdTo, insertStep.DataToInsert);

                if (set.Data != null && set.Data.Length > 0)
                {
                    state.Steps.Add(new Step(0, new InsertStep(insertStep.IdFrom, new List<string>(), 
                        insertStep.IdTo, new List<SimpleDataObject>(set.Data), true)));
                }
            }
            
            foreach (var data in insertStep.DataToInsert)
            {
                _persistenceLayer.Insert(data);
            }
        }

        public override IActionResult SyncPost(ValidateStep validateStep)
        {
            // Console.WriteLine("SyncPost Begin");
            SyncState state = new SyncState(0, new List<Step>());
            HandleValidateStep(validateStep, state);
            // Console.WriteLine("SyncPost End");
            return Ok(new InlineResponse(state));
        }

        public override IActionResult SyncPut(InlineResponse inlineResponse)
        {
            var syncState = inlineResponse.Syncstate;
            // Console.WriteLine("SyncPut Begin");
            if (syncState.Steps == null || syncState.Steps.Count == 0)
            {
                // Console.WriteLine("Bad SyncState");
                return BadRequest();
            }
            SyncState state = new SyncState(0, new List<Step>()); 
            foreach (var step in syncState.Steps)
            {
                if (step.CurrentStep.GetType() == typeof(ValidateStep))
                {
                    HandleValidateStep((ValidateStep)step.CurrentStep, state);
                }
                else if (step.CurrentStep.GetType() == typeof(InsertStep))
                {
                    HandleInsertStep((InsertStep)step.CurrentStep, state);
                }
                else
                {
                    // Console.WriteLine("Wrong Step Type");
                    return Forbid();
                }
            }
            // Console.WriteLine("SyncPut End");
            return Ok(new InlineResponse(state));
        }


    }
}
