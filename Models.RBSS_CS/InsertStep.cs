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
    /// InsertStep
    /// </summary>
    [DataContract]
        public partial class InsertStep : OneOfValidateStepInsertStep, IEquatable<InsertStep>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertStep" /> class.
        /// </summary>
        /// <param name="idFrom">idFrom.</param>
        /// <param name="idNext">idNext.</param>
        /// <param name="idTo">idTo.</param>
        /// <param name="dataToInsert">should be handled, outside see page 48, or use hash in calculation &#x3D;&gt; conflict have to be solved.</param>
        /// <param name="handled">both have to be update dataToInsert in their own set (\&quot;recursion anker\&quot;).</param>
        public InsertStep(string idFrom = default(string), List<string> idNext = default(List<string>), string idTo = default(string), List<SimpleDataObject> dataToInsert = default(List<SimpleDataObject>), bool? handled = default(bool?))
        {
            this.IdFrom = idFrom;
            this.IdNext = idNext;
            this.IdTo = idTo;
            this.DataToInsert = dataToInsert;
            this.Handled = handled;
        }

        /// <summary>
        /// Gets or Sets IdNext
        /// </summary>
        [DataMember(Name="idNext", EmitDefaultValue=false)]
        public List<string> IdNext { get; set; }

        /// <summary>
        /// should be handled, outside see page 48, or use hash in calculation &#x3D;&gt; conflict have to be solved
        /// </summary>
        /// <value>should be handled, outside see page 48, or use hash in calculation &#x3D;&gt; conflict have to be solved</value>
        [DataMember(Name="dataToInsert", EmitDefaultValue=false)]
        public List<SimpleDataObject> DataToInsert { get; set; }

        /// <summary>
        /// both have to be update dataToInsert in their own set (\&quot;recursion anker\&quot;)
        /// </summary>
        /// <value>both have to be update dataToInsert in their own set (\&quot;recursion anker\&quot;)</value>
        [DataMember(Name="handled", EmitDefaultValue=false)]
        public bool? Handled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InsertStep {\n");
            sb.Append("  IdFrom: ").Append(IdFrom).Append("\n");
            sb.Append("  IdNext: ").Append(IdNext).Append("\n");
            sb.Append("  IdTo: ").Append(IdTo).Append("\n");
            sb.Append("  DataToInsert: ").Append(string.Join( ",", DataToInsert.Select(s => s.Id))).Append("\n");
            sb.Append("  Handled: ").Append(Handled).Append("\n");
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
            return this.Equals(input as InsertStep);
        }

        /// <summary>
        /// Returns true if InsertStep instances are equal
        /// </summary>
        /// <param name="input">Instance of InsertStep to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InsertStep input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.IdFrom == input.IdFrom ||
                    (this.IdFrom != null &&
                    this.IdFrom.Equals(input.IdFrom))
                ) && 
                (
                    this.IdNext == input.IdNext ||
                    this.IdNext != null &&
                    input.IdNext != null &&
                    this.IdNext.SequenceEqual(input.IdNext)
                ) && 
                (
                    this.IdTo == input.IdTo ||
                    (this.IdTo != null &&
                    this.IdTo.Equals(input.IdTo))
                ) && 
                (
                    this.DataToInsert == input.DataToInsert ||
                    this.DataToInsert != null &&
                    input.DataToInsert != null &&
                    this.DataToInsert.SequenceEqual(input.DataToInsert)
                ) && 
                (
                    this.Handled == input.Handled ||
                    (this.Handled != null &&
                    this.Handled.Equals(input.Handled))
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
                if (this.IdFrom != null)
                    hashCode = hashCode * 59 + this.IdFrom.GetHashCode();
                if (this.IdNext != null)
                    hashCode = hashCode * 59 + this.IdNext.GetHashCode();
                if (this.IdTo != null)
                    hashCode = hashCode * 59 + this.IdTo.GetHashCode();
                if (this.DataToInsert != null)
                    hashCode = hashCode * 59 + this.DataToInsert.GetHashCode();
                if (this.Handled != null)
                    hashCode = hashCode * 59 + this.Handled.GetHashCode();
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
