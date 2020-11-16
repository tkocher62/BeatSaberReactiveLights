using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BeatSaberColorDetector
{
	class Tcp
	{
		private string ip;
		private int port;
		internal static Socket socket;

		public Tcp(string ip, int port)
		{
			this.ip = ip;
			this.port = port;
		}

		internal void Init()
		{
			try
			{
				new Thread(AttemptConnection).Start();
			}
			catch (Exception x)
			{
				Plugin.Log.Warn("Failed to connect to server.");
			}
		}

		private void AttemptConnection()
		{
			while (!IsConnected())
			{
				try
				{
					Plugin.Log.Info("Attempting server connection...");

					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect(ip, port);

					Plugin.Log.Info("Successfully connected to server!");
				}
				catch (Exception x)
				{
					Plugin.Log.Warn("Failed to connect to server, retrying in 10 seconds...");
				}
				Thread.Sleep(10000);
			}
		}

		internal void SendData(object data)
		{
			socket.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
		}

		internal void SendData(byte[] data)
		{
			if (IsConnected())
			{
				socket.Send(data);
			}
		}

		private bool IsConnected()
		{
			if (socket == null)
			{
				return false;
			}
			try
			{
				return !((socket.Poll(1000, SelectMode.SelectRead) && (socket.Available == 0)) || !socket.Connected);
			}
			catch (Exception x)
			{
				return false;
			}
		}

		internal void Disconnect()
		{
			socket.Shutdown(SocketShutdown.Both);
		}
	}
}
