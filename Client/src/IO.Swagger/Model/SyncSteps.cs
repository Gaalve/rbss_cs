/* 
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * OpenAPI spec version: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;

namespace IO.Swagger.Model
{
    /// <summary>
    /// SyncSteps
    /// </summary>
    [DataContract]
        public partial class SyncSteps :  IEquatable<SyncSteps>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncSteps" /> class.
        /// </summary>
        /// <param name="timeSent">timestap packet has been sent.</param>
        /// <param name="steps">steps.</param>
        public SyncSteps(decimal? timeSent = default(decimal?), List<Step> steps = default(List<Step>))
        {
            this.TimeSent = timeSent;
            this.Steps = steps;
        }
        
        /// <summary>
        /// timestap packet has been sent
        /// </summary>
        /// <value>timestap packet has been sent</value>
        [DataMember(Name="timeSent", EmitDefaultValue=false)]
        public decimal? TimeSent { get; set; }

        /// <summary>
        /// Gets or Sets Steps
        /// </summary>
        [DataMember(Name="steps", EmitDefaultValue=false)]
        public List<Step> Steps { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SyncSteps {\n");
            sb.Append("  TimeSent: ").Append(TimeSent).Append("\n");
            sb.Append("  Steps: ").Append(Steps).Append("\n");
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
            return this.Equals(input as SyncSteps);
        }

        /// <summary>
        /// Returns true if SyncSteps instances are equal
        /// </summary>
        /// <param name="input">Instance of SyncSteps to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SyncSteps input)
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
                    this.Steps == input.Steps ||
                    this.Steps != null &&
                    input.Steps != null &&
                    this.Steps.SequenceEqual(input.Steps)
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
                if (this.Steps != null)
                    hashCode = hashCode * 59 + this.Steps.GetHashCode();
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
