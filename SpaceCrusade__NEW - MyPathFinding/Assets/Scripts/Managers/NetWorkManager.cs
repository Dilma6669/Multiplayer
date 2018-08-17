using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : NetworkManager {

	SyncedVars _syncedVars;

    // Need the Awake() function for HUD
     void Awake() {
         Debug.Log("NETWORKMANAGER: Awake");
    }


	// called on the SERVER when a client connects
	public override void OnServerConnect(NetworkConnection Conn)
	{
        Debug.Log("NETWORKMANAGER: Client Connect!! Con: " + Conn.hostId);

        _syncedVars = FindObjectOfType<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (Conn.hostId == -1)
        {
            int globalSeed = Random.Range(0, 100);
            Random.InitState(globalSeed);
            _syncedVars.GlobalSeed = globalSeed;
        }

        _syncedVars.PlayerCount = 1;
	}


}
