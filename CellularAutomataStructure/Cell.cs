using System.Collections.Generic;
using UnityEngine;


public class Cell
{
    CellGrid _grid;


    public int CellSize = 1;      // UNIT CELLS for now
    public Vector3Int Location;
    public Vector3 Center;

    public bool GoalCell;
    public bool Alive;


    public Agent agent;
    


    



    public Cell(Vector3Int location, CellGrid grid)
    {
        _grid = grid;
        Location = location;
        Center = new Vector3(_3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMin) + 
                 new Vector3(location.x + 0.5f, location.y + 0.5f, location.z + 0.5f) * CellSize;
        GoalCell = false;
        Alive = false;
    }







    /////////////////   CELL NEIGHBOURHOODS   /////////////////

    // GetAllNeighbours: Gets all 26 neighbours of a cubic cell (Moore Neighbourhood)
    public List<Cell> GetAllNeighbours()
    {
        List<Cell> listNeighbours = new List<Cell>();

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
                        listNeighbours.Add(_grid.Cells[i, j, k]);
                }
            }
        }
        return listNeighbours;
    }


    // GetAllNeighboursExtended: Gets all neighbours of a cubic cell for a specified reach (Extended Moore Neighbourhood)
    public List<Cell> GetAllNeighboursExtended(int reach)
    {
        List<Cell> listNeighbours = new List<Cell>();

        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int size = _grid.AreaSize;

        for (int i = x - reach; i <= x + reach; i++)
        {
            if (i < 0 || i > size - 1) continue;

            for (int j = y - 1; j <= y + 1; j++)
            {
                if (j < 0 || j > size - 1) continue;

                for (int k = z - 1; k <= z + 1; k++)
                {
                    if (k < 0 || k > size - 1) continue;

                    if (!(i == x && j == y && k == z))
                        listNeighbours.Add(_grid.Cells[i, j, k]);
                }
            }
        }
        return listNeighbours;
    }


    // GetFaceNeighbours: Gets all 6 face neighbours of a cubic cell (Von Neumann Neighbourhood)
    public List<Cell> GetFaceNeighbours()
    {
        List<Cell> listNeighbours = new List<Cell>();

        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int size = _grid.AreaSize;

        if (x != 0) listNeighbours.Add(_grid.Cells[x - 1, y, z]);
        if (x != size - 1) listNeighbours.Add(_grid.Cells[x + 1, y, z]);

        if (y != 0) listNeighbours.Add(_grid.Cells[x, y - 1, z]);
        if (y != size - 1) listNeighbours.Add(_grid.Cells[x, y + 1, z]);

        if (z != 0) listNeighbours.Add(_grid.Cells[x, y, z - 1]);
        if (z != size - 1) listNeighbours.Add(_grid.Cells[x, y, z + 1]);

        return listNeighbours;
    }


    // GetHorizontalFaceNeighbours: Gets only the (4: North, South, East, West) horizontal face neighbours of a cubic cell
    public List<Cell> GetHorizontalFaceNeighbours()
    {
        List<Cell> listNeighbours = new List<Cell>();

        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int size = _grid.AreaSize;

        if (x != 0) listNeighbours.Add(_grid.Cells[x - 1, y, z]);
        if (x != size - 1) listNeighbours.Add(_grid.Cells[x + 1, y, z]);

        if (z != 0) listNeighbours.Add(_grid.Cells[x, y, z - 1]);
        if (z != size - 1) listNeighbours.Add(_grid.Cells[x, y, z + 1]);

        return listNeighbours;
    }


    // GetVerticalFaceNeighbours: Gets only the (2: up and down) vertical face neighbours of a cubic cell
    public List<Cell> GetVerticalFaceNeighbours()
    {
        List<Cell> listNeighbours = new List<Cell>();

        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int size = _grid.AreaSize;

        if (y != 0) listNeighbours.Add(_grid.Cells[x, y - 1, z]);
        if (y != size - 1) listNeighbours.Add(_grid.Cells[x, y + 1, z]);

        return listNeighbours;
    }


    // OppositeNeighbours: Checks if two cells are opposite neighbours of a given cell
    public bool OppositeNeighbours(Cell otherNeighbour) 
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        if (otherNeighbour.Location == new Vector3Int(x + 2, y, z) || otherNeighbour.Location == new Vector3Int(x - 2, y, z) ||
            otherNeighbour.Location == new Vector3Int(x, y + 2, z) || otherNeighbour.Location == new Vector3Int(x, y - 2, z) ||
            otherNeighbour.Location == new Vector3Int(x, y, z + 2) || otherNeighbour.Location == new Vector3Int(x, y, z - 2))
            return true;
        else
            return false;
    }







    /////////////////   INDIVIDUAL CELL NEIGHBOURS   /////////////////

    // MIDDLE SECTION
    // ME (Middle East): Cell located east, middle row, of the current cell
    public Cell MiddleEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int east = x + 1;

        return (east >= _grid.AreaSize) ? null : _grid.Cells[east, y, z];
    }

    // MW (Middle West): Cell located west, middle row, of the current cell
    public Cell MiddleWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int west = x - 1;

        return (west < 0) ? null : _grid.Cells[west, y, z];
    }

    // MN (Middle North): Cell located north, middle row, of the current cell
    public Cell MiddleNorth()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int north = z + 1;

        return (north >= _grid.AreaSize) ? null : _grid.Cells[x, y, north];
    }

    // MS (Middle South): Cell located south, middle row, of the current cell
    public Cell MiddleSouth()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int south = z - 1;

        return (south < 0) ? null : _grid.Cells[x, y, south];
    }

    // MNE (Middle North East): Cell located north east, middle row, of the current cell
    public Cell MiddleNorthEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int north = z + 1;
        int east = x + 1;

        return (north >= _grid.AreaSize || east >= _grid.AreaSize) ? null : _grid.Cells[east, y, north];
    }

    // MSE (Middle South East): Cell located south east, middle row, of the current cell
    public Cell MiddleSouthEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int south = z - 1;
        int east = x + 1;

        return (south < 0 || east >= _grid.AreaSize) ? null : _grid.Cells[east, y, south];
    }

    // MNW (Middle North West): Cell located north west, middle row, of the current cell
    public Cell MiddleNorthWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int north = z + 1;
        int west = x - 1;

        return (north >= _grid.AreaSize || west < 0) ? null : _grid.Cells[west, y, north];
    }

    // MSW (Middle South West): Cell located south west, middle row, of the current cell
    public Cell MiddleSouthWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int south = z - 1;
        int west = x - 1;

        return (south < 0 || west < 0) ? null : _grid.Cells[west, y, south];
    }



    // UPPER SECTION
    // U (Up): Cell located in the center, upper row, of the current cell
    public Cell Up()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;

        return (up >= _grid.AreaSize) ? null : _grid.Cells[x, up, z];
    }

    // UE (Upper East): Cell located east, upper row, of the current cell
    public Cell UpperEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int east = x + 1;

        return (up >= _grid.AreaSize || east >= _grid.AreaSize) ? null : _grid.Cells[east, up, z];
    }

    // UW (Upper West): Cell located west, upper row, of the current cell
    public Cell UpperWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int west = x - 1;

        return (up >= _grid.AreaSize || west < 0) ? null : _grid.Cells[west, up, z];
    }

    // US (Upper South): Cell located south, upper row, of the current cell
    public Cell UpperSouth()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int south = z - 1;

        return (up >= _grid.AreaSize || south < 0) ? null : _grid.Cells[x, up, south];
    }

    // UN (Upper North): Cell located north, upper row, of the current cell
    public Cell UpperNorth()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int north = z + 1;

        return (up >= _grid.AreaSize || north >= _grid.AreaSize) ? null : _grid.Cells[x, up, north];
    }

    // UNE (Upper North East): Cell located north east, upper row, of the current cell
    public Cell UpperNorthEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int north = z + 1;
        int east = x + 1;

        return (up >= _grid.AreaSize || north >= _grid.AreaSize || east >= _grid.AreaSize) ? null : _grid.Cells[east, up, north];
    }

    // USE (Upper South East): Cell located south east, upper row, of the current cell
    public Cell UpperSouthEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int south = z - 1;
        int east = x + 1;

        return (up >= _grid.AreaSize || south < 0 || east >= _grid.AreaSize) ? null : _grid.Cells[east, up, south];
    }

    // UNW (Upper North West): Cell located north west, upper row, of the current cell
    public Cell UpperNorthWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int north = z + 1;
        int west = x - 1;

        return (up >= _grid.AreaSize || north >= _grid.AreaSize || west < 0) ? null : _grid.Cells[west, up, north];
    }

    // USW (Upper South West): Cell located south west, upper row, of the current cell
    public Cell UpperSouthWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int south = z - 1;
        int west = x - 1;

        return (up >= _grid.AreaSize || south < 0 || west < 0) ? null : _grid.Cells[west, up, south];
    }



    // BOTTOM SECTION
    // B (Bottom): Cell located in the center, bottom row, of the current cell
    public Cell Bottom()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;

        return (bottom < 0) ? null : _grid.Cells[x, bottom, z];
    }

    // BE (Bottom East): Cell located east, bottom row, of the current cell
    public Cell BottomEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int east = x + 1;

        return (bottom < 0 || east >= _grid.AreaSize) ? null : _grid.Cells[east, bottom, z];
    }

    // BW (Bottom West): Cell located west, bottom row, of the current cell
    public Cell BottomWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int west = x - 1;

        return (bottom < 0 || west < 0) ? null : _grid.Cells[west, bottom, z];
    }

    // BS (Bottom South): Cell located south, bottom row, of the current cell
    public Cell BottomSouth()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int south = z - 1;

        return (bottom < 0 || south < 0) ? null : _grid.Cells[x, bottom, south];
    }

    // BN (Bottom North): Cell located north, bottom row, of the current cell
    public Cell BottomNorth()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int north = z + 1;

        return (bottom < 0 || north >= _grid.AreaSize) ? null : _grid.Cells[x, bottom, north];
    }

    // BNE (Bottom North East): Cell located north east, bottom row, of the current cell
    public Cell BottomNorthEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int north = z + 1;
        int east = x + 1;

        return (bottom < 0 || north >= _grid.AreaSize || east >= _grid.AreaSize) ? null : _grid.Cells[east, bottom, north];
    }

    // BSE (Bottom South East): Cell located south east, bottom row, of the current cell
    public Cell BottomSouthEast()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int south = z - 1;
        int east = x + 1;

        return (bottom < 0 || south < 0 || east >= _grid.AreaSize) ? null : _grid.Cells[east, bottom, south];
    }

    // BNW (Bottom North West): Cell located north west, bottom row, of the current cell
    public Cell BottomNorthWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int north = z + 1;
        int west = x - 1;

        return (bottom < 0 || north >= _grid.AreaSize || west < 0) ? null : _grid.Cells[west, bottom, north];
    }

    // BSW (Bottom South West): Cell located south west, bottom row, of the current cell
    public Cell BottomSouthWest()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int south = z - 1;
        int west = x - 1;

        return (bottom < 0 || south < 0 || west < 0) ? null : _grid.Cells[west, bottom, south];
    }





    // OTHERS
    // U2 (Up2): Cell located in the center, second upper row, of the current cell
    public Cell Up2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up2 = y + 2;

        return (up2 >= _grid.AreaSize) ? null : _grid.Cells[x, up2, z];
    }

    // U2N (Up2North): Cell located north, second upper row, of the current cell
    public Cell Up2North()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up2 = y + 2;
        int north = z + 1;

        return (up2 >= _grid.AreaSize || north >= _grid.AreaSize) ? null : _grid.Cells[x, up2, north];
    }

    // U2S (Up2South): Cell located south, second upper row, of the current cell
    public Cell Up2South()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up2 = y + 2;
        int south = z - 1;

        return (up2 >= _grid.AreaSize || south < 0) ? null : _grid.Cells[x, up2, south];
    }

    // U2E (Up2East): Cell located east, second upper row, of the current cell
    public Cell Up2East()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up2 = y + 2;
        int east = x + 1;

        return (up2 >= _grid.AreaSize || east >= _grid.AreaSize) ? null : _grid.Cells[east, up2, z];
    }

    // U2W (Up2West): Cell located west, second upper row, of the current cell
    public Cell Up2West()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up2 = y + 2;
        int west = x - 1;

        return (up2 >= _grid.AreaSize || west < 0) ? null : _grid.Cells[west, up2, z];
    }



    // B2 (Bottom2): Cell located in the center, second bottom row, of the current cell
    public Cell Bottom2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom2 = y - 2;

        return (bottom2 < 0) ? null : _grid.Cells[x, bottom2, z];
    }

    // B2N (Bottom2North): Cell located north, second bottom row, of the current cell
    public Cell Bottom2North()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom2 = y - 2;
        int north = z + 1;

        return (bottom2 < 0 || north >= _grid.AreaSize) ? null : _grid.Cells[x, bottom2, north];
    }

    // B2S (Bottom2South): Cell located south, second bottom row, of the current cell
    public Cell Bottom2South()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom2 = y - 2;
        int south = z - 1;

        return (bottom2 < 0 || south < 0) ? null : _grid.Cells[x, bottom2, south];
    }

    // B2E (Bottom2East): Cell located east, second bottom row, of the current cell
    public Cell Bottom2East()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom2 = y - 2;
        int east = x + 1;

        return (bottom2 < 0 || east >= _grid.AreaSize) ? null : _grid.Cells[east, bottom2, z];
    }

    // B2W (Bottom2West): Cell located west, second bottom row, of the current cell
    public Cell Bottom2West()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom2 = y - 2;
        int west = x - 1;

        return (bottom2 < 0 || west < 0) ? null : _grid.Cells[west, bottom2, z];
    }



    // N2 (North2): Cell located two cells north, middle row, of the current cell
    public Cell North2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int north2 = z + 2;

        return (north2 >= _grid.AreaSize) ? null : _grid.Cells[x, y, north2];
    }

    // UN2 (North2): Cell located two cells north, middle row, of the current cell
    public Cell UpperNorth2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int north2 = z + 2;

        return (north2 >= _grid.AreaSize || up >= _grid.AreaSize) ? null : _grid.Cells[x, up, north2];
    }

    // BN2 (North2): Cell located two cells north, bottom row, of the current cell
    public Cell BottomNorth2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int north2 = z + 2;

        return (north2 >= _grid.AreaSize || bottom < 0) ? null : _grid.Cells[x, bottom, north2];
    }

    // N2E (North2East): Cell located two cells north and east, middle row, of the current cell
    public Cell North2East()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int east = x + 1;
        int north2 = z + 2;

        return (north2 >= _grid.AreaSize || east >= _grid.AreaSize) ? null : _grid.Cells[east, y, north2];
    }

    // N2W (North2West): Cell located two cells north and west, middle row, of the current cell
    public Cell North2West()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int west = x - 1;
        int north2 = z + 2;

        return (north2 >= _grid.AreaSize || west < 0) ? null : _grid.Cells[west, y, north2];
    }



    // S2 (South2): Cell located two cells south, middle row, of the current cell
    public Cell South2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int south2 = z - 2;

        return (south2 < 0) ? null : _grid.Cells[x, y, south2];
    }

    // US2 (UpperSouth2): Cell located two cells south, upper row, of the current cell
    public Cell UpperSouth2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int south2 = z - 2;

        return (south2 < 0 || up >= _grid.AreaSize) ? null : _grid.Cells[x, up, south2];
    }

    // BS2 (BottomSouth2): Cell located two cells south, bottom row, of the current cell
    public Cell BottomSouth2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int south2 = z - 2;

        return (south2 < 0 || bottom < 0) ? null : _grid.Cells[x, bottom, south2];
    }

    // U2E (South2East): Cell located two cells south and east, middle row, of the current cell
    public Cell South2East()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int east = x + 1;
        int south2 = z - 2;

        return (south2 < 0 || east >= _grid.AreaSize) ? null : _grid.Cells[east, y, south2];
    }

    // U2W (South2West): Cell located two cells south and west, middle row, of the current cell
    public Cell South2West()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int west = x - 1;
        int south2 = z - 2;

        return (south2 < 0 || west < 0) ? null : _grid.Cells[west, y, south2];
    }



    // E2 (East2): Cell located two cells east, middle row, of the current cell
    public Cell East2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int east2 = x + 2;

        return (east2 >= _grid.AreaSize) ? null : _grid.Cells[east2, y, z];
    }

    // UE2 (UpperEast2): Cell located two cells east, upper row, of the current cell
    public Cell UpperEast2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int east2 = x + 2;

        return (east2 >= _grid.AreaSize || up >= _grid.AreaSize) ? null : _grid.Cells[east2, up, z];
    }

    // BE2 (BottomEast2): Cell located two cells east, bottom row, of the current cell
    public Cell BottomEast2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int east2 = x + 2;

        return (east2 >= _grid.AreaSize || bottom < 0) ? null : _grid.Cells[east2, bottom, z];
    }

    // NE2 (NorthEast2): Cell located two cells east and north, middle row, of the current cell
    public Cell NorthEast2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int north = z + 1;
        int east2 = x + 2;

        return (east2 >= _grid.AreaSize || north >= _grid.AreaSize) ? null : _grid.Cells[east2, y, north];
    }

    // SE2 (SouthEast2): Cell located two cells east and south, middle row, of the current cell
    public Cell SouthEast2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int south = z - 1;
        int east2 = x + 2;

        return (east2 >= _grid.AreaSize || south < 0) ? null : _grid.Cells[east2, y, south];
    }



    // W2 (West2): Cell located two cells west, middle row, of the current cell
    public Cell West2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int west2 = x - 2;

        return (west2 < 0) ? null : _grid.Cells[west2, y, z];
    }

    // UW2 (UpperWest2): Cell located two cells west, upper row, of the current cell
    public Cell UpperWest2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int up = y + 1;
        int west2 = x - 2;

        return (west2 < 0 || up >= _grid.AreaSize) ? null : _grid.Cells[west2, up, z];
    }

    // BW2 (BottomWest2): Cell located two cells west, bottom row, of the current cell
    public Cell BottomWest2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int bottom = y - 1;
        int west2 = x - 2;

        return (west2 < 0 || bottom < 0) ? null : _grid.Cells[west2, bottom, z];
    }

    // NW2 (NorthWest2): Cell located two cells west and north, middle row, of the current cell
    public Cell NorthWest2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int north = z + 1;
        int west2 = x - 2;

        return (west2 < 0 || north >= _grid.AreaSize) ? null : _grid.Cells[west2, y, north];
    }

    // SW2 (SouthWest2): Cell located two cells west and south, middle row, of the current cell
    public Cell SouthWest2()
    {
        int x = Location.x;
        int y = Location.y;
        int z = Location.z;

        int south = z - 1;
        int west2 = x - 2;

        return (west2 < 0 || south < 0) ? null : _grid.Cells[west2, y, south];
    }
}

