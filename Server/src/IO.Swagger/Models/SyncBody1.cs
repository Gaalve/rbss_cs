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
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace IO.Swagger.Server.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class SyncBody1 : IEquatable<SyncBody1>
    { 
        /// <summary>
        /// The Id from what the sync process may start
        /// </summary>
        /// <value>The Id from what the sync process may start</value>

        [DataMember(Name="idFrom")]
        public string IdFrom { get; set; }

        /// <summary>
        /// The Id latest the sync process have to work
        /// </summary>
        /// <value>The Id latest the sync process have to work</value>

        [DataMember(Name="idNext")]
        public string IdNext { get; set; }

        /// <summary>
        /// The fingerprint for the data available
        /// </summary>
        /// <value>The fingerprint for the data available</value>

        [DataMember(Name="fpOfData")]
        public string FpOfData { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SyncBody1 {\n");
            sb.Append("  IdFrom: ").Append(IdFrom).Append("\n");
            sb.Append("  IdNext: ").Append(IdNext).Append("\n");
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
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SyncBody1)obj);
        }

        /// <summary>
        /// Returns true if SyncBody1 instances are equal
        /// </summary>
        /// <param name="other">Instance of SyncBody1 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SyncBody1 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    IdFrom == other.IdFrom ||
                    IdFrom != null &&
                    IdFrom.Equals(other.IdFrom)
                ) && 
                (
                    IdNext == other.IdNext ||
                    IdNext != null &&
                    IdNext.Equals(other.IdNext)
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
                    if (IdNext != null)
                    hashCode = hashCode * 59 + IdNext.GetHashCode();
                    if (FpOfData != null)
                    hashCode = hashCode * 59 + FpOfData.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(SyncBody1 left, SyncBody1 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SyncBody1 left, SyncBody1 right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
