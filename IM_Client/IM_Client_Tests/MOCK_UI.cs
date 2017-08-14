using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client_Tests
{
    class MOCK_UI : IM_Client.Observer
    {

        public int ShowLogonScreen_CallNumber;

        public int ShowMainScreen_CallNumber;

        public int updateAddContact_CallNumber;
        public List<bool> updateAddContact_Params;

        public int updateAddToChat_CallNumber;
        public List<Tuple<bool, string, string>> updateAddToChat_Params;

        public int updateChatUserJoined_CallNumber;

        public int updateChatUserLeft_CallNumber;
        public List<Tuple<string, string>> updateChatUserLeft_Params;

        public int updateContactData_CallNumber;

        public int updateLogin_CallNumber;
        public List<bool> updateLogin_Params;

        public int updateReceiveChat_CallNumber;

        public int updateRemoveContact_CallNumber;
        public List<bool> updateRemoveContact_Params;

        public int updateSignup_CallNumber;
        public List<bool> updateSignup_Params;

        public int updateStartChat_CallNumber;
        public List<bool> updateStartChat_Params;

        public void ResetTest()
        {
            ShowLogonScreen_CallNumber      = 0;
            ShowMainScreen_CallNumber       = 0;
            updateAddContact_CallNumber     = 0;
            updateAddToChat_CallNumber      = 0;
            updateChatUserJoined_CallNumber = 0;
            updateChatUserLeft_CallNumber   = 0;
            updateContactData_CallNumber    = 0;
            updateLogin_CallNumber          = 0;
            updateReceiveChat_CallNumber    = 0;
            updateRemoveContact_CallNumber  = 0;
            updateSignup_CallNumber         = 0;
            updateStartChat_CallNumber      = 0;

            updateAddContact_Params = new List<bool>();
            updateAddToChat_Params = new List<Tuple<bool, string, string>>();
            updateChatUserLeft_Params = new List<Tuple<string, string>>();
            updateLogin_Params = new List<bool>();
            updateRemoveContact_Params = new List<bool>();
            updateSignup_Params = new List<bool>();
            updateStartChat_Params = new List<bool>();
        }

        public void ShowLogonScreen()
        {
            ShowLogonScreen_CallNumber++;
        }

        public void ShowMainScreen()
        {
            ShowMainScreen_CallNumber++;
        }

        public void updateAddContact(bool result)
        {
            updateAddContact_CallNumber++;
            updateAddContact_Params.Add(result);
        }

        public void updateAddToChat(bool result, string room, string user)
        {
            updateAddToChat_CallNumber++;
            updateAddToChat_Params.Add(new Tuple<bool, string, string>(result, room, user));
        }

        public void updateChatUserJoined()
        {
            updateChatUserJoined_CallNumber++;
        }

        public void updateChatUserLeft(string user, string room)
        {
            updateChatUserLeft_CallNumber++;
            updateChatUserLeft_Params.Add(new Tuple<string, string>(user, room));
        }

        public void updateContactData()
        {
            updateContactData_CallNumber++;
        }

        public void updateLogin(bool result)
        {
            updateLogin_CallNumber++;
            updateLogin_Params.Add(result);
        }

        public void updateReceiveChat()
        {
            updateReceiveChat_CallNumber++;
        }

        public void updateRemoveContact(bool result)
        {
            updateRemoveContact_CallNumber++;
            updateRemoveContact_Params.Add(result);
        }

        public void updateSignup(bool result)
        {
            updateSignup_CallNumber++;
            updateSignup_Params.Add(result);
        }

        public void updateStartChat(bool result)
        {
            updateStartChat_CallNumber++;
            updateStartChat_Params.Add(result);
        }
    }
}
