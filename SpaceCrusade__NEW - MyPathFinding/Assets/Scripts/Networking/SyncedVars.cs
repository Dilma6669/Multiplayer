using UnityEngine;
using UnityEngine.Networking;

public class SyncedVars : NetworkBehaviour {

	PlayerManager _playerManager; 

	private static SyncedVars instance = null;

//	[SyncVar]
//	public int _connectedPlayers = 0;


	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_playerManager = FindObjectOfType<PlayerManager> ();

		_playerManager = transform.parent.GetComponentInChildren<PlayerManager> ();
		if(_playerManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

	}
		
//	[Command]
//	public void ChangeConnectedPlayers(int change) {
//		_playerManager._playerAgents[0].CreatePlayer ();
//	}
}
