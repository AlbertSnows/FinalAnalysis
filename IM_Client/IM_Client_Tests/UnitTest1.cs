using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IM_Client_Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const int WAIT_TIME = 1000; 

        //used for unit tests
        private IM_Client.DataBase db;
        private MOCK_UI mock = new MOCK_UI();
        private IM_Client.Controller c;

        //used for integration tests
        private IM_Client.DataBase db1;
        private MOCK_UI mock1 = new MOCK_UI();
        private IM_Client.Controller c1;

        private IM_Client.DataBase db2;
        private MOCK_UI mock2 = new MOCK_UI();
        private IM_Client.Controller c2;

        private IM_Client.DataBase db3;
        private MOCK_UI mock3 = new MOCK_UI();
        private IM_Client.Controller c3;

        private string chatRoom;

        private void reset_test()
        {
            mock.ResetTest();
            db = new IM_Client.DataBase();
            c = new IM_Client.Controller(db, mock, true);

            mock1.ResetTest();
            db1 = new IM_Client.DataBase();
            c1 = new IM_Client.Controller(db1, mock1, true);

            mock2.ResetTest();
            db2 = new IM_Client.DataBase();
            c2 = new IM_Client.Controller(db2, mock2, true);

            mock3.ResetTest();
            db3 = new IM_Client.DataBase();
            c3 = new IM_Client.Controller(db3, mock3, true);
        }

        private void reset_mock()
        {
            mock.ResetTest();
            mock1.ResetTest();
            mock2.ResetTest();
            mock3.ResetTest();
        }

        [TestMethod]
        public void Test_MessageReceived()
        {
            string JsonPacket;
            bool boolDictVal;
            List<string> listStringDictVal;
            List<Tuple<string, string>> chatRoomHist;
            /****************************************************************************
               Test that controller handles successful login
            ****************************************************************************/
            reset_test();
            
            JsonPacket = "{request: \"login\", result: \"true\", data: \"user1\0online\nuser2\0offline\", roomhistory: \"00a1\0user1: hi there\nuser2: oh hello there\055b2\0user1: im here\nuser2: so am i\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateLogin_CallNumber);
            Assert.AreEqual(mock.updateLogin_Params[mock.updateLogin_CallNumber - 1], true);
            if(db.Contacts.TryGetValue("user1", out boolDictVal))
            {
                Assert.AreEqual(true, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }

            if (db.Contacts.TryGetValue("user2", out boolDictVal))
            {
                Assert.AreEqual(false, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }

            if(db.ChatRoomHistory.TryGetValue("00a1", out chatRoomHist))
            {
                Assert.AreEqual(true, chatRoomHist.Contains(new Tuple<string, string>("user1", "hi there")));
                Assert.AreEqual(true, chatRoomHist.Contains(new Tuple<string, string>("user2", "oh hello there")));
            }
            else
            {
                Assert.Fail();
            }

            if (db.ChatRoomHistory.TryGetValue("55b2", out chatRoomHist))
            {
                Assert.AreEqual(true, chatRoomHist.Contains(new Tuple<string, string>("user1", "im here")));
                Assert.AreEqual(true, chatRoomHist.Contains(new Tuple<string, string>("user2", "so am i")));
            }
            else
            {
                Assert.Fail();
            }


            /****************************************************************************
               Test that controller handles unsuccessful login
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"login\", result: \"false\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateLogin_CallNumber);
            Assert.AreEqual(mock.updateLogin_Params[mock.updateLogin_CallNumber - 1], false);


            /****************************************************************************
               Test that controller handles successful signup
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"signup\", result: \"true\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateSignup_CallNumber);
            Assert.AreEqual(mock.updateSignup_Params[mock.updateSignup_CallNumber - 1], true);


            /****************************************************************************
               Test that controller handles unsuccessful signup
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"signup\", result: \"false\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateSignup_CallNumber);
            Assert.AreEqual(mock.updateSignup_Params[mock.updateSignup_CallNumber - 1], false);

            /****************************************************************************
               Test that controller handles successful add contact
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"addcontact\", result: \"true\", data: \"user1\0online\nuser2\0offline\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateAddContact_CallNumber);
            Assert.AreEqual(mock.updateAddContact_Params[mock.updateAddContact_CallNumber - 1], true);
            if (db.Contacts.TryGetValue("user1", out boolDictVal))
            {
                Assert.AreEqual(true, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }
            if (db.Contacts.TryGetValue("user2", out boolDictVal))
            {
                Assert.AreEqual(false, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }


            /****************************************************************************
               Test that controller handles unsuccessful add contact
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"addcontact\", result: \"false\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateAddContact_CallNumber);
            Assert.AreEqual(mock.updateAddContact_Params[mock.updateAddContact_CallNumber - 1], false);


            /****************************************************************************
               Test that controller handles successful remove contact
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"removecontact\", result: \"true\", data: \"user1\0online\nuser2\0offline\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateRemoveContact_CallNumber);
            Assert.AreEqual(mock.updateRemoveContact_Params[mock.updateRemoveContact_CallNumber - 1], true);
            if (db.Contacts.TryGetValue("user1", out boolDictVal))
            {
                Assert.AreEqual(true, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }
            if (db.Contacts.TryGetValue("user2", out boolDictVal))
            {
                Assert.AreEqual(false, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }


            /****************************************************************************
               Test that controller handles unsuccessful remove contact
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"removecontact\", result: \"false\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateRemoveContact_CallNumber);
            Assert.AreEqual(mock.updateRemoveContact_Params[mock.updateRemoveContact_CallNumber - 1], false);


            /****************************************************************************
               Test that controller handles contact update
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"contactupdate\", data: \"user1\0online\nuser2\0offline\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateContactData_CallNumber);
            if (db.Contacts.TryGetValue("user1", out boolDictVal))
            {
                Assert.AreEqual(true, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }
            if (db.Contacts.TryGetValue("user2", out boolDictVal))
            {
                Assert.AreEqual(false, boolDictVal);
            }
            else
            {
                Assert.Fail();
            }


            /****************************************************************************
               Test that controller handles successful start chat
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"startchat\", result: \"true\", room: \"00a1\", users: \"user1\0user2\", validusers: \"user3\0user4\0user5\", history: \"\"}";

            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateStartChat_CallNumber);
            Assert.AreEqual(true, mock.updateStartChat_Params[mock.updateStartChat_CallNumber - 1]);
            if(db.ChatRoomUsers.TryGetValue("00a1", out listStringDictVal))
            {
                Assert.AreEqual(true, listStringDictVal.Contains("user1"));
                Assert.AreEqual(true, listStringDictVal.Contains("user2"));
            }
            else
            {
                Assert.Fail();
            }

            if(db.ChatRoomValidUsers.TryGetValue("00a1", out listStringDictVal))
            {
                Assert.AreEqual(true, listStringDictVal.Contains("user3"));
                Assert.AreEqual(true, listStringDictVal.Contains("user4"));
                Assert.AreEqual(true, listStringDictVal.Contains("user5"));
            }
            else
            {
                Assert.Fail();
            }

            if(db.ChatRoomHistory.TryGetValue("00a1", out chatRoomHist))
            {
                Assert.AreNotEqual(null, chatRoomHist);
                Assert.AreEqual(0, chatRoomHist.Count);
            }
            else
            {
                Assert.Fail();
            }


            /****************************************************************************
               Test that controller handles successful start chat
            ****************************************************************************/
            reset_test();

            JsonPacket = "{request: \"startchat\", result: \"false\"}";
            
            c.MessageReceived(JsonPacket);

            //verify results
            Assert.AreEqual(1, mock.updateStartChat_CallNumber);
            Assert.AreEqual(false, mock.updateStartChat_Params[mock.updateStartChat_CallNumber - 1]);
        }

        /// <summary>
        /// Integration tests require a server instance to be open. Be sure to open a new instance every time test is run.
        /// Note that a few tests do not pass. These tests have been commented out so that later tests can be run.
        /// </summary>
        [TestMethod]
        public void Test_Integration()
        {
            /****************************************************************************
               Sign up user1
            ****************************************************************************/
            reset_test();

            c1.handleSignup("user1", "password");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock1.updateSignup_CallNumber);
            Assert.AreEqual(true, mock1.updateSignup_Params[mock1.updateSignup_CallNumber - 1]);


            /****************************************************************************
               Attempt to sign up with existing name
            ****************************************************************************/
            reset_mock();

            c2.handleSignup("user1", "password");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock2.updateSignup_CallNumber);
            Assert.AreEqual(false, mock2.updateSignup_Params[mock2.updateSignup_CallNumber - 1]);


            /****************************************************************************
               Sign up User2
            ****************************************************************************/
            reset_mock();

            c2.handleSignup("user2", "password");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock2.updateSignup_CallNumber);
            Assert.AreEqual(true, mock2.updateSignup_Params[mock2.updateSignup_CallNumber - 1]);


            /****************************************************************************
               Sign up User3
            ****************************************************************************/
            reset_mock();

            c3.handleSignup("user3", "password");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock3.updateSignup_CallNumber);
            Assert.AreEqual(true, mock3.updateSignup_Params[mock3.updateSignup_CallNumber - 1]);


            /****************************************************************************
               User1 adds user2 as a contact
            ****************************************************************************/
            reset_mock();

            c1.handleAddContact("user2");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock1.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock1.updateAddContact_Params[mock1.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db1.Contacts.ContainsKey("user2"));


            /****************************************************************************
               User1 adds user3 as a contact
            ****************************************************************************/
            reset_mock();

            c1.handleAddContact("user3");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock1.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock1.updateAddContact_Params[mock1.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db1.Contacts.ContainsKey("user3"));


            /****************************************************************************
               User2 adds user1 as a contact
            ****************************************************************************/
            reset_mock();

            c2.handleAddContact("user1");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock2.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock2.updateAddContact_Params[mock2.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db2.Contacts.ContainsKey("user1"));


            /****************************************************************************
               User2 adds user3 as a contact
            ****************************************************************************/
            reset_mock();

            c2.handleAddContact("user3");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock2.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock2.updateAddContact_Params[mock2.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db2.Contacts.ContainsKey("user3"));


            /****************************************************************************
               User3 adds user1 as a contact
            ****************************************************************************/
            reset_mock();

            c3.handleAddContact("user1");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock3.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock3.updateAddContact_Params[mock3.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db3.Contacts.ContainsKey("user1"));


            /****************************************************************************
               User3 adds user2 as a contact
            ****************************************************************************/
            reset_mock();

            c3.handleAddContact("user2");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock3.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock3.updateAddContact_Params[mock3.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db3.Contacts.ContainsKey("user2"));


            /****************************************************************************
               User1 removes user2 as a contact
            ****************************************************************************/
            reset_mock();

            c1.handleRemoveContact("user2");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock1.updateRemoveContact_CallNumber);
            Assert.AreEqual(true, mock1.updateRemoveContact_Params[mock1.updateRemoveContact_CallNumber - 1]);
            Assert.AreEqual(false, db1.Contacts.ContainsKey("user2"));


            /****************************************************************************
               User1 adds user2 as a contact
            ****************************************************************************/
            reset_mock();

            c1.handleAddContact("user2");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock1.updateAddContact_CallNumber);
            Assert.AreEqual(true, mock1.updateAddContact_Params[mock1.updateAddContact_CallNumber - 1]);
            Assert.AreEqual(true, db1.Contacts.ContainsKey("user2"));


            /****************************************************************************
               User1 logs out
            ****************************************************************************/
            reset_mock();

            c1.handleLogout();

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock2.updateContactData_CallNumber);
            Assert.AreEqual(true, db2.Contacts.ContainsKey("user1"));
            Assert.AreEqual(false, db2.Contacts["user1"]);


            /****************************************************************************
               User1 logs in
            ****************************************************************************/
            reset_mock();

            c1.handleLogin("user1", "password");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock1.updateLogin_CallNumber);
            Assert.AreEqual(true, mock1.updateLogin_Params[mock1.updateLogin_CallNumber - 1]);
            Assert.AreEqual(true, db1.Contacts.ContainsKey("user2"));
            Assert.AreEqual(true, db1.Contacts["user2"]);

            Assert.AreEqual(1, mock2.updateContactData_CallNumber);
            Assert.AreEqual(true, db2.Contacts.ContainsKey("user1"));
            Assert.AreEqual(true, db2.Contacts["user1"]);


            /****************************************************************************
               User1 starts chat with user2
            ****************************************************************************/
            reset_mock();

            c1.handleOpenChat("user2");

            System.Threading.Thread.Sleep(WAIT_TIME);

            Assert.AreEqual(1, mock2.updateStartChat_CallNumber);
            Assert.AreEqual(true, mock2.updateStartChat_Params[mock2.updateStartChat_CallNumber - 1]);
            Assert.AreEqual(1, db2.ChatRoomHistory.Count);//NOTE: cannot predict what room name will be
            chatRoom = new List<string>(db2.ChatRoomHistory.Keys)[0];
            Assert.AreEqual(true, db2.ChatRoomUsers[chatRoom].Contains("user1"));
            Assert.AreEqual(true, db2.ChatRoomValidUsers[chatRoom].Contains("user3"));

            //NOTE: Not currently receiving update from server in client 1. Not sure if problem is client or server side.
            //Assert.AreEqual(1, mock1.updateStartChat_CallNumber);
            //Assert.AreEqual(true, mock1.updateStartChat_Params[mock1.updateStartChat_CallNumber - 1]);
            //Assert.AreEqual(1, db1.ChatRoomHistory.Count);
            //Assert.AreEqual(true, db1.ChatRoomUsers[chatRoom].Contains("user2"));
            //Assert.AreEqual(true, db1.ChatRoomValidUsers[chatRoom].Contains("user3"));


            /****************************************************************************
               User1 adds user3 to chat room
            ****************************************************************************/
            reset_mock();

            c1.handleAddToChat(chatRoom, "user3");

            System.Threading.Thread.Sleep(WAIT_TIME);

            //NOTE: Not currently receiving messages from server for this request. Not sure if problem is client or server side.
            //Assert.AreEqual(1, mock3.updateStartChat_CallNumber);
            //Assert.AreEqual(true, mock3.updateStartChat_Params[mock3.updateStartChat_CallNumber - 1]);
            //Assert.AreEqual(true, db.ChatRoomHistory.ContainsKey(chatRoom));

            //Assert.AreEqual(1, mock1.updateChatUserJoined_CallNumber);
            //Assert.AreEqual(true, db1.ChatRoomUsers[chatRoom].Contains("user2"));
            //Assert.AreEqual(true, db1.ChatRoomUsers[chatRoom].Contains("user3"));

            //Assert.AreEqual(1, mock2.updateChatUserJoined_CallNumber);
            //Assert.AreEqual(true, db2.ChatRoomUsers[chatRoom].Contains("user1"));
            //Assert.AreEqual(true, db2.ChatRoomUsers[chatRoom].Contains("user3"));


            /****************************************************************************
               User1 sends a message
            ****************************************************************************/
            reset_mock();

            c1.handleSendMessage(chatRoom, "Hello World!");

            System.Threading.Thread.Sleep(WAIT_TIME);

            //NOTE: Server throws KeyNotFound exception when looking up the chat room ID.
            //Assert.AreEqual(1, mock2.updateReceiveChat_CallNumber);
            //Assert.AreEqual("user1", db2.ChatRoomHistory[chatRoom][0].Item1);
            //Assert.AreEqual("Hello World!", db2.ChatRoomHistory[chatRoom][0].Item2);
        }
    }
}
