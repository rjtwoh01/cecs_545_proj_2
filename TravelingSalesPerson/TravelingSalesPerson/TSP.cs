using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Collections;

namespace TravelingSalesPerson
{
    class TSP
    {
        public Point canvasOffset; //Not sure if we need this yet
        public Point maxPoint;
        public Point minPoint;
        public List<Point> points;
        public List<Point> tempFinalList;
        public List<TSPConnection> connectedPoints;

        public double shortestDistance;

        public TSP(List<Point> points)
        {
            //Values for UI purposes, creating offsets for the grid
            this.points = new List<Point>();
            this.minPoint = points.First();
            this.maxPoint = points.First();
            this.tempFinalList = new List<Point>();

            foreach(Point point in points)
            {
                this.points.Add(point);
            }

            for (int i = 0; i < points.Count; i++)
            {
                Point point = new Point(points[i].X, points[i].Y);

                if (point.X < this.minPoint.X) { this.minPoint.X = point.X; }
                else if (point.X > this.maxPoint.X) { this.maxPoint.X = point.X; }
                if (point.Y < this.minPoint.Y) { this.minPoint.Y = point.Y; }                
                else if (point.Y > this.maxPoint.Y) { this.maxPoint.Y = point.Y; }
            }

            this.canvasOffset = new Point(10, 10);

            if (this.minPoint.X > 0) { this.canvasOffset.X -= this.minPoint.X; }
            else { this.canvasOffset.X += this.minPoint.X; }
            if (this.minPoint.Y > 0) { this.canvasOffset.X -= this.minPoint.X; }
            else { this.canvasOffset.X += this.minPoint.X; }

            this.shortestDistance = 0;
        }

        //Setup for DFS and BFS
        public TSP(List<TSPConnection> tSPConnections)
        {
            this.connectedPoints = new List<TSPConnection>();

            this.minPoint = tSPConnections.First().startCity;
            this.maxPoint = tSPConnections.First().startCity;
            this.tempFinalList = new List<Point>();

            foreach (TSPConnection point in connectedPoints)
            {
                this.connectedPoints.Add(point);
            }

            for (int i = 0; i < connectedPoints.Count; i++)
            {
                Point point = new Point(connectedPoints[i].startCity.X, connectedPoints[i].startCity.Y);

                if (point.X < this.minPoint.X) { this.minPoint.X = point.X; }
                else if (point.X > this.maxPoint.X) { this.maxPoint.X = point.X; }
                if (point.Y < this.minPoint.Y) { this.minPoint.Y = point.Y; }
                else if (point.Y > this.maxPoint.Y) { this.maxPoint.Y = point.Y; }
            }

            this.canvasOffset = new Point(10, 10);

            if (this.minPoint.X > 0) { this.canvasOffset.X -= this.minPoint.X; }
            else { this.canvasOffset.X += this.minPoint.X; }
            if (this.minPoint.Y > 0) { this.canvasOffset.X -= this.minPoint.X; }
            else { this.canvasOffset.X += this.minPoint.X; }

            this.shortestDistance = 0;
        }

        public double distance(Point pointOne, Point pointTwo)
        {
            return Math.Sqrt(Math.Pow((pointTwo.X - pointOne.X), 2) + Math.Pow((pointTwo.Y - pointOne.Y), 2));
        }

        public List<Point> BruteForce()
        {
            //This final list will represent the correct order - or path - to take
            List<Point> finalList = new List<Point>();
            var tempList = new List<Point>();
            var newList = new List<Point>();
            double localDistance = 0;
            shortestDistance = 0;
            int totalPermutations = 0;
            int initialCount = 0;

            foreach (Point point in this.points)
            {
                tempList.Add(point);
            }

            initialCount = tempList.Count();

            Point firstElement = tempList.First();
            List<Point> rest = tempList;
            rest.RemoveAt(0);

            //Iterate through each permutaion
            foreach(var perm in Permutate(rest, rest.Count()))
            {
                double shortestSoFar = shortestDistance;
                localDistance = 0;
                newList.Clear();
                newList.Add(firstElement); //we start with the same city every time
                //Iterate through each element in this particular permutation
                foreach (var i in perm)
                {
                    //We need to read the element as a string because it is no longer recognized as a point
                    //Once we have the strong, it can be converted back to a point and added to the new list
                    string[] parts = i.ToString().Split(',');
                    Point tempPoint = new Point(Convert.ToDouble(parts[0]), Convert.ToDouble(parts[1]));
                    newList.Add(tempPoint);
                }
                newList.Add(firstElement); //we end with the same city every time
                //Calculate the distance
                for (int i = 0; i < newList.Count(); i++)
                {
                    if ((i + 1) != newList.Count())
                        localDistance += distance(newList[i], newList[i + 1]);
                }
                //Check if this should be a canidate for the final list
                if (shortestDistance > localDistance || shortestDistance == 0)
                {
                    shortestDistance = localDistance;
                    finalList.Clear();
                    finalList = newList.ToList(); //Save computation time of foreach
                }
            }

            int city = 1;
            Debug.WriteLine("\nFinal list: ");
            foreach (Point point in finalList)
            {
                Debug.WriteLine(city + ": " + point);
                city++;
            }
            Debug.WriteLine("\nTotal Run Distance: " + shortestDistance + "\nTotal Permutations: " + totalPermutations);

            return finalList;
        }

        public List<Point> BFS()
        {
            List<Point> finalList = new List<Point>();

            return finalList;
        }

        public List<Point> DFS()
        {
            List<Point> finalList = new List<Point>();

            return finalList;
        }

        #region Permutation

        //The following two functions are implemented from: https://www.codeproject.com/Articles/43767/A-C-List-Permutation-Iterator

        public static void RotateRight(IList sequence, int count)
        {
            object tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        public static IEnumerable<IList> Permutate(IList sequence, int count)
        {
            if (count == 1) yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in Permutate(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }

        #endregion
    }
}
