using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    [ApiController]
    public class SyncApi : SyncApiController
    {

        private AbstractStep? createStep(RangeSet set)
        {
            if (set.Data == null || set.Data.Length == 0) return null;
            else if (set.Data.Length == 1)
                return new InsertStep(set.IdFrom, new List<string>(), set.IdTo, new List<SimpleDataObject>(collection: set.Data), false);

            return new ValidateStep(set.IdFrom, set.IdTo, set.Fingerprint);
        }

        private void HandleValidateStep(ValidateStep validateStep, SyncState state)
        {
            Console.WriteLine("\tHandleValidateStep: " + validateStep.ToString());
            var equalFp = PersistenceLayer.Instance.GetFingerprint(validateStep.IdFrom, validateStep.IdTo).ToString() ==
                          validateStep.FpOfData;
            if (equalFp) return;
            var ranges = PersistenceLayer.Instance.SplitRange(validateStep.IdFrom, validateStep.IdTo);

            var step1 = createStep(ranges[0]);
            var step2 = createStep(ranges[1]);

            if (step1 != null) state.Steps.Add(new Step(0, new OneOfValidateStepInsertStep(step1)));
            if (step2 != null) state.Steps.Add(new Step(0, new OneOfValidateStepInsertStep(step2)));
        }

        private void HandleInsertStep(InsertStep insertStep, SyncState state)
        {
            Console.WriteLine("\tInsertStep: " + insertStep.ToString());


            if (insertStep.Handled == false)
            {
                RangeSet set = PersistenceLayer.Instance.CreateRangeSet(insertStep.IdFrom, insertStep.IdTo, insertStep.DataToInsert);

                if (set.Data != null && set.Data.Length > 0)
                {
                    state.Steps.Add(new Step(0, new OneOfValidateStepInsertStep(new InsertStep(insertStep.IdFrom, new List<string>(), 
                        insertStep.IdTo, new List<SimpleDataObject>(set.Data), true))));
                }
            }
            
            foreach (var data in insertStep.DataToInsert)
            {
                PersistenceLayer.Instance.Insert(data);
            }
        }

        public override IActionResult SyncPost(ValidateStep validateStep)
        {
            Console.WriteLine("SyncPost Begin");
            SyncState state = new SyncState(0, new List<Step>());
            HandleValidateStep(validateStep, state);
            Console.WriteLine("SyncPost End");
            return Ok(state);
        }

        public override IActionResult SyncPut(SyncState syncState)
        {
            Console.WriteLine("SyncPut Begin");
            if (syncState.Steps == null || syncState.Steps.Count == 0)
            {
                Console.WriteLine("Bad SyncState");
                return BadRequest();
            }
            SyncState state = new SyncState(0, new List<Step>()); 
            foreach (var step in syncState.Steps)
            {
                if (step.CurrentStep.Step.GetType() == typeof(ValidateStep))
                {
                    HandleValidateStep((ValidateStep)step.CurrentStep.Step, state);
                }
                else if (step.CurrentStep.Step.GetType() == typeof(InsertStep))
                {
                    HandleInsertStep((InsertStep)step.CurrentStep.Step, state);
                }
                else
                {
                    Console.WriteLine("Wrong Step Type");
                    return Forbid();
                }
            }
            Console.WriteLine("SyncPut End");
            return Ok(state);
        }
    }
}
