using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    // Singleton instance
    public static Map Instance { get; private set; }

    // List of positions (placeholders for now)
    public List<Transform> positions;

    // Player game object
    public GameObject player;

    private int currentPositionIndex = 0;

    void Awake()
    {
        // Ensure only one instance of Map exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to roll the dice
    public int RollDice()
    {
        return Random.Range(1, 7); // Returns a number between 1 and 6
    }

    // Method to move the player
    public void MovePlayer(int steps)
    {
        currentPositionIndex += steps;
        if (currentPositionIndex >= positions.Count)
        {
            currentPositionIndex = positions.Count - 1; // Ensure index is within bounds
        }
        player.transform.position = positions[currentPositionIndex].position;
    }
}
