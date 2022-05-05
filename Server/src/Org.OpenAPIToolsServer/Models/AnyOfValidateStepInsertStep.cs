﻿/*
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * The version of the OpenAPI document: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Runtime.Serialization;

namespace Org.OpenAPIToolsServer.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class AnyOfValidateStepInsertStep : IEquatable<AnyOfValidateStepInsertStep>
    {
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        [DataMember(Name="step", EmitDefaultValue=true)]
        public object Step { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            return Step.ToString();
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
            return obj.GetType() == GetType() && Equals((AnyOfValidateStepInsertStep)obj);
        }

        /// <summary>
        /// Returns true if Step instances are equal
        /// </summary>
        /// <param name="other">Instance of Step to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AnyOfValidateStepInsertStep other)
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
    }
}