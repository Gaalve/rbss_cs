/* 
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * OpenAPI spec version: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Models.RBSS_CS
{
    /// <summary>
    /// Step
    /// </summary>
    [DataContract]
        public partial class Step :  IEquatable<Step>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Step" /> class.
        /// </summary>
        /// <param name="timeSent">timestamp of the step.</param>
        /// <param name="currentStep">currentStep.</param>
        public Step(long? timeSent = default(long?), OneOfValidateStepInsertStep currentStep = default(OneOfValidateStepInsertStep))
        {
            this.TimeSent = timeSent;
            this.CurrentStep = currentStep;
        }
        
        /// <summary>
        /// timestamp of the step
        /// </summary>
        /// <value>timestamp of the step</value>
        [DataMember(Name="timeSent", EmitDefaultValue=false)]
        public long? TimeSent { get; set; }

        /// <summary>
        /// Gets or Sets CurrentStep
        /// </summary>
        [DataMember(Name="currentStep", EmitDefaultValue=false)]
        public OneOfValidateStepInsertStep CurrentStep { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Step {\n");
            sb.Append("  TimeSent: ").Append(TimeSent).Append("\n");
            sb.Append("  CurrentStep: ").Append(CurrentStep).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        

  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Step);
        }

        /// <summary>
        /// Returns true if Step instances are equal
        /// </summary>
        /// <param name="input">Instance of Step to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Step input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.TimeSent == input.TimeSent ||
                    (this.TimeSent != null &&
                    this.TimeSent.Equals(input.TimeSent))
                ) && 
                (
                    this.CurrentStep == input.CurrentStep ||
                    (this.CurrentStep != null &&
                    this.CurrentStep.Equals(input.CurrentStep))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.TimeSent != null)
                    hashCode = hashCode * 59 + this.TimeSent.GetHashCode();
                if (this.CurrentStep != null)
                    hashCode = hashCode * 59 + this.CurrentStep.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
