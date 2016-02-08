using Projector.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.DDD
{
    /// <summary>
    /// Base Entity class from which all the entities should inherit, it will have its
    /// key defined inside the variable of type K that accepts as a generic. This variable can be
    /// of any type, even a custom type to define complex keys
    /// </summary>
    /// <typeparam name="K">Type of the entity key</typeparam>
    public abstract class BaseEntity<K> : IKeyedItem<K>,IEquatable<BaseEntity<K>> where K : struct
    {
        /// <summary>
        /// Prime number used to calculate Hash Code
        /// </summary>
        protected const int HashMultiplier = 31;

        /// <summary>
        /// To avoid excessive calculations we will cache the hash codes
        /// </summary>
        protected int? cachedHashcode;

        private K id = Activator.CreateInstance<K>();

        /// <summary>
        /// Key of the entity, will hold default value until assigned. Attempting to change the Id once
        /// it has been set will result in an InvalidOperationException being thrown
        /// </summary>
        public virtual K Id
        {
            get
            {
                return id;
            }
            set
            {
                if (!id.Equals(value))
                {
                    if (!id.Equals(default(K)))
                    {
                        throw new InvalidOperationException(message: "You cannot change the entity key once it has been set");
                    }

                    id = value;
                }
            }
        }

        /// <summary>
        /// Overriden base == operator to make it be key based instead of reference based
        /// </summary>
        /// <param name="baseEntityObject1">Left side entity</param>
        /// <param name="baseEntityObject2">Right side entity</param>
        /// <returns>true if equal, false if not</returns>
        public static bool operator ==(BaseEntity<K> baseEntityObject1, BaseEntity<K> baseEntityObject2)
        {
            if ((object)baseEntityObject1 == null)
            {
                return (object)baseEntityObject2 == null;
            }

            return baseEntityObject1.Equals(baseEntityObject2);
        }

        /// <summary>
        /// Overriden base != operator to make it be key based instead of reference based
        /// </summary>
        /// <param name="baseEntityObject1">Left side entity</param>
        /// <param name="baseEntityObject2">Right side entity</param>
        /// <returns>false if equal, true if not</returns>
        public static bool operator !=(BaseEntity<K> baseEntityObject1, BaseEntity<K> baseEntityObject2)
        {
            return !(baseEntityObject1 == baseEntityObject2);
        }

        /// <summary>
        /// Overriden base equality to make it be key based instead of reference based
        /// </summary>
        /// <param name="obj">The object to compare the instance against</param>
        /// <returns>true if equal, false if not</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            { 
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return Equals(obj as BaseEntity<K>);
        }

        /// <summary>
        /// IEquatable member, provides a method to compare objects of this class
        /// </summary>
        /// <param name="other">The object to compare the instance against</param>
        /// <returns>true if equal, false if not</returns>
        public bool Equals(BaseEntity<K> other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            if(this.GetHashCode() != other.GetHashCode())
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        /// <summary>
        /// Overriden Hash Code method to take into account type and Id Properties
        /// </summary>
        /// <returns>The hash code as an int</returns>
        public override int GetHashCode()
        {
            if (this.cachedHashcode.HasValue)
            {
                return this.cachedHashcode.Value;
            }
            unchecked
            {
                // It's possible for two objects to return the same hash code based on 
                // identically valued properties, even if they're of two different types, 
                // so we include the object's type in the hash calculation
                int hashCode = this.GetType().GetHashCode() * Id.GetHashCode() * HashMultiplier;

                this.cachedHashcode = hashCode;
            }

            return this.cachedHashcode.Value;
        }

    }
}
