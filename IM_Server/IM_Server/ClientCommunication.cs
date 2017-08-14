using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Web.Script.Serialization;
using System.Web.Helpers;

namespace IM_Server
{
    /// <summary>
    /// Defines the direct custom communication from and to the client.
    /// Messages are recived here, passed to the controller, and then responses are sent here.
    /// </summary>
    public class ClientCommunication : WebSocketBehavior
    {
        /// <summary>
        /// JSON Encode and Decode Serialization Object.
        /// </summary>
        JavaScriptSerializer jss = new JavaScriptSerializer();

        /// <summary>
        /// A standard message listener.
        /// Most messages will appear here.
        /// </summary>
        /// <param name="e">Contains client message data.</param>
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Message received from: " + ID);
            //Dictionary<string, string> decodedData = Json.Decode(e.Data);
            Dictionary<string, string> decodedData = jss.Deserialize<Dictionary<string, string>>(e.Data);
            SendResponses(Program.GetController().ClientMessage(new ClientID(this.ID), decodedData));
        }

        /// <summary>
        /// A connection closed listener.
        /// Only called when the client disconnects for some reason (good or bad).
        /// This will simulate the client sending "logout" to the server.
        /// </summary>
        /// <param name="e">Contains client message data.</param>
        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Client closed connection: " + ID);
            Dictionary<string, string> message = new Dictionary<string, string>();
            message["request"] = "logout";
            SendResponses(Program.GetController().ClientMessage(new ClientID(this.ID), message));
        }

        /// <summary>
        /// An error listener.
        /// Called when a communication mishap occurs.
        /// Does nothing but prints an error notification to the console.
        /// </summary>
        /// <param name="e">Contains client message data.</param>
        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("Client errored: " + ID);
        }

        /// <summary>
        /// A connection establisted listener.
        /// Called when a new client is joining.
        /// Currently sends a connect message to the server, since the client cannot send anything in this stage.
        /// </summary>
        protected override void OnOpen()
        {
            Console.WriteLine("Client openned connection: " + ID);
            Dictionary<string, string> message = new Dictionary<string, string>();
            message["request"] = "connect";
            ControlInterface cont = Program.GetController();
            cont.ClientMessage(new ClientID(this.ID), message);
        }

        /// <summary>
        /// Send all responses received from the controller (in respones to some client message) to each designated sender.
        /// </summary>
        /// <param name="responses">This is a complex, dictionary-list-dictionary that defines all messages to send to all players. See the controller for information on this object.</param>
        private void SendResponses(Dictionary<ClientID, List<Dictionary<string, string>>> responses)
        {
            foreach (ClientID id in responses.Keys) //For every client that has a list of messages ready to send...
            {
                foreach(Dictionary<string, string> msg in responses[id])    //For each message in the single client's list of messages (multiple messages to one client is allowed)...
                {
                    string encodedData = jss.Serialize(msg);          //Encode the message in JSON
                    this.Sessions.SendTo(id.ID, encodedData);       //Send the message to the client's ID
                }
            }
        }
    }
}