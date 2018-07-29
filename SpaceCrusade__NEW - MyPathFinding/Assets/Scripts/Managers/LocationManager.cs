using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;


public class LocationManager : NetworkBehaviour {

	private static LocationManager instance = null;

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

		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_gridBuilder = GetComponentInChildren<GridBuilder> ();
		if(_gridBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapPieceBuilder = GetComponentInChildren<MapPieceBuilder> ();
		if(_mapPieceBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cubeConnections = GetComponent<CubeConnections> ();
		if(_cubeConnections == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}

	}

	public string BuildMapForHost () {

		_gridBuilder.InitialiseGridManager ();
		_gridBuilder.StartGridBuilding();
		_LocationLookup = _gridBuilder.GetGridLocations ();
		List<Vector3> MapNodeGridLocLookup = _gridBuilder.GetMapNodes (); // Might want to remove grid from host

		return GetMapRules(MapNodeGridLocLookup);	
	}


	public void BuildMapForClient (string rules) {

		_gridBuilder.InitialiseGridManager ();
		_gridBuilder.StartGridBuilding();
		_LocationLookup = _gridBuilder.GetGridLocations ();
		List<Vector3> MapNodeGridLocLookup = _gridBuilder.GetMapNodes ();

		Debug.Log ("mapRules: " + rules);


		int[] convertedArray = ChopStringIntoChunks(rules, 3);

		_mapPieceBuilder.AttachMapPieceToMapNode (convertedArray);
		//	SetCubeNeighbours ();
	}
		

	public static int[] ChopStringIntoChunks(string value, int length)
	{
		int strLength = value.Length;
		int strCount = (strLength + length - 1) / length;
		int[] result = new int[strCount];
		for (int i = 0; i < strCount; ++i)
		{
			result[i] = int.Parse(value.Substring(i * length, Mathf.Min(length, strLength)));
			strLength -= length;
		}
		return result;
	}


	public string GetMapRules(List<Vector3> nodePos)
	{
		int sizeSquared = _mapSettings.numMapPiecesXZ * _mapSettings.numMapPiecesXZ;

		string stringToReturn = "";

		int nodeCount = 0;
		int layerCount = -1;
		bool oddEvent = false;

		//int multiplier = 6;
		//int i = 0;
		foreach (Vector3 pos in nodePos) {

			string posX = pos.x.ToString ("000");
			string posY = pos.y.ToString ("000");
			string posZ = pos.z.ToString ("000");

			if (nodeCount % sizeSquared == 0) { // clever way to figure out each increase in Layer
				layerCount += 1;
				oddEvent = !oddEvent;
			}

			string mapPieceType;
			string mapPiece;
			string rotation;

			if (oddEvent) { // Floor

				mapPieceType = MAP_PIECE_TYPE_FLOOR.ToString ("000");
				mapPiece = Random.Range (0, NUM_FLOOR_MAPS).ToString ("000");
				rotation = Random.Range (0, 4).ToString ("000");

			} else { // Vent

				mapPieceType = MAP_PIECE_TYPE_VENTS.ToString ("000");
				mapPiece = Random.Range (0, NUM_VENT_MAPS).ToString ("000");
				rotation = Random.Range (0, 4).ToString ("000");

			}

			stringToReturn += posX + posY + posZ + mapPieceType + mapPiece + rotation;

			nodeCount += 1;
		}
		return stringToReturn;
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
