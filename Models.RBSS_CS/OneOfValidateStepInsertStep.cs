using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Models.RBSS_CS;

namespace Models.RBB_CS
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class OneOfValidateStepInsertStep : IEquatable<OneOfValidateStepInsertStep>, IValidatableObject
    {
        public OneOfValidateStepInsertStep(AbstractStep step)
        {
            this.Step = step;
        }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        [DataMember(Name = "step", EmitDefaultValue = true)]
        public AbstractStep Step
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Step {\n");
            sb.Append("  Step: ").Append(Step).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((OneOfValidateStepInsertStep)obj);
        }

        /// <summary>
        /// Returns true if Step instances are equal
        /// </summary>
        /// <param name="other">Instance of Step to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OneOfValidateStepInsertStep other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
            (
                Step == other.Step ||

                Step.Equals(other.Step)
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
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)

                hashCode = hashCode * 59 + Step.GetHashCode();
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