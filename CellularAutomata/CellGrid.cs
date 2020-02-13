using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
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


    // GetGridLocation: Gets the grid location of a given Vector3 position 
    public Vector3Int GetGridLocation(Vector3 position)
    {
        int coordX;
        int coordY;
        int coordZ;


        if (position.x >= AreaMax)
            coordX = AreaSize - 1;
        else if (position.x <= AreaMin)
            coordX = 0;
        else
            coordX = (int) (position.x - AreaMin);


        if (position.y >= AreaMax)
            coordY = AreaSize - 1;
        else if (position.y <= AreaMin)
            coordY = 0;
        else
            coordY = (int) (position.y - AreaMin);


        if (position.z >= AreaMax)
            coordZ = AreaSize - 1;
        else if (position.z <= AreaMin)
            coordZ = 0;
        else
            coordZ = (int) (position.z - AreaMin);


        return new Vector3Int(coordX, coordY, coordZ);
    }

    public Cell GetCell(Vector3Int location) => Cells[location.x, location.y, location.z];


    bool OutsideBoundaries(Vector3Int location)      /// Do I need this?
    {
        if (location.x < 0) return false;
        if (location.y < 0) return false;
        if (location.z < 0) return false;
        if (location.x >= AreaSize) return false;
        if (location.y >= AreaSize) return false;
        if (location.z >= AreaSize) return false;
        return true;
    }



    /////////////////   NEIGHBORHOOD LOCATIONS   /////////////////

    // MIDDLE SECTION
    // ME (Middle East): Cell located east, middle row, of the current cell
    public Cell ME(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int east = loc.x + 1;

        return (east >= AreaSize) ? null : cells[east, loc.y, loc.z];
    }

    // MW (Middle West): Cell located west, middle row, of the current cell
    public Cell MW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int west = loc.x - 1;

        return (west < 0) ? null : cells[west, loc.y, loc.z];
    }

    // MN (Middle North): Cell located north, middle row, of the current cell
    public Cell MN(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int north = loc.z + 1;

        return (north >= AreaSize) ? null : cells[loc.x, loc.y, north];
    }

    // MS (Middle South): Cell located south, middle row, of the current cell
    public Cell MS(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int south = loc.z - 1;

        return (south < 0) ? null : cells[loc.x, loc.y, south];
    }

    // MNE (Middle North East): Cell located north east, middle row, of the current cell
    public Cell MNE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int north = loc.z + 1;
        int east = loc.x + 1;

        return (north >= AreaSize || east >= AreaSize) ? null : cells[east, loc.y, north];
    }

    // MSE (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MSE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int south = loc.z - 1;
        int east = loc.x + 1;

        return (south < 0 || east >= AreaSize) ? null : cells[east, loc.y, south];
    }

    // MNW (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MNW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int north = loc.z + 1;
        int west = loc.x - 1;

        return (north >= AreaSize || west < 0) ? null : cells[west, loc.y, north];
    }

    // MSW (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MSW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int south = loc.z - 1;
        int west = loc.x - 1;

        return (south < 0 || west < 0) ? null : cells[west, loc.y, south];
    }



    // UPPER SECTION
    // U (Up): Cell located in the center, upper row, of the current cell
    public Cell U(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;

        return (up >= AreaSize) ? null : cells[loc.x, up, loc.z];
    }

    // UE (Upper East): Cell located east, upper row, of the current cell
    public Cell UE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int east = loc.x + 1;

        return (up >= AreaSize || east >= AreaSize) ? null : cells[east, up, loc.z];
    }

    // UW (Upper West): Cell located west, upper row, of the current cell
    public Cell UW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int west = loc.x - 1;

        return (up >= AreaSize || west < 0) ? null : cells[west, up, loc.z];
    }

    // US (Upper South): Cell located south, upper row, of the current cell
    public Cell US(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int south = loc.z - 1;

        return (up >= AreaSize || south < 0) ? null : cells[loc.x, up, south];
    }

    // UN (Upper North): Cell located north, upper row, of the current cell
    public Cell UN(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int north = loc.z + 1;

        return (up >= AreaSize || north >= AreaSize) ? null : cells[loc.x, up, north];
    }

    // UNE (Upper North East): Cell located north east, upper row, of the current cell
    public Cell UNE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int north = loc.z + 1;
        int east = loc.x + 1;

        return (up >= AreaSize || north >= AreaSize || east >= AreaSize) ? null : cells[east, up, north];
    }

    // USE (Upper South East): Cell located south east, upper row, of the current cell
    public Cell USE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int south = loc.z - 1;
        int east = loc.x + 1;

        return (up >= AreaSize || south < 0 || east >= AreaSize) ? null : cells[east, up, south];
    }

    // UNW (Upper North West): Cell located north west, upper row, of the current cell
    public Cell UNW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int north = loc.z + 1;
        int west = loc.x - 1;

        return (up >= AreaSize || north >= AreaSize || west < 0) ? null : cells[west, up, north];
    }

    // USW (Upper South West): Cell located south west, upper row, of the current cell
    public Cell USW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int up = loc.y + 1;
        int south = loc.z - 1;
        int west = loc.x - 1;

        return (up >= AreaSize || south < 0 || west < 0) ? null : cells[west, up, south];
    }



    // BOTTOM SECTION
    // B (Bottom): Cell located in the center, bottom row, of the current cell
    public Cell B(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;

        return (bottom < 0) ? null : cells[loc.x, bottom, loc.z];
    }

    // BE (Bottom East): Cell located east, bottom row, of the current cell
    public Cell BE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int east = loc.x + 1;

        return (bottom < 0 || east >= AreaSize) ? null : cells[east, bottom, loc.z];
    }

    // BW (Bottom West): Cell located west, bottom row, of the current cell
    public Cell BW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int west = loc.x - 1;

        return (bottom < 0 || west < 0) ? null : cells[west, bottom, loc.z];
    }

    // BS (Bottom South): Cell located south, bottom row, of the current cell
    public Cell BS(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int south = loc.z - 1;

        return (bottom < 0 || south < 0) ? null : cells[loc.x, bottom, south];
    }

    // BN (Bottom North): Cell located north, bottom row, of the current cell
    public Cell BN(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int north = loc.z + 1;

        return (bottom < 0 || north >= AreaSize) ? null : cells[loc.x, bottom, north];
    }

    // BNE (Bottom North East): Cell located north east, bottom row, of the current cell
    public Cell BNE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int north = loc.z + 1;
        int east = loc.x + 1;

        return (bottom < 0 || north >= AreaSize || east >= AreaSize) ? null : cells[east, bottom, north];
    }

    // BSE (Bottom South East): Cell located south east, bottom row, of the current cell
    public Cell BSE(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int south = loc.z - 1;
        int east = loc.x + 1;

        return (bottom < 0 || south < 0 || east >= AreaSize) ? null : cells[east, bottom, south];
    }

    // BNW (Bottom North West): Cell located north west, bottom row, of the current cell
    public Cell BNW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int north = loc.z + 1;
        int west = loc.x - 1;

        return (bottom < 0 || north >= AreaSize || west < 0) ? null : cells[west, bottom, north];
    }

    // BSW (Bottom South West): Cell located south west, bottom row, of the current cell
    public Cell BSW(Cell[,,] cells, Cell currentCell)
    {
        Vector3Int loc = currentCell.Location;
        int bottom = loc.y - 1;
        int south = loc.z - 1;
        int west = loc.x - 1;

        return (bottom < 0 || south < 0 || west < 0) ? null : cells[west, bottom, south];
    }
}