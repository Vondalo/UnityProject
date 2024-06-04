using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField]
    Tilemap backGround;
    [SerializeField]
    private Tilemap Art;

    public List<Vector3> doorPositions = new List<Vector3>();

    void Awake()
    {
        // Find all door positions in the room
        FindDoors(transform);
    }

    void FindDoors(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("DOOR"))
            {
                doorPositions.Add(child.position);
            }
            else
            {
                // Recursively search for doors in children
                FindDoors(child);
            }
        }
    }
}
