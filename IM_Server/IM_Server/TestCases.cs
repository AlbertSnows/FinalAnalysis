using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Server
{
    class TestCases
    {
        public static void RunTestCases()
        {
            ControlInterface c = Program.GetController();                           //This is how the WebSocketSharp communicates with the controller.
            Dictionary<string, string> message;                                     //This object simulates the messages sent by the client.
            Dictionary<ClientID, List<Dictionary<string, string>>> response;        //This object simulates the responses from the server. Each ClientID goes to one client, and there can be a list (multiple) messages to that one client.
            ClientID user1ID = new ClientID("asdfasdf");                            //Unique id's assigned by the WebSocketSharp to each client communcation
            ClientID user2ID = new ClientID("asdf1234");                            
            ClientID user3ID = new ClientID("1234asdf");

            //Test signup ability
            message = new Dictionary<string, string>();
            message["request"] = "signup";
            message["user"] = "user1";
            message["password"] = "abc123";
            response = c.ClientMessage(user1ID, message);                   //This line (and repeated ones below) "sends" the message to the server from the provided client id (user1ID).
//Assertion 1
            Assert(response[user1ID][0]["request"] == "signup");            //Assert lines are boolean statements that we are asserting to be true (this is where the testing takes place - compare this info to the API)
//Assertion 2
            Assert(response[user1ID][0]["result"] == "true");

            //Signup other users now
            message = new Dictionary<string, string>();
            message["request"] = "signup";
            message["user"] = "user2";
            message["password"] = "abc123"; //could have different / unique API if necessary
            response = c.ClientMessage(user2ID, message);
//Assertion 3
            Assert(response[user2ID][0]["request"] == "signup");
//Assertion 4       
            Assert(response[user2ID][0]["result"] == "true");

            message = new Dictionary<string, string>();
            message["request"] = "signup";
            message["user"] = "user3";
            message["password"] = "abc123";
            response = c.ClientMessage(user3ID, message);
//Assertion 5
            Assert(response[user3ID][0]["request"] == "signup");
//Assertion 6
            Assert(response[user3ID][0]["result"] == "true");

            //Test failed password login (rejection)
            message = new Dictionary<string, string>();
            message["request"] = "login";
            message["user"] = "user1";
            message["password"] = "wrongpass";
            response = c.ClientMessage(user1ID, message);
//Assertion 7
            Assert(response[user1ID][0]["request"] == "login");
//Assertion 8
            Assert(response[user1ID][0]["result"] == "false");  //Enemy user tried to login with wrong password and failed to login.

            //Test adding contact one way (user1 adding user2)
            message = new Dictionary<string, string>();
            message["request"] = "addcontact";
            message["user"] = "user2";
            response = c.ClientMessage(user1ID, message);
//Assertion 9
            Assert(response[user1ID][0]["request"] == "addcontact");
//Assertion 10
            Assert(response[user1ID][0]["result"] == "true");
//Assertion 11
            Assert(response[user1ID][0]["data"] == "user2\0online");   //Data contact string (split by \0)

            //Add another contact for user1 (user1 adding user3)
            message = new Dictionary<string, string>();
            message["request"] = "addcontact";
            message["user"] = "user3";
            response = c.ClientMessage(user1ID, message);
//Assertion 12
            Assert(response[user1ID][0]["request"] == "addcontact");
//Assertion 13
            Assert(response[user1ID][0]["result"] == "true");
//Assertion 14
            Assert(response[user1ID][0]["data"] == "user2\0online\nuser3\0online"); //Data contact string (multiple users split by \n, split status by \0)

            //Have user2 add user1
            message = new Dictionary<string, string>();
            message["request"] = "addcontact";
            message["user"] = "user1";
            response = c.ClientMessage(user2ID, message);
//Assertion 15
            Assert(response[user2ID][0]["request"] == "addcontact");
//Assertion 16
            Assert(response[user2ID][0]["result"] == "true");
//Assertion 17
            Assert(response[user2ID][0]["data"] == "user1\0online");

            //User1 creates a room with user2
            message = new Dictionary<string, string>();
            message["request"] = "startchat";
            message["user"] = "user2";
            response = c.ClientMessage(user1ID, message);
//Assertion 18
            Assert(response[user1ID][0]["request"] == "startchat");
//Assertion 19
            Assert(response[user1ID][0]["result"] == "true");
//Assertion 20
            Assert(response[user1ID][0]["room"].Length > 0);    //given a room id that is not empty
            string room1Key = response[user1ID][0]["room"];         //saving this room key for future use / access when sending chat / etc
//Assertion 21
            Assert(response[user1ID][0]["users"] == "user1\0user2");    //User 1 and 2 in the room
//Assertion 22
            Assert(response[user1ID][0]["validusers"].Length == 0); //No valid users to add
//Assertion 23
            Assert(response[user1ID][0]["history"].Length == 0);    //No history yet, new room
            //user2 notified of new room
//Assertion 24
            Assert(response[user2ID][0]["request"] == "startchat");
//Assertion 25
            Assert(response[user2ID][0]["result"] == "true");
//Assertion 26
            Assert(response[user2ID][0]["room"] == room1Key);   //same room key
//Assertion 27
            Assert(response[user2ID][0]["users"] == "user1\0user2");    //Users 1 and 2 in the room
//Assertion 28
            Assert(response[user2ID][0]["validusers"].Length == 0); //No valid users yet (could be some if user1 and user2 were friends with more people).
//Assertion 29
            Assert(response[user2ID][0]["history"].Length == 0);

            //User 1 sends chat to user 2
            message = new Dictionary<string, string>();
            message["request"] = "sendchat";
            message["room"] = room1Key;
            message["message"] = "hello!";
            response = c.ClientMessage(user1ID, message);
//Assertion 30
            Assert(response[user1ID][0] == response[user2ID][0]);   //both messages are identical
//Assertion 31
            Assert(response[user1ID][0]["request"] == "receivechat");
//Assertion 32
            Assert(response[user1ID][0]["room"] == room1Key);
//Assertion 33
            Assert(response[user1ID][0]["user"] == "user1");
//Assertion 34
            Assert(response[user1ID][0]["message"] == "hello!");

            //User 1 tries to add user 3 to the chat, but fails, because user2 is not friends with user3
            message = new Dictionary<string, string>();
            message["request"] = "addtochat";
            message["room"] = room1Key;
            message["user"] = "user3";
            response = c.ClientMessage(user1ID, message);
//Assertion 35
            Assert(response[user1ID][0]["request"] == "addtochat");
//Assertion 36
            Assert(response[user1ID][0]["room"] == room1Key);
//Assertion 37
            Assert(response[user1ID][0]["user"] == "user3");
//Assertion 38
            Assert(response[user1ID][0]["result"] == "false");  //They are not mutual friends, won't work - not in the valid users list.

            //User 3 and user 2 now become mutual friends
            message = new Dictionary<string, string>();
            message["request"] = "addcontact";
            message["user"] = "user2";
            response = c.ClientMessage(user3ID, message);
//Assertion 39
            Assert(response[user3ID][0]["request"] == "addcontact");
//Assertion 40
            Assert(response[user3ID][0]["result"] == "true");
//Assertion 41
            Assert(response[user3ID][0]["data"] == "user2\0online");

            message = new Dictionary<string, string>();
            message["request"] = "addcontact";
            message["user"] = "user3";
            response = c.ClientMessage(user2ID, message);
//Assertion 42
            Assert(response[user2ID][0]["request"] == "addcontact");
//Assertion 43
            Assert(response[user2ID][0]["result"] == "true");
//Assertion 44
            Assert(response[user2ID][0]["data"] == "user1\0online\nuser3\0online");
            
            //And user 3 friends with user 1
            message = new Dictionary<string, string>();
            message["request"] = "addcontact";
            message["user"] = "user1";
            response = c.ClientMessage(user3ID, message);
//Assertion 45
            Assert(response[user3ID][0]["request"] == "addcontact");
//Assertion 46
            Assert(response[user3ID][0]["result"] == "true");
//Assertion 47
            Assert(response[user3ID][0]["data"] == "user2\0online\nuser1\0online");

            //Now user2 adds user3 to the chat room
            message = new Dictionary<string, string>();
            message["request"] = "addtochat";
            message["user"] = "user3";
            message["room"] = room1Key;
            response = c.ClientMessage(user2ID, message);
            //All users' initially in the room "chat user joined" message is the same.
//Assertion 48
            Assert(response[user1ID][0] == response[user2ID][0]);
//Assertion 49
            Assert(response[user1ID][0]["request"] == "chatuserjoined");
//Assertion 50
            Assert(response[user1ID][0]["room"] == room1Key);
//Assertion 51
            Assert(response[user1ID][0]["user"] == "user3");
//Assertion 52
            Assert(response[user1ID][0]["validusers"] == "");
            //User3 notified to join the room, and given the chat history
//Assertion 53
            Assert(response[user3ID][0]["request"] == "startchat");
//Assertion 54
            Assert(response[user3ID][0]["room"] == room1Key);
//Assertion 55
            Assert(response[user3ID][0]["users"] == "user1\0user2\0user3");
//Assertion 56
            Assert(response[user3ID][0]["validusers"] == "");
//Assertion 57
            Assert(response[user3ID][0]["history"] == room1Key + "\0user1:hello!\n");

            //User3 goes offline
            message = new Dictionary<string, string>();
            message["request"] = "logout";
            response = c.ClientMessage(user3ID, message);
            //User1 and user2 contact data updated as to who is online/offline
//Assertion 58
            Assert(response[user1ID][0]["request"] == "contactupdate");
//Assertion 59
            Assert(response[user1ID][0]["data"] == "user2\0online\nuser3\0offline");    //Now user 3 is marked offline
//Assertion 60
            Assert(response[user2ID][0]["request"] == "contactupdate");
//Assertion 61
            Assert(response[user2ID][0]["data"] == "user1\0online\nuser3\0offline");    //user 3 marked offline

            //User2 sends a message (that user3 will not see since offline)
            message = new Dictionary<string, string>();
            message["request"] = "sendchat";
            message["room"] = room1Key;
            message["message"] = "This is a chat message.";
            response = c.ClientMessage(user2ID, message);
//Assertion 62
            Assert(response[user1ID][0] == response[user2ID][0]);   //both messages are identical
//Assertion 63
            Assert(response[user1ID][0]["request"] == "receivechat");
//Assertion 64
            Assert(response[user1ID][0]["room"] == room1Key);
//Assertion 65
            Assert(response[user1ID][0]["user"] == "user2");
//Assertion 66
            Assert(response[user1ID][0]["message"] == "This is a chat message.");

            //User 3 now comes back online and get the room history
            message = new Dictionary<string, string>();
            message["request"] = "login";
            message["user"] = "user3";
            message["password"] = "abc123";
            response = c.ClientMessage(user3ID, message);
//Assertion 67
            Assert(response[user3ID][0]["request"] == "login");
//Assertion 68
            Assert(response[user3ID][0]["result"] == "true");
//Assertion 69
            Assert(response[user3ID][0]["data"] == "user2\0online\nuser1\0online");
//Assertion 70
            Assert(response[user3ID][0]["roomhistory"] == room1Key + "\0user1:hello!\nuser2:This is a chat message.");      //This is the entire chat history. "user1:hello!" was the first message, then "user2:This is a chat message." is the second message
            //User2 and User3 are notified of user3 coming online
//Assertion 71
            Assert(response[user1ID][0]["request"] == "contactupdate");
//Assertion 72
            Assert(response[user1ID][0]["data"] == "user2\0online\nuser3\0online");
//Assertion 73
            Assert(response[user2ID][0]["request"] == "contactupdate");
//Assertion 74
            Assert(response[user2ID][0]["data"] == "user1\0online\nuser3\0online");

            //User 3 now removes user 2 from their contact list, kicking user 3 from the room.
            message = new Dictionary<string, string>();
            message["request"] = "removecontact";
            message["user"] = "user2";
            response = c.ClientMessage(user3ID, message);
            //User 3 is sent multiple message (one removing him from the room, one confirming the contact removal)
            //Message 1 (index 0) - User 3 removed from room
//Assertion 75
            Assert(response[user3ID][0] == response[user2ID][0] && response[user3ID][0] == response[user1ID][0]);   //removal message is sent to everybody and is identical
//Assertion 76
            Assert(response[user3ID][0]["request"] == "chatuserleft");
//Assertion 77
            Assert(response[user3ID][0]["room"] == room1Key);
//Assertion 78
            Assert(response[user3ID][0]["user"] == "user3");
//Assertion 79
            Assert(response[user3ID][0]["validusers"].Length == 0);
            //Message 2 (index 1) - Confirming contact removal (user 2 isn't notified of contact removal by design)
//Assertion 80
            Assert(response[user3ID][1]["request"] == "removecontact");
//Assertion 81
            Assert(response[user3ID][1]["result"] == "true");
//Assertion 82
            Assert(response[user3ID][1]["data"] == "user1\0online");

            //2 people left in the room. User 2 leaves, kicking user 1 from the room as well, and removing the room.
            message = new Dictionary<string, string>();
            message["request"] = "leavechat";
            message["room"] = room1Key;
            response = c.ClientMessage(user2ID, message);
            //Both users notified that they have been removed from the room.
//Assertion 83
            Assert(response[user2ID][0]["request"] == "chatuserleft");
//Assertion 84
            Assert(response[user2ID][0]["room"] == room1Key);
//Assertion 85
            Assert(response[user2ID][0]["user"] == "user2");
//Assertion 86
            Assert(response[user2ID][0]["validusers"].Length == 0);
//Assertion 87
            Assert(response[user1ID][0]["request"] == "chatuserleft");
//Assertion 88
            Assert(response[user1ID][0]["room"] == room1Key);
//Assertion 89
            Assert(response[user1ID][0]["user"] == "user1");
//Assertion 90
            Assert(response[user1ID][0]["validusers"].Length == 0);
            //Both were removed from the room, and the room now doesn't exist. The validusers isn't used in these cases.
            //Review complete!
        }

        //Keep track of assertions
        private static int assertionNumber = 0;
        /// <summary>
        /// Assume something is true. If it is not, an error is shown.
        /// </summary>
        /// <param name="statement">The statement assumed to be true (assertion).</param>
        private static void Assert(bool statement)
        {
            assertionNumber++;
            if(!statement)
            {
                Console.WriteLine("Assersion " + assertionNumber + " Failed!");
            }
            else
            {
                Console.WriteLine("Assersion " + assertionNumber + " True.");
            }
        }
    }
}
