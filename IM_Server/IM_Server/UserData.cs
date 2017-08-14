using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IM_Server
{
    /// <summary>
    /// Represents a registered user and various information to be stored for him/her.
    /// </summary>
    public class UserData : AbstractUserData
    {
        /// <summary>
        /// The name of the user.
        /// This is unique, and one name matches to exactly one UserData.
        /// </summary>
        private string name;

        /// <summary>
        /// Online/offline status of the user.
        /// True if online.
        /// </summary>
        private bool online;

        /// <summary>
        /// List of contacts this user has friended (and can make chat rooms with).
        /// </summary>
        private List<UserData> contacts;

        /// <summary>
        /// Password for this user. Cannot be changed once account is created.
        /// Required to match exactly on login.
        /// </summary>
        private string password;

        /// <summary>
        /// Get the unique identifying name for this user.
        /// </summary>
        /// <returns>Name of this user.</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Get the online status of this user.
        /// </summary>
        /// <returns>True if online; false otherwise.</returns>
        public bool IsOnline()
        {
            return online;
        }

        /// <summary>
        /// Set the online status of this user.
        /// </summary>
        /// <param name="isOnline">Provide true to mark the user online; false to mark offline.</param>
        public void SetOnline(bool isOnline)
        {
            online = isOnline;
        }

        /// <summary>
        /// Check if the password provided is exactly equal to this user's password.
        /// Used in login operations.
        /// </summary>
        /// <param name="pass">The password to check against the real one.</param>
        /// <returns>True if passwords match and login is permitted; false otherwise.</returns>
        public bool CheckPassword(string pass)
        {
            return password.Equals(pass);
        }

        /// <summary>
        /// Get a uniquely formatted string representing the online status of all the users in this user's contact list.
        /// The format for this string is defined in the IM Team 7 API.
        /// username1\nonline\0username2\noffline\0username3\noffline\0username4\nonline
        /// </summary>
        /// <returns>The formatted string representing users and their online/offline status.</returns>
        public string GetContactString()
        {
            StringBuilder builder = new StringBuilder();
            foreach(UserData user in contacts)
            {
                builder.Append(user.GetName());
                builder.Append("\0");
                builder.Append((user.IsOnline() ? "online" : "offline"));
                builder.Append("\n");
            }
            if(builder.Length > 0)
            {
                builder.Length--;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Construct a new user.
        /// </summary>
        /// <param name="username">Username of the new user. Should be unique.</param>
        /// <param name="pass">Password for the new user. Cannot be changed.</param>
        public UserData(string username, string pass)
        {
            this.name = username;
            this.password = pass;
            this.online = true;  //TODO should the user be logged on after signup or must they log in?
            this.contacts = new List<UserData>();
        }
        
        /// <summary>
        /// Add a user contact to this user's contact list.
        /// Both users must add each other to create a room.
        /// </summary>
        /// <param name="contact">The user to add.</param>
        /// <returns>True if the user has been added; false if the user is already in their contacts.</returns>
        public bool AddContact(UserData contact)
        {
            if(contacts.Contains(contact))
            {
                return false;
            }
            else
            {
                contacts.Add(contact);
                return true;
            }
        }

        /// <summary>
        /// Remove the specified user from this user's contact list.
        /// They will be unable to create rooms with one another.
        /// </summary>
        /// <param name="contact">The user to remove.</param>
        /// <returns>True if the user has been removed; false if the user isn't in the contacts list in the first place.</returns>
        public bool RemoveContact(UserData contact)
        {
            if (contacts.Contains(contact))
            {
                contacts.Remove(contact);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get a list of contacts typed as AbstractUserData.
        /// Satisfies the interface requirements.
        /// </summary>
        /// <returns>The list of contacts.</returns>
        public List<AbstractUserData> GetContacts()
        {
            List<AbstractUserData> c = new List<AbstractUserData>();
            c.AddRange(contacts);
            return c;
        }
    }
}