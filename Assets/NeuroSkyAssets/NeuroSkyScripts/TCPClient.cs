using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;



/// <summary>
///
/// Created by Hardik Shah
/// for more info mail me : hardik.s@theintellify.com
/// 
/// </summary>


public class TCPClient : MonoBehaviour
{

	public static TCPClient Instance;

	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	private NetworkStream Streams;
	private string pattern = @"^{""eSense"".*$";
	private Regex regex;
	public BrainWaveData data = new BrainWaveData();
	#endregion
	// Use this for initialization



	#region Event & Deligates

	public delegate void UpdateIntValueDelegate(int value);
	public delegate void UpdateFloatValueDelegate(float value);

	public static event UpdateIntValueDelegate UpdatePoorSignalEvent;
	public static event UpdateIntValueDelegate UpdateAttentionEvent;
	public static event UpdateIntValueDelegate UpdateMeditationEvent;
	public static event UpdateIntValueDelegate UpdateRawdataEvent;
	public static event UpdateIntValueDelegate UpdateBlinkEvent;

	public static event UpdateFloatValueDelegate UpdateDeltaEvent;
	public static event UpdateFloatValueDelegate UpdateThetaEvent;
	public static event UpdateFloatValueDelegate UpdateLowAlphaEvent;
	public static event UpdateFloatValueDelegate UpdateHighAlphaEvent;
	public static event UpdateFloatValueDelegate UpdateLowBetaEvent;
	public static event UpdateFloatValueDelegate UpdateHighBetaEvent;
	public static event UpdateFloatValueDelegate UpdateLowGammaEvent;
	public static event UpdateFloatValueDelegate UpdateHighGammaEvent;

    #endregion


    private void Awake()
    {
		if (Instance == null) {
			Instance = this;
		}
    }

    void Start()
	{
		Debug.Log("Start");
		ConnectToTcpServer();

		regex = new Regex(pattern, RegexOptions.Compiled);
	}
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SendMessage();
		}
	}


	public void StartMethod() {

		SendMessage();
	}


    private void OnDisable()
    {
		try
		{
			clientReceiveThread.Abort();
			Streams.Close();
			socketConnection.Close();

		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
		}
	}

    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient("127.0.0.1", 13854);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (Streams = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = Streams.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						//Debug.Log("server message received as: " + serverMessage);

						if (regex.IsMatch(serverMessage))
						{
							Debug.Log("<color=green>server message received as: </color>" + serverMessage);
							data = JsonUtility.FromJson<BrainWaveData>(serverMessage);


							/***
							 * Set Signal data
							 * */
							if (UpdatePoorSignalEvent != null)
							{
								UpdatePoorSignalEvent(data.poorSignalLevel);
							}

							/***
							 * Set Sense Class Data
							 * */
							if (UpdateAttentionEvent != null)
							{
								UpdateAttentionEvent(data.eSense.attention);
							}
							if (UpdateMeditationEvent != null)
							{
								UpdateMeditationEvent(data.eSense.meditation);
							}


							/***
							 * Set EegPower Class Data
							 * */
							if (UpdateDeltaEvent != null)
							{
								UpdateDeltaEvent(data.eegPower.delta);
							}
							if (UpdateThetaEvent != null)
							{
								UpdateThetaEvent(data.eegPower.theta);
							}
							if (UpdateLowAlphaEvent != null)
							{
								UpdateLowAlphaEvent(data.eegPower.lowAlpha);
							}
							if (UpdateHighAlphaEvent != null)
							{
								UpdateHighAlphaEvent(data.eegPower.highAlpha);
							}
							if (UpdateLowBetaEvent != null)
							{
								UpdateLowBetaEvent(data.eegPower.lowBeta);
							}
							if (UpdateHighBetaEvent != null)
							{
								UpdateHighBetaEvent(data.eegPower.highBeta);
							}
							if (UpdateLowGammaEvent != null)
							{
								UpdateLowGammaEvent(data.eegPower.lowGamma);
							}
							if (UpdateHighGammaEvent != null)
							{
								UpdateHighGammaEvent(data.eegPower.highGamma);
							}

						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		if (socketConnection == null)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				//string clientMessage = "This is a message from one of your clients.";
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
}
