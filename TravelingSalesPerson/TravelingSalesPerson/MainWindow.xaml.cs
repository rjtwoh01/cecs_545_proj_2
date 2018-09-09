using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;

namespace TravelingSalesPerson
{
    public partial class MainWindow : Window
    {
        private string tspFileName;
        private List<Point> tspPoints;
        private Canvas canvas;
        private Viewbox viewbox;
        private TSP tsp;
        private string type;


        public MainWindow()
        {
            InitializeComponent();
            tspFileName = "";
            type = "";
            hideRunTime();
            hideSolveButton();
            hideType();
            tspPoints = new List<Point>();
        }

        #region Data Points

        private void populatePoints()
        {
            List<Point> newPoints = new List<Point>();
            using (StreamReader stream = new StreamReader(tspFileName))
            {
                string fileLine;
                bool coordinates = false;

                while ((fileLine = stream.ReadLine()) != null)
                {
                    string[] parts = fileLine.Split(' ');
                    if (coordinates)
                    {
                        if (parts.Length >= 3)
                        {
                            Point newPoint = new Point(Convert.ToDouble(parts[1]), Convert.ToDouble(parts[2]));
                            tspPoints.Add(newPoint);
                            newPoints.Add(newPoint);
                            Debug.WriteLine(newPoint);
                        }
                    }
                    else if (fileLine == "NODE_COORD_SECTION")
                    {
                        coordinates = true;
                    }
                }
            }
            plotPoints(newPoints);
        }

        private void plotPoints(List<Point> points)
        {
            int city = 1; //we start at the first city
            tsp = new TSP(points);

            viewbox = new Viewbox();
            viewbox.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewbox.VerticalAlignment = VerticalAlignment.Stretch;

            canvas = new Canvas();

            foreach (Point point in points)
            {
                Debug.WriteLine("City: " + city);
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 4;
                ellipse.Height = 4;
                ellipse.Fill = Brushes.Red;
                ellipse.Stroke = Brushes.Black;

                ellipse.ToolTip = city + ": (" + point.X + "," + point.Y + ")";

                // Position point on canvas
                Canvas.SetLeft(ellipse, point.X + tsp.canvasOffset.X);
                Canvas.SetTop(ellipse, point.Y + tsp.canvasOffset.Y);

                canvas.Children.Add(ellipse);

                city++;
            }

            canvas.Height = tsp.maxPoint.Y - tsp.minPoint.Y + 80;
            canvas.Width = tsp.maxPoint.X - tsp.minPoint.X + 80;
            Debug.WriteLine(canvas.Height);
            Debug.WriteLine(canvas.Width);

            viewbox.Child = canvas;
            mainGrid.Children.Add(viewbox);

            Debug.WriteLine(mainGrid.Children[0]);

            //this.UpdateLayout();
            Debug.WriteLine("Finished populating points");
            
        }

        public void drawLines(List<Point> fastestRoute)
        {
            Ellipse ellipse = canvas.Children[0] as Ellipse;
            if (ellipse != null)
            {
                ellipse.Fill = Brushes.Green;
            }

            for (int i = 0; i < fastestRoute.Count(); i++)
            {
                if ((i + 1) != fastestRoute.Count())
                {
                    //(fastestRoute[i], fastestRoute[i + 1]);
                    Point p1 = fastestRoute[i];
                    Point p2 = fastestRoute[i + 1];
                    p1.X += tsp.canvasOffset.X + 2;
                    p1.Y += tsp.canvasOffset.Y + 2;
                    p2.X += tsp.canvasOffset.X + 2;
                    p2.Y += tsp.canvasOffset.Y + 2;

                    //pathLine.Points.Add(point);

                    Shape pathLine = DrawLinkArrow(p1, p2);

                    canvas.Children.Insert(0, pathLine);
                }
            }
        }

        //Taken (and adapted based off comments) from (and adapted based off comments): https://stackoverflow.com/questions/5188877/how-to-have-arrow-symbol-on-a-line-in-c-wpf
        private static Shape DrawLinkArrow(Point p1, Point p2) 
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1), p1.Y + ((p2.Y - p1.Y) / 1));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 3, p.Y + 5);
            Point rpoint = new Point(p.X - 3, p.Y + 5);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 1;
            path.Stroke = Brushes.Black;

            return path;
        }

        #endregion

        #region UI Elements

        public void emptyCanvas()
        {
            if (canvas != null)
                this.canvas.Children.Clear();
            this.tspPoints.Clear();
        }

        public void hideSolveButton()
        {
            this.btnSolve.Visibility = Visibility.Hidden;
        }

        public void showSolveButton()
        {
            this.btnSolve.Visibility = Visibility.Visible;
        }

        public void displayRunTime()
        {
            this.lblRunTime.Visibility = Visibility.Visible;
            this.UpdateLayout();
        }

        public void hideRunTime()
        {
            this.lblRunTime.Visibility = Visibility.Hidden;
            this.UpdateLayout();
        }

        public void displayType()
        {
            this.btnSelectTSPType.Visibility = Visibility.Visible;
            this.UpdateLayout();
        }

        public void hideType()
        {
            this.btnSelectTSPType.Visibility = Visibility.Hidden;
            this.UpdateLayout();
        }

        #endregion

        #region UI Events

        //Slightly modified for my purposes from: https://stackoverflow.com/questions/10315188/open-file-dialog-and-select-a-file-using-wpf-controls-and-c-sharp
        private void btnFileUpload_Click(object sender, RoutedEventArgs e)
        {
            hideSolveButton();
            emptyCanvas();
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".tsp";
            dlg.Filter = "TSP Files|*.tsp";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                Debug.WriteLine(fileName);
                this.tspFileName = fileName;
                populatePoints();
            }

            displayType();
            hideRunTime();            
        }


        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            if (type == "bruteForce")
            {
                Stopwatch sw = Stopwatch.StartNew();
                List<Point> tempResult = tsp.BruteForce();
                sw.Stop();

                TimeSpan elapsedTime = sw.Elapsed;
                string shortestDistance = String.Format("{0:0.00}", tsp.shortestDistance);
                this.lblRunTime.Content = "Distance: " + shortestDistance + "\nRun Time: " + elapsedTime.ToString();

                displayRunTime();
                drawLines(tempResult);
            }
            else if (type == "dfs")
            {
                Stopwatch sw = Stopwatch.StartNew();
                List<Point> tempResult = tsp.BruteForce();
                sw.Stop();

                TimeSpan elapsedTime = sw.Elapsed;
                string shortestDistance = String.Format("{0:0.00}", tsp.shortestDistance);
                this.lblRunTime.Content = "Distance: " + shortestDistance + "\nRun Time: " + elapsedTime.ToString();

                displayRunTime();
                drawLines(tempResult);
            }
            else if (type == "bfs")
            {
                Stopwatch sw = Stopwatch.StartNew();
                List<Point> tempResult = tsp.BruteForce();
                sw.Stop();

                TimeSpan elapsedTime = sw.Elapsed;
                string shortestDistance = String.Format("{0:0.00}", tsp.shortestDistance);
                this.lblRunTime.Content = "Distance: " + shortestDistance + "\nRun Time: " + elapsedTime.ToString();

                displayRunTime();
                drawLines(tempResult);
            }
            else
            {
                MessageBox.Show(type + " is not implemented yet");
            }
        }

        //Implented from: https://dotnetlearning.wordpress.com/2011/02/20/dropdown-menu-in-wpf/
        private void btnSelectTSPType_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void bruteForceClick(object sender, RoutedEventArgs e)
        {
            showSolveButton();
            type = "bruteForce";
            Debug.WriteLine(type);
        }

        private void bfsClick(object sender, RoutedEventArgs e)
        {
            showSolveButton();
            type = "bfs";
            Debug.WriteLine(type);
        }

        private void dfsClick(object sender, RoutedEventArgs e)
        {
            showSolveButton();
            type = "dfs";
            Debug.WriteLine(type);
        }


        #endregion
    }
}