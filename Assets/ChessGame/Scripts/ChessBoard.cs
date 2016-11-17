using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChessBoard : MonoBehaviour
{
    GameObject chessBoard;
    GameObject King;
    GameObject Envoy;
    List<FloorPieceWrapper> pieces;

    public int ChessboardHeight = 8;
    public int ChessboardWidth = 8;
    public int FloorPieceWidth = 1;
    public int FloorPieceHeight = 1;

    private const float SpawnHeight = 0.5f;

    void Awake()
    {
        pieces = new List<FloorPieceWrapper>();
        chessBoard = gameObject;
        Envoy = GameObject.FindGameObjectWithTag("Envoy");
        King = GameObject.FindGameObjectWithTag("King");
    }
    void Start()
    {
        GameObject floorPieceWhite = Resources.Load("FloorPieceWhite") as GameObject;
        GameObject floorPieceBrown = Resources.Load("FloorPieceBrown") as GameObject;
        GameObject wallPiece = Resources.Load("WallPiece") as GameObject;
        GameObject King = Resources.Load("King") as GameObject;
        GameObject Envoy = Resources.Load("Envoy") as GameObject;

        float x = 0f, y = 0f, z = 0f;
        int CoordinateX = 0;
        int CoordinateY = 0;

        Vector3 KingSpawnPosition = new Vector3(0, SpawnHeight, 0);
        Vector3 EnvoySpawnPosition = new Vector3((ChessboardWidth - 1f), SpawnHeight, 0);
        Vector3 onePieceSpawnPosition = new Vector3(x, y, z);
        Quaternion onePieceSpawnRotation = new Quaternion();
        onePieceSpawnRotation.SetLookRotation(Vector3.up);

        for(var row = 0; row < ChessboardWidth; row++)
        {
            onePieceSpawnPosition.z = 0; // Reset position on z axel so we can start a new column
            CoordinateY = 0;
            for (var col = 0; col < ChessboardHeight; col++)
            {
                CreateFloorPiece(onePieceSpawnPosition, onePieceSpawnRotation, CoordinateX, CoordinateY, floorPieceWhite, floorPieceBrown); // Method for creating an individual piece of floor
                CreateWallPiece(wallPiece, onePieceSpawnPosition, onePieceSpawnRotation); // Method for creating an individual wall piece
                onePieceSpawnPosition.z += FloorPieceHeight; // Move piece "up" so they do not spawn on top of each other
                CoordinateY++;
            }
            onePieceSpawnPosition.x += FloorPieceWidth; // Move piece to right so they do not spawn on top of each other
            CoordinateX++; // Move to next row
        }
    }

    private void CreateWallPiece(GameObject wallPiece, Vector3 onePieceSpawnPosition, Quaternion onePieceSpawnRotation)
    {
        GameObject createdWallPiece;
        float x = onePieceSpawnPosition.x, y = onePieceSpawnPosition.y, z = onePieceSpawnPosition.z;
        if (x == 0)
        {
            if(z == 0)
            {
                createdWallPiece = Instantiate(wallPiece, new Vector3(x - 1, y, z - 1), onePieceSpawnRotation) as GameObject;
                createdWallPiece.transform.SetParent(chessBoard.transform);
            }
            else if(z == (ChessboardHeight - 1))
            {
                createdWallPiece = Instantiate(wallPiece, new Vector3(x - 1, y, z + 1), onePieceSpawnRotation) as GameObject;
                createdWallPiece.transform.SetParent(chessBoard.transform);
            }
            createdWallPiece = Instantiate(wallPiece, new Vector3(x - 1, y, z), onePieceSpawnRotation) as GameObject;
            createdWallPiece.transform.SetParent(chessBoard.transform);
        }
        if (z == 0)
        {
            createdWallPiece = Instantiate(wallPiece, new Vector3(x, y, z - 1), onePieceSpawnRotation) as GameObject;
            createdWallPiece.transform.SetParent(chessBoard.transform);
        }
        if (x == (ChessboardWidth - 1))
        {
            if (z == 0)
            {
                createdWallPiece = Instantiate(wallPiece, new Vector3(x + 1, y, z - 1), onePieceSpawnRotation) as GameObject;
                createdWallPiece.transform.SetParent(chessBoard.transform);
            }
            else if (z == (ChessboardHeight - 1))
            {
                createdWallPiece = Instantiate(wallPiece, new Vector3(x + 1, y, z + 1), onePieceSpawnRotation) as GameObject;
                createdWallPiece.transform.SetParent(chessBoard.transform);
            }
            createdWallPiece = Instantiate(wallPiece, new Vector3(x + 1, y, z), onePieceSpawnRotation) as GameObject;
            createdWallPiece.transform.SetParent(chessBoard.transform);
        }
        if (z == (ChessboardHeight - 1))
        {
            createdWallPiece = Instantiate(wallPiece, new Vector3(x, y, z + 1), onePieceSpawnRotation) as GameObject;
            createdWallPiece.transform.SetParent(chessBoard.transform);
        }
    }

    private void CreateFloorPiece(Vector3 onePieceSpawnPosition, Quaternion onePieceSpawnRotation, int CoordinateX, int CoordinateY, GameObject floorPieceWhite, GameObject floorPieceBrown)
    {
        // We check if the differential is divisible by two and based on that we select material for piece
        // now every other is brown and every other is white
        GameObject createdPiece;
        if (Mathf.Abs((CoordinateX - CoordinateY)) % 2 == 0) 
        {
            createdPiece = Instantiate(floorPieceBrown, onePieceSpawnPosition, onePieceSpawnRotation) as GameObject;
        }
        else
        {
            createdPiece = Instantiate(floorPieceWhite, onePieceSpawnPosition, onePieceSpawnRotation) as GameObject;
        }

        createdPiece.transform.SetParent(chessBoard.transform); // Assign chessboard as a parent for our piece so all pieces are listed underneath it.
        var floorPieceLocation = createdPiece.GetComponent<FloorPieceScript>();
        floorPieceLocation.coordinateX = CoordinateX; // Assign X coordinate for piece object
        floorPieceLocation.coordinateY = CoordinateY; // Assign Y coordinate for piece object
        pieces.Add(new FloorPieceWrapper { FloorPieceScript = floorPieceLocation, Piece = createdPiece});
    }

    // Update is called once per frame
    void Update()
    {
        SelectPiece(Envoy, King);
        CheckIfChess(Envoy, King);
    }

    private void CheckIfChess(GameObject envoy, GameObject king)
    {
        int envoyX = Mathf.RoundToInt(envoy.transform.position.x), envoyZ = Mathf.RoundToInt(envoy.transform.position.z); // We use our envoys transform position rounded to int as a coordinate
        int kingX = Mathf.RoundToInt(king.transform.position.x), kingZ = Mathf.RoundToInt(king.transform.position.z); // We use our kings transform position rounded to int as a coordinate
        if (((Mathf.Abs(envoyZ - kingZ) - Mathf.Abs(envoyX - kingX))) == 0) // Something that I learned in job interview. If absolute value of coordinates differential is zero, envoy wins.
        {
            king.GetComponent<Rigidbody>().AddForce(Vector3.back, ForceMode.Impulse); // King flies off from board
        }
        else if(Mathf.Abs(kingX - envoyX) <= 1 && Mathf.Abs(kingZ - envoyZ) <= 1) // If our king is within range of one cube to envoy, king wins the game.
        {
            envoy.GetComponent<Rigidbody>().AddForce(Vector3.forward, ForceMode.Impulse); // Envoy flies off from board
        }

    }

    private void SelectPiece(GameObject Envoy, GameObject King)
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit mouseHitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mouseHitInfo); // If our mouse click has hit a gameobjects collider
            if (hit)
            {
                Envoy.GetComponent<ChessPiece>().IsSelected = false; // Reset selected
                King.GetComponent<ChessPiece>().IsSelected = false;
                var chessPiece = mouseHitInfo.transform.gameObject.GetComponent<ChessPiece>();
                chessPiece.IsSelected = true;
            }
        }
    }

    private void CheckOccupiedPieces()
    {
        foreach (var piece in pieces)
        {
            var pieceInfo = piece.FloorPieceScript;
            if (pieceInfo.pieceIsOccupied)
            {
                //Debug.Log(string.Format("{0} is standing on piece: {1}, {2}", pieceInfo.pieceStanding.tag, pieceInfo.coordinateX, pieceInfo.coordinateY));
            }
        }
    }

}
public class FloorPieceWrapper
{
    public FloorPieceScript FloorPieceScript { get; set; }
    public GameObject Piece { get; set; }
}