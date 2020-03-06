using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Agents
{
    CommonMethods CM = new CommonMethods();
    CellGrid _grid = new CellGrid(_3DSelfAssembly.AreaMin, _3DSelfAssembly.AreaMax);


    GameObject agent;
    Material material;
    int NumberOfAgents;



    public Agent[] listAgents;





    // Constructor
    public Agents(GameObject prefab, Material _material, int _NumberOfAgents)
    {
        agent = prefab;
        material = _material;
        NumberOfAgents = _NumberOfAgents;
        listAgents = new Agent[NumberOfAgents];
    }





    ////////////////////////////   CELL PLACEMENT METHODS  ////////////////////////////

    // FillCellsWithAgents: Fills the entire grid of cells with agents
    public Agent[] FillCellsWithAgents(Cell[,,] cells)
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int z = 0; z < cells.GetLength(2); z++)
                {
                    Cell currentCell = cells[x, y, z];
                    currentCell.Alive = true;
                    
                    Agent newAgent = new Agent(agent, material, currentCell.Center);
                    newAgent.Location = currentCell;
                    int i = cells.GetLength(1) * x + cells.GetLength(2) * y + z;
                    listAgents[i] = newAgent;
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsInRowsCells: Places the agents in a 2D regular grid
    public Agent[] PlaceAgentsIn2DGrid(Vector3Int placement, Cell[,,] cells)     // REVIEW 2D PART
    {
        int rows = (int) Mathf.Ceil(Mathf.Sqrt(NumberOfAgents));
        int columns = (int) Mathf.Ceil(Mathf.Sqrt(NumberOfAgents));

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = columns * i + j;

                if (index < NumberOfAgents)
                {
                    Cell cell = cells[i + placement.x, placement.y, j + placement.z];   // CHECK EXCEPTION OF IF OUTSIDE BOUNDARIES (UP Y?)
                    cell.Alive = true;

                    Agent newAgent = new Agent(agent, material, cell.Center);
                    newAgent.Location = cell;
                    listAgents[index] = newAgent;
                }
            }
        }
        return listAgents;
    }


    // PlaceAgentsInGivenGeometry: Places the agents in a given list of points that forms a 3D shape (only for points inside the lattice)
    public Agent[] PlaceAgentsInGivenGeometry(IEnumerable<Vector3> listPositions)
    {
        for (int i = 0; i < listPositions.Count(); i++)
        {
            Vector3Int location = _grid.GetCellLocation(listPositions.ElementAt(i));

            Cell currentCell = _grid.GetCell(location);
            currentCell.Alive = true;

            Agent newAgent = new Agent(agent, material, currentCell.Center);
            newAgent.Location = currentCell;
            listAgents[i] = newAgent;
        }
        return listAgents;
    }


    // PlaceAgentsRandomly: Randomly places the agents in the cell grid with no overlap/intersections
    public Agent[] PlaceAgentsRandomly(int AreaMin, int AreaMax)
    {
        int tries = 10000;    ///// tries -> loop failsafe
        int index = 0;

        while (index < NumberOfAgents && tries-- > 0)
        {
            Vector3Int cellLocation = CM.RandomPosition(AreaMin, AreaMax);
            Cell currentCell = _grid.GetCell(cellLocation);

            if (currentCell.Alive == false)
            {
                currentCell.Alive = true;

                Agent newAgent = new Agent(agent, material, currentCell.Center);
                newAgent.Location = currentCell;
                listAgents[index] = newAgent;
                index++;
            }
        }
        return listAgents;
    }


    // PlaceConnectedAgentsRandomly: Randomly places the agents in the cell grid with no overlap/intersections and NO DISCONNECTIONS in the structure
    public Agent[] PlaceConnectedAgentsRandomly(Vector3Int placement)
    {
        int tries = 10000;    ///// tries -> loop failsafe
        int index = 0;

        Cell firstCell = _grid.GetCell(placement);
        firstCell.Alive = true;

        Agent firstAgent = new Agent(agent, material, firstCell.Center);
        firstAgent.Location = firstCell;
        listAgents[index] = firstAgent;

        while (index < NumberOfAgents && tries-- > 0)
        {
            Agent randomAgent = listAgents[Random.Range(0, index)];
            Cell agentCell = randomAgent.Location;

            var faceNeighbours = agentCell.GetFaceNeighbours().Where(cell => cell.Alive == false).ToList();

            if (faceNeighbours.Count != 0)
            {
                Cell randomNeighbour = faceNeighbours[Random.Range(0, faceNeighbours.Count)];
                randomNeighbour.Alive = true;
                
                Agent newAgent = new Agent(agent, material, randomNeighbour.Center);
                newAgent.Location = randomNeighbour;
                listAgents[index] = newAgent;
                index++;
            }
        }
        return listAgents;
    }
}