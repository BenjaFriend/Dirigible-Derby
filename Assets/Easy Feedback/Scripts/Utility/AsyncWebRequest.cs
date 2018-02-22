using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

#if UNITY_5_3
using UnityEngine.Experimental.Networking;
#else
using UnityEngine.Networking;
#endif

namespace EasyFeedback
{
    public class AsyncWebRequest
    {
        public UnityWebRequest Request;
        public Exception UploadException;

        public float Timeout = 15f;
        private float elapsedTime;

        public int Attempts = 3;

        public bool RequestIsError
        {
            get
            {
#if UNITY_2017_3
                return Request.isHttpError || Request.isNetworkError;
#else
                return Request.isError;
#endif
            }
        }

        public IEnumerator Post(string uri, WWWForm data, Action<UnityWebRequest, Exception> onFinished = null)
        {
            Request = UnityWebRequest.Post(uri, data);
            Request.chunkedTransfer = false; // required so the request sends the content-length header

            yield return sendRequest();

            if (onFinished != null)
                onFinished(Request, UploadException);
        }

        public IEnumerator Get(string uri, Action<UnityWebRequest, Exception> onFinished = null)
        {
            Request = UnityWebRequest.Get(uri);

            yield return sendRequest();

            if (onFinished != null)
                onFinished(Request, UploadException);
        }

        private IEnumerator sendRequest()
        {
            // attempt to make the request again if it times out
            while(Attempts > 0)
            {
                // decrement attempts counter
                Attempts -= 1;

                // send the request
                AsyncOperation op = null;
                try
                {
#if UNITY_2017_3
                    op = Request.SendWebRequest();
#else
                    op = Request.Send();
#endif

                }
                catch (Exception e)
                {
                    UploadException = e;
                    yield break;
                }

                // start timeout counter
                elapsedTime = 0;

                // block until request is finished
                while (!op.isDone && elapsedTime < Timeout)
                {
                    yield return new WaitForEndOfFrame();
                    elapsedTime += Time.deltaTime;
                }

                // break out of attempts loop if operation is done
                if (op.isDone) break;
            }
        }

    }
}
