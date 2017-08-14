using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Server
{
    /// <summary>
    /// A protection / simplification class that prevents the mishandling of a client id.
    /// Simply a dummy wrapper for a string.
    /// This defines a ClinetID object to help clarify parameter types in various dictionaries and methods.
    /// </summary>
    public class ClientID
    {
        /// <summary>
        /// The set client's id.
        /// </summary>
        private string id;

        /// <summary>
        /// Get the client's id.
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
        }
        /// <summary>
        /// Create a new client id.
        /// Done usually on login success.
        /// </summary>
        /// <param name="id">The client id received from the client message.</param>
        public ClientID(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// Custom Equals definition to ensure correct identification.
        /// </summary>
        /// <param name="obj">The object to compare equality to.</param>
        /// <returns>True if the second object referes to the same client as this object.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return id == ((ClientID) obj).id;
        }

        /// <summary>
        /// Overridden hashcode definition.
        /// </summary>
        /// <returns>A hashed integer identifying something close to this object.</returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /// <summary>
        /// Custom == definition to ensure correct identification.
        /// </summary>
        /// <param name="id1">The first client id to compare.</param>
        /// <param name="id2">The second client id to compare.</param>
        /// <returns>True if both objects refer to the same client.</returns>
        public static bool operator==(ClientID id1, ClientID id2)
        {
            return id1.id == id2.id;
        }

        /// <summary>
        /// Custom != definition to ensure correct identification.
        /// </summary>
        /// <param name="id1">The first client id to compare.</param>
        /// <param name="id2">The second client id to compare.</param>
        /// <returns>True if the two ids refer to separate clients.</returns>
        public static bool operator!=(ClientID id1, ClientID id2)
        {
            return id1.id != id2.id;
        }
    }
}
