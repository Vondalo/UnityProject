using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MapGen : NetworkBehaviour
{
    public Action FinishedGenerationEvent;

    private bool finishedGeneration;

    public bool FinishedGeneration
    {
        get { return finishedGeneration; }
        private set
        {
            finishedGeneration = value;
            if (finishedGeneration)
            {
                if (IsServer)
                {
                    FinishedGenerationNetworkVariable.Value = true;
                }
                FinishedGenerationEvent?.Invoke();
                FinishedGenerationEvent = null;

            }
        }
    }

    [Serializable]
    public struct PrefabProbability
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float probability;
    }

    [Serializable]
    public struct PointOfInterest
    {
        public PrefabProbability poiPrefab;
        public int maxCount;
    }

    [Header("Room-Prefabs and Spawn-Probability")]
    [SerializeField] public PrefabProbability[] roomPrefabs;

    [Header("Hallway-Prefabs and Spawn-Probability")]
    [SerializeField] public PrefabProbability[] hallwayPrefabs;

    [Header("POI-Prefabs and Occurrence-Counter")]
    [SerializeField] public PointOfInterest[] poiData;

    private GameObject[] roomPrefabArray;
    private GameObject[] hallwayPrefabArray;
    private GameObject[] poiPrefabArray;

    private float[] roomPrefabSpawnProb;
    private float[] hallwayPrefabSpawnProb;
    private int[] poiMaxCountsArray;

    [Header("Map Properties")]
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float spacingX = 50f;
    public float spacingY = 50f;
    public float spacingZ = 50f;

    public Room[,] rooms;

    [Header("Seed")]
    public NetworkVariable<int> seedNetworkVariable = new NetworkVariable<int>(0);
    public NetworkVariable<bool> FinishedGenerationNetworkVariable = new NetworkVariable<bool>(false);
    private int usedSeed; // Variable to store the actual seed used
    private System.Random random; // Random instance for deterministic random number generation

    void ExtractPrefabsAndProbabilities()
    {
        roomPrefabArray = new GameObject[roomPrefabs.Length];
        hallwayPrefabArray = new GameObject[hallwayPrefabs.Length];
        poiPrefabArray = new GameObject[poiData.Length];
        poiMaxCountsArray = new int[poiData.Length];

        roomPrefabSpawnProb = new float[roomPrefabs.Length];
        hallwayPrefabSpawnProb = new float[hallwayPrefabs.Length];

        ExtractPrefabsAndProbabilities(roomPrefabs, ref roomPrefabArray, ref roomPrefabSpawnProb);
        ExtractPrefabsAndProbabilities(hallwayPrefabs, ref hallwayPrefabArray, ref hallwayPrefabSpawnProb);

        for (int i = 0; i < poiData.Length; i++)
        {
            poiPrefabArray[i] = poiData[i].poiPrefab.prefab;
            poiMaxCountsArray[i] = poiData[i].maxCount;
        }
    }

    void ExtractPrefabsAndProbabilities(PrefabProbability[] prefabProbabilities, ref GameObject[] prefabArray, ref float[] spawnProbabilities)
    {
        for (int i = 0; i < prefabProbabilities.Length; i++)
        {
            prefabArray[i] = prefabProbabilities[i].prefab;
            spawnProbabilities[i] = prefabProbabilities[i].probability;
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("sex");
        base.OnNetworkSpawn();
        if (IsServer)
        {
            // If the server, generate and set the seed if it hasn't been set already
            if (seedNetworkVariable.Value == 0)
            {
                seedNetworkVariable.Value = DateTime.Now.GetHashCode();
            }

            // Initialize the random number generator with the seed
           

           
           
        }
        else
        {
            // Client will initialize the random number generator with the synced seed
            seedNetworkVariable.OnValueChanged += OnSeedValueChanged;
            
            


        }
        usedSeed = seedNetworkVariable.Value;
        random = new System.Random(usedSeed);
        // Log the seed used for generation
        Debug.Log($"Seed used for map generation: {usedSeed}");

        FinishedGeneration = false;
        ExtractPrefabsAndProbabilities();

        rooms = new Room[gridHeight, gridWidth];
        GenerateMap();
        ConnectRooms();
        DeleteRoomsWithNoDoors();

        // Log information about each room including door positions
        Debug.Log("Rooms:");
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                Room room = rooms[i, j];
                if (room == null) continue;

                Debug.Log($"Room Index: {room.RoomNumber}, Neighbors: [{string.Join(", ", room.Neighbors.Select(r => r.RoomNumber))}]");

                foreach ((Vector3 doorPosition, dir doorDirection, GameObject doorGameObject) in room.Exits)
                {
                    Debug.Log($"Door Position: {doorPosition}, Door Direction: {doorDirection}");
                }
            }
        }

        // Log room indices in a grid format
        Debug.Log("Rooms:");
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            string line = "";
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                Room room = rooms[i, j];
                line += room != null ? $"{room.RoomNumber}  " : "X  ";
            }
            Debug.Log(line);
        }
        FinishedGeneration = true;

    }

    private void OnSeedValueChanged(int oldSeed, int newSeed)
    {
        Debug.Log("nigga");
        usedSeed = newSeed;
        random = new System.Random(usedSeed);

        // Log the seed used for generation
        Debug.Log($"Client received seed for map generation: {usedSeed}");

        // Clients can regenerate the map based on the received seed if necessary
        FinishedGeneration = false;
        ExtractPrefabsAndProbabilities();

        rooms = new Room[gridHeight, gridWidth];
        GenerateMap();
        ConnectRooms();
        DeleteRoomsWithNoDoors();

        // Log information about each room including door positions
        Debug.Log("Rooms:");
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                Room room = rooms[i, j];
                if (room == null) continue;

                Debug.Log($"Room Index: {room.RoomNumber}, Neighbors: [{string.Join(", ", room.Neighbors.Select(r => r.RoomNumber))}]");

                foreach ((Vector3 doorPosition, dir doorDirection, GameObject doorGameObject) in room.Exits)
                {
                    Debug.Log($"Door Position: {doorPosition}, Door Direction: {doorDirection}");
                }
            }
        }

        // Log room indices in a grid format
        Debug.Log("Rooms:");
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            string line = "";
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                Room room = rooms[i, j];
                line += room != null ? $"{room.RoomNumber}  " : "X  ";
            }
            Debug.Log(line);
        }
        FinishedGeneration = true;
    }

    void GenerateMap()
    {
        float gapProbability = 0.2f; // 20% chance to skip a room and create a gap
        int poiCounter = 0; // Counter to track POI placement
        Dictionary<GameObject, int> poiCounts = new Dictionary<GameObject, int>();

        for (int i = 0; i < poiPrefabArray.Length; i++)
        {
            poiCounts[poiPrefabArray[i]] = 0;
        }

        // Create a list of all grid positions and shuffle it
        List<(int x, int y)> gridPositions = new List<(int x, int y)>();
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                gridPositions.Add((x, y));
            }
        }
        gridPositions = gridPositions.OrderBy(pos => random.Next()).ToList();

        foreach (var (x, y) in gridPositions)
        {
            if (random.NextDouble() < gapProbability)
            {
                // Leave a gap in the grid
                rooms[y, x] = null;
                continue;
            }

            GameObject selectedRoomPrefab = null;

            // Place POIs according to their constraints
            if (poiCounter < poiPrefabArray.Length)
            {
                var poi = poiPrefabArray[poiCounter];
                if (poiCounts[poi] < poiMaxCountsArray[poiCounter])
                {
                    selectedRoomPrefab = poi;
                    poiCounts[poi]++;
                    poiCounter++;
                }
            }

            // If no POI is selected, determine whether to place a room or hallway
            if (selectedRoomPrefab == null)
            {
                // Place hallways in a checkered pattern
                if ((x + y) % 2 == 1)
                {
                    // Place a hallway
                    selectedRoomPrefab = SelectPrefab(hallwayPrefabArray, hallwayPrefabSpawnProb);
                }
                else
                {
                    // Place a room
                    selectedRoomPrefab = SelectPrefab(roomPrefabArray, roomPrefabSpawnProb);
                }
            }

            // Calculate the position for each room
            Vector3 position = new Vector3(x * spacingX, (gridHeight - 1 - y) * spacingY, (gridHeight - 1 - y) * spacingZ);
            // Instantiate the room prefab at the calculated position
            GameObject roomInstance = Instantiate(selectedRoomPrefab, position, Quaternion.identity);
            
            
            Room roomScript = roomInstance.GetComponent<Room>();
            roomScript.RoomNumber = y * gridWidth + x; // Set the room number
            rooms[y, x] = roomScript;
            roomInstance.name = roomScript.RoomNumber.ToString();
        }
    }

    GameObject SelectPrefab(GameObject[] prefabs, float[] spawnProbs)
    {
        float randomValue = (float)random.NextDouble();
        float cumulativeProbability = 0f;

        for (int i = 0; i < prefabs.Length; i++)
        {
            cumulativeProbability += spawnProbs[i];
            if (randomValue <= cumulativeProbability)
            {
                return prefabs[i];
            }
        }

        // If no prefab was selected (edge case), select the last prefab
        return prefabs[prefabs.Length - 1];
    }

    void ConnectRooms()
    {
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                Room currentRoom = rooms[i, j];
                if (currentRoom == null) continue; // Skip if there's a gap

                Debug.Log($"Checking connections for Room {currentRoom.RoomNumber}");

                // Add room above if exists and there is a door leading up
                if (i > 0 && rooms[i - 1, j] != null && currentRoom.Exits.Exists(door => door.Item2 == dir.up))
                {
                    Room above = rooms[i - 1, j];
                    ConnectDoors(currentRoom, above, dir.up);
                    if (above != null)
                    {
                        currentRoom.Neighbors.Add(above); // Add the above room as a neighbor
                        Debug.Log($"Room {currentRoom.RoomNumber} added Room {above.RoomNumber} as a neighbor above.");
                    }
                    else
                    {
                        Debug.LogWarning($"Room {currentRoom.RoomNumber} failed to add Room {above.RoomNumber} as a neighbor above.");
                    }
                }
                else
                {
                    DestroyDoor(currentRoom, dir.up);
                    Debug.Log($"Room {currentRoom.RoomNumber} does not have an exit leading up or it is on the top row.");
                }

                // Add room below if exists and there is a door leading down
                if (i < rooms.GetLength(0) - 1 && rooms[i + 1, j] != null && currentRoom.Exits.Exists(door => door.Item2 == dir.down))
                {
                    Room below = rooms[i + 1, j];
                    ConnectDoors(currentRoom, below, dir.down);
                    if (below != null)
                    {
                        currentRoom.Neighbors.Add(below); // Add the below room as a neighbor
                        Debug.Log($"Room {currentRoom.RoomNumber} added Room {below.RoomNumber} as a neighbor below.");
                    }
                    else
                    {
                        Debug.LogWarning($"Room {currentRoom.RoomNumber} failed to add Room {below.RoomNumber} as a neighbor below.");
                    }
                }
                else
                {
                    DestroyDoor(currentRoom, dir.down);
                    Debug.Log($"Room {currentRoom.RoomNumber} does not have an exit leading down or it is on the bottom row.");
                }

                // Add room to the right if exists and there is a door leading right
                if (j < rooms.GetLength(1) - 1 && rooms[i, j + 1] != null && currentRoom.Exits.Exists(door => door.Item2 == dir.right))
                {
                    Room right = rooms[i, j + 1];
                    ConnectDoors(currentRoom, right, dir.right);
                    if (right != null)
                    {
                        currentRoom.Neighbors.Add(right); // Add the right room as a neighbor
                        Debug.Log($"Room {currentRoom.RoomNumber} added Room {right.RoomNumber} as a neighbor to the right.");
                    }
                    else
                    {
                        Debug.LogWarning($"Room {currentRoom.RoomNumber} failed to add Room {right.RoomNumber} as a neighbor to the right.");
                    }
                }
                else
                {
                    DestroyDoor(currentRoom, dir.right);
                    Debug.Log($"Room {currentRoom.RoomNumber} does not have an exit leading right or it is on the far right.");
                }

                // Add room to the left if exists and there is a door leading left
                if (j > 0 && rooms[i, j - 1] != null && currentRoom.Exits.Exists(door => door.Item2 == dir.left))
                {
                    Room left = rooms[i, j - 1];
                    ConnectDoors(currentRoom, left, dir.left);
                    if (left != null)
                    {
                        currentRoom.Neighbors.Add(left); // Add the left room as a neighbor
                        Debug.Log($"Room {currentRoom.RoomNumber} added Room {left.RoomNumber} as a neighbor to the left.");
                    }
                    else
                    {
                        Debug.LogWarning($"Room {currentRoom.RoomNumber} failed to add Room {left.RoomNumber} as a neighbor to the left.");
                    }
                }
                else
                {
                    DestroyDoor(currentRoom, dir.left);
                    Debug.Log($"Room {currentRoom.RoomNumber} does not have an exit leading left or it is on the far left.");
                }
            }
        }
    }

    bool IsValidHallwayLocation(int x, int y)
    {
        // Check if the neighboring rooms allow a hallway to be spawned
        if ((x > 0 && rooms[y, x - 1] != null && rooms[y, x - 1].roomType == RoomType.Regular) ||
            (x < rooms.GetLength(1) - 1 && rooms[y, x + 1] != null && rooms[y, x + 1].roomType == RoomType.Regular) ||
            (y > 0 && rooms[y - 1, x] != null && rooms[y - 1, x].roomType == RoomType.Regular) ||
            (y < rooms.GetLength(0) - 1 && rooms[y + 1, x] != null && rooms[y + 1, x].roomType == RoomType.Regular))
        {
            return true;
        }

        return false;
    }

    void ConnectDoors(Room room1, Room room2, dir direction)
    {
        // Find the door in room1 that matches the specified direction
        GameObject door1Object = room1.Exits.FirstOrDefault(door => door.Item2 == direction).Item3;
        if (door1Object == null) return;
        Door door1 = door1Object.GetComponent<Door>();

        // Find the door in room2 that matches the opposite direction
        dir oppositeDirection = GetOppositeDirection(direction);
        GameObject door2Object = room2.Exits.FirstOrDefault(door => door.Item2 == oppositeDirection).Item3;
        if (door2Object == null) return;
        Door door2 = door2Object.GetComponent<Door>();

        // Connect the doors
        door1.connectedDoor = door2Object;
        door2.connectedDoor = door1Object;

        // Log connection details
        Debug.Log($"Room {room1.RoomNumber} door {direction} connected to Room {room2.RoomNumber} door {oppositeDirection}");
    }

    void DestroyDoor(Room room, dir direction)
    {
        var door = room.Exits.FirstOrDefault(d => d.Item2 == direction);
        if (door != default)
        {
            Destroy(door.Item3); // Destroy the door GameObject
            room.Exits.Remove(door); // Remove it from the list of exits
        }
    }

    void DeleteRoomsWithNoDoors()
    {
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                Room room = rooms[i, j];
                if (room != null && room.Exits.Count == 0)
                {
                    Destroy(room.gameObject); // Destroy the room GameObject
                    rooms[i, j] = null; // Set the array element to null
                    Debug.Log($"Room {room.RoomNumber} has been destroyed due to no exits.");
                }
            }
        }
    }

    dir GetOppositeDirection(dir direction)
    {
        switch (direction)
        {
            case dir.up: return dir.down;
            case dir.down: return dir.up;
            case dir.left: return dir.right;
            case dir.right: return dir.left;
            default: return dir.up;
        }
    }
}
