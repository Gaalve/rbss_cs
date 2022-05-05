/*
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * The version of the OpenAPI document: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.OpenAPIToolsServer.Converters;

namespace Org.OpenAPIToolsServer.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Step : IEquatable<Step>
    {
        /// <summary>
        /// internal identifier for step sended
        /// </summary>
        /// <value>internal identifier for step sended</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or Sets CurrentStep
        /// </summary>
        [DataMember(Name="currentStep", EmitDefaultValue=true)]
        public OneOfValidateStepInsertStep CurrentStep { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Step {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  CurrentStep: ").Append(CurrentStep).Append("\n");
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
            return obj.GetType() == GetType() && Equals((Step)obj);
        }

        /// <summary>
        /// Returns true if Step instances are equal
        /// </summary>
        /// <param name="other">Instance of Step to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Step other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Id == other.Id ||
                    
                    Id.Equals(other.Id)
                ) && 
                (
                    CurrentStep == other.CurrentStep ||
                    CurrentStep != null &&
                    CurrentStep.Equals(other.CurrentStep)
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
                    
                    hashCode = hashCode * 59 + Id.GetHashCode();
                    if (CurrentStep != null)
                    hashCode = hashCode * 59 + CurrentStep.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(Step left, Step right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Step left, Step right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
