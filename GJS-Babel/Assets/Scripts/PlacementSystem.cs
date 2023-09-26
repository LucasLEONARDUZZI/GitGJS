using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Candidat
{
    public GameObject goCandidat;
    public bool rightAvailable;
    public bool leftAvailable;
    public bool topAvailable;
    public bool downAvailable;
    public bool backAvailable;
    public bool forthAvailable;

    public Candidat(GameObject goCandidat)
    {
        this.goCandidat = goCandidat;
    }
}

public class PlacementSystem : MonoBehaviour
{

    public enum BlockType
    {
        empty,
        city
    }
    public enum CandidatDirection
    {
        CRight,CLeft,CTop,CDown,CBack,CForth
    }

    public BlockType[,,] worldGrid = new BlockType[20, 20, 20];
    public int width = 20;
    public int height = 20;
    public int lenght = 20;
    private List<Vector3Int> candidatsNew = new List<Vector3Int>();
    //private List<GameObject> growTopSubscribers = new List<GameObject>();
    private List<GameObject> growSubscribers = new List<GameObject>();
    private List<Candidat> candidats = new List<Candidat>();
    public Vector3 Control;
    public GameObject selector, voxIndicator;
    public Vector3 offset;
    public Grid grid;
    public float tic = 0.5f;
    public int countTest = 0;
    public int NumberOfBuildings = 10;

    [Range(0.0f, 100.0f)]
    public float goUpStrenght = 20f;
    [Range(0.0f, 100.0f)] 
    public float goSidesStrenght = 10f;
    [Range(0.0f, 100.0f)]
    public float goDownStrenght = 5f;
    public float goUpLuck;
    public float goSidesLuck;
    public float goDownLuck;

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
        goUpLuck = goUpStrenght / (goUpStrenght + goSidesStrenght + goDownStrenght) * 100f;
        //goUpLuck = 45f;
        goSidesLuck = goSidesStrenght / (goUpStrenght + goSidesStrenght + goDownStrenght) * 100f;
        goDownLuck = goDownStrenght / (goUpStrenght + goSidesStrenght + goDownStrenght) * 100f;
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

        CandidatDirection randomDirection = CandidatDirection.CTop;
        float dice = Random.Range(1f, 100f);

        if (dice <= goUpLuck)
        {
            randomDirection = CandidatDirection.CTop;
        }else if (dice <= goUpLuck + goSidesLuck)
        {
            int diceFour = Random.Range(0, 4);
            switch (diceFour)
            {
                case 0:
                    randomDirection = CandidatDirection.CRight;
                    break;
                case 1:
                    randomDirection = CandidatDirection.CLeft;
                    break;
                case 2:
                    randomDirection = CandidatDirection.CBack;
                    break;
                case 3:
                    randomDirection = CandidatDirection.CForth;
                    break;
            }
        }
        else
        {
            randomDirection = CandidatDirection.CDown;
        }
        

        Grow(randomDirection);

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

    public void Grow(CandidatDirection candidatDirection)
    {
        Candidat[] filteredCandidats = null;
        Vector3Int deltaToSpawn = new Vector3Int(0,0,0);

        switch (candidatDirection)
        {
            case CandidatDirection.CRight:
                filteredCandidats = candidats.Where(c => c.rightAvailable).ToArray();
                deltaToSpawn = new Vector3Int(1,0,0);
                break;
            case CandidatDirection.CLeft:
                filteredCandidats = candidats.Where(c => c.leftAvailable).ToArray();
                deltaToSpawn = new Vector3Int(-1,0,0);
                break;
            case CandidatDirection.CTop:
                filteredCandidats = candidats.Where(c => c.topAvailable).ToArray();
                deltaToSpawn = new Vector3Int(0,1,0);
                break;
            case CandidatDirection.CDown:
                filteredCandidats = candidats.Where(c => c.downAvailable).ToArray();
                deltaToSpawn = new Vector3Int(0,-1,0);
                break;
            case CandidatDirection.CBack:
                filteredCandidats = candidats.Where(c => c.backAvailable).ToArray();
                deltaToSpawn = new Vector3Int(0,0,1);
                break;
            case CandidatDirection.CForth:
                filteredCandidats = candidats.Where(c => c.forthAvailable).ToArray();
                deltaToSpawn = new Vector3Int(0,0,-1);
                break;
            default:
                break;
        }
        

        if (filteredCandidats.Count() > 0)
        {
            int randomIndex = Random.Range(0, filteredCandidats.Count());

            Candidat electedCandidat = filteredCandidats[randomIndex];

            AddBlock(electedCandidat.goCandidat.GetComponent<CubeScript>().positionOnGrid + deltaToSpawn);
        }
    }


    public void SubscribeToGrow(GameObject subscriber)
    {
        if (!growSubscribers.Contains(subscriber))
        {
            growSubscribers.Add(subscriber);
        }

        Candidat newCandidat = new Candidat(subscriber);

        if (!candidats.Contains(newCandidat))
        {
            candidats.Add(newCandidat);
        }

        Candidat candidatToUpdate = candidats.Find(c => c.goCandidat == subscriber);
        CubeScript subscriberScript = subscriber.GetComponent<CubeScript>();

        if (candidatToUpdate != null)
        {
            candidatToUpdate.rightAvailable = subscriberScript.neighbors[0].availability;
            candidatToUpdate.leftAvailable = subscriberScript.neighbors[1].availability;
            candidatToUpdate.topAvailable = subscriberScript.neighbors[2].availability;
            candidatToUpdate.downAvailable = subscriberScript.neighbors[3].availability;
            candidatToUpdate.backAvailable = subscriberScript.neighbors[4].availability;
            candidatToUpdate.forthAvailable = subscriberScript.neighbors[5].availability;
        }

    }

    public void UnsubscribeToGrow(GameObject subscriber)
    {
        growSubscribers.Remove(subscriber);

        Candidat candidatToRemove = candidats.Find(c => c.goCandidat == subscriber);

        if(candidatToRemove != null)
        {
            candidats.Remove(candidatToRemove);
        }
    }
}
