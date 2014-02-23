using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sales_Management.WebserviceAccess;
using Sales_Management.StaticData;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using Sales_Management.Entities;
using System.Diagnostics;

namespace Sales_Management
{
    public partial class OptimalSoultion : PhoneApplicationPage
    {

        // changed here
        private int[][] nodeEdgeWeightMatrix = new int[11][];

        private List<Button> _listButtons = new List<Button>();

        private List<Button> _listQuestionNodes = new List<Button>();

        private List<Button> _listNonQuestionNodes = new List<Button>();

        private List<Button> _listMergedNodes = new List<Button>();

        private List<int> _listNonQuestionNodesInTravelPath = new List<int>();

        private List<int> travelPath = new List<int>();

        private List<List<int>> allPossibleRoutes = new List<List<int>>();

        private Button _startButton = new Button();

        private Button _restaurantButton = new Button();

        private Dictionary<String, int> mapNodeMaxAmount = new Dictionary<string, int>();

        private TimeTravelledMoneyEarned TimeMoneyPath = new TimeTravelledMoneyEarned();

        private int TotalTimeSpent = 0;

        private int TOTAL_TIME = 80;

        private int NODE_TRAVERSAL_TIME = 5;

        private int NODE_VISIT_TIME = 10;

        private int TotalAmountEarned = 0;

        // changed here
        private int BRUTE_FORCE_TOTAL_TIME = 80;

        private int BruteForceMoneyEarned = 0;

        private bool stopGetting = false;

        public OptimalSoultion()
        {
            // initialize the components
            InitializeComponent();

            // initialize 2D array 
            Initialize2DArray();

            // create the question nodes
            CreateQuestionNodes();

            // create non question nodes
            CreateNonQuestionNodes();

            // create start node
            CreateStartNode();

            // create restaurant node
            CreateRestaurantNode();

            // add the buttons dynamically to grid
            AddQuestionNonQuestionNodesToGrid();

            //All Time Consuming Task
            AllTimeConsumingTask();
        }

        private void Initialize2DArray()
        {
            for (int counter = 0; counter < nodeEdgeWeightMatrix.Length; counter++)
            {
                // changed here
                nodeEdgeWeightMatrix[counter] = new int[11];
            }

        }

        private void CreateQuestionNodes()
        {
            // create 16 question nodes
            // changed here
            for (int counter = 1; counter <= 9; counter++)
            {
                Button button = new Button();
                button.Name = counter.ToString();
                button.Style = Resources["QuestionNodeStyle"] as Style;
                button.IsEnabled = false;
                _listQuestionNodes.Add(button);
            }
        }

        private void CreateNonQuestionNodes()
        {
            // create 16 question nodes
            // changed here
            for (int counter = 12; counter <= 36; counter++)
            {
                Button button = new Button();
                button.Name = counter.ToString();
                button.Style = Resources["NonQuestionNodeStyle"] as Style;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "   " + "X" + "  " + "1000";
                textBlock.FontSize = 19;
                textBlock.Visibility = Visibility.Collapsed;
                button.Content = textBlock;
                button.IsEnabled = false;
                _listNonQuestionNodes.Add(button);
            }
        }

        private void CreateStartNode()
        {
            _startButton = new Button();
            // changed here
            _startButton.Name = 10 + "";
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Start";
            _startButton.Content = textBlock;
            _startButton.FontSize = 19;
            _startButton.Style = Resources["StartNodeStyle"] as Style;
            _startButton.IsEnabled = false;
            Grid.SetRow(_startButton, 5);
            Grid.SetColumn(_startButton, 2);

        }

        private void CreateRestaurantNode()
        {
            _restaurantButton = new Button();
            // changed here
            _restaurantButton.Name = 11 + "";
            _restaurantButton.Style = Resources["RestaurantNodeStyle"] as Style;
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "   " + "R" + " " + "    ";
            _restaurantButton.Content = textBlock;
            _restaurantButton.FontSize = 19;
            _restaurantButton.IsEnabled = false;
            Grid.SetRow(_restaurantButton, 2);
            Grid.SetColumn(_restaurantButton, 3);

        }

        private void AddQuestionNonQuestionNodesToGrid()
        {
            _listMergedNodes.Clear();
            _listMergedNodes = _listQuestionNodes.Union(_listNonQuestionNodes).ToList<Button>();
            foreach (Button tempButton in ApplicationStaticData.listQuestionNonQuestionNode)
            {
                foreach (Button newButton in _listMergedNodes)
                {
                    if (tempButton.Name.Equals(newButton.Name))
                    {
                        int rowPosition = Grid.GetRow(tempButton);
                        int columnPosition = Grid.GetColumn(tempButton);
                        Grid.SetColumn(newButton, columnPosition);
                        Grid.SetRow(newButton, rowPosition);
                        PlayAreaGrid.Children.Add(newButton);
                        break;
                    }
                }
            }

            _listMergedNodes.Add(_startButton);
            _listMergedNodes.Add(_restaurantButton);
            PlayAreaGrid.Children.Add(_startButton);
            PlayAreaGrid.Children.Add(_restaurantButton);
        }

        private async Task<String> AllTimeConsumingTask()
        {

            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Calculating Optimal Path ... Please Wait"
                
            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            //Populate Data 
            await PopulateNodeData();

            // create the matrix of node edge and their weight
            CreateNodeWeightMatrix();

            // create the Brute Force All possible Transition
            CreateBruteForceAllPaths();

            //generate all possible paths in 9 customer nodes - This is Brute Force algorithm works always but very time
            // consuming so used only 9 customer nodes initially and not 16
            GenerateAllPossiblePaths();

            // calculate optimal solution - This is the nearest neighbor algorithm doesn't work always
            // CalculateOptimalPath();

            // highlight the optimal path
            HighlightOptimalPath();

            // set the time and money
            SetTimeMoney();

            progressIndicator.IsVisible = false;

            return null;

        }


 
        private async Task<String> PopulateNodeData()
        {
            // this is just added to see the loading title bar .... and to have fun seeing loading animation
            // setting this to null makes the webservice to hit the database again :-)
            WebServiceReadData.questionOptionDetails = null;

            mapNodeMaxAmount = await WebServiceReadData.GetMaximumAmountNodeMap();

            foreach (Button questionNode in _listQuestionNodes)
            {
                String name = questionNode.Name;
                switch (name)
                {
                    case "1":
                        {
                            String nodeName = "A";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "2":
                        {
                            String nodeName = "B";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "3":
                        {
                            String nodeName = "C";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "4":
                        {
                            String nodeName = "D";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "5":
                        {
                            String nodeName = "E";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "6":
                        {
                            String nodeName = "F";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "7":
                        {
                            String nodeName = "G";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "8":
                        {
                            String nodeName = "H";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "9":
                        {
                            String nodeName = "I";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "10":
                        {
                            String nodeName = "J";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "11":
                        {
                            String nodeName = "K";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "12":
                        {
                            String nodeName = "L";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "13":
                        {
                            String nodeName = "M";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "14":
                        {
                            String nodeName = "N";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "15":
                        {
                            String nodeName = "O";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }

                    case "16":
                        {
                            String nodeName = "P";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName, maxAmount);
                            break;
                        }
                }
            }

            return null;
        }

        private TextBlock createTextBlock(String nodeName, int maxAmount)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "   " + nodeName + "  " + maxAmount;
            textBlock.FontSize = 19;
            textBlock.TextWrapping = TextWrapping.Wrap;
            return textBlock;
        }

        private void CreateNodeWeightMatrix()
        {
            foreach (Button node in _listMergedNodes)
            {
                if (CheckCustomerStartNode(node))
                {
                    foreach (Button distanceNode in _listMergedNodes)
                    {
                        if (CheckCustomerStartNode(distanceNode))
                        {
                            CalculatePositionAndStoreInMatrix(node, distanceNode);
                        }
                    }
                }

            }

        }

        private bool CheckCustomerStartNode(Button node)
        {
            // changed here
            return (int.Parse(node.Name) <= 10);
        }

        private void CalculatePositionAndStoreInMatrix(Button node, Button distanceNode)
        {
            int rowPosition = Grid.GetRow(node);
            int columnPosition = Grid.GetColumn(node);
            int distanceNodeRowPosition = Grid.GetRow(distanceNode);
            int distanceNodeColumnPosition = Grid.GetColumn(distanceNode);

            int nodeName = int.Parse(node.Name);
            int distanceNodeName = int.Parse(distanceNode.Name);

            // they are the same nodes
            if (node.Name.Equals(distanceNode.Name))
            {
                nodeEdgeWeightMatrix[nodeName][distanceNodeName] = 100;
            }
            else
            {
                int rowDistance = Math.Abs(rowPosition - distanceNodeRowPosition);
                int columnDistance = Math.Abs(columnPosition - distanceNodeColumnPosition);
                int totalDistance = rowDistance + columnDistance;
                nodeEdgeWeightMatrix[nodeName][distanceNodeName] = totalDistance;
            }

        }

        private void CreateBruteForceAllPaths()
        {
            List<int> lst = new List<int>();
            // changed here
            for (int i = 10; i > 0; --i)
                lst.Add(i);
            List<int> route = new List<int>();
            BruteForceFindBestRoute(route, lst);
        }


        private void BruteForceFindBestRoute(List<int> route, List<int> citiesNotInRoute)
        {
            if (!(citiesNotInRoute.Count == 0) && !stopGetting)
            {
                for (int i = 0; i < citiesNotInRoute.Count(); i++)
                {
                    int justRemoved = citiesNotInRoute.ElementAt(0);

                    citiesNotInRoute.RemoveAt(0);

                    List<int> newRoute = new List<int>(route);

                    newRoute.Add(justRemoved);

                    BruteForceFindBestRoute(newRoute, citiesNotInRoute);

                    citiesNotInRoute.Add(justRemoved);
                }
            }
            else // if(citiesNotInRoute.isEmpty())
            {
                // changed here
                if (route.ElementAt(0) == 10)
                {
                    allPossibleRoutes.Add(route);
                }
                else
                {
                    stopGetting = true;
                }
            }
        }


        private void GenerateAllPossiblePaths()
        {
            using (StreamReader sr = new StreamReader("filename.txt"))
            {
                //String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                //while ((line = sr.ReadLine()) != null)
                foreach(List<int> intArray in allPossibleRoutes)
                {
                    String line = string.Join(",",intArray);
                    String[] arraySplit = line.Split(',');
                    int amountEarned = 0;
                    int timeSpent = 0;
                    List<int> TempTravelPath = new List<int>();
                    TempTravelPath.Add(int.Parse(arraySplit[0].Trim()));

                    for (int counter = 0; counter < arraySplit.Length - 1; counter++)
                    {
                        int position1 = int.Parse(arraySplit[counter].Trim());
                        int position2 = int.Parse(arraySplit[counter + 1].Trim());
                        int distanceBetweenThem = nodeEdgeWeightMatrix[position1][position2];
                        int timeNeededToVisitNode = distanceBetweenThem * NODE_TRAVERSAL_TIME + NODE_VISIT_TIME;
                        timeSpent = timeSpent + timeNeededToVisitNode;
                        if (timeSpent <= BRUTE_FORCE_TOTAL_TIME)
                        {
                            TempTravelPath.Add(position2);
                            amountEarned = amountEarned + GetCustomerAmount(position2);
                        }
                        else
                        {
                            timeSpent = timeSpent - timeNeededToVisitNode;
                            break;
                        }
                    }

                    if (amountEarned > BruteForceMoneyEarned)
                    {
                        BruteForceMoneyEarned = amountEarned;
                        TimeMoneyPath.TotalMoneyPath = amountEarned;
                        TimeMoneyPath.TotalTimePath = timeSpent;
                        TimeMoneyPath.ActualPath = TempTravelPath;
                    }
                }
            }

            for (int counter = 0; counter < TimeMoneyPath.ActualPath.Count() - 1; counter++)
            {
                int firstCustomer = TimeMoneyPath.ActualPath.ElementAt(counter);
                int secondCustomer = TimeMoneyPath.ActualPath.ElementAt(counter + 1);
                _listNonQuestionNodesInTravelPath = _listNonQuestionNodesInTravelPath.Union(
                                        GetNonQuestionNodeForHighligtingPath(firstCustomer, secondCustomer)).ToList<int>();
            }

            _listNonQuestionNodesInTravelPath.Add(TimeMoneyPath.ActualPath.ElementAt(0));
            TimeMoneyPath.ActualPath.RemoveAt(0);
            travelPath = TimeMoneyPath.ActualPath;
            //travelPath = TimeMoneyPath.ActualPath.Union(nonCustomerNodeTravelPath).ToList<int>();
        }

        private int GetCustomerAmount(int nodeNumber)
        {
            int currentAmount = 0;
            switch (nodeNumber)
            {
                case 1:
                    currentAmount = mapNodeMaxAmount["A"];
                    break;
                case 2:
                    currentAmount = mapNodeMaxAmount["B"];
                    break;
                case 3:
                    currentAmount = mapNodeMaxAmount["C"];
                    break;
                case 4:
                    currentAmount = mapNodeMaxAmount["D"];
                    break;
                case 5:
                    currentAmount = mapNodeMaxAmount["E"];
                    break;
                case 6:
                    currentAmount = mapNodeMaxAmount["F"];
                    break;
                case 7:
                    currentAmount = mapNodeMaxAmount["G"];
                    break;
                case 8:
                    currentAmount = mapNodeMaxAmount["H"];
                    break;
                case 9:
                    currentAmount = mapNodeMaxAmount["I"];
                    break;
                case 10:
                    currentAmount = mapNodeMaxAmount["J"];
                    break;
                case 11:
                    currentAmount = mapNodeMaxAmount["K"];
                    break;
                case 12:
                    currentAmount = mapNodeMaxAmount["L"];
                    break;
                case 13:
                    currentAmount = mapNodeMaxAmount["M"];
                    break;
                case 14:
                    currentAmount = mapNodeMaxAmount["N"];
                    break;
                case 15:
                    currentAmount = mapNodeMaxAmount["O"];
                    break;
                case 16:
                    currentAmount = mapNodeMaxAmount["P"];
                    break;
            }
            return currentAmount;
        }

        private void CalculateOptimalPath()
        {

            // list of non customer nodes for highlighting
            List<int> nonCustomerNodes = new List<int>();

            // initialize current customer to 17 as the start node
            // changed here
            int currentCustomer = 10;

            // one customer prior to current customer
            // changed here
            int previousCustomer = 10;

            // row array representing one customer
            int[] rowArray = null;


            while (TotalTimeSpent < TOTAL_TIME && (TOTAL_TIME - TotalTimeSpent) > 14)
            {
                // add the current customer to travels list
                travelPath.Add(currentCustomer);

                // start traveling from start node located at 17th position
                //rowArray =  (int [])SliceRow(nodeEdgeWeightMatrix, currentCustomer);
                rowArray = nodeEdgeWeightMatrix[currentCustomer];

                // distance between two nodes maximum can be 11
                int maximumPossibleDistance = 11;

                // list of minimum nodes
                List<int> minimumValueNodes = new List<int>();

                // cost of travel
                int costTravel = 0;

                // changed here
                for (int customerNode = 1; customerNode < 11; customerNode++)
                {
                    if (rowArray[customerNode] < maximumPossibleDistance
                                    && !travelPath.Contains(customerNode))
                    {
                        maximumPossibleDistance = rowArray[customerNode];
                        minimumValueNodes.Clear();
                        minimumValueNodes.Add(customerNode);
                        costTravel = rowArray[customerNode];

                    }
                    else if (rowArray[customerNode] == maximumPossibleDistance
                          && !travelPath.Contains(customerNode))
                    {
                        minimumValueNodes.Add(customerNode);
                        costTravel = rowArray[customerNode];
                    }
                }

                // capture the previous customer
                previousCustomer = currentCustomer;

                // out of many list select the customer having maximum amount
                currentCustomer = GetCustomerHighAmount(minimumValueNodes);

                // calculate the time spent within the interval of 5 for traversing
                // and 10 slot for visiting the customer
                TotalTimeSpent = TotalTimeSpent
                        + (costTravel * NODE_TRAVERSAL_TIME) + NODE_VISIT_TIME;

                // get the inbetween nodes to 
                List<int> travelPathNonQuestionNodes = GetNonQuestionNodeForHighligtingPath(previousCustomer, currentCustomer);

                _listNonQuestionNodesInTravelPath = _listNonQuestionNodesInTravelPath.Union(travelPathNonQuestionNodes).ToList<int>();

            }

            travelPath = travelPath.Union(_listNonQuestionNodesInTravelPath).ToList<int>();
        }

        private int GetCustomerHighAmount(List<int> minimumValueNodes)
        {

            int customerWithMaximumAmount = 0;
            int finalAmount = 0;

            foreach (int currentCustomer in minimumValueNodes)
            {
                int currentAmount = 0;

                switch (currentCustomer)
                {

                    case 1:
                        currentAmount = mapNodeMaxAmount["A"];
                        break;
                    case 2:
                        currentAmount = mapNodeMaxAmount["B"];
                        break;
                    case 3:
                        currentAmount = mapNodeMaxAmount["C"];
                        break;
                    case 4:
                        currentAmount = mapNodeMaxAmount["D"];
                        break;
                    case 5:
                        currentAmount = mapNodeMaxAmount["E"];
                        break;
                    case 6:
                        currentAmount = mapNodeMaxAmount["F"];
                        break;
                    case 7:
                        currentAmount = mapNodeMaxAmount["G"];
                        break;
                    case 8:
                        currentAmount = mapNodeMaxAmount["H"];
                        break;
                    case 9:
                        currentAmount = mapNodeMaxAmount["I"];
                        break;
                    case 10:
                        currentAmount = mapNodeMaxAmount["J"];
                        break;
                    case 11:
                        currentAmount = mapNodeMaxAmount["K"];
                        break;
                    case 12:
                        currentAmount = mapNodeMaxAmount["L"];
                        break;
                    case 13:
                        currentAmount = mapNodeMaxAmount["M"];
                        break;
                    case 14:
                        currentAmount = mapNodeMaxAmount["N"];
                        break;
                    case 15:
                        currentAmount = mapNodeMaxAmount["O"];
                        break;
                    case 16:
                        currentAmount = mapNodeMaxAmount["P"];
                        break;
                }

                if (currentAmount > finalAmount)
                {
                    finalAmount = currentAmount;
                    customerWithMaximumAmount = currentCustomer;
                }

            }

            TotalAmountEarned = TotalAmountEarned + finalAmount;

            return customerWithMaximumAmount;
        }


        private List<int> GetNonQuestionNodeForHighligtingPath(int previousCustomer, int currentCustomer)
        {
            int previousRow = 0;
            int previousColumn = 0;
            int currentRow = 0;
            int currentColumn = 0;
            int rowDifferences = 0;
            int columnDifferences = 0;


            foreach (Button button in _listMergedNodes)
            {
                if (button.Name.Equals(previousCustomer + ""))
                {
                    previousRow = Grid.GetRow(button);
                    previousColumn = Grid.GetColumn(button);

                }

                if (button.Name.Equals(currentCustomer + ""))
                {
                    currentRow = Grid.GetRow(button);
                    currentColumn = Grid.GetColumn(button);

                }

            }

            List<String> listPosition = new List<String>();

            if (previousRow > currentRow)
            {
                rowDifferences = previousRow - currentRow;
                for (int i = 1; i <= rowDifferences; i++)
                {
                    String position = ((previousRow - i) + ":"  + previousColumn);
                    listPosition.Add(position);
                }
                if (previousColumn > currentColumn)
                {
                    columnDifferences = previousColumn - currentColumn;
                    for (int i = 1; i <= columnDifferences; i++)
                    {
                        String position = (currentRow + ":"  + (previousColumn - i));
                        listPosition.Add(position);
                    }
                }
                else if (previousColumn < currentColumn)
                {
                    columnDifferences = currentColumn - previousColumn;
                    for (int i = 1; i <= columnDifferences; i++)
                    {
                        String position = (currentRow + ":" + (previousColumn + i));
                        listPosition.Add(position);
                    }
                }
                else if (previousColumn == currentColumn)
                {
                    columnDifferences = 0;
                }

            }
            else if (previousRow < currentRow)
            {
                rowDifferences = currentRow - previousRow;
                for (int i = 1; i <= rowDifferences; i++)
                {
                    String position = ((previousRow + i) + ":" + previousColumn);
                    listPosition.Add(position);
                }
                if (previousColumn > currentColumn)
                {
                    columnDifferences = previousColumn - currentColumn;
                    for (int i = 1; i <= columnDifferences; i++)
                    {
                        String position = (currentRow + ":" + (previousColumn - i));
                        listPosition.Add(position);
                    }
                }
                else if (previousColumn < currentColumn)
                {
                    columnDifferences = currentColumn - previousColumn;
                    for (int i = 1; i <= columnDifferences; i++)
                    {
                        String position = (currentRow + ":" + (previousColumn + i));
                        listPosition.Add(position);
                    }
                }
                else if (previousColumn == currentColumn)
                {
                    columnDifferences = 0;
                }

            }
            else if (previousRow == currentRow)
            {
                if (previousColumn > currentColumn)
                {
                    columnDifferences = previousColumn - currentColumn;
                    for (int i = 1; i <= columnDifferences; i++)
                    {
                        String position = (previousRow + ":" + (previousColumn - i));
                        listPosition.Add(position);
                    }
                }
                else if (previousColumn < currentColumn)
                {
                    columnDifferences = currentColumn - previousColumn;
                    for (int i = 1; i <= columnDifferences; i++)
                    {
                        String position = (previousRow + ":" + (previousColumn + i));
                        listPosition.Add(position);
                    }
                }
                else if (previousColumn == currentColumn)
                {
                    columnDifferences = 0;
                }
            }

            return GetNonCustomerNodesFromPostion(listPosition,previousRow,previousColumn,currentRow,currentColumn);

        }


        private List<int> GetNonCustomerNodesFromPostion(List<string> listPosition, int previousRow, int previousColumn,
                                                            int currentRow, int currentColumn)
        {
            List<int> travelPathNonQuestionNodes = new List<int>();

            foreach (String position in listPosition)
            {
                int capturedRowPostion = int.Parse(position.Split(':')[0]);
                int capturedColumnPosition = int.Parse(position.Split(':')[1]);

                if (!((capturedRowPostion == previousRow && capturedColumnPosition == previousColumn) ||
                        (capturedRowPostion == currentRow && capturedColumnPosition == currentColumn)))
                {
                    foreach (Button button in _listMergedNodes)
                    {
                        int buttonRowPosition = Grid.GetRow(button);
                        int buttonColumnPosition = Grid.GetColumn(button);

                        if (capturedRowPostion == buttonRowPosition && capturedColumnPosition == buttonColumnPosition)
                        {
                            travelPathNonQuestionNodes.Add(int.Parse(button.Name));
                            break;
                        }
                    }
                }
            }

            return travelPathNonQuestionNodes;
        }

        private void HighlightOptimalPath()
        {
            foreach (int tempButton in travelPath)
            {
                foreach (Button newButton in _listMergedNodes)
                {
                    if ((tempButton + "").Equals(newButton.Name))
                    {
                        newButton.Style = Resources["QuestionNodeStyleVisited"] as Style;
                        break;
                    }
                }
            }

            foreach (int tempButton in _listNonQuestionNodesInTravelPath)
            {
                foreach (Button newButton in _listMergedNodes)
                {
                    if ((tempButton + "").Equals(newButton.Name))
                    {
                        // start button
                        // change here
                        if (tempButton == 10)
                        {
                            newButton.Style = Resources["StartNodeDifferentBorderStyle"] as Style;
                            break;
                        }// restaurant node
                        // change here
                        else if (tempButton == 11)
                        {
                            newButton.Style = Resources["RestaurantNodeDifferentBorderStyle"] as Style;
                            break;
                        }
                        // question nodes
                        // change here
                        else if (tempButton <= 9)
                        {
                            newButton.Style = Resources["QuestionNodeDiffferentBorderStyle"] as Style;
                            break;
                        }
                        else
                        {
                            newButton.Style = Resources["NonQuestionNodeStyleVisited"] as Style;
                            break;
                        }
                    }
                }
            }
        }

        private void SetTimeMoney()
        {
            //timeTextBox.Text = "Timer : " + TotalTimeSpent;
            //moneyTextBox.Text = "$ : " + TotalAmountEarned;
            timeTextBox.Text = "Timer : " + TimeMoneyPath.TotalTimePath;
            moneyTextBox.Text = "$ : " + TimeMoneyPath.TotalMoneyPath;
        }

        private void solutionsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}