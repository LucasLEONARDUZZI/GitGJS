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
    private int width = 20;
    private int height = 20;
    private int lenght = 20;
    private List<Vector3Int> candidatsNew = new List<Vector3Int>();
    private List<Vector3Int> candidatsUp = new List<Vector3Int>();
    public Vector3 Control;
    public GameObject selector, voxIndicator;
    public Vector3 offset;
    public Grid grid;
    public float tic = 0.5f;

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
                AddBloc(gridPosition);
            }
        }
    }

    private void AddBloc(Vector3Int gridPosition)
    {
        Vector3 spawnPosition = grid.CellToWorld(gridPosition) + offset;
        Instantiate(voxIndicator, spawnPosition, Quaternion.identity);
        worldGrid[gridPosition.x, gridPosition.y, gridPosition.z] = BlockType.city;
        
    }

    private void WorldUpdate()
    {
        if (candidatsNew.Count > 0)
        {
            NewBuilding(Random.Range(0, candidatsNew.Count));
        }

        if (candidatsUp.Count > 0)
        {
            GrowBuilding(Random.Range(0, candidatsUp.Count));
        }

    }


    private void NewBuilding(int spawnIndex)
    {
        Vector3Int elected = candidatsNew[spawnIndex];
        if (worldGrid[elected.x, elected.y + 1, elected.z] == BlockType.empty)
        {
            AddBloc(new Vector3Int(elected.x, elected.y + 1, elected.z));
            candidatsNew.RemoveAt(spawnIndex);
            candidatsUp.Add(new Vector3Int(elected.x, elected.y+1, elected.z));
        }
    }

    private void GrowBuilding(int spawnIndex)
    {
        Vector3Int elected = candidatsUp[spawnIndex];
        if (worldGrid[elected.x, elected.y + 1, elected.z] == BlockType.empty)
        {
            AddBloc(new Vector3Int(elected.x, elected.y + 1, elected.z));
            candidatsUp.RemoveAt(spawnIndex);
            candidatsUp.Add(new Vector3Int(elected.x, elected.y + 1, elected.z));
        }
    }
}
