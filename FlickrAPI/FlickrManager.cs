using Newtonsoft.Json;
using OAuth;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace FlickrAPI
{
    public class FlickrManager
    {
        private OAuthRequest client;
        private readonly string apiKey = "your_api_key";
        private readonly string userId = "your_userId";
        public void GetOAuth(string requestUrl)
        {
            client = new OAuthRequest
            {
                Method = "POST",
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = apiKey,
                ConsumerSecret = "your_consumersecret",
                RequestUrl = requestUrl,
                Version = "1.0a",
                Verifier = "your_writer",
                CallbackUrl = "your_callbackUrl",
                Token = "your_token",
                TokenSecret = "your_token_secret",
            };
        }

        public FlickrResponse UploadPhoto(byte[] sendPhoto, string fileName, string contentType)
        {
            string auth = client.GetAuthorizationHeader();
            var restClient = new RestClient(client.RequestUrl);
            restClient.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", auth);
            request.AddFileBytes("photo", sendPhoto, fileName, contentType);
            IRestResponse response = restClient.Execute(request);
            var xmlDocumant = GetXmlDocument(response.Content);
            // To convert an XML node contained in string xml into a JSON string
            return new FlickrResponse
            {
                Response = GetElementValue("photoid", xmlDocumant),
                IsSucces = ResponseStatus(response.Content)
            };
        }

        public FlickrResponse DeletePhoto(IDictionary<string, string> parameters, string photoId)
        {
            parameters.Add("format", "json");
            parameters.Add("nojsoncallback", "1");
            string auth = client.GetAuthorizationHeader(parameters);
            var restClient = new RestClient(client.RequestUrl
                + "?method=flickr.photos.delete&photo_id=" + photoId
                + "&format=json&nojsoncallback=1");
            restClient.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", auth);
            IRestResponse response = restClient.Execute(request);
            return new FlickrResponse 
            { 
                Response = response.Content, 
                IsSucces = response.IsSuccessful 
            };
        }

        public string GetPhotoList(int page, int size)
        {
            var requestUrl = ("https://www.flickr.com/services/rest/?" +
                "method=flickr.people.getPhotos&" +
                "api_key=" + apiKey + "&" +
                "user_id=" + userId + "&" +
                "per_page=" + size + "&" +
                "page=" + page + "&" +
                "format=json&nojsoncallback=1");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                response.Close();
                return responseFromServer;
            }
        }

        public string GetPhotoInfo(string photoId)
        {
            var requestUrl = ("https://www.flickr.com/services/rest/?" +
                "method=flickr.photos.getInfo&" +
                "api_key=" + apiKey + "&" +
                "photo_id=" + photoId + "&" +
                "format=json&nojsoncallback=1");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                response.Close();
                return responseFromServer;
            }
        }

        public string GetPhotoUrl(string getPhotoInfoResponse)
        {
            var photoInfo = JsonDeserialize<FlickrPhotoInfo>(getPhotoInfoResponse).Photo;
            var createPhotoUrl = string.Format("https://live.staticflickr.com/{0}/{1}_{2}.jpg",
                photoInfo.Server,photoInfo.Id,photoInfo.Secret);
            return createPhotoUrl;
        }

        private T JsonDeserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            { NullValueHandling = NullValueHandling.Ignore });
        }

        private string GetElementValue(string element, XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("//" + element + "[1]");
            if (node != null)
                return node.InnerText;
            else
                return null;
        }

        private XmlDocument GetXmlDocument(string responseXml)
        {
            var document = new XmlDocument();
            document.LoadXml(responseXml);

            return document;
        }

        private bool ResponseStatus(string originalXml)
        {
            var reader = XmlReader.Create(new StringReader(originalXml), new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            if (!reader.ReadToDescendant("rsp"))
            {
                throw new Exception("Unable to find response element 'rsp' in Flickr response");
            }
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == "stat" && reader.Value == "fail")
                    return false;
            }
            return true;
        }
    }
}
