using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum RoomType { Regular, Hallway };

public class Room : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private Tilemap backGround;
    [SerializeField] private Tilemap Art;

    [Header("Room Properties")]
    [SerializeField] public List<(Vector3, dir, GameObject)> Exits = new List<(Vector3, dir, GameObject)>();
    [SerializeField] public int Index;
    [SerializeField] public int RoomNumber;
    [SerializeField] public List<Room> Neighbors = new List<Room>();
    public RoomType roomType;  // Add room type field

    void Start(){
        if(Neighbors.Count == 0){
            Destroy(this.gameObject);
        
        }
    }

    void Awake()
    {
        FindDoors(this.gameObject);
    }

    void FindDoors(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag("DOOR"))
            {
                var doorComponent = child.GetComponent<Door>();

                if (doorComponent != null)
                {
                    Exits.Add((child.transform.position, doorComponent.direction, child.gameObject));
                    Debug.Log($"Found Door in Room {RoomNumber} with direction {doorComponent.direction}");
                }
            }
            else
            {
                // Recursively search for doors in children
                FindDoors(child.gameObject);
            }
        }
    }
}
