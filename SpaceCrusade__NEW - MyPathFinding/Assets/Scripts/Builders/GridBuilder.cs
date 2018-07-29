using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

	public static GridBuilder instance = null;

	MapSettings _mapSettings;

	public GameObject _defaultCubePrefab; // Debugging purposes
	public GameObject _gridObjectPrefab; // Debugging purposes
	public GameObject _spawnPrefab; // object that runs around world space creating the locations
	public GameObject _mapNodePrefab; // object that shows Map nodes

	private GameObject _spawn;

	public bool _debugGridObjects; // debugging purposes

	//public Dictionary<Vector3, Vector3> _GridLocToWorldLocLookup; 	// making a lookUp table for Grid locations matching world space locations
	public Dictionary<Vector3, CubeLocationScript> _GridLocToScriptLookup; 	// making a lookUp table for objects located at Vector3 Grid locations

	private List<Vector3> _MapNodeGridLocLookup;


	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}



	// Use this for initialization
	public void InitialiseGridManager () {

		//_GridLocToWorldLocLookup = new Dictionary<Vector3, Vector3> ();
		_GridLocToScriptLookup = new Dictionary <Vector3, CubeLocationScript>();

		_MapNodeGridLocLookup = new List<Vector3>();

		// SPAWN //
		_spawn = Instantiate (_spawnPrefab, transform, false);
		////////////////
	}


	public Dictionary <Vector3, CubeLocationScript> GetGridLocations() {
		return _GridLocToScriptLookup;
	}

	public List<Vector3> GetMapNodes() {
		return _MapNodeGridLocLookup;
	}

	public void StartGridBuilding() {
		
		// these are the bottom left corner axis for EACH map node
		int startGridLocX = 0;
		int startGridLocY = 0; // starting height of each new layer, 0, 10, 20. etc
		int startGridLocZ = 0;

		// Load each Y layer of grids in a loop, not nessacary but just did it this way for some reason
		for (int mapLayer = 0; mapLayer < _mapSettings.numMapPiecesY; mapLayer++) {
		
			// build map Layer grid locations
			BuildGridLocations (startGridLocX, startGridLocY, startGridLocZ);

			startGridLocX = 0;
			startGridLocY = (mapLayer+1) * _mapSettings.heightOfMapPieces + (2 * (mapLayer+1));
			startGridLocZ = 0;

		}

	}
		

	public void BuildGridLocations(int startX, int startY, int startZ) {

		int gridLocX = startX;
		int gridLocY = startY;
		int gridLocZ = startZ;

		float spawnPosX;
		float spawnPosY;
		float spawnPosZ;

		// Floors layer
		for (int y = startY; y < (startY + _mapSettings.heightOfMapPieces); y++) {

			spawnPosX = startX;
			spawnPosZ = startZ;
			spawnPosY = y * _mapSettings.sizeOfCubes;

			gridLocX = startX;
			gridLocZ = startZ;

				
			for (int z = startZ; z < _mapSettings.totalXZCubes; z++) {

				spawnPosX = startX;
				_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);

				gridLocX = startX;

				for (int x = startX; x < _mapSettings.totalXZCubes; x++) {

					// Create location positions
					///////////////////////////////
					// put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
					Vector3 gridLoc = new Vector3 (gridLocX, gridLocY, gridLocZ);
					//Vector3 worldLoc = new Vector3 (_spawn.transform.localPosition.x, _spawn.transform.localPosition.y, _spawn.transform.localPosition.z);
					//Debug.Log ("Vector3 (gridLoc): x: " + gridLoc.x + " z: " +  gridLoc.z + " y: " +  gridLoc.y);
					//Debug.Log ("Vector3 (worldLoc): x: " + worldLoc.x + " y: " +  worldLoc.y + " z: " +  worldLoc.z);

					GameObject cubeObject = Instantiate (_defaultCubePrefab, transform, false); // empty cube
					cubeObject.transform.SetParent (transform);
					cubeObject.transform.position = gridLoc;
					CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript> ();
					cubeScript.cubeLoc = gridLoc;


					_GridLocToScriptLookup.Add (gridLoc, cubeScript);
					/////////////////////////////////

					// Create empty objects at locations to see the locations (debugging purposes)
					////////////////////////////////////////
					if (_debugGridObjects) {
						GameObject GridObject = Instantiate (_gridObjectPrefab, _spawn.transform, false);
						GridObject.transform.SetParent (this.gameObject.transform);
						GridObject.transform.localScale = new Vector3 (_mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes);
					}
					//////////////////////////////////////////

					// node objects are spawned at bottom corner each map piece
					//////////////////////////////////////////
					int multiple = _mapSettings.totalXZCubes / _mapSettings.numMapPiecesXZ;
					if (gridLocX % multiple == 0 && gridLocZ % multiple == 0 && gridLocY == startY) {
						GameObject nodeObject = Instantiate (_mapNodePrefab, _spawn.transform, false);
						nodeObject.transform.position = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
						nodeObject.transform.SetParent (this.gameObject.transform);
						nodeObject.transform.localScale = new Vector3 (_mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes);
						_MapNodeGridLocLookup.Add (gridLoc);
					}
					/////////////////////////////////////////////

					spawnPosX += _mapSettings.sizeOfCubes;
					_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
					gridLocX += 1;

				}
				spawnPosZ += _mapSettings.sizeOfCubes;
				gridLocZ += 1;
			}
			gridLocY += 1;
		}


		// Vents layer
		// An attempt to build the vents layer //seems to be working
		gridLocX = startX;
		//gridLocY = gridLocY;
		gridLocZ = startZ;

		startY += _mapSettings.heightOfMapPieces;

		for (int y = startY; y < (startY + 2); y++) {
			
			spawnPosX = startX;
			spawnPosZ = startZ;
			spawnPosY = y * _mapSettings.sizeOfCubes;

			gridLocX = startX;
			gridLocZ = startZ;


			for (int z = startZ; z < _mapSettings.totalXZCubes; z++) {

				spawnPosX = startX;
				_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);

				gridLocX = startX;

				for (int x = startX; x < _mapSettings.totalXZCubes; x++) {

					// Create location positions
					///////////////////////////////
					// put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
					Vector3 gridLoc = new Vector3 (gridLocX, gridLocY, gridLocZ);
					//Vector3 worldLoc = new Vector3 (_spawn.transform.localPosition.x, _spawn.transform.localPosition.y, _spawn.transform.localPosition.z);
					//Debug.Log ("Vector3 (gridLoc): x: " + gridLoc.x + " z: " +  gridLoc.z + " y: " +  gridLoc.y);
					//Debug.Log ("Vector3 (worldLoc): x: " + worldLoc.x + " y: " +  worldLoc.y + " z: " +  worldLoc.z);
					GameObject cubeObject = Instantiate (_defaultCubePrefab, transform, false); // empty cube
					cubeObject.transform.SetParent (transform);
					cubeObject.transform.position = gridLoc;
					CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript> ();
					cubeScript.cubeLoc = gridLoc;

					_GridLocToScriptLookup.Add (gridLoc, cubeScript);
					/////////////////////////////////

					// Create empty objects at locations to see the locations (debugging purposes)
					////////////////////////////////////////
					if (_debugGridObjects) {
						GameObject GridObject = Instantiate (_gridObjectPrefab, _spawn.transform, false);
						GridObject.transform.SetParent (this.gameObject.transform);
						GridObject.transform.localScale = new Vector3 (_mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes);
					}
					//////////////////////////////////////////

					// node objects are spawned at bottom corner each map piece
					//////////////////////////////////////////
					int multiple = _mapSettings.totalXZCubes / _mapSettings.numMapPiecesXZ;
					if (gridLocX % multiple == 0 && gridLocZ % multiple == 0 && gridLocY == startY) {
						GameObject nodeObject = Instantiate (_mapNodePrefab, _spawn.transform, false);
						nodeObject.transform.position = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
						nodeObject.transform.SetParent (this.gameObject.transform);
						nodeObject.transform.localScale = new Vector3 (_mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes, _mapSettings.sizeOfCubes);
						_MapNodeGridLocLookup.Add (gridLoc);
					}
					/////////////////////////////////////////////

					spawnPosX += _mapSettings.sizeOfCubes;
					_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
					gridLocX += 1;

				}
				spawnPosZ += _mapSettings.sizeOfCubes;
				gridLocZ += 1;
			}
			gridLocY += 1;
		}
	}

}
	

