using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

	MapSettings _mapSettings;

	public GameObject _defaultCubePrefab; // Debugging purposes
	public GameObject _gridObjectPrefab; // Debugging purposes
	public GameObject _spawnPrefab; // object that runs around world space creating the locations

    public GameObject _mapNodePrefab; // object that shows Map nodes
    public GameObject _connectNodePrefab; // object that shows Map nodes
    public GameObject _shipNodePrefab; // object that shows Map nodes


    private GameObject _spawn;

	public bool _debugGridObjects; // debugging purposes

	//public Dictionary<Vector3, Vector3> _GridLocToWorldLocLookup; 	// making a lookUp table for Grid locations matching world space locations
	public Dictionary<Vector3, CubeLocationScript> _GridLocToScriptLookup; 	// making a lookUp table for objects located at Vector3 Grid locations

	private List<Vector3> _MapPieceNodes;
    private List<Vector3> _ShipPieceNodes;
    private List<Vector3> _ConnectPieceNodes;


    void Awake() {

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}



	// Use this for initialization
	public void InitialiseGridManager () {

		//_GridLocToWorldLocLookup = new Dictionary<Vector3, Vector3> ();
		_GridLocToScriptLookup = new Dictionary <Vector3, CubeLocationScript>();

        _MapPieceNodes = new List<Vector3>();
        _ShipPieceNodes = new List<Vector3>();
        _ConnectPieceNodes = new List<Vector3>();


        // SPAWN //
        _spawn = Instantiate (_spawnPrefab, transform, false);
		////////////////
	}


	public Dictionary <Vector3, CubeLocationScript> GetGridLocations() {
		return _GridLocToScriptLookup;
	}

	public List<Vector3> GetMapNodes() {
		return _MapPieceNodes;
	}

    public List<Vector3> GetConnectNodes()
    {
        return _ConnectPieceNodes;
    }

    public List<Vector3> GetShipNodes()
    {
        return _ShipPieceNodes;
    }

    public void StartGridBuilding() {

        int heightOfMapPieces = _mapSettings.heightOfMapPieces;
        int numMapPiecesY = _mapSettings.numMapPiecesY;

        // these are the bottom left corner axis for EACH map node
        int startGridLocX = 0;
		int startGridLocY = 0; // starting height of each new layer, 0, 10, 20. etc
		int startGridLocZ = 0;

		// Load each Y layer of grids in a loop, not nessacary but just did it this way for some reason
		for (int mapLayer = 0; mapLayer < numMapPiecesY; mapLayer++) {
		
			// build map Layer grid locations
			BuildGridLocations (startGridLocX, startGridLocY, startGridLocZ);

			startGridLocX = 0;
			startGridLocY = (mapLayer+1) * heightOfMapPieces + (2 * (mapLayer+1));
			startGridLocZ = 0;

		}

	}
		

	public void BuildGridLocations(int startX, int startY, int startZ) {

        int totalXZCubes = _mapSettings.totalXZCubes;
        int sizeOfMapPieces = _mapSettings.sizeOfMapPieces;
        int sizeOfCubes = _mapSettings.sizeOfCubes;
        int heightOfMapPieces = _mapSettings.heightOfMapPieces;

        int gridLocX = startX;
		int gridLocY = startY;
		int gridLocZ = startZ;

        int emptySpacePadding = 3; //1=connection, 2 & 3 = for a 2piece wide ship

        int finishX = totalXZCubes + (sizeOfMapPieces * (emptySpacePadding * 2)) ;
        int finishZ = totalXZCubes + (sizeOfMapPieces * (emptySpacePadding * 2));

        float spawnPosX;
		float spawnPosY;
		float spawnPosZ;

		// Floors layer
		for (int y = startY; y < startY + heightOfMapPieces; y++) {

			spawnPosX = startX;
			spawnPosZ = startZ;
			spawnPosY = y * sizeOfCubes;

			gridLocX = startX;
			gridLocZ = startZ;

				
			for (int z = startZ; z < finishZ; z++) {

				spawnPosX = startX;
				_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

					// Create location positions
					///////////////////////////////
					// put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
					Vector3 gridLoc = new Vector3 (gridLocX, gridLocY, gridLocZ);

                    // Create empty objects at locations to see the locations (debugging purposes)
                    if (_debugGridObjects)
                    {
                        MakeDebugObject();
                    }

                    // Adds null script for optimization
                    _GridLocToScriptLookup.Add(gridLoc, null);

                    // node objects are spawned at bottom corner each map piece
                    MakeMapNodeObject(gridLocX, gridLocY, gridLocZ, startY, finishX, finishZ, emptySpacePadding);

                    spawnPosX += sizeOfCubes;
					_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
					gridLocX += 1;

				}
				spawnPosZ += sizeOfCubes;
				gridLocZ += 1;
			}
			gridLocY += 1;
		}


		// Vents layer
		// An attempt to build the vents layer //seems to be working
		gridLocX = startX;
		//gridLocY = gridLocY;
		gridLocZ = startZ;

        startY += heightOfMapPieces;

		for (int y = startY; y < (startY + 2); y++) {
			
			spawnPosX = startX;
			spawnPosZ = startZ;
			spawnPosY = y * sizeOfCubes;

			gridLocX = startX;
			gridLocZ = startZ;


			for (int z = startZ; z < finishZ; z++) {

				spawnPosX = startX;
				_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

					// Create location positions
					///////////////////////////////
					// put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
					Vector3 gridLoc = new Vector3 (gridLocX, gridLocY, gridLocZ);

                    // Create empty objects at locations to see the locations (debugging purposes)
                    if (_debugGridObjects)
                    {
                        MakeDebugObject();
                    }

                    // Adds null script for optimization
                    _GridLocToScriptLookup.Add(gridLoc, null);

                    // node objects are spawned at bottom corner each map piece
                    MakeMapNodeObject(gridLocX, gridLocY, gridLocZ, startY, finishX, finishZ, emptySpacePadding);


                    spawnPosX += sizeOfCubes;
					_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
					gridLocX += 1;

				}
				spawnPosZ += sizeOfCubes;
				gridLocZ += 1;
			}
			gridLocY += 1;
		}
	}

    // Create empty objects at locations to see the locations (debugging purposes)
    private void MakeDebugObject()
    {
        int sizeOfCubes = _mapSettings.sizeOfCubes;

        GameObject GridObject = Instantiate(_gridObjectPrefab, _spawn.transform, false);
        GridObject.transform.SetParent(this.gameObject.transform);
        GridObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
    }

    // node objects are spawned at bottom corner each map piece
    private void MakeMapNodeObject(int gridLocX, int gridLocY, int gridLocZ, int startY, int finishX, int finishZ, int emptySpacePadding)
    {
        //////////////////////////////////////////
        int totalXZCubes = _mapSettings.totalXZCubes;
        int numMapPiecesXZ = _mapSettings.numMapPiecesXZ;
        int sizeOfMapPieces = _mapSettings.sizeOfMapPieces;
        int sizeOfCubes = _mapSettings.sizeOfCubes;

        int multiple = totalXZCubes / numMapPiecesXZ;

        // The central Ship
        if (gridLocX >= sizeOfMapPieces * emptySpacePadding && gridLocX < (finishX - sizeOfMapPieces * emptySpacePadding)
            && gridLocZ >= sizeOfMapPieces * emptySpacePadding && gridLocZ < (finishZ - sizeOfMapPieces * emptySpacePadding))
        {
            if (gridLocX % multiple == 0 && gridLocZ % multiple == 0 && gridLocY == startY)
            {
                //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
                GameObject nodeObject = Instantiate(_mapNodePrefab, _spawn.transform, false);
               // nodeObject.GetComponent<>
                nodeObject.transform.position = new Vector3(gridLocX, gridLocY, gridLocZ);
                nodeObject.transform.SetParent(this.gameObject.transform);
                nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
                _MapPieceNodes.Add(new Vector3(gridLocX, gridLocY, gridLocZ));
            }
        }
        // the entrances
        else if (gridLocX >= sizeOfMapPieces * (emptySpacePadding - 1) && gridLocX < (finishX - sizeOfMapPieces * (emptySpacePadding - 1))
            && gridLocZ >= sizeOfMapPieces * (emptySpacePadding - 1) && gridLocZ < (finishZ - sizeOfMapPieces * (emptySpacePadding - 1)))
        {
            if (gridLocX % multiple == 0 && gridLocZ % multiple == 0 && gridLocY == startY)
            {
                //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
                GameObject nodeObject = Instantiate(_connectNodePrefab, _spawn.transform, false);
                nodeObject.transform.position = new Vector3(gridLocX, gridLocY, gridLocZ);
                nodeObject.transform.SetParent(this.gameObject.transform);
                nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
                _ConnectPieceNodes.Add(new Vector3(gridLocX, gridLocY, gridLocZ));
            }
        }
        // The Players ships
        else
        { 
            if (gridLocX % multiple == 0 && gridLocZ % multiple == 0 && gridLocY == startY)
            {
                //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
                GameObject nodeObject = Instantiate(_shipNodePrefab, _spawn.transform, false);
                nodeObject.transform.position = new Vector3(gridLocX, gridLocY, gridLocZ);
                nodeObject.transform.SetParent(this.gameObject.transform);
                nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
                _ShipPieceNodes.Add(new Vector3(gridLocX, gridLocY, gridLocZ));
            }
        }
        /////////////////////////////////////////////
    }

}
	

