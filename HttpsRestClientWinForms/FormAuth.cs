using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HttpsRestClientWinForms
{
    public partial class FormAuth : Form
    {
        private Dictionary<string, Label> LabelDic = new Dictionary<string, Label>();
        private TextBox textBoxUserName;
        private TextBox textBoxPassword;
        private Button buttonAuth;
        private Button buttonCancel;

        public FormAuth()
        {
            InitializeComponent();
            FormDesignSetting();
        }

        private void FormAuth_Load(object sender, EventArgs e)
        {
        }

        private void buttonAuth_Click(object sender, EventArgs e)
        {
            Authentication.UserID = textBoxUserName.Text;
            Authentication.Password = textBoxPassword.Text;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Label LabelsSetting(string name, string txt, int x, int y, int w, int h)
        {
            Label label = new Label();
            label.Name = name;
            LabelDic.Add(label.Name, label);
            label.AutoSize = false;
            label.Text = txt;
            label.Location = new Point(x, y);
            label.Size = new Size(w, h);
            Controls.Add(label);

            return label;
        }

        private Control ControlsSetting(Control ctl, string name, int x, int y, int w, int h)
        {
            ctl.Name = name;
            ctl.Location = new Point(x, y);
            ctl.Size = new Size(w, h);
            Controls.Add(ctl);

            return ctl;
        }

        private void FormDesignSetting()
        {

            this.Name = "FormAuth";
            this.Text = "認証";
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(600, 250);
            this.Size = new Size(450, 250);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Load += new System.EventHandler(this.FormAuth_Load);

            LabelsSetting(@"labelMessage", @"認証が必要です。ユーザー名とパスワードを入力して下さい。", 25, 25, 350, 25);
            LabelsSetting(@"labelUserName", @"ユーザー:", 25, 50, 80, 25);
            LabelsSetting(@"labelPassword", @"パスワード:", 25, 100, 80, 25);

            textBoxUserName = new TextBox();
            textBoxUserName.TabIndex = 0;
            textBoxUserName = (TextBox)ControlsSetting(textBoxUserName, @"textBoxUserName", 120, 50, 250, 25);

            textBoxPassword = new TextBox();
            textBoxPassword.TabIndex = 1;
            textBoxPassword.PasswordChar = '*';
            textBoxPassword = (TextBox)ControlsSetting(textBoxPassword, @"textBoxPassword", 120, 100, 250, 25);

            this.buttonAuth = new Button();
            this.buttonAuth.Text = @"認証";
            this.buttonAuth.TabIndex = 2;
            this.buttonAuth.UseVisualStyleBackColor = true;
            buttonAuth = (Button)ControlsSetting(buttonAuth, @"buttonAuth", 25, 150, 150, 50);
            this.buttonAuth.Click += new System.EventHandler(this.buttonAuth_Click);

            this.buttonCancel = new Button();
            this.buttonCancel.Text = @"キャンセル";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel = (Button)ControlsSetting(buttonCancel, @"buttonCancel", 250, 150, 150, 50);
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
        }
    }
}