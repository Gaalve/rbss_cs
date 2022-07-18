
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Models.RBSS_CS
{
    /// <summary>
    /// DebugInsert
    /// </summary>
    [DataContract]
    public partial class DebugInsert :  IEquatable<DebugInsert>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugInsert" /> class.
        /// </summary>
        [JsonConstructor]
        public DebugInsert(SimpleDataObject[] dataSet)
        {
            DataSet = dataSet;
        }


        [DataMember(Name="dataSet", EmitDefaultValue=false)]
        public SimpleDataObject[] DataSet { get; set; }
        

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DebugInsert {\n");
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
            return this.Equals(input as DebugInsert);
        }

        /// <summary>
        /// Returns true if DebugInsert instances are equal
        /// </summary>
        /// <param name="input">Instance of DebugInsert to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(DebugInsert input)
        {
            if (input == null)
                return false;

            return this.DataSet == input.DataSet;
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
                hashCode = hashCode * 59 + this.DataSet.GetHashCode();
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
