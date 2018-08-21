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


	void Awake() {

		_locationManager = transform.parent.GetComponent<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder> ();
		if(_cubeBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


	public void AttachMapPieceToMapNode(List<Vector3> nodes) {

		StartCoroutine (BuildMapsByIEnum (nodes, 0.001f));
	}
		

	private IEnumerator BuildMapsByIEnum(List<Vector3> nodes, float waitTime) {


        int numMapPiecesXZ = _mapSettings.numMapPiecesXZ;
        int sizeSquared = numMapPiecesXZ * numMapPiecesXZ;
        int sizeOfMapPieces = _mapSettings.sizeOfMapPieces;

        Vector3 GridLoc;

		List<int[,]> layers = new List<int[,]>();
		int[,] floor;


		int nodeCount = 0;
		int layerCount = -1;

        // This is to have same roofs to special floors
        int[,] pieceRotStore = new int[sizeSquared, 2];

        for (int j = 0; j < nodes.Count; j++)
        {
            int posX = (int)nodes[j].x;
            int posY = (int)nodes[j].y;
            int posZ = (int)nodes[j].z;

            int modulusResult = nodeCount % sizeSquared;

            if (modulusResult == 0)
            { // clever way to figure out each increase in Layer
                layerCount += 1;
            }

            int mapPieceType = (layerCount % 2 == 0) ? 0 : 1; // floors/Vents
            int mapPiece = Random.Range(1, 4); //Map pieces // 0 = Entrance so dont use here
            int rotation = Random.Range(0, 4);

            // Working Out Corners
            bool corner = false;
            if (modulusResult == 0 ||
                modulusResult == (numMapPiecesXZ - 1) ||
                modulusResult == (sizeSquared - 1) - (numMapPiecesXZ - 1) ||
                modulusResult == sizeSquared - 1)
            {
                corner = true;
                mapPiece = 2;
            }

            // Ships Edges without corners
            if (!corner)
            {
                if (modulusResult >= 0 && modulusResult <= (numMapPiecesXZ - 1)) // Bottom entrances
                {
                    rotation = 3;
                    mapPiece = 0;
                }
                else if (modulusResult >= (sizeSquared - 1) - (numMapPiecesXZ - 1) && modulusResult <= sizeSquared - 1) // Top Entrances
                {
                    rotation = 1;
                    mapPiece = 0;
                }
                else if (nodeCount % numMapPiecesXZ == 0) // Left Entrances ?
                {
                    rotation = 2;
                    mapPiece = 0;
                }
                else if (nodeCount % numMapPiecesXZ == (numMapPiecesXZ - 1)) // Right Entrances ?
                {
                    rotation = 4;
                    mapPiece = 0;
                }
            }

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
            nodeCount += 1;

            yield return new WaitForSeconds(waitTime);
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
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Corridor_02>();
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
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Corridor_02>();
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
