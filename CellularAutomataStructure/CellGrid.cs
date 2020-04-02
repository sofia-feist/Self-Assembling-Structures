using UnityEngine;


public class CellGrid
{
    CommonMethods CM = new CommonMethods();

    int AreaMin;
    int AreaMax;

    public Cell[,,] Cells;
    public int AreaSize;



    public CellGrid(int areaMin, int areaMax)
    {
        AreaMin = areaMin;
        AreaMax = areaMax;
        AreaSize = AreaMax - AreaMin;

        Cells = new Cell[AreaSize, AreaSize, AreaSize];

        for (int x = 0; x < AreaSize; x++)
        {
            for (int y = 0; y < AreaSize; y++)
            {
                for (int z = 0; z < AreaSize; z++)
                {
                    Cells[x, y, z] = new Cell(new Vector3Int(x, y, z), this);
                }
            }
        }
    }


    // GetGridLocation: Gets the grid location (Vector3Int) of a given Vector3 position
    public Vector3Int GetCellLocation(Vector3 position)
    {  
        if (CM.OutsideBoundaries(position, AreaMin, AreaMax))
        { 
            throw new System.InvalidOperationException("CellGrid.GetCellLocation: Vector position outside Grid boundaries.");
        }
        else
        {
            int x = (int)Mathf.Floor((position.x - AreaMin));
            int y = (int)Mathf.Floor((position.y - AreaMin));
            int z = (int)Mathf.Floor((position.z - AreaMin));

            return new Vector3Int(x, y, z);
        }
    }


    // GetCell: Gets the cell corresponding to a given Vector3Int location
    public Cell GetCell(Vector3Int location) => Cells[location.x, location.y, location.z];

}