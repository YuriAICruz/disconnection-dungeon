using UnityEngine;
using System.Collections;
using System;

namespace Graphene.WebSocketsNetworking.Example
{
	public class EchoTest : MonoBehaviour
	{
		public string url = "ws://echo.websocket.org";

		IEnumerator Start()
		{
			WebSocket w = new WebSocket(new Uri(url));
			yield return StartCoroutine(w.Connect());
			w.SendString("Hi there");
			int i = 0;
			while (true)
			{
				string reply = w.RecvString();
				if (reply != null)
				{
					Debug.Log("Received: " + reply);
					w.SendString("Hi there" + i++);
				}
				if (w.error != null)
				{
					Debug.LogError("Error: " + w.error);
					break;
				}
				yield return new WaitForSeconds(1);
			}
			w.Close();
		}
	}
}
