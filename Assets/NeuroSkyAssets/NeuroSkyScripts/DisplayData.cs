using UnityEngine;
using System.Collections;

public class DisplayData : MonoBehaviour
{
	public Texture2D[] signalIcons;
	
	private int indexSignalIcons = 1;

	//TCPClient controller;

    private int poorSignal1;
    private int attention1;
    private int meditation1;
	
	private float delta;

    void Start()
    {

		//controller = GameObject.Find("Main Camera").GetComponent<TCPClient>();

		TCPClient.UpdatePoorSignalEvent += OnUpdatePoorSignal;
		TCPClient.UpdateAttentionEvent += OnUpdateAttention;
		TCPClient.UpdateMeditationEvent += OnUpdateMeditation;
		TCPClient.UpdateDeltaEvent += OnUpdateDelta;
		
    }
	
	void OnUpdatePoorSignal(int value){
		poorSignal1 = value;
		if(value < 25){
      		indexSignalIcons = 0;
		}else if(value >= 25 && value < 51){
      		indexSignalIcons = 4;
		}else if(value >= 51 && value < 78){
      		indexSignalIcons = 3;
		}else if(value >= 78 && value < 107){
      		indexSignalIcons = 2;
		}else if(value >= 107){
      		indexSignalIcons = 1;
		}
	}
	void OnUpdateAttention(int value){
		attention1 = value;
	}
	void OnUpdateMeditation(int value){
		meditation1 = value;
	}
	void OnUpdateDelta(float value){
		delta = value;
	}


    void OnGUI()
    {
		GUILayout.BeginHorizontal();
		
		
        if (GUILayout.Button("Connect"))
        {
            TCPClient.Instance.StartMethod();
        }
        if (GUILayout.Button("DisConnect"))
        {
           // controller.Disconnect();
			indexSignalIcons = 1;
        }
		
		GUILayout.Space(Screen.width-250);
		GUILayout.Label(signalIcons[indexSignalIcons]);
		
		GUILayout.EndHorizontal();

		
        GUILayout.Label("PoorSignal1:" + poorSignal1);
        GUILayout.Label("Attention1:" + attention1);
        GUILayout.Label("Meditation1:" + meditation1);
		GUILayout.Label("Delta:" + delta);

    }
}
