namespace IM_Client
{
    partial class MainViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RoomListLabel = new System.Windows.Forms.Label();
            this.RoomListBox = new System.Windows.Forms.ListBox();
            this.UserListLabel = new System.Windows.Forms.Label();
            this.ChatLabel = new System.Windows.Forms.Label();
            this.MessageListBox = new System.Windows.Forms.ListBox();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.UserListBox = new System.Windows.Forms.CheckedListBox();
            this.addContactButton = new System.Windows.Forms.Button();
            this.addToChatButton = new System.Windows.Forms.Button();
            this.startChatButton = new System.Windows.Forms.Button();
            this.ContactListBox = new System.Windows.Forms.CheckedListBox();
            this.ContactListLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // RoomListLabel
            // 
            this.RoomListLabel.AutoSize = true;
            this.RoomListLabel.Location = new System.Drawing.Point(0, 21);
            this.RoomListLabel.Name = "RoomListLabel";
            this.RoomListLabel.Size = new System.Drawing.Size(111, 26);
            this.RoomListLabel.TabIndex = 1;
            this.RoomListLabel.Text = "Room List\r\n(Double Click To Join)";
            // 
            // RoomListBox
            // 
            this.RoomListBox.FormattingEnabled = true;
            this.RoomListBox.Location = new System.Drawing.Point(3, 56);
            this.RoomListBox.Name = "RoomListBox";
            this.RoomListBox.Size = new System.Drawing.Size(100, 264);
            this.RoomListBox.TabIndex = 2;
            this.RoomListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RoomListBox_MouseDoubleClick);
            // 
            // UserListLabel
            // 
            this.UserListLabel.AutoSize = true;
            this.UserListLabel.Location = new System.Drawing.Point(525, 21);
            this.UserListLabel.Name = "UserListLabel";
            this.UserListLabel.Size = new System.Drawing.Size(48, 13);
            this.UserListLabel.TabIndex = 4;
            this.UserListLabel.Text = "User List";
            // 
            // ChatLabel
            // 
            this.ChatLabel.AutoSize = true;
            this.ChatLabel.Location = new System.Drawing.Point(162, 21);
            this.ChatLabel.Name = "ChatLabel";
            this.ChatLabel.Size = new System.Drawing.Size(29, 13);
            this.ChatLabel.TabIndex = 5;
            this.ChatLabel.Text = "Chat";
            // 
            // MessageListBox
            // 
            this.MessageListBox.FormattingEnabled = true;
            this.MessageListBox.Location = new System.Drawing.Point(165, 56);
            this.MessageListBox.Name = "MessageListBox";
            this.MessageListBox.Size = new System.Drawing.Size(336, 264);
            this.MessageListBox.TabIndex = 0;
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Location = new System.Drawing.Point(101, 349);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(59, 13);
            this.MessageLabel.TabIndex = 6;
            this.MessageLabel.Text = "Message : ";
            // 
            // messageTextBox
            // 
            this.messageTextBox.Location = new System.Drawing.Point(166, 346);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(336, 20);
            this.messageTextBox.TabIndex = 7;
            // 
            // UserListBox
            // 
            this.UserListBox.FormattingEnabled = true;
            this.UserListBox.Location = new System.Drawing.Point(528, 56);
            this.UserListBox.Name = "UserListBox";
            this.UserListBox.Size = new System.Drawing.Size(110, 259);
            this.UserListBox.TabIndex = 8;
            // 
            // addContactButton
            // 
            this.addContactButton.Location = new System.Drawing.Point(528, 339);
            this.addContactButton.Name = "addContactButton";
            this.addContactButton.Size = new System.Drawing.Size(110, 32);
            this.addContactButton.TabIndex = 9;
            this.addContactButton.Text = "Add To Contact";
            this.addContactButton.UseVisualStyleBackColor = true;
            this.addContactButton.Click += new System.EventHandler(this.addContactButton_Click);
            // 
            // addToChatButton
            // 
            this.addToChatButton.Location = new System.Drawing.Point(674, 339);
            this.addToChatButton.Name = "addToChatButton";
            this.addToChatButton.Size = new System.Drawing.Size(105, 32);
            this.addToChatButton.TabIndex = 10;
            this.addToChatButton.Text = "Add To Chat";
            this.addToChatButton.UseVisualStyleBackColor = true;
            this.addToChatButton.Click += new System.EventHandler(this.addToChatButton_Click);
            // 
            // startChatButton
            // 
            this.startChatButton.Location = new System.Drawing.Point(12, 339);
            this.startChatButton.Name = "startChatButton";
            this.startChatButton.Size = new System.Drawing.Size(83, 32);
            this.startChatButton.TabIndex = 11;
            this.startChatButton.Text = "Start Chat";
            this.startChatButton.UseVisualStyleBackColor = true;
            this.startChatButton.Click += new System.EventHandler(this.startChatButton_Click);
            // 
            // ContactListBox
            // 
            this.ContactListBox.FormattingEnabled = true;
            this.ContactListBox.Location = new System.Drawing.Point(674, 56);
            this.ContactListBox.Name = "ContactListBox";
            this.ContactListBox.Size = new System.Drawing.Size(105, 259);
            this.ContactListBox.TabIndex = 12;
            // 
            // ContactListLabel
            // 
            this.ContactListLabel.AutoSize = true;
            this.ContactListLabel.Location = new System.Drawing.Point(671, 21);
            this.ContactListLabel.Name = "ContactListLabel";
            this.ContactListLabel.Size = new System.Drawing.Size(63, 13);
            this.ContactListLabel.TabIndex = 13;
            this.ContactListLabel.Text = "Contact List";
            // 
            // MainViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 410);
            this.Controls.Add(this.ContactListLabel);
            this.Controls.Add(this.ContactListBox);
            this.Controls.Add(this.startChatButton);
            this.Controls.Add(this.addToChatButton);
            this.Controls.Add(this.addContactButton);
            this.Controls.Add(this.UserListBox);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.MessageListBox);
            this.Controls.Add(this.ChatLabel);
            this.Controls.Add(this.UserListLabel);
            this.Controls.Add(this.RoomListBox);
            this.Controls.Add(this.RoomListLabel);
            this.Name = "MainViewForm";
            this.Text = "Online Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label RoomListLabel;
        private System.Windows.Forms.ListBox RoomListBox;
        private System.Windows.Forms.Label UserListLabel;
        private System.Windows.Forms.Label ChatLabel;
        private System.Windows.Forms.ListBox MessageListBox;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.CheckedListBox UserListBox;
        private System.Windows.Forms.Button addContactButton;
        private System.Windows.Forms.Button addToChatButton;
        private System.Windows.Forms.Button startChatButton;
        private System.Windows.Forms.CheckedListBox ContactListBox;
        private System.Windows.Forms.Label ContactListLabel;
    }
}

