using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LocationManager : NetworkBehaviour {


	GridBuilder _gridBuilder;
	MapPieceBuilder _mapPieceBuilder;
	CubeConnections _cubeConnections;

	MapSettings _mapSettings;


	private int MAP_PIECE_TYPE_FLOOR = 0;
	private int MAP_PIECE_TYPE_VENTS = 1;

	private int NUM_FLOOR_MAPS = 2;
	private int NUM_VENT_MAPS = 2;

	public Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();

	void Awake() {

		_gridBuilder = GetComponentInChildren<GridBuilder> ();
		if(_gridBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapPieceBuilder = GetComponentInChildren<MapPieceBuilder> ();
		if(_mapPieceBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cubeConnections = GetComponent<CubeConnections> ();
		if(_cubeConnections == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}

	}
    /*
	public string BuildMapForHost () {

		_gridBuilder.InitialiseGridManager ();
		_gridBuilder.StartGridBuilding();
		_LocationLookup = _gridBuilder.GetGridLocations ();
		List<Vector3> MapNodeGridLocLookup = _gridBuilder.GetMapNodes (); // Might want to remove grid from host

		return GetMapRules(MapNodeGridLocLookup);	
	}
    */

	public void BuildMapForClient () {

		_gridBuilder.InitialiseGridManager ();
		_gridBuilder.StartGridBuilding();
		_LocationLookup = _gridBuilder.GetGridLocations ();
		List<Vector3> MapNodeGridLocLookup = _gridBuilder.GetMapNodes ();


		_mapPieceBuilder.AttachMapPieceToMapNode (MapNodeGridLocLookup);
		//	SetCubeNeighbours ();
	}
		



	public bool CheckIfLocationExists(Vector3 loc) {

		if (_LocationLookup.ContainsKey (loc)) {
			return true;
		}
		return false;
	}


	public CubeLocationScript GetLocationScript(Vector3 loc) {

		if (CheckIfLocationExists(loc)) {
			return _LocationLookup[loc];
		}
		//Debug.LogError ("Returning a null Location for: " + loc);
		return null;
	}

	public CubeLocationScript CheckIfCanMoveToCube(Vector3 loc) {

		CubeLocationScript cubeScript = GetLocationScript(loc);

		if (cubeScript != null) {
			if (cubeScript._cubeOccupied) {
				cubeScript = null;
			}
		}
		return cubeScript;
	}


	public void SetCubeNeighbours() {

		_cubeConnections.SetCubeNeighbours (_LocationLookup);

	}
}
