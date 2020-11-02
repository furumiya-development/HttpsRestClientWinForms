using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;

namespace HttpsRestClientWinForms
{
    /// <summary>メインフォーム</summary>
    /// <remarks>Nuget Package : Install-Package Microsoft.Extensions.Http -Version 3.1.9</remarks>
    public partial class Form1 : Form
    {
        private static System.Net.Http.HttpClient httpClient;
        private readonly System.Threading.SynchronizationContext synchronizationContext;
        private DataGridView dataGridView1;
        private BindingSource bindingSource1;
        private Dictionary<string, Label> LabelDic = new Dictionary<string, Label>();
        private TextBox textBoxUri;
        private ComboBox cmbHttpVer;
        private ComboBox cmbSslProtocol;
        private TextBox textBoxReqBody;
        private RichTextBox richTextBox1;
        private Label labelNumId;
        private TextBox textBoxShohinNum;
        private TextBox textBoxShohinName;
        private TextBox textBoxNote;
        private Button buttonQuery;
        private Button buttonInsert;
        private Button buttonUpdate;
        private Button buttonDelete;
        private bool Fauthentication = false; // 認証済みOK/NG
        private string CONTENT_TYPE = @"application/json";
        private HttpSettings Settings = new HttpSettings();

        public Form1(System.Net.Http.IHttpClientFactory factory)
        {
            InitializeComponent();
            FormDesignSetting();
            synchronizationContext = System.Threading.SynchronizationContext.Current;
            httpClient = factory.CreateClient(); //using System.Net.Httpが必要
            httpClient.Timeout = TimeSpan.FromMilliseconds(-1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.cmbSslProtocol.DataSource = Settings.SslProtocol;
            this.cmbSslProtocol.DataBindings.Add(new Binding("SelectedItem", Settings, "SslProtocolVerStr"));
            this.cmbHttpVer.DataSource = Settings.HttpVersion;
            this.cmbHttpVer.DataBindings.Add(new Binding("SelectedItem", Settings, "HttpVerStr"));
            cmbHttpVer.SelectedIndex = 2;
            cmbSslProtocol.SelectedIndex = 2;
        }

        private async void buttonQuery_Click(object sender, EventArgs e)
        {
            var response = new HttpResponseMessage();
            var uri = new Uri(textBoxUri.Text);
            string JsonData = "";
            bool Rtype = true;

            if (Rtype == true)
            {
                response = await HttpRequest(HttpMethod.Get, uri, @"dummy text");
                if (Fauthentication)
                    JsonData = await response.Content.ReadAsStringAsync();
            }
            else
            {
                JsonData = await httpClient.GetStringAsync(uri);
            }
            if (Fauthentication)
            {
                List<ShohinMap> list = System.Text.Json.JsonSerializer.Deserialize<List<ShohinMap>>(JsonData);
                bindingSource1.DataSource = list;
                dataGridView1.DataSource = bindingSource1;
                DataGridSetting();
            }

        }

        private async void buttonInsert_Click(object sender, EventArgs e)
        {
            var uri = new Uri(textBoxUri.Text);
            string JsonStr = CreateJsonStr();
            await HttpRequest(HttpMethod.Post, uri, JsonStr);
        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            var uri = new Uri(textBoxUri.Text + "/" + labelNumId.Text);
            string JsonStr = CreateJsonStr();
            await HttpRequest(HttpMethod.Put, uri, JsonStr);
        }

        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            var uri = new Uri(textBoxUri.Text + "/" + labelNumId.Text);
            await HttpRequest(HttpMethod.Delete, uri, @"dummy text");
        }

        private void dataGridView1_CellClick(Object sender, DataGridViewCellEventArgs e)
        {
            labelNumId.Text = dataGridView1.CurrentRow.Cells["NumId"].Value.ToString();
            textBoxShohinNum.Text = dataGridView1.CurrentRow.Cells["ShohinCode"].Value.ToString();
            textBoxShohinName.Text = dataGridView1.CurrentRow.Cells["ShohinName"].Value.ToString();
            textBoxNote.Text = dataGridView1.CurrentRow.Cells["Note"].Value.ToString();
        }

        private string CreateJsonStr()
        {
            string Str = "{ \"shohinCode\":" + textBoxShohinNum.Text;
            Str += ", \"shohinName\": \"" + textBoxShohinName.Text + "\", \"note\": \"" + textBoxNote.Text + "\" }";

            return Str;
        }

        private async Task<HttpResponseMessage> HttpRequest(HttpMethod method, Uri uri, string content)
        {
            var response = new HttpResponseMessage();
            string resStr = "";

            response = await httpClient.SendAsync(RequestSetting(method, uri, content));
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent)
            {
                resStr = await response.Content.ReadAsStringAsync();
                richTextBox1.AppendText(response.Headers.ToString());
            }
            else
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized: // 認証が必要
                        richTextBox1.AppendText(response.Headers.ToString());
                        new FormAuth().ShowDialog();
                        Fauthentication = true;
                        response = await httpClient.SendAsync(RequestSetting(method, uri, content));
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Authentication.UserID = "";
                            Authentication.Password = "";
                            Fauthentication = false;
                            MessageBox.Show(response.Headers.ToString(), "認証に失敗しました。");
                        }
                        else
                        {
                            resStr = await response.Content.ReadAsStringAsync();
                            richTextBox1.AppendText(response.Headers.ToString());
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                        resStr = await response.Content.ReadAsStringAsync();
                        richTextBox1.AppendText(response.Headers.ToString());
                        break;
                    default: //その他のエラー
                        MessageBox.Show(response.Headers.ToString(), response.StatusCode.ToString());
                        break;
                }
            }
            textBoxReqBody.Text = resStr;

            return response;
        }

        private HttpRequestMessage RequestSetting(HttpMethod method, Uri uri, string content)
        {
            ServicePointManager.SecurityProtocol = Settings.SslProtocolVer;
            var request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = uri;
            request.Version = Settings.HttpVer;
            request.Content = new StringContent(content, Encoding.UTF8, CONTENT_TYPE);
            if (Fauthentication)
            {
                string BasicStr = Authentication.BasicRequestHeader();
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", BasicStr);
            }

            return request;
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

            this.Name = "Form1";
            this.Text = "RESTful API クライアント(HttpClient)";
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(500, 200);
            this.Size = new Size(800, 600);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Load += new System.EventHandler(this.Form1_Load);

            this.dataGridView1 = new DataGridView();
            this.dataGridView1.TabIndex = 7;
            dataGridView1 = (DataGridView)ControlsSetting(dataGridView1, "dataGridView1", 25, 25, 730, 200);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            //this.Controls.Add(this.dataGridView1);

            this.bindingSource1 = new BindingSource();

            LabelsSetting(@"labelUri", @"URI:", 25, 235, 50, 25);

            this.textBoxUri = new TextBox();
            this.textBoxUri.TabIndex = 0;
            this.textBoxUri.Text = @"https://localhost:5001/api/ShohinEntities";
            textBoxUri = (TextBox)ControlsSetting(textBoxUri, @"textBoxUri", 75, 235, 300, 25);
            //this.Controls.Add(this.textBoxUri);

            LabelsSetting(@"labelHttpVer", @"HTTPバージョン：", 375, 235, 110, 25);

            this.cmbHttpVer = new ComboBox();
            cmbHttpVer = (ComboBox)ControlsSetting(cmbHttpVer, @"cmbHttpVer", 485, 235, 80, 25);

            LabelsSetting(@"labelSslProtocol", @"SSLプロトコル：", 575, 235, 100, 25);

            this.cmbSslProtocol = new ComboBox();
            cmbSslProtocol = (ComboBox)ControlsSetting(cmbSslProtocol, @"cmbSslProtocol", 675, 235, 80, 25);

            LabelsSetting(@"labelReqBody", @"レスポンスBody:", 25, 260, 80, 25);

            this.textBoxReqBody = new TextBox();
            this.textBoxReqBody.TabIndex = 1;
            textBoxReqBody = (TextBox)ControlsSetting(textBoxReqBody, @"textBoxReqBody", 105, 260, 600, 25);
            //this.Controls.Add(this.textBoxReqBody);

            this.richTextBox1 = new RichTextBox();
            this.richTextBox1.TabIndex = 8;
            richTextBox1 = (RichTextBox)ControlsSetting(richTextBox1, @"richTextBox1", 25, 285, 350, 200);
            //this.Controls.Add(this.richTextBox1);

            LabelsSetting(@"label1", @"商品ID:", 400, 300, 100, 25);
            LabelsSetting(@"label2", @"商品番号:", 400, 350, 100, 25);
            LabelsSetting(@"label3", @"商品名:", 400, 400, 100, 25);
            LabelsSetting(@"label4", @"備考:", 400, 450, 50, 25);

            this.labelNumId = new Label();
            this.labelNumId.AutoSize = false;
            this.labelNumId.TextAlign = ContentAlignment.TopRight;
            labelNumId = (Label)ControlsSetting(labelNumId, @"labelNumId", 700, 300, 50, 25);
            //this.Controls.Add(this.labelNumId);

            this.textBoxShohinNum = new TextBox();
            this.textBoxShohinNum.TabIndex = 2;
            textBoxShohinNum = (TextBox)ControlsSetting(textBoxShohinNum, @"textBoxShohinNum", 600, 350, 150, 25);
            //this.Controls.Add(this.textBoxShohinNum);

            this.textBoxShohinName = new TextBox();
            this.textBoxShohinName.TabIndex = 3;
            textBoxShohinName = (TextBox)ControlsSetting(textBoxShohinName, @"textBoxShohinName", 550, 400, 200, 25);
            //this.Controls.Add(this.textBoxShohinName);

            this.textBoxNote = new TextBox();
            this.textBoxNote.TabIndex = 4;
            textBoxNote = (TextBox)ControlsSetting(textBoxNote, @"textBoxNote", 450, 450, 300, 25);
            //this.Controls.Add(this.textBoxNote);

            this.buttonQuery = new Button();
            this.buttonQuery.Text = @"抽出(GET)";
            this.buttonQuery.TabIndex = 5;
            this.buttonQuery.UseVisualStyleBackColor = true;
            buttonQuery = (Button)ControlsSetting(buttonQuery, @"buttonQuery", 25, 490, 150, 50);
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            //this.Controls.Add(this.buttonQuery);

            this.buttonInsert = new Button();
            this.buttonInsert.Text = @"追加(POST)";
            this.buttonInsert.TabIndex = 6;
            this.buttonInsert.UseVisualStyleBackColor = true;
            buttonInsert = (Button)ControlsSetting(buttonInsert, @"buttonInsert", 220, 490, 150, 50);
            this.buttonInsert.Click += new System.EventHandler(this.buttonInsert_Click);
            //this.Controls.Add(this.buttonInsert);

            this.buttonUpdate = new Button();
            this.buttonUpdate.Text = @"更新(PUT)";
            this.buttonUpdate.TabIndex = 7;
            this.buttonUpdate.UseVisualStyleBackColor = true;
            buttonUpdate = (Button)ControlsSetting(buttonUpdate, @"buttonUpdate", 410, 490, 150, 50);
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            //this.Controls.Add(this.buttonUpdate);

            this.buttonDelete = new Button();
            this.buttonDelete.Text = @"削除(DELETE)";
            this.buttonDelete.TabIndex = 8;
            this.buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete = (Button)ControlsSetting(buttonDelete, @"buttonDelete", 600, 490, 150, 50);
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            //this.Controls.Add(this.buttonDelete);

        }

        private void DataGridSetting()
        {
            dataGridView1.Columns["NumId"].HeaderText = "商品ID";
            dataGridView1.Columns["ShohinCode"].HeaderText = "商品番号";
            dataGridView1.Columns["ShohinName"].HeaderText = "商品名";
            dataGridView1.Columns["EditDate"].HeaderText = "編集日付";
            dataGridView1.Columns["EditTime"].HeaderText = "編集時刻";
            dataGridView1.Columns["Note"].HeaderText = "備考";
            dataGridView1.Columns["NumId"].Width = 70;
            dataGridView1.Columns["Note"].Width = 250;
            dataGridView1.Columns["EditDate"].DefaultCellStyle.Format = "0000/00/00";
            dataGridView1.Columns["EditTime"].DefaultCellStyle.Format = "00:00:00";
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
        }

        private void TextBoxClear()
        {
            labelNumId.Text = "";
            textBoxShohinNum.Text = "";
            textBoxShohinName.Text = "";
            textBoxNote.Text = "";
        }
    }
}