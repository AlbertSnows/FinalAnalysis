using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client
{
    public interface InputHandler
    {
        bool handleLogin(string user, string pw);

        bool handleSignup(string user, string pw);

        bool handleAddContact(string user);

        bool handleRemoveContact(string user);

        void handleLogout();

        void handleLeaveChat(string room);

        bool handleOpenChat(string user);

        void handleSendMessage(string room, string message);

        bool handleAddToChat(string room, string user);
    }
}
