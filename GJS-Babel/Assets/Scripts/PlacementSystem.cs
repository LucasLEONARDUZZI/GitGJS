using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    public enum BlockType
    {
        empty,
        city
    }
    public BlockType[,,] worldGrid = new BlockType[20, 20, 20];
    public int width = 20;
    public int height = 20;
    public int lenght = 20;
    private List<Vector3Int> candidatsNew = new List<Vector3Int>();
    //private List<GameObject> growTopSubscribers = new List<GameObject>();
    private List<GameObject> growSubscribers = new List<GameObject>();
    public Vector3 Control;
    public GameObject selector, voxIndicator;
    public Vector3 offset;
    public Grid grid;
    public float tic = 0.5f;
    public int countTest = 0;
    public int NumberOfBuildings = 10;

    private void Start()
    {
        InvokeRepeating("WorldUpdate", 0f, tic);
        for (int x = 0; x < width; x++) {
            for(int z = 0; z < lenght; z++)
            {
                candidatsNew.Add(new Vector3Int(x, 0, z));
            }
        }

            for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < lenght; z++)
                {
                    worldGrid[x, y, z] = BlockType.empty;
                }
            }
        }

        for (int i = 0; i < NumberOfBuildings; i++)
        {
            NewBuilding(Random.Range(0, candidatsNew.Count));
        }
    }

    private void Update()
    {
        Vector3 selectorPosition = selector.transform.position;
        Vector3Int gridPosition = grid.WorldToCell(selectorPosition);
        Control = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
        voxIndicator.transform.position = grid.CellToWorld(gridPosition)+offset;

        if (gridPosition.x >= 0 && gridPosition.x < worldGrid.GetLength(0) &&
            gridPosition.y >= 0 && gridPosition.y < worldGrid.GetLength(1) &&
            gridPosition.z >= 0 && gridPosition.z < worldGrid.GetLength(2))
        {

            if (worldGrid[gridPosition.x, gridPosition.y, gridPosition.z] == BlockType.empty)
            {
                AddBlock(gridPosition);
            }
        }
    }

    private void AddBlock(Vector3Int gridPosition)
    {
        Vector3 spawnPosition = grid.CellToWorld(gridPosition) + offset;
        GameObject newBlock = Instantiate(voxIndicator, spawnPosition, Quaternion.identity);
        newBlock.GetComponent<CubeScript>().placementSystem = this;
        newBlock.GetComponent<CubeScript>().positionOnGrid = gridPosition;
        worldGrid[gridPosition.x, gridPosition.y, gridPosition.z] = BlockType.city;
        
    }

    private void WorldUpdate()
    {
        /*if (candidatsNew.Count > 0 && NumberOfBuildings >0)
        {
            NewBuilding(Random.Range(0, candidatsNew.Count));
            NumberOfBuildings--;
        }*/

        if (growSubscribers.Count > 0)
        {
            GrowBuilding(Random.Range(0, growSubscribers.Count));
        }

    }


    private void NewBuilding(int spawnIndex)
    {
        Vector3Int elected = candidatsNew[spawnIndex];
        if (worldGrid[elected.x, elected.y, elected.z] == BlockType.empty)
        {
            AddBlock(new Vector3Int(elected.x, elected.y, elected.z));
            candidatsNew.RemoveAt(spawnIndex);
        }
    }

    /*
    public void GrowTop(int subscriberIndex)
    {
        GameObject GOsubscriber = growTopSubscribers[subscriberIndex];
        Vector3Int elected = GOsubscriber.GetComponent<CubeScript>().positionOnGrid;
        if (worldGrid[elected.x, elected.y + 1, elected.z] == BlockType.empty)
        {
            AddBlock(new Vector3Int(elected.x, elected.y + 1, elected.z));
            UnsubscribeToGrowTop(GOsubscriber);
        }
    }*/

    public void GrowBuilding(int subscriberIndex)
    {
        GameObject GOsubscriber = growSubscribers[subscriberIndex];
        Vector3Int elected = GOsubscriber.GetComponent<CubeScript>().ElectCandidate();
        
        AddBlock(new Vector3Int(elected.x, elected.y, elected.z));
    }

    public void SubscribeToGrow(GameObject subscriber)
    {
        if (!growSubscribers.Contains(subscriber))
        {
            growSubscribers.Add(subscriber);
        }
    }

    public void UnsubscribeToGrow(GameObject subscriber)
    {
        growSubscribers.Remove(subscriber);
    }
}
