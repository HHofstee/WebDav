using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TallComponents.com
{
    public class WebDav
    {
        private string username;
        private string password;

        public WebDav(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        private WebRequest GetWebRequest(string url, string method = "GET")
        {
            var request = WebRequest.Create(url);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = method;
            request.PreAuthenticate = true;
            return request;
        }

        public async Task PutFile(string url, string content, bool overwrite = false)
        {
            var request = (HttpWebRequest)GetWebRequest(url, "PUT");

            request.Headers.Add("Overwrite", overwrite ? "T": "F");

            request.ContentLength = content.Length;
            request.SendChunked = true;

            // write the file
            var requestStream = request.GetRequestStream();
            requestStream.Write(Encoding.UTF8.GetBytes((string)content), 0, content.Length);
            requestStream.Close();

            // get the response
            var response = (HttpWebResponse)await request.GetResponseAsync();
            Console.WriteLine("PUT Response: {0}", response.StatusDescription);
        }

        public async Task CopyFile(string fromUrl, string toUrl, bool overwrite = false)
        {
            var request = GetWebRequest(fromUrl, "COPY");

            request.Headers.Add("Destination", toUrl);
            request.Headers.Add("Overwrite", overwrite ? "T" : "F");

            // Retrieve the response.
            var httpCopyResponse = (HttpWebResponse)await request.GetResponseAsync();

            // Write the response status to the console.
            Console.WriteLine("COPY Response: {0}", httpCopyResponse.StatusDescription);
        }

        public async Task MoveFile(string fromUrl, string toUrl, bool overwrite = false)
        {
            var request = GetWebRequest(fromUrl, "MOVE");

            request.Headers.Add("Destination", toUrl);
            request.Headers.Add("Overwrite", overwrite ? "T" : "F");

            // Retrieve the response.
            var response = (HttpWebResponse)await request.GetResponseAsync();

            // Write the response status to the console.
            Console.WriteLine("MOVE Response: {0}", response.StatusDescription);
        }

        public async Task GetFile(string url)
        {
            var request = GetWebRequest(url);

            request.Headers.Add("Translate", "F");

            // Retrieve the response.
            var response = (HttpWebResponse)await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();
            var streamReader = new StreamReader(responseStream, Encoding.UTF8);

            // Write the response status to the console.
            Console.WriteLine("GET Response: {0}", response.StatusDescription);
            Console.WriteLine("  Response Length: {0}", response.ContentLength);
            Console.WriteLine("  Response Text: {0}", streamReader.ReadToEnd());

            // Close the response streams.
            streamReader.Close();
            responseStream.Close();
        }

        public async Task DeleteFile(string url)
        {
            var request = GetWebRequest(url, "DELETE");

            var response = (HttpWebResponse)await request.GetResponseAsync();
            Console.WriteLine("DELETE Response: {0}", response.StatusDescription);
        }

        public async Task GetFolder(string url)
        {
            var request = GetWebRequest(url, "PROPFIND");

            string query = @"<?xml version=""1.0"" encoding=""utf-8""?>   <propfind xmlns=""DAV:"">     <propname/>   </propfind>";
            var bytes = Encoding.UTF8.GetBytes((string)query);
            request.ContentLength = bytes.Length;
            request.ContentType = @"application/xml; charset=""utf-8""";

            request.Headers.Add("Depth", "1");

            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            var response = await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();

            var reader = new XmlTextReader(responseStream);

            string output = "";

            while (reader.Read())
            {
                if (reader.Value.Contains("http"))
                {
                    output += reader.Value.ToString() + "\n";
                }
            }

            Console.WriteLine(output);

            reader.Close();
            responseStream.Close();
            response.Close();
        }

        public async Task CreateFolder(string url)
        {
            var request = GetWebRequest(url, "MKCOL");

            var response = (HttpWebResponse)await request.GetResponseAsync();
            Console.WriteLine("MKCOL Response: {0}", response.StatusDescription);
        }

        public async Task DeleteFolder(string url)
        {
            var request = GetWebRequest(url, "DELETE");

            var response = (HttpWebResponse)await request.GetResponseAsync();
            Console.WriteLine("DELETE Response: {0}", response.StatusDescription);
        }
        
    }
}