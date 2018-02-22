﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_5_3
using UnityEngine.Experimental.Networking;
#endif

namespace EasyFeedback.APIs
{
    public class Trello
    {
        #region keys
        public const string TEMPLATE_BOARD_ID = "589d1b02a4856195b7cc31c9";
        public const string APP_KEY = "9babe077311b8a24fddaebb73de1df6a";
        public const string API_URI = "https://trello.com/1";
        #endregion

        public const int MAX_CHAR_LENGTH = 16384;
        public const string CATEGORY_TAG = "(EF)";

        public static string AuthURL
        {
            get { return string.Format("{0}/authorize?expiration=never&scope=read,write,account&response_type=token&name=Easy%20Feedback&key={1}", API_URI, APP_KEY); }
        }

        private string token;

        public UnityWebRequest LastRequest;
        public AddCardResponse LastAddCardResponse;

        public bool UploadError;
        public bool IsDoneUploading;
        public Exception UploadException;
        public string ErrorMessage;

        public Trello(string token)
        {
            this.token = token;
        }

        /// <summary>
        /// Returns a fully formed and authenticated request URI for the Trello API path provided
        /// </summary>
        /// <param name="apiPath">The Trello API endpoint path (starting with /)</param>
        /// <returns></returns>
        public string getURI(string apiPath)
        {
            return string.Format("{0}{1}?key={2}&token={3}", API_URI, apiPath, APP_KEY, token);
        }

        /// <summary>
        /// Sends a request 
        /// </summary>
        /// <param name="uri">The full URI of the request</param>
        /// <param name="method">The request method (GET/POST)</param>
        /// <param name="form">A WWWForm to include with the request</param>
        /// <param name="onFinished">Called when the request is finished</param>
        /// <param name="silent">Whether or not to log errors while making the request</param>
        /// <returns></returns>
        public IEnumerator makeRequestAsync(string uri, string method, WWWForm form = null, Action<string> onFinished = null, bool silent = false)
        {
            ErrorMessage = null;
            IsDoneUploading = false;
            UploadError = false;
            UploadException = null;

            AsyncWebRequest request = new AsyncWebRequest();

            if (method == "GET")
            {
                yield return request.Get(uri);
            }
            else if (method == "POST")
            {
                yield return request.Post(uri, form);
            }
            else
            {
                UploadError = true;
                ErrorMessage = "Unsupported request method: " + method;

                if (!silent)
                {
                    Debug.LogError("Error making request to Trello API!");
                    Debug.LogError(ErrorMessage);
                }

                yield break;
            }

            IsDoneUploading = true;

            string response = null;

            if (request.UploadException != null)
            {
                UploadException = request.UploadException;
                UploadError = true;
                ErrorMessage = "Error making request to Trello API!";

                // log error
                if (!silent)
                {
                    Debug.LogError("Error Making Request to Trello API!");
                    Debug.LogException(request.UploadException);
                }
            }
            else if (request.Request.responseCode != (long)HttpStatusCode.OK && !silent)
            {
                UploadError = true;
                ErrorMessage = "Trello API: Error " + request.Request.responseCode;

                // log error
                if (!silent)
                {
                    Debug.LogError("Error Making Request to Trello API!");
                    Debug.LogError("Status Code " + request.Request.responseCode + ": " + request.Request.downloadHandler.text);
                }
            }
            else if (request.RequestIsError && !silent)
            {
                UploadError = true;
                ErrorMessage = "Error making request to Trello API!";

                // log error
                if (!silent)
                {
                    Debug.LogError("Error Making Request to Trello API!");
                    Debug.LogError(request.Request.error);
                }
            }
            else
            {
                // get response text
                response = request.Request.downloadHandler.text;
            }

            // handle callback
            if (onFinished != null)
                onFinished(response);
        }

        /// <summary>
        /// Makes a request from the editor
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="silent">Whether or not to log errors while making the request</param>
        /// <returns></returns>
        public string makeRequestEditor(HttpWebRequest request, bool silent = false)
        {
            // set remote certificate validation callback if not set already
            if (ServicePointManager.ServerCertificateValidationCallback != RemoteCertificateValidationCallback)
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

            // get response string
            string respString = null;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                respString = reader.ReadToEnd();

                if (response.StatusCode != HttpStatusCode.OK && !silent)
                {
                    Debug.LogError("Error making request to Trello API");
                    Debug.LogError("Status code " + response.StatusCode + ": " + response.StatusDescription);
                    Debug.LogError(respString);
                }
            }

            return respString;
        }

        /// <summary>
        /// Makes a web request from the editor
        /// </summary>
        /// <param name="uri">The full uri of the request</param>
        /// <param name="method">The request method (GET/POST/etc.)</param>
        /// <param name="contentType">The content type of the included data</param>
        /// <param name="data">The data to be sent with the request</param>
        /// <param name="silent">Whether or not to log errors while making the request</param>
        /// <returns></returns>
        public string makeRequestEditor(string uri, string method, string contentType = null, byte[] data = null, bool silent = false)
        {
            // set remote certificate validation callback if not set already
            if (ServicePointManager.ServerCertificateValidationCallback != RemoteCertificateValidationCallback)
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

            // create the request
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = method;

            // set content type
            if (contentType != null)
                request.ContentType = contentType;

            // write data to request stream
            if(data != null)
            {
                request.ContentLength = data.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }
            }

            // send the request
            return makeRequestEditor(request, silent);
        }
        
        /// <summary>
        /// Makes a web request from the editor
        /// </summary>
        /// <param name="uri">The full uri of the request</param>
        /// <param name="method">The request method (GET/POST)</param>
        /// <param name="form">A WWWForm to include with the request</param>
        /// <param name="silent">Whether or not to log errors while making the request</param>
        /// <returns>The response text</returns>
        public string makeRequestEditor(string uri, string method, WWWForm form, bool silent = false)
        {
            // set remote certificate validation callback if not set already
            if (ServicePointManager.ServerCertificateValidationCallback != RemoteCertificateValidationCallback)
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

            // create the request
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = method;

            // send form with request if it is not null
            if (form != null)
                return makeRequestEditor(uri, method, "application/x-www-form-urlencoded", form.data, silent);

            // send the request
            return makeRequestEditor(request, silent);
        }

        /// <summary>
        /// Checks if a token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsValidToken(string token, bool silent = false)
        {
            // make a dummy request to Trello
            // (GET /1/members/me)
            string requestUrl = string.Format("{0}/members/me?key={1}&token={2}", API_URI, APP_KEY, token);

            // make request
            WWW request = new WWW(requestUrl);
            while (!request.isDone)
            {
                // wait until request is finished
#if UNITY_EDITOR
                if (!silent)
                    UnityEditor.EditorUtility.DisplayProgressBar("Testing token", "", request.progress);
#endif
            }

#if UNITY_EDITOR
            if (!silent)
                UnityEditor.EditorUtility.ClearProgressBar();
#endif

            return String.IsNullOrEmpty(request.error);
        }

        /// <summary>
        /// Adds a card to a board
        /// </summary>
        /// <param name="name">Title of the card</param>
        /// <param name="description">Description of the card</param>
        /// <param name="labels">Any labels on the card</param>
        /// <param name="list">The list the card belongs to</param>
        public IEnumerator AddCard(string name, string description, string labels, string list, byte[] fileSource = null)
        {
            IsDoneUploading = false;
            UploadError = false;
            ErrorMessage = string.Empty;
            UploadException = null;

            WWWForm form = new WWWForm();
            form.AddField("key", APP_KEY);
            form.AddField("token", token);
            form.AddField("name", name);

            if (description.Length > MAX_CHAR_LENGTH)
            {
                Debug.LogError("Card description length is higher than maximum length of " + MAX_CHAR_LENGTH + ". Truncating...");
                description = description.Remove(MAX_CHAR_LENGTH - 1);
            }

            form.AddField("desc", description);
            form.AddField("idLabels", labels);
            form.AddField("idList", list);
            if (fileSource != null)
                form.AddBinaryData("fileSource", fileSource);

            yield return makeRequestAsync("https://api.trello.com/1/cards", "POST", form, (resp) =>
            {
                if (resp != null)
                {
                    LastAddCardResponse = JsonUtility.FromJson<AddCardResponse>(resp);
                }
            });
        }

        public IEnumerator AddAttachmentAsync(string cardID, byte[] file = null, string url = null, string name = null, string mimeType = null)
        {
            IsDoneUploading = false;
            UploadError = false;
            ErrorMessage = string.Empty;
            UploadException = null;

            WWWForm form = new WWWForm();

            if (file != null)
                form.AddBinaryData("file", file, name ?? "file.dat");

            if (url != null)
                form.AddField("url", url);

            if (name != null)
                form.AddField("name", name);

            if (mimeType != null)
                form.AddField("mimeType", mimeType);

            string uri = getURI("/cards/" + cardID + "/attachments");

            yield return makeRequestAsync(uri, "POST", form);
        }

        public IEnumerator GetLabelsAsync(string boardID, Action<Label[]> onFinished)
        {
            string uri = getURI("/boards/" + boardID + "/labels");

            // make the request
            yield return makeRequestAsync(uri, "GET", null, (resp) =>
            {
                resp = resp.WrapToClass("labels");

                // create labels from json
                Label[] labels = JsonUtility.FromJson<LabelCollection>(resp).labels;

                // call onFinished
                onFinished(labels);
            });
        }

        public IEnumerator GetListsAsync(string boardID, Action<List[]> onFinished)
        {
            string uri = getURI("/boards/" + boardID + "/lists");

            // make the request
            yield return makeRequestAsync(uri, "GET", null, (resp) =>
            {
                // get json
                resp = resp.WrapToClass("lists");

                //Debug.Log(respString);

                // get lists from json
                List[] lists = JsonUtility.FromJson<ListCollection>(resp).lists;

                // call onFinished
                onFinished(lists);
            });
        }

        /// <summary>
        /// Editor-safe method for getting the lists on a board
        /// </summary>
        /// <param name="boardID"></param>
        /// <returns></returns>
        public List[] GetLists(string boardID)
        {
            // get the uri
            string uri = getURI("/boards/" + boardID + "/lists");

            // make the request
            string resp = makeRequestEditor(uri, "GET");

            // get json
            resp = resp.WrapToClass("lists");

            // get board array
            List[] lists = JsonUtility.FromJson<ListCollection>(resp).lists;

            return lists;
        }

        /// <summary>
        /// Editor-safe method for adding a board
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultLabels"></param>
        /// <param name="defaultLists"></param>
        /// <param name="desc"></param>
        /// <param name="idOrganization"></param>
        /// <param name="idBoardSource"></param>
        /// <param name="keepFromSource"></param>
        /// <param name="powerUps"></param>
        /// <param name="prefs"></param>
        /// <returns></returns>
        public Board AddBoard(
            string name,
            bool defaultLabels = true,
            bool defaultLists = true,
            string desc = null,
            string idOrganization = null,
            string idBoardSource = null,
            string keepFromSource = "all",
            string powerUps = "all",
            Prefs? prefs = null
            )
        {
            // prepare web request
            string uri = "https://api.trello.com/1/boards";
            WWWForm form = new WWWForm();

            // authentication
            form.AddField("key", APP_KEY);
            form.AddField("token", token);

            // card info
            form.AddField("name", name);
            form.AddField("defaultLabels", defaultLabels.ToString().ToLower());
            form.AddField("defaultLists", defaultLists.ToString().ToLower());

            if (desc != null)
                form.AddField("desc", desc);

            if (idOrganization != null)
                form.AddField("idOrganization", idOrganization);

            if (idBoardSource != null)
                form.AddField("idBoardSource", idBoardSource);

            form.AddField("keepFromSource", keepFromSource);
            form.AddField("powerUps", powerUps);

            if (prefs.HasValue)
            {
                Prefs p = prefs.Value;
                if (p.permissionLevel.HasValue)
                    form.AddField("prefs_permissionLevel", p.permissionLevel.Value.ToString());

                if (p.voting.HasValue)
                    form.AddField("prefs_voting", p.voting.Value.ToString());

                if (p.comments.HasValue)
                    form.AddField("prefs_comments", p.comments.Value.ToString());

                if (p.invitations.HasValue)
                    form.AddField("prefs_invitations", p.invitations.Value.ToString());

                if (p.selfJoin.HasValue)
                    form.AddField("prefs_selfJoin", p.selfJoin.Value.ToString().ToLower());

                if (p.cardCovers.HasValue)
                    form.AddField("prefs_cardCovers", p.cardCovers.Value.ToString().ToLower());

                if (p.background != null)
                    form.AddField("prefs_background", p.background);

                if (p.cardAging.HasValue)
                    form.AddField("prefs_cardAging", p.cardAging.Value.ToString());
            }

            // make the request
            string resp = makeRequestEditor(uri, "POST", form);

            // get board from response
            Board board = JsonUtility.FromJson<Board>(resp);
            return board;
        }

        public IEnumerator GetBoardsAsync(Action<Board[]> onFinished)
        {
            string uri = getURI("/members/me/boards");

            // make the request
            yield return makeRequestAsync(uri, "GET", null, (resp) =>
            {
                // get json
                resp = resp.WrapToClass("boards");

                // get board array
                Board[] boards = JsonUtility.FromJson<BoardCollection>(resp).boards;

                // call onfinished
                onFinished(boards);
            });
        }

        /// <summary>
        /// Editor-safe method for getting the boards on the authorized Trello account
        /// </summary>
        /// <returns></returns>
        public Board[] GetBoards()
        {
            // get the uri
            string uri = getURI("/members/me/boards");

            // make the request
            string respString = makeRequestEditor(uri, "GET");

            // get json
            respString = respString.WrapToClass("boards");

            // get board array
            Board[] boards = JsonUtility.FromJson<BoardCollection>(respString).boards;

            return boards;
        }

        /// <summary>
        /// Editor-safe method for getting labels from a board
        /// </summary>
        /// <returns></returns>
        public Label[] GetLabels(string boardID)
        {
            // get the uri
            string uri = getURI("/boards/" + boardID + "/labels");

            // make the request
            string resp = makeRequestEditor(uri, "GET");

            // get json
            resp = resp.WrapToClass("labels");

            // get board array
            Label[] labels = JsonUtility.FromJson<LabelCollection>(resp).labels;

            return labels;
        }

        /// <summary>
        /// Returns whether or not the authenticated user is subscribed to a board
        /// </summary>
        /// <param name="boardID">The board</param>
        /// <returns>Whether or not the authenticated user is subscribed to the board</returns>
        public bool GetSubscribed(string boardID)
        {
            // construct the URI
            string uri = getURI("/boards/" + boardID + "/subscribed");

            // make the request
            string respString = makeRequestEditor(uri, "GET");

            // get response object
            Subscribed sub = JsonUtility.FromJson<Subscribed>(respString);

            // return value
            return sub._value;
        }

        /// <summary>
        /// Sets a user's subscribed state for a board
        /// </summary>
        /// <param name="boardID">The board</param>
        /// <param name="value">The subscribed state</param>
        public void PutSubscribed(string boardID, bool value)
        {
            // construct the URI
            string uri = getURI("/boards/" + boardID + "/subscribed");

            //// hacky way to make the json request 
            //// TODO: change me
            string jsonData = "{ \"value\": " + value.ToString().ToLower() + " }";

            // send the request
            makeRequestEditor(uri, "PUT", "application/json", Encoding.ASCII.GetBytes(jsonData));
        }

        /// <remarks>
        /// see: https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}
