using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Timers;
//using System.Web.Helpers;

namespace IM_Client
{
    public class Controller: InputHandler
    {
        private WebSocket ws;
        private JavaScriptSerializer jss;
        private Dictionary<string, string> data;
        private Regex rgUser;
        private Queue<string> messageQueue;
        private DataBase db;
        private Observer ob;
        private double dequeue_rate = 500; //number of milliseconds before controller attempts to send message from queue;

        public delegate void MessageEventHandler(string message);

        public event MessageEventHandler OnMessage;

        private Timer t;

        public Controller(DataBase db, Observer ob, bool testmode = false)
        {
            ws = new WebSocket("ws://127.0.0.1:8001/chatserver");//this may need to be changed. was copied from example solution
            jss = new JavaScriptSerializer();
            rgUser = new Regex(@"^[a-zA-Z0-9]*$");
            messageQueue = new Queue<string>();
            ws.OnMessage += (sender, e) => { if (OnMessage != null) OnMessage(e.Data); };
            OnMessage += new MessageEventHandler(MessageReceived);
            this.db = db;
            this.ob = ob;
            t = new Timer(dequeue_rate);
            //only start timer if this is not a unit test
            if(!testmode)
            {
                t.Start();
                t.Elapsed += (source, args) => { sendMessageToServer(""); };
            }
            ws.Connect();
        }

        public bool handleLogin(string user, string pw)
        {
            if(!rgUser.IsMatch(user))//may also limit pw chars
            {
                return false;
            }

            data = new Dictionary<string, string>();
            data["request"] = "login";
            data["user"] = user;
            data["password"] = pw;
            sendMessageToServer(jss.Serialize(data));

            return true;
        }

        public bool handleSignup(string user, string pw)
        {
            if (!rgUser.IsMatch(user))//may also limit pw chars
            {
                return false;
            }

            data = new Dictionary<string, string>();
            data["request"] = "signup";
            data["user"] = user;
            data["password"] = pw;
            sendMessageToServer(jss.Serialize(data));

            return true;
        }

        public bool handleAddContact(string user)
        {
            if (!rgUser.IsMatch(user))
            {
                return false;
            }

            data = new Dictionary<string, string>();
            data["request"] = "addcontact";
            data["user"] = user;
            sendMessageToServer(jss.Serialize(data));

            return true;
        }

        public bool handleRemoveContact(string user)
        {
            if (!rgUser.IsMatch(user))
            {
                return false;
            }

            data = new Dictionary<string, string>();
            data["request"] = "removecontact";
            data["user"] = user;
            sendMessageToServer(jss.Serialize(data));

            return true;
        }

        public void handleLogout()
        {
            data = new Dictionary<string, string>();
            data["request"] = "logout";
            sendMessageToServer(jss.Serialize(data));
        }

        public void handleLeaveChat(string room)
        {
            data = new Dictionary<string, string>();
            data["request"] = "leavechat";
            data["room"] = room;
            sendMessageToServer(jss.Serialize(data));
        }

        public bool handleOpenChat(string user)
        {
            if (!rgUser.IsMatch(user))
            {
                return false;
            }

            data = new Dictionary<string, string>();
            data["request"] = "startchat";
            data["user"] = user;
            sendMessageToServer(jss.Serialize(data));

            return true;
        }

        public void handleSendMessage(string room, string message)
        {
            data = new Dictionary<string, string>();
            data["request"] = "sendchat";
            data["room"] = room;
            data["message"] = message;
            sendMessageToServer(jss.Serialize(data));
        }

        public bool handleAddToChat(string room, string user)
        {
            if (!rgUser.IsMatch(user))
            {
                return false;
            }

            data = new Dictionary<string, string>();
            data["request"] = "addtochat";
            data["room"] = room;
            data["user"] = user;
            sendMessageToServer(jss.Serialize(data));

            return true;
        }

        public void MessageReceived(string JsonPacket)
        {
            Dictionary<string, string> message;
            message = jss.Deserialize<Dictionary<string, string>>(JsonPacket);
            string s;

            if(message.TryGetValue("request", out s))
            {
                switch(s)
                {
                    case "login":
                        sLogin(message);
                        break;
                    case "signup":
                        sSignup(message);
                        break;
                    case "addcontact":
                        sAddContact(message);
                        break;
                    case "removecontact":
                        sRemoveContact(message);
                        break;
                    case "contactupdate":
                        sContactUpdate(message);
                        break;
                    case "chatuserleft":
                        sChatUserLeft(message);
                        break;
                    case "startchat":
                        sStartChat(message);
                        break;
                    case "receivechat":
                        sReceiveChat(message);
                        break;
                    case "addtochat":
                        sAddToChat(message);
                        break;
                    case "logout":
                        sLogout(message);
                        break;
                    default:
                        //unrecognised request
                        break;
                }
            }
            else
            {
                //Json packet does not match expected format
            }
        }

        private void sendMessageToServer(string JsonPacket)
        {

            if (!JsonPacket.IsNullOrEmpty())
            {
                messageQueue.Enqueue(JsonPacket);
            }
            
            while(messageQueue.Count > 0 && ws.IsAlive)
            {
                ws.Send(messageQueue.Dequeue());
            }
        }

        /// <summary>
        /// Store contact data in database
        /// </summary>
        /// <param name="s">string representing contact data</param>
        private void storeContactData(string s)
        {
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            if (!s.IsNullOrEmpty())
            {
                foreach (string contact in s.Split('\n'))
                {
                    string[] c = contact.Split('\0');
                    d[c[0]] = c[1].Equals("online") ? true : false;
                }
            }
            db.Contacts = d;
        }

        /// <summary>
        /// Update valid users for a chat room
        /// </summary>
        /// <param name="room">room being updated</param>
        /// <param name="s">string representing valid users for room</param>
        private void storeChatRoomValidUsers(string room, string s)
        {
            List<string> users = new List<string>();
            foreach(string user in s.Split('\0'))
            {
                users.Add(user);
            }
            db.ChatRoomValidUsers[room] = users;
        }


        private void storeChatRoomHistory(string hist)
        {
            if(hist.IsNullOrEmpty())
            {
                db.ChatRoomHistory = new Dictionary<string, List<Tuple<string, string>>>();
                return;
            }
            string[] data = hist.Split('\0');
            for (int i = 0; i < data.Count(); i++)
            {
                string room = data[i];
                string[] messages = data[++i].Split('\n');

                db.ChatRoomHistory[room] = new List<Tuple<string, string>>();

                foreach(string message in messages)
                {
                    int colonLocation = message.IndexOf(':');
                    string user = message.Substring(0, colonLocation);
                    string text = message.Substring(colonLocation + 2, message.Length - colonLocation - 2);
                    db.ChatRoomHistory[room].Add(new Tuple<string, string>(user, text));
                }
            }
        }

        /// <summary>
        /// server has sent login request
        /// </summary>
        /// <param name="m">data sent from server</param>
        private void sLogin(Dictionary<string,string> m)
        {
            string result, data, roomhistory;
            if (m.TryGetValue("result", out result))
            {
                bool r = Boolean.Parse(result);

                if (r) // successful login
                {
                    if(m.TryGetValue("data", out data) && m.TryGetValue("roomhistory", out roomhistory))
                    {
                        storeContactData(data);
                        storeChatRoomHistory(roomhistory);
                        ob.updateLogin(r);
                    }
                    else
                    {
                        //invalid packet
                    }
                }
                else // unsuccessful login
                {
                    ob.updateLogin(r);
                }
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent signup request
        /// </summary>
        /// <param name="m">data from server</param>
        private void sSignup(Dictionary<string, string> m)
        {
            string result;
            if(m.TryGetValue("result", out result))
            {
                bool r = Boolean.Parse(result);
                ob.updateSignup(r);
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent add contact request 
        /// </summary>
        /// <param name="m">data from server</param>
        private void sAddContact(Dictionary<string, string> m)
        {
            string result, data;
            if (m.TryGetValue("result", out result))
            {
                bool r = Boolean.Parse(result);
                if(r)
                {
                    if(m.TryGetValue("data", out data))
                    {
                        storeContactData(data);
                    }
                    else
                    {
                        //invalid packet
                    }
                }
                ob.updateAddContact(r);
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent remove contact request 
        /// </summary>
        /// <param name="m">data from server</param>
        private void sRemoveContact(Dictionary<string, string> m)
        {
            string result, data;
            if (m.TryGetValue("result", out result))
            {
                bool r = Boolean.Parse(result);
                if (r)
                {
                    if (m.TryGetValue("data", out data))
                    {
                        storeContactData(data);
                    }
                    else
                    {
                        //invalid packet
                    }
                }
                ob.updateRemoveContact(r);
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent an update to contact data
        /// </summary>
        /// <param name="m">data from server</param>
        private void sContactUpdate(Dictionary<string, string> m)
        {
            string data;
            if(m.TryGetValue("data", out data))
            {
                storeContactData(data);
                ob.updateContactData();
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent a notification that a user has left a chat room
        /// </summary>
        /// <param name="m">data from server</param>
        private void sChatUserLeft(Dictionary<string, string> m)
        {
            string room, user, validusers;
            if(m.TryGetValue("room", out room) && m.TryGetValue("user", out user) && m.TryGetValue("validusers", out validusers))
            {
                db.ChatRoomValidUsers[room].Remove(user);
                storeChatRoomValidUsers(room, validusers);
                ob.updateChatUserLeft(user, room);
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent a response to a request to open a chat room
        /// </summary>
        /// <param name="m">data from server</param>
        private void sStartChat(Dictionary<string, string> m)
        {
            string result, room, users, validusers;
            if(m.TryGetValue("result", out result))
            {
                bool r = Boolean.Parse(result);
                if (r)
                {
                    if(m.TryGetValue("room", out room) && m.TryGetValue("users", out users) && m.TryGetValue("validusers", out validusers))
                    {
                        db.ChatRoomUsers[room] = new List<string>(users.Split('\0'));
                        db.ChatRoomHistory[room] = new List<Tuple<string, string>>();
                        storeChatRoomValidUsers(room, validusers);
                    }
                    else
                    {
                        //invalid packet
                    }
                }
                ob.updateStartChat(r);
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent a chat message
        /// </summary>
        /// <param name="m">data from server</param>
        private void sReceiveChat(Dictionary<string, string> m)
        {
            string room, user, message;
            if(m.TryGetValue("room", out room) && m.TryGetValue("user", out user) && m.TryGetValue("message", out message))
            {
                db.ChatRoomHistory[room].Add(new Tuple<string, string>(user, message));
            }
        }

        /// <summary>
        /// server has sent a response to request to add a new user to a chat
        /// currently, result should always be false
        /// </summary>
        /// <param name="m">data from server</param>
        private void sAddToChat(Dictionary<string,string> m)
        {
            string result, room, user;
            if (m.TryGetValue("result", out result) && m.TryGetValue("room", out room) && m.TryGetValue("user", out user))
            {
                bool r = Boolean.Parse(result);
                if(r)
                {
                    //should never happen
                }
                else
                {
                    ob.updateAddToChat(r, room, user);
                }
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has sent a notification that a new user has joined a chat room
        /// </summary>
        /// <param name="m">data from server</param>
        private void sChatUserJoined(Dictionary<string, string> m)
        {
            string room, user, validusers;
            if(m.TryGetValue("room", out room) && m.TryGetValue("user", out user) && m.TryGetValue("validusers", out validusers))
            {
                db.ChatRoomUsers[room].Add(user);
                storeChatRoomValidUsers(room, validusers);
            }
            else
            {
                //invalid packet
            }
        }

        /// <summary>
        /// server has forced this client instance to log out.
        /// </summary>
        /// <param name="m">data from server</param>
        private void sLogout(Dictionary<string,string> m)
        {
            string result;
            if(m.TryGetValue("result", out result))
            {
                bool r = Boolean.Parse(result);
                if(r)
                {
                    db = new DataBase();
                    ob.ShowLogonScreen();
                }
                else
                {
                    //should never occur
                }
            }
            else
            {
                //invalid packet
            }
        }
    }
}

