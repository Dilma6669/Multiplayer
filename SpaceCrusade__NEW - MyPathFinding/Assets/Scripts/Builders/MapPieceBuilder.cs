using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPieceBuilder: MonoBehaviour {

	LocationManager _locationManager;
	CubeBuilder _cubeBuilder;
	MapSettings _mapSettings;


	public List<int[,]> floors = new List<int[,]>();
	public List<int[,]> vents = new List<int[,]>();

	private bool loadVents = false;

    int numMapPiecesXZ;
    int sizeSquared;
    int sizeOfMapPieces;

    int nodeCount = 0;
    int layerCount = -1;
    int cameraLayerCount = 0;

    int[,] pieceRotStore;

    void Awake() {

		_locationManager = transform.parent.GetComponent<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder> ();
		if(_cubeBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


	public void AttachMapPieceToMapNode(List<Vector3> nodes, int _LayerCount, float waitTime) {

        numMapPiecesXZ = _mapSettings.numMapPiecesXZ;
        sizeSquared = numMapPiecesXZ * numMapPiecesXZ;
        sizeOfMapPieces = _mapSettings.sizeOfMapPiecesXZ;

        nodeCount = 0;
        layerCount = _LayerCount;

        // This is to have same roofs to special floors
        pieceRotStore = new int[sizeSquared, 2];

        for (int j = 0; j < nodes.Count; j++)
        {
            //Debug.Log("fucken layerCount 2<<<<<: " + layerCount);
            BuildMapsByIEnum(nodes[j], j);

            nodeCount += 1;

         //yield return new WaitForSeconds(waitTime);
        }
    }


    private void BuildMapsByIEnum(Vector3 nodeLoc, int j)
    {
        int posX = (int)nodeLoc.x;
        int posY = (int)nodeLoc.y;
        int posZ = (int)nodeLoc.z;

        Vector3 GridLoc;

        List<int[,]> layers = new List<int[,]>();
        int[,] floor;

        int modulusResult = nodeCount % sizeSquared;

        // clever way to figure out each increase in Layer
        if (modulusResult == 0)
        {
            layerCount += 1;
        }
        //Debug.Log("fucken layerCount 3<<<<<: " + layerCount);
        int mapPieceType = (layerCount % 2 == 0) ? 0 : 1; // floors/Vents
        int mapPiece = Random.Range(1, 4); //Map pieces // 0 = Entrance so dont use here
        int rotation = Random.Range(0, 4);

        // If floor rememeber settings to apply to vents (NOTE: this could be used to make some pieces not deterministic)
        if (mapPieceType == 0)
        {
            pieceRotStore[j % sizeSquared, 0] = mapPiece;
            pieceRotStore[j % sizeSquared, 1] = rotation;
        }
        else
        {
            mapPiece = pieceRotStore[j % sizeSquared, 0];
            rotation = pieceRotStore[j % sizeSquared, 1];
        } 
        

        layers = GetMapPiece(mapPieceType, mapPiece);
        int rotations = rotation;

        int objectsCountX = posX;
        int objectsCountY = posY;
        int objectsCountZ = posZ;

        for (int y = 0; y < layers.Count; y++)
        {

            objectsCountX = posX;
            objectsCountZ = posZ;

            floor = layers[y];

            for (int r = 0; r < rotations; r++)
            {
                floor = TransposeArray(floor, sizeOfMapPieces - 1);
            }

            for (int z = 0; z < floor.GetLength(0); z++)
            {

                objectsCountX = posX;

                for (int x = 0; x < floor.GetLength(1); x++)
                {

                    int cubeType = floor[z, x];
                    GridLoc = new Vector3(objectsCountX, objectsCountY, objectsCountZ);

                    _cubeBuilder.CreateCubeObject(GridLoc, cubeType, rotations, layerCount); // Create the cube

                    objectsCountX += 1;
                }
                objectsCountZ += 1;
            }
            objectsCountY += 1;
        }
    }



	// Get map by type and piece (NOTE: at the moment needs to be same amount of case's in each type)
	private List<int[,]> GetMapPiece(int type, int map) {

		switch (type) { 
		case 0: // Floor
			BaseMapPiece mapPiece = null;
                switch (map)
                {
                    case 0:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Entrance_01>();
                        break;
                    case 1:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Corridor_01>();
                        break;
                    case 2:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Room_01>();
                        break;
                    case 3:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Room_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE!");
                        break;
                }
			return mapPiece.floors;

		case 1:
                BaseMapPiece ventPiece = null;
                switch (map)
                {
                    case 0:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    case 1:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    case 2:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    case 3:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE!");
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
