using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbors
{
    public string directionName;
    public bool inTheGrid;
    public bool availability;
    public Vector3Int gridCoordinates;
    public float luckStrenght;

    public Neighbors(string directionName, bool theAvailability, Vector3Int theGridCoordinates, float theLuckStrenght, bool cubeInthegrid = true)
    {
        this.directionName = directionName;
        inTheGrid = cubeInthegrid;
        availability = theAvailability;
        gridCoordinates = theGridCoordinates;
        luckStrenght = theLuckStrenght;
    }
}

public class CubeScript : MonoBehaviour
{
    public PlacementSystem placementSystem;
    public Vector3Int positionOnGrid;
    public GameObject neihborsCube;
    public enum BlockType
    {
        empty,
        city
    }

    //Neighbors of the cube in that order : right/Left/Top/Down/Back/Forth
    public Neighbors[] neighbors = new Neighbors[6];

    public float sideLuckStrenght = 0f;
    public float topLuckStrenght = 50f;
    public float bottomLuckStrenght = 0f;

    public BlockType type;

    public bool right;
    public bool left;
    public bool top;
    public bool down;
    public bool back;
    public bool forth;

    // Start is called before the first frame update
    void Start()
    {
        placementSystem.countTest++;
        neighbors[0] = new Neighbors("Right",false, positionOnGrid + new Vector3Int(1,0,0), placementSystem.goSidesStrenght);
        neighbors[1] = new Neighbors("Left", false, positionOnGrid + new Vector3Int(-1, 0, 0), placementSystem.goSidesStrenght);
        neighbors[2] = new Neighbors("Top", false, positionOnGrid + new Vector3Int(0, 1, 0), placementSystem.goUpStrenght);
        neighbors[3] = new Neighbors("Down", false, positionOnGrid + new Vector3Int(0, -1, 0), placementSystem.goDownStrenght);
        neighbors[4] = new Neighbors("Back", false, positionOnGrid + new Vector3Int(0, 0, 1), placementSystem.goSidesStrenght);
        neighbors[5] = new Neighbors("Forth", false, positionOnGrid + new Vector3Int(0, 0, -1), placementSystem.goSidesStrenght);
        for (int i = 0; i < 6; i++)
        {
            if(neighbors[i].gridCoordinates.x<0 ||
               neighbors[i].gridCoordinates.x >= placementSystem.width ||
               neighbors[i].gridCoordinates.y < 0 ||
               neighbors[i].gridCoordinates.y >= placementSystem.height ||
               neighbors[i].gridCoordinates.z < 0 ||
               neighbors[i].gridCoordinates.z >= placementSystem.lenght)
            {
                neighbors[i].inTheGrid = false;
            }
        }
        if (neighbors[3].inTheGrid)
        {
            //AddBlock(neighbors[3].gridCoordinates);
        }     
        CheckAvailability();
    }

    // Update is called once per frame
    void Update()
    {
        if(placementSystem.worldGrid[positionOnGrid.x, positionOnGrid.y, positionOnGrid.z] == PlacementSystem.BlockType.empty)
        {
            type = BlockType.empty;
        }else{
            type = BlockType.city;
        }
        CheckAvailability();
    }

    public void CheckAvailability()
    {
        bool atLeastOnSubscriber = false;
        for(int i = 0; i<6; i++)
        {
            if(neighbors[i].inTheGrid && placementSystem.worldGrid[neighbors[i].gridCoordinates.x, neighbors[i].gridCoordinates.y, neighbors[i].gridCoordinates.z] == PlacementSystem.BlockType.empty)
            {
                neighbors[i].availability = true;
                atLeastOnSubscriber = true;
            }
            else
            {
                neighbors[i].availability = false;
            }

            right = neighbors[0].availability;
            left = neighbors[1].availability;
            top = neighbors[2].availability;
            down = neighbors[3].availability;
            back = neighbors[4].availability;
            forth = neighbors[5].availability;
        }

        if (atLeastOnSubscriber)
        {
            placementSystem.SubscribeToGrow(gameObject);
        }
        else
        {
            placementSystem.UnsubscribeToGrow(gameObject);
        }
    }

    private void AddBlock(Vector3Int gridPosition)
    {
        Vector3 spawnPosition = placementSystem.grid.CellToWorld(gridPosition) + placementSystem.offset;
        GameObject newBlock = Instantiate(neihborsCube, spawnPosition, Quaternion.identity);

    }
}
