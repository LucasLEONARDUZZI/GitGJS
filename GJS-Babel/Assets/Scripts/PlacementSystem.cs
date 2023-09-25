using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PlacementSystem : MonoBehaviour
{
   
   public GameObject selector, voxIndicator;
    
    public Grid grid;

    private void Update()
    {
        Vector3 selectorPosition = selector.transform.position;
        Vector3Int gridPosition = grid.WorldToCell(selectorPosition);
        voxIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
