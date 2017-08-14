using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IM_Server
{
    /// <summary>
    /// Defines an abstract protection for data reguarding a registered user.
    /// Instead of giving some applications full access to something like UserData, they only need to know a few parts of it.
    /// This interface provides security so that different parts cannot change the user's information.
    /// </summary>
    public interface AbstractUserData
    {
        /// <summary>
        /// Get the name of the registered user.
        /// Is unique.
        /// </summary>
        /// <returns>A string containing the unique user name for this user.</returns>
        string GetName();

        /// <summary>
        /// Gets a list of all the users this user has added to their contact list.
        /// Users in this list may not be two-way friends.
        /// </summary>
        /// <returns>A list of AbstractUserData representing friended other users.</returns>
        List<AbstractUserData> GetContacts();

        /// <summary>
        /// Determine if this user has logged in and is active.
        /// </summary>
        /// <returns>True if the user is logged in, false if they have logged out or became disconnected.</returns>
        bool IsOnline();
    }
}