using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace IM_Server
{
    /// <summary>
    /// Primary controller for the IM server.
    /// Manages the communication between 
    /// </summary>
    public class Controller : ControlInterface
    {
        /// <summary>
        /// List of all registered users (online/offline), matching their username to their UserData object.
        /// </summary>
        private Dictionary<string, UserData> users;

        /// <summary>
        /// List of all rooms, matching the unique room id's to their ChatRoom objects.
        /// </summary>
        private Dictionary<string, ChatRoom> rooms;

        /// <summary>
        /// Get a username from a registered connected client id.
        /// Only contains online and logged in users.
        /// </summary>
        private Dictionary<ClientID, string> idToUser;

        /// <summary>
        /// Get a registed connected client id from a username.
        /// Only contains online and logged in users.
        /// </summary>
        private Dictionary<string, ClientID> userToId;

        /// <summary>
        /// Web server used to communicate with clients.
        /// </summary>
        private WebSocketServer wss;

        /// <summary>
        /// Construct a new controller.
        /// Does not start the server, only prepares it.
        /// </summary>
        public Controller()
        {
            users = new Dictionary<string, UserData>();
            rooms = new Dictionary<string, ChatRoom>();
            idToUser = new Dictionary<ClientID, string>();
            userToId = new Dictionary<string, ClientID>();
            wss = new WebSocketServer(8001);
            wss.AddWebSocketService<ClientCommunication>("/chatserver");
        }

        /// <summary>
        /// Start the server. Synchronous call which will not return until
        /// the server is stopped by the command line (pressing enter).
        /// </summary>
        public void Run()
        {
            wss.Start();
            Console.WriteLine("Press enter to close the server:");
            Console.ReadLine();
            wss.Stop();
        }

        /// <summary>
        /// Message handler that the client communication threads will call.
        /// Will manage the request, one at a time, and send out responses by returning a complex dictionary of data.
        /// </summary>
        /// <param name="ID">The ID of the sending client. Can be accessed by use of the userToId or idToUser dictionaries.</param>
        /// <param name="message">The message from the client, where, the keys are the types of information being passed, and the values of each key is the specific data for that information type.</param>
        /// <returns>The returning double-dictionary is a complex structure containing all information to send to all clients reguarding this message.
        /// The top level keys are the ID's of the clients to send messages to.
        /// The values of those keys are the dictionary of messages to send to that specific client.
        /// That dictionary contains info type as keys and info data as values.</returns>
        public Dictionary<ClientID, List<Dictionary<string, string>>> ClientMessage(ClientID ID, Dictionary<string, string> message)
        {
            lock(this) {
            Dictionary<ClientID, List<Dictionary<string, string>>> responses = new Dictionary<ClientID, List<Dictionary<string, string>>>();
            UserData user = null;
            if (idToUser.ContainsKey(ID))
            {
                users.TryGetValue(idToUser[ID], out user);  //Will not work when client is trying to connect, login, or signup
            }
            ///Message is invalid if
            ///  doesn't contain a request key
            ///  user cannot be identified and user isn't connecting nor signing up (trying to do something while not valid)
            ///  user is not signed in (online) and isn't connecting nor logging in (trying to do something else while not logged in)  

            string request;
            message.TryGetValue("request", out request);

            if ((request == null) ||
                (user == null && !(request == "signup" || request == "connect" || request == "login")) ||
                ((user != null && !user.IsOnline()) && !(request == "connect" || request == "login")))
            {
                if (request != null)
                    AddErrorMessage(ref responses, ID, request);
            }
            else
            {
                switch (message["request"])
                {
                    case "connect":
                        //Do nothing for now. We may not need this.
                        break;
                    case "login":
                        {   //review: good      verified: notyet        tested: notyet
                            string name, pass;
                            if (message.TryGetValue("user", out name)
                                && message.TryGetValue("password", out pass)
                                && users.ContainsKey(name)
                                && users[name].CheckPassword(pass)
                                && (!users[name].IsOnline()))
                            {
                                user = users[name];

                                user.SetOnline(true);
                                idToUser[ID] = name;
                                userToId[name] = ID;

                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "login";
                                resp["result"] = "true";
                                resp["data"] = user.GetContactString();
                                resp["roomhistory"] = GetRoomHistory(user);
                                AddMessageToResponses(ref responses, resp, ID);
                                AddContactDatas(ref responses, user);
                            }
                            else
                            {
                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "login";
                                resp["result"] = "false";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            break;
                        }
                    case "signup":
                        {   //review: good      verified: notyet        tested: notyet
                            string name, pass;
                            if (message.TryGetValue("user", out name)
                                && message.TryGetValue("password", out pass)
                                && (!users.ContainsKey(name)))
                            {
                                user = new UserData(name, pass);
                                users[name] = user;
                                if (user.IsOnline())
                                {
                                    userToId[name] = ID;
                                    idToUser[ID] = name;
                                }

                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "signup";
                                resp["result"] = "true";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            else
                            {
                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "signup";
                                resp["result"] = "false";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            break;
                        }
                    case "addcontact":
                        {   //review: good      verified: notyet        tested: notyet
                            string targetUser;
                            if (message.TryGetValue("user", out targetUser)
                                && users.ContainsKey(targetUser)
                                && user.AddContact(users[targetUser]))
                            {
                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "addcontact";
                                resp["result"] = "true";
                                resp["data"] = user.GetContactString();
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            else
                            {
                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "addcontact";
                                resp["result"] = "false";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            break;
                        }
                    case "removecontact":
                        {   //review: good      verified: notyet        tested: notyet
                            Dictionary<string, string> resp = new Dictionary<string, string>();
                            string targetUser;
                            if (message.TryGetValue("user", out targetUser)
                                && users.ContainsKey(targetUser)
                                && user.RemoveContact(users[targetUser])
                                )
                            {
                                AbstractUserData userToRemove = users[targetUser];
                                foreach (string key in rooms.Keys)
                                {
                                    ChatRoom currentRoom = rooms[key];
                                    if (currentRoom.ContainsUser(user) && currentRoom.ContainsUser(userToRemove))
                                    {
                                        RemoveUserFromRoom(ref responses, currentRoom, user);
                                    }
                                }
                                resp["request"] = "removecontact";
                                resp["result"] = "true";
                                resp["data"] = user.GetContactString();
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            else
                            {
                                resp["request"] = "removecontact";
                                resp["result"] = "false";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            break;
                        }
                    case "logout":
                        {   //review: good      verified: notyet        tested: notyet
                            Dictionary<string, string> resp = new Dictionary<string, string>();
                            user.SetOnline(false);

                            AddContactDatas(ref responses, user);

                            resp["request"] = "logout";
                            resp["result"] = "true";
                            AddMessageToResponses(ref responses, resp, ID);

                            userToId.Remove(idToUser[ID]);
                            idToUser.Remove(ID);
                            break;
                        }
                    case "leavechat":
                        {   //review: good      verified: notyet        tested: notyet
                            string roomId;
                            if (message.TryGetValue("room", out roomId)
                                && rooms[roomId].ContainsUser(user))
                            {
                                RemoveUserFromRoom(ref responses, rooms[roomId], user);
                            }
                            break;
                        }
                    case "startchat":
                        {   //review: good      verified: notyet        tested: notyet
                            string targetUser;
                            if (message.TryGetValue("user", out targetUser)
                                && users.ContainsKey(targetUser)
                                && user.GetContacts().Contains(users[targetUser])
                                && users[targetUser].GetContacts().Contains(user))
                            {
                                UserData user2 = users[targetUser];
                                ChatRoom room = new ChatRoom(GetUniqueRoomId(), user, user2);
                                rooms[room.GetRoomId()] = room;
                                Dictionary<string, string> resp1 = new Dictionary<string, string>();
                                Dictionary<string, string> resp2 = new Dictionary<string, string>();

                                resp1["request"] = "startchat";
                                resp1["result"] = "true";
                                resp1["room"] = room.GetRoomId();
                                resp1["users"] = room.GetUserString();
                                resp1["validusers"] = room.GetValidUsersString();
                                resp1["history"] = "";
                                AddMessageToResponses(ref responses, resp1, ID);

                                if (user2.IsOnline())
                                {
                                    resp2["request"] = "startchat";
                                    resp2["result"] = "true";
                                    resp2["room"] = room.GetRoomId();
                                    resp2["users"] = room.GetUserString();
                                    resp2["validusers"] = room.GetValidUsersString();
                                    resp2["history"] = "";
                                    AddMessageToResponses(ref responses, resp2, userToId[targetUser]);
                                }
                            }
                            else
                            {
                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "startchat";
                                resp["result"] = "false";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            break;
                        }
                    case "sendchat":
                        {   //review: good      verified: notyet        tested: notyet
                            string userMessage, roomId;
                            if (message.TryGetValue("room", out roomId)
                                && message.TryGetValue("message", out userMessage)
                                && rooms[roomId].ContainsUser(user))
                            {
                                ChatRoom room = rooms[roomId];
                                room.RecordChatLine(user.GetName() + ":" + userMessage);

                                Dictionary<string, string> resp = new Dictionary<string, string>();
                                resp["request"] = "receivechat";
                                resp["room"] = room.GetRoomId();
                                resp["user"] = user.GetName();
                                resp["message"] = userMessage;

                                foreach (AbstractUserData chatPerson in room.GetUsers())
                                {
                                    if(chatPerson.IsOnline())
                                        AddMessageToResponses(ref responses, resp, userToId[chatPerson.GetName()]);
                                }
                            }
                            break;
                        }
                    case "addtochat":
                        {   //review: good      verified: notyet        tested: notyet
                            Dictionary<string, string> resp = new Dictionary<string, string>();
                            if (message.ContainsKey("room")                                                 //contains room message
                                && message.ContainsKey("user")                                             //contains user message
                                && rooms.ContainsKey(message["room"])                                      //room exists
                                && users.ContainsKey(message["user"])                                      //user exists
                                && rooms[message["room"]].AddUser(users[message["user"]]))                          //user is in the room's valid users list and successfully added to the room
                            {
                                ChatRoom room = rooms[message["room"]];
                                UserData targetUser = users[message["user"]];

                                ///Tell all the current users in the room that a new person has arrived.
                                ///They only need to know his name and the new valid users list.
                                resp["request"] = "chatuserjoined";
                                resp["room"] = room.GetRoomId();
                                resp["user"] = targetUser.GetName();
                                resp["validusers"] = room.GetValidUsersString();
                                foreach (AbstractUserData chatPerson in room.GetUsers())
                                {
                                    ///Skip the target user that was just added.
                                    ///They also have to be online for them to be notified.
                                    if (chatPerson.IsOnline() && chatPerson != targetUser)
                                    {
                                        AddMessageToResponses(ref responses, resp, userToId[chatPerson.GetName()]);
                                    }
                                }
                                ///If the target user that was added is online, tell them to join.
                                if (targetUser.IsOnline())
                                {
                                    resp = new Dictionary<string, string>();
                                    resp["request"] = "startchat";
                                    resp["room"] = room.GetRoomId();
                                    resp["users"] = room.GetUserString();
                                    resp["validusers"] = room.GetValidUsersString();
                                    resp["history"] = room.GetRoomHistory();
                                    AddMessageToResponses(ref responses, resp, userToId[targetUser.GetName()]);
                                }
                            }
                            else
                            {
                                resp["request"] = "addtochat";
                                resp["room"] = message["room"];
                                resp["user"] = message["user"];
                                resp["result"] = "false";
                                AddMessageToResponses(ref responses, resp, ID);
                            }
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid request input from client: '" + message["request"] + "'. Ignored.");
                            break;
                        }
                }
            }
            return responses;
            }
        }

        /// <summary>
        /// Add a message to the responses to each of the users left in the ChatRoom that the specified user has left the room.
        /// The userToRemove should be removed from the room before this is called.
        /// </summary>
        /// <param name="responses">Reference to the responses object.</param>
        /// <param name="room">Room from which the user left.</param>
        /// <param name="userToRemove">The user that left the room.</param>
        private void AddChatUserLeft(ref Dictionary<ClientID, List<Dictionary<string, string>>> responses, ChatRoom room, AbstractUserData userToRemove)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();
            resp["request"] = "chatuserleft";
            resp["room"] = room.GetRoomId();
            resp["user"] = userToRemove.GetName();
            resp["validusers"] = room.GetValidUsersString();
            foreach (AbstractUserData user in room.GetUsers())
            {
                AddMessageToResponses(ref responses, resp, userToId[user.GetName()]);
            }
            AddMessageToResponses(ref responses, resp, userToId[userToRemove.GetName()]);
        }

        /// <summary>
        /// Remove the provided user from the room.
        /// If there is only one user left in the room, they are removed as well, and the room is removed.
        /// </summary>
        /// <param name="responses">Reference to the responses object.</param>
        /// <param name="room">Room from which the user should be removed.</param>
        /// <param name="userToRemove">THe user that is set to be removed from the room.</param>
        private void RemoveUserFromRoom(ref Dictionary<ClientID, List<Dictionary<string, string>>> responses, ChatRoom room, AbstractUserData userToRemove)
        {
            int numLeft = room.RemoveUser(userToRemove);
            if(numLeft <= 1)
            {
                //need to remove the other user, and notify them both, and delete the room
                AbstractUserData secondUser = room.GetUsers()[0];
                room.RemoveUser(secondUser);

                AddChatUserLeft(ref responses, room, userToRemove);
                AddChatUserLeft(ref responses, room, secondUser);

                rooms.Remove(room.GetRoomId());
            }
            else
            {
                //notify everybody left in the room
                AddChatUserLeft(ref responses, room, userToRemove);
            }
        }

        /// <summary>
        /// Get a unique room id for creation of a new room.
        /// Guaranteed to not be a duplicate of any rooms in the rooms list.
        /// </summary>
        /// <returns>The new unique ID.</returns>
        private string GetUniqueRoomId()
        {
            Random ran = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            do
            {
                builder.Clear();
                for (int i = 0; i < 12; i++)
                {
                    builder.Append(chars[ran.Next(chars.Length)]);
                }
            } while (rooms.ContainsKey(builder.ToString()));
            return builder.ToString();
        }
        /// <summary>
        /// Sends new contact data to all users related to (friends with) the given related user.
        /// Should be called whenever a user's status changes.
        /// </summary>
        /// <param name="responses">Reference to the responses object.</param>
        /// <param name="relatedToUser">The user to find relations to.</param>
        private void AddContactDatas(ref Dictionary<ClientID, List<Dictionary<string, string>>> responses, AbstractUserData relatedToUser)
        {
            foreach (String username in users.Keys)
            {
                if (users[username] != relatedToUser
                    && users[username].IsOnline()
                    && users[username].GetContacts().Contains(relatedToUser))
                {
                    Dictionary<string, string> response = new Dictionary<string, string>();
                    response["request"] = "contactupdate";
                    response["data"] = users[username].GetContactString();

                    AddMessageToResponses(ref responses, response, userToId[username]);
                }
            }
        }
        /// <summary>
        /// Returns a specially formatted room history for a specific user.
        /// Provies information on the rooms a user is part of, as well as the recent history for those rooms.
        /// The string format is defined in the IM Team 7 API.
        /// </summary>
        /// <param name="user">The user to get the room history for.</param>
        /// <returns>The specially formatted room string.</returns>
        private string GetRoomHistory(AbstractUserData user)
        {
            StringBuilder builder = new StringBuilder();
            foreach (String key in rooms.Keys)
            {
                if (rooms[key].ContainsUser(user))
                {
                    builder.Append(rooms[key].GetRoomHistory());
                    builder.Append("\0");
                }
            }
            if (builder.Length >= 2)
            {
                builder.Length -= 2;
            }
            return builder.ToString();
        }
        /// <summary>
        /// Add a message to be sent to the given user ID.
        /// </summary>
        /// <param name="responses">Reference to the responses object.</param>
        /// <param name="message">The message to send to the user.</param>
        /// <param name="ID">The user's current client ID.</param>
        private void AddMessageToResponses(ref Dictionary<ClientID, List<Dictionary<string, string>>> responses, Dictionary<string, string> message, ClientID ID)
        {
            if (!responses.ContainsKey(ID))
            {
                responses[ID] = new List<Dictionary<string, string>>();
            }

            responses[ID].Add(message);
        }

        /// <summary>
        /// Send an error message to a user incase something goes wrong.
        /// </summary>
        /// <param name="responses">Reference to the responses object.</param>
        /// <param name="ID">The user's current client ID to send the error to.</param>
        /// <param name="request">The request the error occurred on.</param>
        private void AddErrorMessage(ref Dictionary<ClientID, List<Dictionary<string, string>>> responses, ClientID ID, string request)
        {
            Console.WriteLine("Sending error message in response to a '" + request + "'.");
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["request"] = request;
            response["result"] = "false";
            AddMessageToResponses(ref responses, response, ID);
        }
    }
}
