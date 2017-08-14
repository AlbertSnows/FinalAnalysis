using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client
{
    public interface Observer
    {
        /// <summary>
        /// Show logon screen.
        /// </summary>
        void ShowLogonScreen();

        /// <summary>
        /// Show main chat screen.
        /// </summary>
        void ShowMainScreen();

        /// <summary>
        /// Server has responed to Login request
        /// </summary>
        /// <param name="result">server response</param>
        void updateLogin(bool result);

        /// <summary>
        /// Server has responded to Signup request
        /// </summary>
        /// <param name="result">server response</param>
        void updateSignup(bool result);

        /// <summary>
        /// Server has responded to Add Contact request
        /// </summary>
        /// <param name="result">server response</param>
        void updateAddContact(bool result);

        /// <summary>
        /// Server has responded to Remove Contact request
        /// </summary>
        /// <param name="result">server response</param>
        void updateRemoveContact(bool result);

        /// <summary>
        /// Server has responed to request to open chat room.
        /// </summary>
        /// <param name="result">server response. will only be false if request originated from current user</param>
        void updateStartChat(bool result);

        /// <summary>
        /// Server has sent a message to an active chat room.
        /// </summary>
        void updateReceiveChat();

        /// <summary>
        /// Server has responed to request to add user to active chat room. Request originated from current user.
        /// note that result should always be false
        /// </summary>
        /// <param name="result">server response</param>
        /// <param name="room">room that user was attempted to be added to </param>
        /// <param name="user">user that was attempted to be added</param>
        void updateAddToChat(bool result, string room, string user);

        /// <summary>
        /// A new user has joined an open chat room.
        /// </summary>
        void updateChatUserJoined();

        /// <summary>
        /// A user has left the chat room.
        /// </summary>
        /// <param name="user">name of user who left</param>
        /// <param name="room">room that user left</param>
        void updateChatUserLeft(string user, string room);

        /// <summary>
        /// Server has sent new contact data.
        /// </summary>
        void updateContactData();
    }
}
