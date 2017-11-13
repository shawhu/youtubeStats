using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using youtube_wintool.Model;

namespace youtube_wintool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //check resolution
            //check and init all sdk
            //...
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var result = await Youtube.ChannelList();
            txtResult.Text = result;
            //binding results
            var jsonobj = Newtonsoft.Json.Linq.JObject.Parse(result);
            lblId.Text = jsonobj["items"][0]["id"].ToString();
            lblTotalVideos.Text = ReturnCommaNumber(jsonobj["items"][0]["statistics"]["videoCount"].ToString());
            lblTotalSubscribers.Text = ReturnCommaNumber(jsonobj["items"][0]["statistics"]["subscriberCount"].ToString());
            lblViewCount.Text = ReturnCommaNumber(jsonobj["items"][0]["statistics"]["viewCount"].ToString());

            //retrieve all uploads
            string playlistUploads = jsonobj["items"][0]["contentDetails"]["relatedPlaylists"]["uploads"].ToString();
            var results = await Youtube.PlaylistList(playlistUploads);
            Console.WriteLine(results.Count.ToString());
            foreach (var video in results)
            {
                txtResult.Text += video+"\r\n";
            }
        }














        private string ReturnCommaNumber(string strvalue)
        {
            int value = int.Parse(strvalue);
            return value.ToString("0,0", CultureInfo.InvariantCulture);
        }
        private string ReturnCommaNumber(int value)
        {
            return value.ToString("0,0", CultureInfo.InvariantCulture);
        }
    }
}
