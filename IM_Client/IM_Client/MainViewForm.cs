using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_Client
{
    public partial class MainViewForm : Form, Observer
    {

        private InputHandler IH;
        private DataBase db;
        private string userName;
        private string password;
        private int count = 0;


        public MainViewForm(DataBase db)
        {
            InitializeComponent();

            this.db = db;
            //NOTE: This method call has been moved to main() to avoid null reference problem.
            //ShowLogonScreen();
           

        }

        public void LinkInputHandler(InputHandler ih)
        {
            IH = ih;
        }

        public void ShowLogonScreen()
        {
            string name = "";
            string pass = "";
            DialogResult resp;
            do
            {
                resp = InputBox("Login", "Enter user name: ", "Enter password: ", ref name, ref pass);
                if (resp == DialogResult.OK)
                {
                    IH.handleLogin(name, pass);
                }
                else if(resp == DialogResult.Retry)
                {
                    IH.handleSignup(name, pass);
                }
                

            }
            //while (IH.handleLogin(name,pass) == false); loop while fail login // error
            while (name == "");


            userName = name;
            password = pass;
        }


        /// <summary>
        /// Show main chat screen.
        /// </summary>
        public void ShowMainScreen()
        {

        }

        /// <summary>
        /// Server has responed to Login request
        /// </summary>
        /// <param name="result">server response</param>
        public void updateLogin(bool result)
        {
            if(result)
            {
                updateContactData();
                updateRoomHistory();
                ShowMainScreen();
            }
            else
            {
                ShowLogonScreen();
            }
        }

        /// <summary>
        /// Server has responded to Signup request
        /// </summary>
        /// <param name="result">server response</param>
        public void updateSignup(bool result)
        {
            if (result)
            {
                updateContactData();
                updateRoomHistory();
                ShowMainScreen();
            }
            else
            {
                ShowLogonScreen();
            }
        }

        /// <summary>
        /// Server has responded to Add Contact request
        /// </summary>
        /// <param name="result">server response</param>
        public void updateAddContact(bool result)
        {
            if(result)
            {
                updateContactData();
            }
            else
            {
                //maybe create popup notifying user that request failed.
            }
        }

        /// <summary>
        /// Server has responded to Remove Contact request
        /// </summary>
        /// <param name="result">server response</param>
        public void updateRemoveContact(bool result)
        {

        }

        /// <summary>
        /// Server has responed to request to open chat room.
        /// </summary>
        /// <param name="result">server response. will only be false if request originated from current user</param>
        public void updateStartChat(bool result)
        {
            //TODO: Add room to RoomListBox and update chat room users and valid users if needed
        }

        /// <summary>
        /// Server has sent a message to an active chat room.
        /// </summary>
        public void updateReceiveChat()
        {

        }

        /// <summary>
        /// Server has responed to request to add user to active chat room. Request originated from current user.
        /// </summary>
        /// <param name="result">server response</param>
        public void updateAddToChat(bool result, string room, string user)
        {
            //
        }

        /// <summary>
        /// A new user has joined an open chat room.
        /// </summary>
        public void updateChatUserJoined()
        {
           // UserListBox.Items.Add();
        }

        /// <summary>
        /// A user has left the chat room.
        /// </summary>
        /// <param name="user">name of user who left</param>
        /// <param name="room">room that user left</param>
        public void updateChatUserLeft(string user, string room)
        {
            UserListBox.Items.Remove(user);
        }

        /// <summary>
        /// Server has sent new contact data.
        /// </summary>
        public void updateContactData()
        {
            Invoke(new Action(() => ContactListBox.Items.Clear()));
            foreach( KeyValuePair<string,bool> contact in db.Contacts)
            {
                if (contact.Value == true)
                {
                    Invoke(new Action(() => ContactListBox.Items.Add(contact.Key + " (Online)")));
                }
                else
                {
                    Invoke(new Action(() => ContactListBox.Items.Add(contact.Key + " (offline)")));
                }
            }
        }

        /// <summary>
        /// updates chat room history from database
        /// </summary>
        private void updateRoomHistory()
        {

        }

        /// <summary>
        /// login/sign up dialog (will only show once at start)
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="t1">label1.text</param>
        /// <param name="t2">label2.text</param>
        /// <param name="name">textbox1.text</param>
        /// <param name="pass">textbox2.text</param>
        /// <returns>which button is clicked</returns>
        public static DialogResult InputBox(string title, string t1,string t2, ref string name, ref string pass)
        {
            Form form = new Form();

            Label userLabel = new Label();
            Label passwordLabel = new Label();

            TextBox userbox = new TextBox();
            TextBox passbox = new TextBox();

            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            Button buttonSignUp = new Button();

            form.Text = title;
            userLabel.Text = t1;
            passwordLabel.Text = t2;
            userbox.Text = name;
            passbox.Text = pass;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonSignUp.Text = "Sign Up";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonSignUp.DialogResult = DialogResult.Retry;

            userLabel.SetBounds(9, 20, 372, 13);
            userbox.SetBounds(100, 19, 337, 20);


            passwordLabel.SetBounds(9, 65, 372, 13);
            passbox.SetBounds(100, 64, 337, 20);

            buttonSignUp.SetBounds(177, 155, 78, 23);
            buttonOk.SetBounds(266, 155, 75, 23);
            buttonCancel.SetBounds(355, 155, 75, 23);

            userLabel.AutoSize = true;
            userbox.Anchor = userbox.Anchor | AnchorStyles.Right;
            passwordLabel.AutoSize = true;
            passbox.Anchor = userbox.Anchor | AnchorStyles.Right;

            passbox.PasswordChar = '*';

            buttonSignUp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(450, 180);
            form.Controls.AddRange(new Control[] { userLabel, userbox, passwordLabel, passbox, buttonSignUp,buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, userLabel.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            name = userbox.Text;
            pass = passbox.Text;
            return dialogResult;
        }

        /// <summary>
        /// click to add selected items into contact list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addContactButton_Click(object sender, EventArgs e)
        {
            //NOTE: The server never sends a list of all users, so we should expect the user to type in the name of the user they wish to add.
            // For the moment, I've made it so it reads the username from the message text box, but this should probably be changed later on.

            /*
            string user = "";
            for(int i = 0; i < UserListBox.CheckedItems.Count; i++)
            {
                user = UserListBox.CheckedItems[i].ToString();
                IH.handleAddContact(user);
            }
            */

            IH.handleAddContact(messageTextBox.Text);
        }
        /// <summary>
        /// click to add selected items into chat room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToChatButton_Click(object sender, EventArgs e)
        {
            string user = "";
            for (int i = 0; i < ContactListBox.CheckedItems.Count; i++)
            {
                user = UserListBox.CheckedItems[i].ToString();
                //IH.handleAddToChat(,user);
            }
        }
        /// <summary>
        /// click to create one new chat room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startChatButton_Click(object sender, EventArgs e)
        {
            //TODO: Provide a way for the user to enter a username instead of using messageTextBox
            IH.handleOpenChat(messageTextBox.Text);
            /*
            RoomListBox.Items.Add("00" + count);

            count++;
            */
        }

        /// <summary>
        /// double click to open a chat room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomListBox_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int index = this.RoomListBox.IndexFromPoint(e.Location);
            //IH.handleLeaveChat();
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                IH.handleOpenChat(userName);
                //
                
            }
        }
    }
}
