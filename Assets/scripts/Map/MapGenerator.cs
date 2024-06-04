using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int numberOfRoomsToGenerate;
    public Transform roomParent;
    public LayerMask roomLayerMask;

    private List<GameObject> placedRooms = new List<GameObject>();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
            
    }
}