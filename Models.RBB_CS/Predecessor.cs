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

namespace Models.RBB_CS
{
    /// <summary>
    /// Predecessor
    /// </summary>
    [DataContract(Name = "Predecessor")]
    public partial class Predecessor : IEquatable<Predecessor>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predecessor" /> class.
        /// </summary>
        /// <param name="predecessorIP">IP of the new predeccessor peer.</param>
        public Predecessor(string predecessorIP = default(string))
        {
            this.PredecessorIP = predecessorIP;
        }

        /// <summary>
        /// IP of the new predeccessor peer
        /// </summary>
        /// <value>IP of the new predeccessor peer</value>
        [DataMember(Name = "predecessorIP", EmitDefaultValue = false)]
        public string PredecessorIP { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Predecessor {\n");
            sb.Append("  PredecessorIP: ").Append(PredecessorIP).Append("\n");
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
            return this.Equals(input as Predecessor);
        }

        /// <summary>
        /// Returns true if Predecessor instances are equal
        /// </summary>
        /// <param name="input">Instance of Predecessor to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Predecessor input)
        {
            if (input == null)
            {
                return false;
            }
            return 
                (
                    this.PredecessorIP == input.PredecessorIP ||
                    (this.PredecessorIP != null &&
                    this.PredecessorIP.Equals(input.PredecessorIP))
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
                if (this.PredecessorIP != null)
                {
                    hashCode = (hashCode * 59) + this.PredecessorIP.GetHashCode();
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
