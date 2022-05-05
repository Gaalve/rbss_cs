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
    public class ValidateStep : AbstractStep, IEquatable<ValidateStep>
    {

        public ValidateStep(string idFrom = default(string), string idTo = default(string), string fpOfData = default(string))
        {
            this.IdFrom = idFrom;
            this.IdTo = idTo;
            this.FpOfData = fpOfData;
        }
        // /// <summary>
        // /// Gets or Sets IdFrom
        // /// </summary>
        // [DataMember(Name="idFrom", EmitDefaultValue=false)]
        // public string IdFrom { get; set; }
        //
        // /// <summary>
        // /// Gets or Sets IdTo
        // /// </summary>
        // [DataMember(Name="idTo", EmitDefaultValue=false)]
        // public string IdTo { get; set; }

        /// <summary>
        /// Gets or Sets FpOfData
        /// </summary>
        [DataMember(Name="fpOfData", EmitDefaultValue=false)]
        public string FpOfData { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ValidateStep {\n");
            sb.Append("  IdFrom: ").Append(IdFrom).Append("\n");
            sb.Append("  IdTo: ").Append(IdTo).Append("\n");
            sb.Append("  FpOfData: ").Append(FpOfData).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ValidateStep)obj);
        }

        /// <summary>
        /// Returns true if ValidateStep instances are equal
        /// </summary>
        /// <param name="other">Instance of ValidateStep to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ValidateStep other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    IdFrom == other.IdFrom ||
                    IdFrom != null &&
                    IdFrom.Equals(other.IdFrom)
                ) && 
                (
                    IdTo == other.IdTo ||
                    IdTo != null &&
                    IdTo.Equals(other.IdTo)
                ) && 
                (
                    FpOfData == other.FpOfData ||
                    FpOfData != null &&
                    FpOfData.Equals(other.FpOfData)
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
                    if (IdFrom != null)
                    hashCode = hashCode * 59 + IdFrom.GetHashCode();
                    if (IdTo != null)
                    hashCode = hashCode * 59 + IdTo.GetHashCode();
                    if (FpOfData != null)
                    hashCode = hashCode * 59 + FpOfData.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(ValidateStep left, ValidateStep right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValidateStep left, ValidateStep right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
