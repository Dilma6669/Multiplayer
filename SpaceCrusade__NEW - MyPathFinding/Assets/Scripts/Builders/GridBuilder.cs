using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

	MapSettings _mapSettings;

	public GameObject _defaultCubePrefab; // Debugging purposes
	public GameObject _spawnPrefab; // object that runs around world space creating the locations

    public GameObject _gridObjectPrefab; // Debugging purposes
    public GameObject _mapNodePrefab; // object that shows Map nodes
    public GameObject _connectNodePrefab; // object that shows Map nodes
    public GameObject _shipNodePrefab; // object that shows Map nodes

    private int prefabSelector = -1;

    private GameObject _spawn;

	public bool _debugGridObjects; // debugging purposes

	//public Dictionary<Vector3, Vector3> _GridLocToWorldLocLookup; 	// making a lookUp table for Grid locations matching world space locations
	public Dictionary<Vector3, CubeLocationScript> _GridLocToScriptLookup; 	// making a lookUp table for objects located at Vector3 Grid locations

	private List<Vector3> _GridNodePositions;

    int numMapPiecesXZ;
    int sizeSquared;
    int sizeOfMapPieces;
    int totalXZCubes;
    int sizeOfCubes;

    void Awake() {

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}


        //_GridLocToWorldLocLookup = new Dictionary<Vector3, Vector3> ();
        _GridLocToScriptLookup = new Dictionary<Vector3, CubeLocationScript>();

        // SPAWN //
        _spawn = Instantiate(_spawnPrefab, transform, false);
    }




	public Dictionary <Vector3, CubeLocationScript> GetGridLocations() {
		return _GridLocToScriptLookup;
	}

	public List<Vector3> GetGridNodePositions() {
		return _GridNodePositions;
	}



    public void BuildLocationGrid(Vector3 worldNode, float waitTime, int prefab)
    {
        numMapPiecesXZ = _mapSettings.numMapPiecesXZ;
        sizeSquared = numMapPiecesXZ * numMapPiecesXZ;
        sizeOfMapPieces = _mapSettings.sizeOfMapPiecesXZ;
        totalXZCubes = _mapSettings.totalXZCubes;
        sizeOfCubes = _mapSettings.sizeOfCubes;

        prefabSelector = prefab;

        _GridNodePositions = new List<Vector3>();


        // these are the bottom left corner axis for EACH map node
        int startGridLocX = (int)worldNode.x;
        int startGridLocY = (int)worldNode.y; // starting height of each new layer, 0, 10, 20. etc
        int startGridLocZ = (int)worldNode.z;

        for (int mapLayer = 0; mapLayer < _mapSettings.numMapPiecesY; mapLayer++)
        {
            //Debug.Log("Vector3 (gridLoc): x: " + startGridLocX + " y: " + startGridLocY + " z: " + startGridLocZ);

            // build map Layer grid locations
            BuildGridLocations(startGridLocX, startGridLocY, startGridLocZ);

            // these are the bottom left corner axis for EACH map node
            startGridLocX = (int)worldNode.x;
            startGridLocY = (mapLayer + 1) * _mapSettings.sizeOfMapPiecesY + (2 * (mapLayer + 1));
            startGridLocZ = (int)worldNode.z;

            //yield return new WaitForSeconds(0);
        }
    }



	public void BuildGridLocations(int startX, int startY, int startZ) {

        int gridLocX = startX;
		int gridLocY = startY;
		int gridLocZ = startZ;

        int finishX = (prefabSelector == 0) ? (startX + (numMapPiecesXZ * sizeOfMapPieces)) : (startX + (sizeOfMapPieces));
        int finishZ = (prefabSelector == 0) ? (startZ + (numMapPiecesXZ * sizeOfMapPieces)) : (startZ + (sizeOfMapPieces));
        int finishY = startY + (_mapSettings.sizeOfMapPiecesY);

        float spawnPosX;
		float spawnPosY;
		float spawnPosZ;

		// Floors layer
		for (int y = startY; y < finishY; y++) { // this needs attention!!!!

			spawnPosX = startX;
			spawnPosZ = startZ;
			spawnPosY = y;

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
                        MakeDebugObject(gridLocX, gridLocY, gridLocZ);
                    }

                    // Adds null script for optimization
                    _GridLocToScriptLookup.Add(gridLoc, null);

                    // node objects are spawned at bottom corner each map piece
                    MakeMapNodeObject(gridLocX, gridLocY, gridLocZ, startY, finishX, finishZ);

                    spawnPosX += sizeOfCubes;
					_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
					gridLocX += sizeOfCubes;

				}
				spawnPosZ += sizeOfCubes;
				gridLocZ += sizeOfCubes;
			}
			gridLocY += sizeOfCubes;
		}

   
		// Vents layer
		// An attempt to build the vents layer //seems to be working
		gridLocX = startX;
		//gridLocY = gridLocY;
		gridLocZ = startZ;

        startY += _mapSettings.sizeOfMapPiecesY;

		for (int y = startY; y < (startY + _mapSettings.sizeOfMapVentsY) ; y++) {
			
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
                        MakeDebugObject(gridLocX, gridLocY, gridLocZ);
                    }

                    // Adds null script for optimization
                    _GridLocToScriptLookup.Add(gridLoc, null);

                    // node objects are spawned at bottom corner each map piece
                    MakeMapNodeObject(gridLocX, gridLocY, gridLocZ, startY, finishX, finishZ);


                    spawnPosX += sizeOfCubes;
					_spawn.transform.localPosition = new Vector3 (spawnPosX, spawnPosY, spawnPosZ);
					gridLocX += 1;

				}
				spawnPosZ += sizeOfCubes;
				gridLocZ += sizeOfCubes;
			}
			gridLocY += sizeOfCubes;
		}
    }


    // Create empty objects at locations to see the locations (debugging purposes)
    private void MakeDebugObject(int gridLocX, int gridLocY, int gridLocZ)
    {
        GameObject GridObject = Instantiate(_gridObjectPrefab, _spawn.transform, false);
        GridObject.transform.position = new Vector3(gridLocX, gridLocY, gridLocZ);
        GridObject.transform.SetParent(this.gameObject.transform);
        GridObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
    }

    // node objects are spawned at bottom corner each map piece
    private void MakeMapNodeObject(int gridLocX, int gridLocY, int gridLocZ, int startY, int finishX, int finishZ)
    {
        //////////////////////////////////////////
        int multiple = totalXZCubes / numMapPiecesXZ;

        if (gridLocX % multiple == 0 && gridLocZ % multiple == 0 && gridLocY == startY)
        {
            //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
            GameObject nodeObject = Instantiate(GetPrefab(), _spawn.transform, false);
            nodeObject.transform.position = new Vector3(gridLocX, gridLocY, gridLocZ);
            nodeObject.transform.SetParent(this.gameObject.transform);
            nodeObject.transform.localScale = new Vector3(sizeOfCubes, sizeOfCubes, sizeOfCubes);
            _GridNodePositions.Add(new Vector3(gridLocX, gridLocY, gridLocZ));
        }
        /////////////////////////////////////////////
    }

    private GameObject GetPrefab()
    {
        switch (prefabSelector)
        {
            case 0:
                return _mapNodePrefab;
            case 1:
                return _connectNodePrefab;
            case 2:
                return _shipNodePrefab;
            default:
                Debug.Log("OPPSALAL WE HAVE AN ISSUE HERE");
                return null;
        }
    }

}
	

