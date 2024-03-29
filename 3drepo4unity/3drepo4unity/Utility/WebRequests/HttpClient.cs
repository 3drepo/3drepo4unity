﻿/*
 *	Copyright (C) 2018 3D Repo Ltd
 *
 *	This program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU Affero General Public License as
 *	published by the Free Software Foundation, either version 3 of the
 *	License, or (at your option) any later version.
 *
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU Affero General Public License for more details.
 *
 *	You should have received a copy of the GNU Affero General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LitJson;

namespace RepoForUnity.Utility
{
    // <summary>
    /// HTTPClient is a helper class mainly designed to encapsulate creation and despatch/recipet of HTTPRequest/Response instances. HTTPClient does not have the
    /// concept of a login function, however it does detect when the server sends a cookie in a response, and will store this, sending it with any future requests.
    /// Therefore an HTTPClient instance should be retained for the lifetime of a users session.
    /// </summary>
    internal class HttpClient
    {
        internal IWebProxy proxy;
        internal int timeout_ms;

        internal string apiKey;

        //internal string cookie = null;
        private CookieContainer cookies;

        private Dictionary<string, Cookie> cookieDict;

        protected string domain;

        internal HttpClient(string domain)
        {
            if (domain[domain.Length - 1] != '/') domain += "/";

            this.domain = domain;

            //do not autodetect proxy - it takes ages!
            proxy = null;
            timeout_ms = 1200000;

            //This function is not called on Windows because Windows does not allow us to override the check. It will be called on mono however - even though we
            //cant see it because we cant target mono....yuck
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
            ServicePointManager.DefaultConnectionLimit = 20;

            cookieDict = new Dictionary<string, Cookie>();

            cookies = new CookieContainer();

            apiKey = null;
        }

        internal void SetApiKey(string apiKey)
        {
            this.apiKey = apiKey;
        }

        private void AppendApiKey(ref string uri)
        {
            if(apiKey != null)
            {
                uri += $"?key={apiKey}";
            }
        }

        /// <summary>
        /// Serialises an object to JSON and sends it to the specified URI, then deserialises the response into another JSON object and returns it. Both JSON objects
        /// must the strongly typed. If a cookie has previously been received, it will be added to the header of this POST request. If the response contains a cookie,
        /// it will be stored in this instance of HTTPClient for future requests.
        /// </summary>
        /// <typeparam name="T_in">The type of the object being sent</typeparam>
        /// <typeparam name="T_out">The type of the object being received</typeparam>
        /// <param name="uri">The URI to Post the serialised string from data to</param>
        /// <param name="data">The object (of type T_in) to serialise into a JSON string</param>
        /// <returns>The response from the server, deserialised into T_out</returns>
        protected T_out HttpPostJson<T_in, T_out>(string uri, T_in data)
        {
            AppendApiKey(ref uri);

            // put together the json object with the login form data
            string parameters = JsonMapper.ToJson(data);
            byte[] postDataBuffer = Encoding.UTF8.GetBytes(parameters);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Proxy = proxy;
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.ContentLength = postDataBuffer.Length;
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ReadWriteTimeout = timeout_ms;
            request.Timeout = timeout_ms;
            request.CookieContainer = cookies;
            Console.WriteLine("POST " + uri + " data: " + parameters);

            Stream postDataStream = request.GetRequestStream();

            postDataStream.Write(postDataBuffer, 0, postDataBuffer.Length);
            postDataStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            StreamReader responseReader = new StreamReader(responseStream);
            string responseData = responseReader.ReadToEnd();

            Cookie responseCookie = response.Cookies[0];
            string responseDomain = responseCookie.Domain;

            // Only assume one cookie per domain
            if (!cookieDict.ContainsKey(responseDomain))
            {
                cookieDict.Add(responseCookie.Domain, responseCookie);
                responseCookie.Domain = "." + responseCookie.Domain;
            }

            Uri myUri = new Uri(uri);

            foreach (KeyValuePair<string, Cookie> entry in cookieDict)
            {
                int uriHostLength = myUri.Host.Length;

                if (myUri.Host.Substring(uriHostLength - responseDomain.Length, responseDomain.Length) == entry.Key)
                {
                    cookies.Add(myUri, entry.Value);
                }
            }

            return JsonMapper.ToObject<T_out>(responseData);
        }

        /// <summary>
        /// Sends a GET request for json data to the specified URI and deserialises the response string into an object of type T.
        /// If this HTTPClient instance has a cookie, it is added to the GET request header.
        /// </summary>
        protected T HttpGetJson<T>(string uri, int tries = 1)
        {
            AppendApiKey(ref uri);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Proxy = proxy;
            request.Method = "GET";
            request.Accept = "application/json";
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ReadWriteTimeout = timeout_ms;
            request.Timeout = timeout_ms;
            request.CookieContainer = cookies;
            Console.WriteLine("GET " + uri + " TRY: " + tries);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string responseData = responseReader.ReadToEnd();

                JsonMapper.RegisterImporter<Double, Single>((Double value) =>
                {
                    return (Single)value;
                });
                return JsonMapper.ToObject<T>(responseData);
            }
            catch (WebException)
            {
                if (--tries == 0)
                    throw;

                return HttpGetJson<T>(uri, tries);
            }
        }

        /// <summary>
        /// Sends a GET request to the specified URI and returns a Stream of bytes.
        /// If this HTTPClient instance has a cookie, it is added to the GET request header.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected virtual Stream HttpGetURI(string uri, int tries = 1)
        {
            AppendApiKey(ref uri);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Proxy = proxy;
            request.Method = "GET";
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ReadWriteTimeout = timeout_ms;
            request.Timeout = timeout_ms;
            request.CookieContainer = cookies;
            Console.WriteLine("GET " + uri + " TRY: " + tries);

            try
            {
                MemoryStream memStream;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    memStream = new MemoryStream();

                    byte[] buffer = new byte[1024];
                    int byteCount;
                    int total = 0;

                    var ns = response.GetResponseStream();
                    do
                    {
                        byteCount = ns.Read(buffer, 0, buffer.Length);
                        memStream.Write(buffer, 0, byteCount);

                        total += byteCount;
                    } while (byteCount > 0);
                    request.Abort();
                }

                return memStream;
            }
            catch (WebException)
            {
                if (--tries == 0)
                    throw;

                return HttpGetURI(uri, tries);
            }
        }

        protected static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Return true if the server certificate is ok
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            bool acceptCertificate = true;
            string msg = "The server could not be validated for the following reason(s):\r\n";

            //The server did not present a certificate
            if ((sslPolicyErrors &
                    SslPolicyErrors.RemoteCertificateNotAvailable) == SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                msg = msg + "\r\n    -The server did not present a certificate.\r\n";
                acceptCertificate = false;
            }
            else
            {
                //The certificate does not match the server name
                if ((sslPolicyErrors &
                        SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    msg = msg + "\r\n    -The certificate name does not match the authenticated name.\r\n";
                    acceptCertificate = false;
                }

                //There is some other problem with the certificate
                if ((sslPolicyErrors &
                        SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    foreach (X509ChainStatus item in chain.ChainStatus)
                    {
                        if (item.Status != X509ChainStatusFlags.RevocationStatusUnknown &&
                            item.Status != X509ChainStatusFlags.OfflineRevocation)
                            break;

                        if (item.Status != X509ChainStatusFlags.NoError)
                        {
                            msg = msg + "\r\n    -" + item.StatusInformation;
                            acceptCertificate = false;
                        }
                    }
                }
            }

            //If Validation failed, present message box
            if (acceptCertificate == false)
            {
                msg = msg + "\r\nDo you wish to override the security check?";
                //          if (MessageBox.Show(msg, "Security Alert: Server could not be validated",
                //                       MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                acceptCertificate = true;
            }

            return acceptCertificate;
        }
    }
}