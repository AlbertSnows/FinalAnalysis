using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client
{
    public class DataBase
    {
        /// <summary>
        /// User that is currently logged in.
        /// </summary>
        public string CurrentUser;

        /// <summary>
        /// Contains current user's contacts and their online status.
        /// </summary>
        public Dictionary<string, bool> Contacts;

        /// <summary>
        /// Contains chat history for each open chat room.
        /// <room, list<tuple<username, message>>>
        /// </summary>
        public Dictionary<string, List<Tuple<string, string>>> ChatRoomHistory;

        /// <summary>
        /// Contains active users for each open chat room.
        /// </summary>
        public Dictionary<string, List<string>> ChatRoomUsers;

        /// <summary>
        /// Contains list of users that can be added to each open chat room.
        /// </summary>
        public Dictionary<string, List<string>> ChatRoomValidUsers;

        public DataBase()
        {
            CurrentUser = "";
            Contacts = new Dictionary<string, bool>();
            ChatRoomHistory = new Dictionary<string, List<Tuple<string, string>>>();
            ChatRoomUsers = new Dictionary<string, List<string>>();
            ChatRoomValidUsers = new Dictionary<string, List<string>>();
        }
    }
}
