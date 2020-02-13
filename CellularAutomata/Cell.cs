using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell
{
    // ASSUMES A CELL SIZE OF 1 (size of a unit game object)
    public int CellSize = 1;

    public Vector3Int Location;
    public Vector3 Center;
    public bool Alive;

    CellGrid _grid;



    public Cell(Vector3Int location, CellGrid grid)
    {
        _grid = grid;
        Location = location;
        Center = new Vector3(location.x + CellSize * 0.5f, location.y + CellSize * 0.5f, location.z + CellSize * 0.5f);
        Alive = false;
    }


    //public IEnumerable<Cell> GetFaceNeighbours()
    //{
    //    int x = Location.x;
    //    int y = Location.y;
    //    int z = Location.z;
    //    int s = _grid.AreaSize;

    //    if (x != 0) yield return _grid.Cells[x - 1, y, z];
    //    if (x != s - 1) yield return _grid.Cells[x + 1, y, z];

    //    if (y != 0) yield return _grid.Cells[x, y - 1, z];
    //    if (y != s - 1) yield return _grid.Cells[x, y + 1, z];

    //    if (z != 0) yield return _grid.Cells[x, y, z - 1];
    //    if (z != s - 1) yield return _grid.Cells[x, y, z + 1];
    //}
}

