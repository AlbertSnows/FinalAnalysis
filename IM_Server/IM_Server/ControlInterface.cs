using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IM_Server
{
    /// <summary>
    /// Protection interface for the View (the client IO) so it can notify the Controller of a message and get the responses that it should send.
    /// </summary>
    public interface ControlInterface
    {
        /// <summary>
        /// Primary controlling method that determines what messages should be sent out based on a client's incomming message.
        /// This operates as the communication layer between the server data and the client's requests.
        /// </summary>
        
        /// <param name="ID">The ID of the client that has sent this message.</param>
        
        /// <param name="message">The decoded message from the client (using JSON). 
        /// Each string key is a type of message being sent (such as "request" or "user"),
        /// while each values related to those keys is the actual value (such as "login" or "myawesomusername").</param>

        /// <returns>The messages to send out to various clients (not just the sender). This complex object has 3 parts:
        /// (1) The initial Dictionary has keys which are ClientIDs. If a message (or set of messages) are to be sent,
        /// the ClientID for the recipient has a key in this Dictionary.
        /// (2) For each key (ClientID - client to send to), there is a List of messages to send them. In some cases, multiple
        /// messages will have to be sent to one user. Each message is one item in this List.
        /// (3) Each item in the list is a Dictionary with string keys and values. Same as the client message (incoming),
        /// the keys refer to the type of message ("request", "result", "user", etc) and the values of those keys refer
        /// to the actual identifying information ("login", "false", "myawesomeusername", etc).</returns>
        Dictionary<ClientID, List<Dictionary<string, string>>> ClientMessage(ClientID ID, Dictionary<string, string> message);
    }
}
