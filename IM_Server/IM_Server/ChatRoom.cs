using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IM_Server
{
    /// <summary>
    /// Keeps track of a chat room where multiple users can send messages to all other users in the room.
    /// Only users who are mutual friends can ever be in one chat room.
    /// Keeps track of history only while the room exists, removed after all users leave the room.
    /// </summary>
    public class ChatRoom
    {
        /// <summary>
        /// The history of the room chat. Strings include the user name and formatting.
        /// </summary>
        private List<string> history;

        /// <summary>
        /// The users (online or offline) in this room.
        /// </summary>
        private List<AbstractUserData> users;

        /// <summary>
        /// The unique ID for this chat room.
        /// Generated on creation, cannot be changed.
        /// </summary>
        private string roomId;

        /// <summary>
        /// Get the unique room id that identifies this room.
        /// </summary>
        /// <returns>A unique string representing this room.</returns>
        public string GetRoomId()
        {
            return roomId;
        }

        /// <summary>
        /// Add the specified user to this room.
        /// This will only succeed if the user is mutual friends with everybody currently in the room, regardless of who invited him.
        /// </summary>
        /// <param name="user">The user to add to the room.</param>
        /// <returns>True if he/she was friends with everyone and was successfully added; false otherwise.</returns>
        public bool AddUser(AbstractUserData user)
        {
            if(GetValidUsers().Contains(user))
            {
                users.Add(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remvoe a user from the room.
        /// They should already be in the room for them to be removed, but has no effect otherwise.
        /// </summary>
        /// <param name="user">The user, currently in this room, that should be removed.</param>
        /// <returns>The number of users still in this room. If this is 1 or less, the room should be removed (and other users notified of its removal, if still in it).</returns>
        public int RemoveUser(AbstractUserData user)
        {
            if(users.Contains(user))
                users.Remove(user);
            return users.Count;
        }

        /// <summary>
        /// Get the list of users (online or offline) that are in this room.
        /// </summary>
        /// <returns>A list of users in this room.</returns>
        public List<AbstractUserData> GetUsers()
        {
            return users.ToList<AbstractUserData>();
        }
        
        /// <summary>
        /// Initialize a new room between two friends.
        /// If the two users identified are not friends, an exception will be thrown.
        /// </summary>
        /// <param name="roomId">A unique ID that should identify this room apart from any other room.</param>
        /// <param name="user1">User 1 to be in this room, who must be friends with user 2.</param>
        /// <param name="user2">User 2 to be in this room, who must be friewds with user 1.</param>
        public ChatRoom(string roomId, AbstractUserData user1, AbstractUserData user2)
        {
            this.roomId = roomId;
            history = new List<string>();
            users = new List<AbstractUserData>();

            if (!user1.GetContacts().Contains(user2))
            {
                throw new Exception("User's are not compatible! Cannot create room!");
            }
            else
            {
                users.Add(user1);
                users.Add(user2);
            }
        }

        /// <summary>
        /// Check if a user is in this room.
        /// </summary>
        /// <param name="user">The user to check for, if he/she are in this room.</param>
        /// <returns>True if he/she is; false otherwise.</returns>
        public bool ContainsUser(AbstractUserData user)
        {
            return users.Contains(user);
        }

        /// <summary>
        /// Get a specially formatted string that represents all the users currently in this room.
        /// The format of this string is defined in the IM Team 7 API.
        /// user1\0user2\0user3\0user4
        /// </summary>
        /// <returns>A string that can identify all users in this room.</returns>
        public string GetUserString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (AbstractUserData user in users)
            {
                builder.Append(user.GetName());
                builder.Append("\0");
            }
            if(builder.Length >= 1)
                builder.Length--;
            return builder.ToString();
        }

        /// <summary>
        /// Get a specially formatted string that represents the chat history of this room.
        /// The format of this string is defined in the IM Team 7 API.
        /// roomid\0line1\nline2\nline3\nline4
        /// </summary>
        /// <returns>A string that can identify the history of this room.</returns>
        public string GetRoomHistory()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(roomId);
            builder.Append("\0");
            foreach(String line in history)
            {
                builder.Append(line);
                builder.Append("\n");
            }
            return builder.ToString();
        }

        public void RecordChatLine(string line)
        {
            history.Add(line);
        }

        /// <summary>
        /// Get a specially formatted string that represents the valid users that can join this room.
        /// All users in this string are mutual friends with everybody in the room currently.
        /// The format of this string is defined in the IM Team 7 API.
        /// user1\0user2\0user3\0user4
        /// </summary>
        /// <returns>A string that can identfy the valid users that can be added to this room.</returns>
        public string GetValidUsersString()
        {
            List<AbstractUserData> users = GetValidUsers();
            StringBuilder builder = new StringBuilder();
            foreach(AbstractUserData user in users)
            {
                builder.Append(user.GetName());
                builder.Append("\0");
            }
            if (builder.Length >= 1)
                builder.Length--;
            return builder.ToString();
        }

        /// <summary>
        /// Get a list of valid users that can be added to this room.
        /// All users in this list are friends with every other user, therefore, they are valid to join this room.
        /// </summary>
        /// <returns>A list of users that can be added to this room.</returns>
        public List<AbstractUserData> GetValidUsers()
        {
            if(users.Count == 0)
            {
                //remove the room?    
                return new List<AbstractUserData>();
            }
            else
            {
                AbstractUserData user = users[0];
                Dictionary<AbstractUserData, int> validUsers = new Dictionary<AbstractUserData, int>();
                foreach(AbstractUserData contact in user.GetContacts())
                {
                    //Get all the contacts from the original user. Set their occurrance count to 1.
                    if(contact.GetContacts().Contains(user))
                        validUsers[contact] = 1;
                }
                //Loop through each user who will limit the contact count in this room.
                foreach (AbstractUserData limiter in users)
                {
                    //Loop through each of the limiter's contacts.
                    foreach(AbstractUserData contact in limiter.GetContacts())
                    {
                        //If the contact exists in the original user's contact list, increase it's occurrance count.
                        if(validUsers.ContainsKey(contact) && contact.GetContacts().Contains(limiter))
                        {
                            validUsers[contact]++;
                        }
                    }
                }
                //Now only the contacts in validUsers that occur users.Count + 1 times are on everybody's list. They are the only valid users.
                for(int i = 0; i < validUsers.Count; i++)
                {
                    if(validUsers[validUsers.Keys.ElementAt(i)] != (users.Count + 1))
                    {
                        validUsers.Remove(validUsers.Keys.ElementAt(i));
                        i--;
                    }
                }
                //The remaining keys are the ones that everybody has.
                return validUsers.Keys.ToList<AbstractUserData>();
            }
        }
    }
}
