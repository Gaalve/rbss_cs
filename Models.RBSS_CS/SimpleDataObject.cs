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
    /// SimpleDataObject
    /// </summary>
    [DataContract]
        public partial class SimpleDataObject :  IEquatable<SimpleDataObject>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDataObject" /> class.
        /// </summary>
        /// <param name="id">unique identifier.</param>
        /// <param name="additionalProperties">additionalProperties.</param>
        public SimpleDataObject(string id = default(string), Object additionalProperties = default(Object))
        {
            this.Id = id;
            this.AdditionalProperties = additionalProperties;
        }
        
        /// <summary>
        /// unique identifier
        /// </summary>
        /// <value>unique identifier</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets AdditionalProperties
        /// </summary>
        [DataMember(Name="additionalProperties", EmitDefaultValue=false)]
        public Object AdditionalProperties { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SimpleDataObject {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  AdditionalProperties: ").Append(AdditionalProperties).Append("\n");
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
            return this.Equals(input as SimpleDataObject);
        }

        /// <summary>
        /// Returns true if SimpleDataObject instances are equal
        /// </summary>
        /// <param name="input">Instance of SimpleDataObject to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SimpleDataObject input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.AdditionalProperties == input.AdditionalProperties ||
                    (this.AdditionalProperties != null &&
                    this.AdditionalProperties.Equals(input.AdditionalProperties))
                );
        }


        private static int GetStableHash(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                // int hashCode = 41;
                // if (this.Id != null)
                //     hashCode = hashCode * 59 + this.Id.GetHashCode();
                // // if (this.AdditionalProperties != null)
                // //     hashCode = hashCode * 59 + this.AdditionalProperties.GetHashCode();
                // return hashCode;

                return GetStableHash(this.ToJson());
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
