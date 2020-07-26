using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Math;
using UnityEngine;

internal class OptimizationAlgorithms
{
    CellGrid grid;
    CommonMethods CM = new CommonMethods();
    readonly static System.Random random = new System.Random(42);

    static double _currentFitness;

    private static int maxIterations;
    private static int timeLimit;  // milliseconds

    internal List<Cell> bestCandidate = new List<Cell>();



    public OptimizationAlgorithms(CellGrid _grid, int seconds, int iterations)
    {
        grid = _grid;
        timeLimit = seconds * 1000;
        maxIterations = iterations;
        _currentFitness = float.NegativeInfinity;
    }



    internal void SimulatedAnnealing(List<Vector3Int> startingPoints, int nModules)
    {
        int iteration = 0;

        const double s = 50;    //start temperature
        const double f = 0.001; //final temperature

        var l = (double)timeLimit;

        var watch = Stopwatch.StartNew();
        long t;

        while ((t = watch.ElapsedMilliseconds) < timeLimit | iteration < maxIterations)
        {
            var candidate = GenerateGoalShape(startingPoints, nModules);

            var fitness = evaluateSimilarity(candidate);
            bool shouldAccept = fitness > _currentFitness;  // Maximize

            if (!shouldAccept)
            {
                var d = _currentFitness - fitness;
                var p = Exp(d * Pow(f / s, -t / l) / s);

                shouldAccept = p > random.NextDouble();
            }

            if (shouldAccept)
            {
                _currentFitness = fitness;
                bestCandidate = candidate;
                //Update();
            }

            iteration++;
        }

        watch.Stop();
        UnityEngine.Debug.Log("Best Fitness: " + _currentFitness + ", Time Elapsed (s): " + watch.Elapsed.TotalSeconds + ", Iterations: " + iteration);
        //Completed();
    }

    internal List<Cell> GenerateGoalShape(List<Vector3Int> startingPoints, int nModules)
    {
        int tries = 100;    ///// tries -> loop failsafe
        int count = 0;

        Cell[] cells = new Cell[startingPoints.Count];
        List<Cell> goalCells = new List<Cell>();

        // Place starting Cells
        foreach (Vector3Int point in startingPoints)
        {
            Cell currentCell = grid.GetCell(point);
            //currentCell.GoalCell = true;
            cells[count] = currentCell;
            goalCells.Add(currentCell);
            count++;
        }

        //Place remaining cells
        while (count < nModules && tries-- > 0)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                var faceNeighbours = cells[i].GetFaceNeighbours().Where(n => !goalCells.Any(cell => cell.Location == n.Location)).ToArray();
                int nNeighbours = faceNeighbours.Length;

                if (nNeighbours != 0)
                {
                    Cell randomNeighbour = faceNeighbours[UnityEngine.Random.Range(0, nNeighbours)];
                    //randomNeighbour.GoalCell = true;
                    cells[i] = randomNeighbour;
                    goalCells.Add(randomNeighbour);
                    count++;
                }
            }
        }

        return goalCells;
    }

    internal float evaluateSimilarity(List<Cell> cells)
    {
        int nMax = cells.Count;
        int nInside = 0;

        for (int i = 0; i < nMax; i++)
        {
            Vector3 point = cells[i].Center;

            if (CM.InsideCollider(point, Vector3.up))
                nInside++;
        }

        return (float)nInside / nMax;
    }
}
