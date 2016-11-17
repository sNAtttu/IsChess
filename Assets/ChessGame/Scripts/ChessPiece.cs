using UnityEngine;
using System.Collections;
using System;
using System.Net;

public class ChessPiece : MonoBehaviour
{
    public bool IsSelected = false;
    WebClient wc;

    // Use this for initialization
    void Start()
    {
        wc = new WebClient();
    }

    #region Envoy Movement Methods
    public void MoveUpRight()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x - 1), Mathf.RoundToInt(transform.position.z - 1));
    }
    public void MoveUpLeft()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x + 1), Mathf.RoundToInt(transform.position.z - 1));
    }
    public void MoveDownRight()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x - 1), Mathf.RoundToInt(transform.position.z + 1));
    }
    public void MoveDownLeft()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x + 1), Mathf.RoundToInt(transform.position.z + 1));
    }
    #endregion

    #region King Movement Methods
    public void MoveUp()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z + 1));
    }
    public void MoveDown()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z - 1));
    }
    public void MoveLeft()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x - 1), Mathf.RoundToInt(transform.position.z));
    }
    public void MoveRight()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), 1f);
        SendMovementDataToServer(Mathf.RoundToInt(transform.position.x + 1), Mathf.RoundToInt(transform.position.z));
    }

    #endregion
    void OnCollisionEnter(Collision collision)
    {
        GameObject currentPiece = collision.gameObject;
        var pieceScript = currentPiece.GetComponent<FloorPieceScript>();
        if (tag == "King" && collision.collider.tag == "FloorPiece" && pieceScript != null)
        {
            pieceScript.pieceIsOccupied = true;
            pieceScript.pieceStanding = gameObject;
            Debug.Log(string.Format("KING IS STANDING ON {0}, {1}", pieceScript.coordinateX, pieceScript.coordinateY));
        }
        else if (tag == "Envoy" && collision.collider.tag == "FloorPiece" && pieceScript != null)
        {
            pieceScript.pieceIsOccupied = true;
            pieceScript.pieceStanding = gameObject;
            Debug.Log(string.Format("ENVOY IS STANDING ON {0}, {1}", pieceScript.coordinateX, pieceScript.coordinateY));
        }
    }
    private void SendMovementDataToServer(int x, int y)
    {
        ChessMovement oneMove = new ChessMovement
        {
            gameId = Guid.NewGuid().ToString(),
            movementX = x,
            movementY = y
        };
        string json = JsonUtility.ToJson(oneMove);
        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        wc.Encoding = System.Text.Encoding.UTF8;
        wc.UploadString("http://localhost:11419/api/ChessMovement/Create", "POST", json);
    }
    [Serializable]
    public class ChessMovement
    {
        [SerializeField]
        public string gameId;
        [SerializeField]
        public int movementX;
        [SerializeField]
        public int movementY;
    }
}
