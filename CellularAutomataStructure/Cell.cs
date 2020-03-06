using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// CELL STATE: Specifies the state of the cell
public enum CellState
{ 
    Sleep,       // 0
    Active,      // 1
    Final        // 2
}


public class Cell
{
    // ASSUMES A CELL SIZE OF 1 (size of a unit game object)
    public int CellSize = 1;

    public Vector3Int Location;
    public Vector3 Center;
    public bool Alive;

    //public int AliveNeighbours;

    public CellState State;  //Instantiate
    public bool Active;      //Instantiate
    public int ScentValue;

    CellGrid _grid;



    public Cell(Vector3Int location, CellGrid grid)
    {
        _grid = grid;
        Location = location;
        Center = new Vector3(_3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMin) + 
                 new Vector3(location.x + 0.5f, location.y + 0.5f, location.z + 0.5f) * CellSize;
        Alive = false;

        State = CellState.Sleep;
        Active = false;
    }



    public IEnumerable<Cell> GetFaceNeighbours()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int size = _grid.AreaSize;

        if (x != 0) yield return _grid.Cells[x - 1, y, z];
        if (x != size - 1) yield return _grid.Cells[x + 1, y, z];

        if (y != 0) yield return _grid.Cells[x, y - 1, z];
        if (y != size - 1) yield return _grid.Cells[x, y + 1, z];

        if (z != 0) yield return _grid.Cells[x, y, z - 1];
        if (z != size - 1) yield return _grid.Cells[x, y, z + 1];
    }


    public IEnumerable<Cell> GetAllNeighbours()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int size = _grid.AreaSize;

        for (int i = x - 1; i <= x + 1; i++)
        {
            if (i == -1 || i == size) continue;

            for (int j = y - 1; j <= y + 1; j++)
            {
                if (j == -1 || j == size) continue;

                for (int k = z - 1; k <= z + 1; k++)
                {
                    if (k == -1 || k == size) continue;

                    if (!(i == x && j == y && k == z))
                        yield return _grid.Cells[i, j, k];
                }
            }
        }
    }


    public bool IsConnector()
    {
        
        return true;
    }
}

