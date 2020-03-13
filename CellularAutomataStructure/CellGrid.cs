using System.Collections;
using System.Collections.Generic;
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
        AreaSize = areaMax - areaMin;
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


    // GetGridLocation: Gets the grid location (Vector3Int) of a given Vector3 position (assumes position exists inside the grid)
    public Vector3Int GetCellLocation(Vector3 position)
    {
        int x = (int) position.x - AreaMin;
        int y = (int) position.y - AreaMin;
        int z = (int) position.z - AreaMin;

        return new Vector3Int(x, y, z);
    }

    // GetCell: Gets the cell corresponding to a given Vector3Int location
    public Cell GetCell(Vector3Int location) => Cells[location.x, location.y, location.z];






    /////////////////   NEIGHBORHOOD CELLS   /////////////////

    // MIDDLE SECTION
    // ME (Middle East): Cell located east, middle row, of the current cell
    public Cell ME(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int east = loc.x + 1;

        return (east >= AreaSize) ? null : Cells[east, loc.y, loc.z];
    }

    // MW (Middle West): Cell located west, middle row, of the current cell
    public Cell MW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int west = loc.x - 1;

        return (west < 0) ? null : Cells[west, loc.y, loc.z];
    }

    // MN (Middle North): Cell located north, middle row, of the current cell
    public Cell MN(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int north = loc.z + 1;

        return (north >= AreaSize) ? null : Cells[loc.x, loc.y, north];
    }

    // MS (Middle South): Cell located south, middle row, of the current cell
    public Cell MS(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int south = loc.z - 1;

        return (south < 0) ? null : Cells[loc.x, loc.y, south];
    }

    // MNE (Middle North East): Cell located north east, middle row, of the current cell
    public Cell MNE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int north = loc.z + 1;
        int east = loc.x + 1;

        return (north >= AreaSize || east >= AreaSize) ? null : Cells[east, loc.y, north];
    }

    // MSE (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MSE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int south = loc.z - 1;
        int east = loc.x + 1;

        return (south < 0 || east >= AreaSize) ? null : Cells[east, loc.y, south];
    }

    // MNW (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MNW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int north = loc.z + 1;
        int west = loc.x - 1;

        return (north >= AreaSize || west < 0) ? null : Cells[west, loc.y, north];
    }

    // MSW (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MSW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int south = loc.z - 1;
        int west = loc.x - 1;

        return (south < 0 || west < 0) ? null : Cells[west, loc.y, south];
    }



    // UPPER SECTION
    // U (Up): Cell located in the center, upper row, of the current cell
    public Cell U(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;

        return (up >= AreaSize) ? null : Cells[loc.x, up, loc.z];
    }

    // UE (Upper East): Cell located east, upper row, of the current cell
    public Cell UE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int east = loc.x + 1;

        return (up >= AreaSize || east >= AreaSize) ? null : Cells[east, up, loc.z];
    }

    // UW (Upper West): Cell located west, upper row, of the current cell
    public Cell UW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int west = loc.x - 1;

        return (up >= AreaSize || west < 0) ? null : Cells[west, up, loc.z];
    }

    // US (Upper South): Cell located south, upper row, of the current cell
    public Cell US(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int south = loc.z - 1;

        return (up >= AreaSize || south < 0) ? null : Cells[loc.x, up, south];
    }

    // UN (Upper North): Cell located north, upper row, of the current cell
    public Cell UN(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int north = loc.z + 1;

        return (up >= AreaSize || north >= AreaSize) ? null : Cells[loc.x, up, north];
    }

    // UNE (Upper North East): Cell located north east, upper row, of the current cell
    public Cell UNE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int north = loc.z + 1;
        int east = loc.x + 1;

        return (up >= AreaSize || north >= AreaSize || east >= AreaSize) ? null : Cells[east, up, north];
    }

    // USE (Upper South East): Cell located south east, upper row, of the current cell
    public Cell USE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int south = loc.z - 1;
        int east = loc.x + 1;

        return (up >= AreaSize || south < 0 || east >= AreaSize) ? null : Cells[east, up, south];
    }

    // UNW (Upper North West): Cell located north west, upper row, of the current cell
    public Cell UNW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int north = loc.z + 1;
        int west = loc.x - 1;

        return (up >= AreaSize || north >= AreaSize || west < 0) ? null : Cells[west, up, north];
    }

    // USW (Upper South West): Cell located south west, upper row, of the current cell
    public Cell USW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int south = loc.z - 1;
        int west = loc.x - 1;

        return (up >= AreaSize || south < 0 || west < 0) ? null : Cells[west, up, south];
    }



    // BOTTOM SECTION
    // B (Bottom): Cell located in the center, bottom row, of the current cell
    public Cell B(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;

        return (bottom < 0) ? null : Cells[loc.x, bottom, loc.z];
    }

    // BE (Bottom East): Cell located east, bottom row, of the current cell
    public Cell BE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int east = loc.x + 1;

        return (bottom < 0 || east >= AreaSize) ? null : Cells[east, bottom, loc.z];
    }

    // BW (Bottom West): Cell located west, bottom row, of the current cell
    public Cell BW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int west = loc.x - 1;

        return (bottom < 0 || west < 0) ? null : Cells[west, bottom, loc.z];
    }

    // BS (Bottom South): Cell located south, bottom row, of the current cell
    public Cell BS(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int south = loc.z - 1;

        return (bottom < 0 || south < 0) ? null : Cells[loc.x, bottom, south];
    }

    // BN (Bottom North): Cell located north, bottom row, of the current cell
    public Cell BN(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int north = loc.z + 1;

        return (bottom < 0 || north >= AreaSize) ? null : Cells[loc.x, bottom, north];
    }

    // BNE (Bottom North East): Cell located north east, bottom row, of the current cell
    public Cell BNE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int north = loc.z + 1;
        int east = loc.x + 1;

        return (bottom < 0 || north >= AreaSize || east >= AreaSize) ? null : Cells[east, bottom, north];
    }

    // BSE (Bottom South East): Cell located south east, bottom row, of the current cell
    public Cell BSE(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int south = loc.z - 1;
        int east = loc.x + 1;

        return (bottom < 0 || south < 0 || east >= AreaSize) ? null : Cells[east, bottom, south];
    }

    // BNW (Bottom North West): Cell located north west, bottom row, of the current cell
    public Cell BNW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int north = loc.z + 1;
        int west = loc.x - 1;

        return (bottom < 0 || north >= AreaSize || west < 0) ? null : Cells[west, bottom, north];
    }

    // BSW (Bottom South West): Cell located south west, bottom row, of the current cell
    public Cell BSW(Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int south = loc.z - 1;
        int west = loc.x - 1;

        return (bottom < 0 || south < 0 || west < 0) ? null : Cells[west, bottom, south];
    }
}