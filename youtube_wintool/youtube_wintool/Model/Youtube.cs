using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace youtube_wintool.Model
{
    class Youtube
    {
        private static string baseurl = "https://www.googleapis.com/youtube/v3";
        private static string youtubeKey = System.Configuration.ConfigurationManager.AppSettings["youtubekey"];

        public static async Task<string> ChannelList(string channelName= "TheKarinaBear")
        {
            InitChecking();
            string part = "snippet%2CcontentDetails%2Cstatistics";//%2C = ,
            string url = $"{baseurl}/channels/?forUsername={channelName}&part={part}&key={youtubeKey}";
            var result = await HttpGetAsync(url);
            return result;
        }
        public static async Task<List<string>> PlaylistList(string listId, string continueToken=null, List<string> result=null)
        {
            if (result == null)
                result = new List<string>();
            if (continueToken == null)
                InitChecking();
            if (string.IsNullOrEmpty(listId))
                throw new ArgumentException("Playlist Id is missing.");
            string part = "snippet";//%2C = ,
            string url = $"{baseurl}/playlistItems/?playlistId={listId}&maxResults=50&part={part}&pageToken={continueToken}&key={youtubeKey}";
            var json = await HttpGetAsync(url);
            var jsonobj = JObject.Parse(json);
            //process data
            var ddd = jsonobj["items"];
            foreach (var video in jsonobj["items"])
            {
                result.Add(video.ToString());
            }

            if (jsonobj["nextPageToken"]==null)
            {
                //no more pages
                return result;
            }
            else
            {
                //continue to retrieve a new page of items
                return await PlaylistList(listId, jsonobj["nextPageToken"].ToString(), result);
            }
        }
        private static async Task<string> HttpGetAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                using (var resp = await request.GetResponseAsync())
                {
                    using (var responseStream = resp.GetResponseStream())
                    {
                        var content = new MemoryStream();
                        await responseStream.CopyToAsync(content);
                        string json = Encoding.UTF8.GetString(content.ToArray());
                        return json;
                    }
                }
            }
            catch (WebException ex)
            {
                //log it and throw it
                throw ex;
            }
        }
        private static void InitChecking()
        {
            if (string.IsNullOrEmpty(youtubeKey))
                throw new ArgumentException("youtube appkey is missing in the App.config file.");
        }
    }
}
