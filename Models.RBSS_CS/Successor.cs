/*
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * The version of the OpenAPI document: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Models.RBSS_CS
{
    /// <summary>
    /// Successor
    /// </summary>
    [DataContract(Name = "Successor")]
    public partial class Successor : IEquatable<Successor>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Successor" /> class.
        /// </summary>
        /// <param name="successorIP">IP of the new successor peer.</param>
        public Successor(string successorIP = default(string))
        {
            this.SuccessorIP = successorIP;
        }

        /// <summary>
        /// IP of the new successor peer
        /// </summary>
        /// <value>IP of the new successor peer</value>
        [DataMember(Name = "successorIP", EmitDefaultValue = false)]
        public string SuccessorIP { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Successor {\n");
            sb.Append("  SuccessorIP: ").Append(SuccessorIP).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Successor);
        }

        /// <summary>
        /// Returns true if Successor instances are equal
        /// </summary>
        /// <param name="input">Instance of Successor to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Successor input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.SuccessorIP == input.SuccessorIP ||
                    (this.SuccessorIP != null &&
                    this.SuccessorIP.Equals(input.SuccessorIP))
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
                if (this.SuccessorIP != null)
                {
                    hashCode = (hashCode * 59) + this.SuccessorIP.GetHashCode();
                }
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}