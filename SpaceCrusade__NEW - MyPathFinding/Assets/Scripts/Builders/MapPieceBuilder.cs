using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class MapPieceBuilder: NetworkBehaviour {

	private static MapPieceBuilder instance = null;

	LocationManager _locationManager;
	CubeBuilder _cubeBuilder;
	MapSettings _mapSettings;


	public List<int[,]> floors = new List<int[,]>();
	public List<int[,]> vents = new List<int[,]>();

	private bool loadVents = false;


	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_locationManager = transform.parent.GetComponent<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cubeBuilder = GetComponentInChildren<CubeBuilder> ();
		if(_cubeBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


	public void AttachMapPieceToMapNode(int[] mapRules) {

		StartCoroutine (BuildMapsByIEnum (mapRules, 0.001f));
	}
		

	private IEnumerator BuildMapsByIEnum(int[] mapRules, float waitTime) {

		Vector3 cubeLoc;
		Vector3 GridLoc;

		List<int[,]> layers = new List<int[,]>();
		int[,] floor;

		int sizeSquared = _mapSettings.numMapPiecesXZ * _mapSettings.numMapPiecesXZ;
		int nodeCount = 0;
		int layerCount = -1;

		int multiplier = 6;
		int i = 0;
		for(int j = 0; j < mapRules.Length/6; j++)
		{
			i = j * multiplier;

			int posX = mapRules[i+0];
			int posY = mapRules[i+1];
			int posZ = mapRules[i+2];

			int mapPieceType = mapRules[i+3];
			int mapPiece = mapRules[i+4];
			int rotation = mapRules[i+5];

			//Debug.Log("data: " + (i+0) + " :( " + posX + ") " + (i+1) + " :( " + posY + ") " + (i+2) + " :( " + posZ + ") " + (i+3) + " :( " + mapPieceType + ") " + (i+4) + " :( " + mapPiece + ") " + (i+5) + " :( " + rotation + ") ");

			if (nodeCount % sizeSquared == 0) { // clever way to figure out each increase in Layer
				layerCount += 1;
			}

			layers = GetMapPiece(mapPieceType, mapPiece);
			int rotations = rotation;

			int objectsCountX = posX;
			int objectsCountY = posY;
			int objectsCountZ = posZ;

			for (int y = 0; y < layers.Count; y++) {

				objectsCountX = posX;
				objectsCountZ = posZ;

				floor = layers [y];

				for (int r = 0; r < rotations; r++) {
					floor = TransposeArray (floor, _mapSettings.sizeOfMapPieces-1);
				}
					
				for (int z = 0; z < floor.GetLength(0); z++) {

					objectsCountX = posX;

					for (int x = 0; x < floor.GetLength(1); x++) {

						int cubeType = floor [z, x];
						GridLoc = new Vector3 (objectsCountX, objectsCountY, objectsCountZ);

						CubeLocationScript cubeScript = _locationManager.GetLocationScript (GridLoc);

						if(cubeScript != null) {
							_cubeBuilder.CreateCubeObject(ref cubeScript, cubeType, rotations, layerCount); // Create the cube
						}

						objectsCountX += 1;
					}
					objectsCountZ += 1;
				}
				objectsCountY += 1;
			}
			nodeCount += 1;

			yield return new WaitForSeconds (waitTime);
		}

		//_gameManager.MapsFinishedLoading ();
	}

	// Get map by type and piece
	private List<int[,]> GetMapPiece(int type, int map) {

		switch (type) { 
		case 0: // Floor
			BaseRoom mapPiece = null;
			switch (map) {
			case 0:
				mapPiece = ScriptableObject.CreateInstance<SquareRoom01> ();
				break;
			case 1:
				mapPiece = ScriptableObject.CreateInstance<SquareRoom02> ();
				break;
			default:
				break;
			}
			return mapPiece.floors;

		case 1:
			BaseRoom ventPiece = null;
			switch (map) {
			case 0:
				ventPiece = ScriptableObject.CreateInstance<SquareVents01> ();
				break;
			case 1:
				ventPiece = ScriptableObject.CreateInstance<SquareVents02> ();
				break;
			default:
				break;
			}
			return ventPiece.floors;

		default:
			Debug.LogError ("OPSALA SOMETHING WRONG HERE!");
			return null;
		}
	}





	private int[,] TransposeArray(int[,] array, int size) {

		int[,] ret = new int[size, size];

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				ret [i, j] = array [size - j - 1, i];
			}
		}

		return ret;
	}
		
}
