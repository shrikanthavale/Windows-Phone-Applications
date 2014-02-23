using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using Sales_Management.WebserviceAccess;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Sales_Management.Entities;
using Sales_Management.StaticData;

namespace Sales_Management
{
    public partial class PlayGrid : PhoneApplicationPage
    {
        private List<Button> _listButtons = new List<Button>();

        private List<Button> _listQuestionNodes = new List<Button>();

        private List<Button> _listNonQuestionNodes = new List<Button>();

        private List<Button> _listMergedNodes = new List<Button>();

        private Button _startButton = new Button();

        private Button _restaurantButton = new Button();

        private int _timer = 0;

        public PlayGrid()
        {
            InitializeComponent();
            // add the button click listener to all the buttons

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

            //Populate Data 
            PopulateNodeData();

            //update the list of visited nodes accordingly
            DisableVisitedNodes();

            // initialize the timer
            InitializeTimeMoney();

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
                button.Click += _nodeClick;
                TextBlock textBlock = new TextBlock();
                button.Content = textBlock;
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
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "   " + "X" + "  " + "1000";
                textBlock.FontSize = 19;
                textBlock.Visibility = Visibility.Collapsed;
                button.Content = textBlock;
                button.Style = Resources["NonQuestionNodeStyle"] as Style;
                button.Click += _nodeClick;
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
            _startButton.Click += _nodeClick;
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
            textBlock.Text = "   " +"R" + " " + "    ";
            _restaurantButton.Content = textBlock;
            _restaurantButton.FontSize = 19;
            _restaurantButton.Click += _nodeClick;
            Grid.SetRow(_restaurantButton, 2);
            Grid.SetColumn(_restaurantButton, 3);

        }

        private void DisableVisitedNodes()
        {
            if (!ApplicationStaticData.restorePreviousGrid)
            {
                ApplicationStaticData.listVisitedNodes.Clear();
                foreach (Button temp in _listQuestionNodes)
                {
                    temp.Style = Resources["QuestionNodeStyle"] as Style;
                }
            }
            else
            {
                foreach (Button tempButton in ApplicationStaticData.listVisitedNodes)
                {
                    foreach (Button newButton in _listQuestionNodes)
                    {
                        if (tempButton.Name.Equals(newButton.Name))
                        {
                            newButton.Style = Resources["QuestionNodeStyleVisited"] as Style;
                            break;
                        }
                    }
                }
            }
        }

        private void InitializeTimeMoney()
        {
            if (!ApplicationStaticData.restorePreviousGrid)
            {
                // changed here
                _timer = 80;
                timeTextBox.Text = "Timer : " + _timer;
                moneyTextBox.Text = "$ : 00000";
                ApplicationStaticData.moneyEarned = 0;
                ApplicationStaticData.timeUsed = _timer;

            }
            else
            {
                _timer = ApplicationStaticData.timeUsed;
                timeTextBox.Text = "Timer : " + _timer;
                if (ApplicationStaticData.moneyEarned == 0)
                {
                    moneyTextBox.Text = "$ : 00000";
                }
                else
                {
                    moneyTextBox.Text = "$ : " + ApplicationStaticData.moneyEarned;
                }
            }
        }

        private void FocusCorrectNode()
        {
            if (!ApplicationStaticData.restorePreviousGrid)
            {
                ApplicationStaticData.focusedButton = _startButton;
                ApplicationStaticData.focusedButton.Focus();
                
            }
            else
            {
                foreach (Button tempButton in _listMergedNodes)
                {
                    if (tempButton.Name.Equals(ApplicationStaticData.focusedButton.Name))
                    {
                        ApplicationStaticData.focusedButton = tempButton;
                        tempButton.Focus();
                        break;
                    }
                }
                bool value = ApplicationStaticData.focusedButton.Focus();
           }

        }


        private async void PopulateNodeData()
        {
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Loading Node Details ... Please Wait"
                
            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            Dictionary<String,int> mapNodeMaxAmount = await WebServiceReadData.GetMaximumAmountNodeMap();

            foreach(Button questionNode in _listQuestionNodes){

                String name = questionNode.Name;

                switch (name)
                {
                    case "1":
                        {
                            String nodeName = "A";
                            int maxAmount = mapNodeMaxAmount[nodeName];
                            questionNode.Content = createTextBlock(nodeName,maxAmount);
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

            progressIndicator.IsVisible = false;
            FocusCorrectNode();

        }

        private TextBlock createTextBlock(String nodeName, int maxAmount)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "   " + nodeName + "  " + maxAmount;
            textBlock.FontSize = 19;
            textBlock.TextWrapping = TextWrapping.Wrap;
            return textBlock;
        }

        void _nodeClick(object sender, RoutedEventArgs e)
        {
            ShowMessage("Use Navigation Keys, You will be redirected to original position", "Use Navigation Keys");
            ApplicationStaticData.focusedButton.Focus();
        }

        private void AddQuestionNonQuestionNodesToGrid()
        {
            if (!ApplicationStaticData.restorePreviousGrid)
            {
                _listMergedNodes.Clear();
                _listMergedNodes = _listQuestionNodes.Union(_listNonQuestionNodes).ToList<Button>();
                PlayGrid.Shuffle(_listMergedNodes);

                int columnPosition = 0;
                int rowPosition = 0;

                foreach (Button tempButton in _listMergedNodes)
                {
                    if (rowPosition == 6)
                    {
                        rowPosition = 0;
                        columnPosition = columnPosition + 1;
                    }

                    if (rowPosition == 2 && columnPosition == 3)
                    {
                        rowPosition = rowPosition + 1;
                    }
                    else if ((rowPosition == 5 && columnPosition == 2))
                    {
                        rowPosition = 0;
                        columnPosition = columnPosition + 1;
                    }

                    Grid.SetColumn(tempButton, columnPosition);
                    Grid.SetRow(tempButton, rowPosition);
                    PlayAreaGrid.Children.Add(tempButton);

                    rowPosition = rowPosition + 1;
                }

                _listMergedNodes.Add(_startButton);
                _listMergedNodes.Add(_restaurantButton);

                PlayAreaGrid.Children.Add(_startButton);
                PlayAreaGrid.Children.Add(_restaurantButton);

                ApplicationStaticData.listQuestionNonQuestionNode = _listMergedNodes;
                
            }
            else
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

        }

        private void UpArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer >= 5)
            {
                // get the row and column position of the 
                int rowPosition = Grid.GetRow(ApplicationStaticData.focusedButton);
                int columnPosition = Grid.GetColumn(ApplicationStaticData.focusedButton);

                rowPosition = rowPosition - 1;
                foreach (Button tempButton in _listMergedNodes)
                {
                    int newRowPosition = Grid.GetRow(tempButton);
                    int newColumnPosition = Grid.GetColumn(tempButton);
                    if (rowPosition == newRowPosition && columnPosition == newColumnPosition)
                    {
                        ApplicationStaticData.focusedButton = tempButton;
                        // update every move timer
                        UpdateTimer(5);
                        break;
                    }
                }
            }
            else
            {
                ShowMessage("Your time is up, End/Restart the game", "Time Up !!!!");
            }

            ApplicationStaticData.focusedButton.Focus();

        }

        private void LeftArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer >= 5)
            {
                // get the row and column position of the 
                int rowPosition = Grid.GetRow(ApplicationStaticData.focusedButton);
                int columnPosition = Grid.GetColumn(ApplicationStaticData.focusedButton);

                columnPosition = columnPosition - 1;
                foreach (Button tempButton in _listMergedNodes)
                {
                    int newRowPosition = Grid.GetRow(tempButton);
                    int newColumnPosition = Grid.GetColumn(tempButton);
                    if (rowPosition == newRowPosition && columnPosition == newColumnPosition)
                    {
                        ApplicationStaticData.focusedButton = tempButton;
                        // update every move timer
                        UpdateTimer(5);
                        break;
                    }
                }
            }
            else
            {
                ShowMessage("Your time is up, End/Restart the game", "Time Up !!!!");
            }

            ApplicationStaticData.focusedButton.Focus();

        }

        private void RightArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer >= 5)
            {
                // get the row and column position of the 
                int rowPosition = Grid.GetRow(ApplicationStaticData.focusedButton);
                int columnPosition = Grid.GetColumn(ApplicationStaticData.focusedButton);

                columnPosition = columnPosition + 1;
                foreach (Button tempButton in _listMergedNodes)
                {
                    int newRowPosition = Grid.GetRow(tempButton);
                    int newColumnPosition = Grid.GetColumn(tempButton);
                    if (rowPosition == newRowPosition && columnPosition == newColumnPosition)
                    {
                        ApplicationStaticData.focusedButton = tempButton;
                        // update every move timer
                        UpdateTimer(5);
                        break;
                    }
                }
            }
            else
            {
                ShowMessage("Your time is up, End/Restart the game", "Time Up !!!!");
            }

            ApplicationStaticData.focusedButton.Focus();
        }

        private void BottomArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer >= 5)
            {
                // get the row and column position of the 
                int rowPosition = Grid.GetRow(ApplicationStaticData.focusedButton);
                int columnPosition = Grid.GetColumn(ApplicationStaticData.focusedButton);

                rowPosition = rowPosition + 1;
                foreach (Button tempButton in _listMergedNodes)
                {
                    int newRowPosition = Grid.GetRow(tempButton);
                    int newColumnPosition = Grid.GetColumn(tempButton);
                    if (rowPosition == newRowPosition && columnPosition == newColumnPosition)
                    {
                        ApplicationStaticData.focusedButton = tempButton;
                        // update every move timer
                        UpdateTimer(5);
                        break;
                    }
                }
            }
            else
            {
                ShowMessage("Your time is up, End/Restart the game", "Time Up !!!!");
            }

            ApplicationStaticData.focusedButton.Focus();
        }

        private void ShowMessage(string errorMessage, string errorMessageTitle)
        {
            MessageBoxResult result = MessageBox.Show(errorMessage, errorMessageTitle, MessageBoxButton.OK);
        }


        private void UpdateTimer(int _time)
        {
            _timer = _timer - _time;
            timeTextBox.Text = "Timer : " + _timer;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            VisitCustomer();
            ApplicationStaticData.focusedButton.Focus();
        }

        private void VisitCustomer()
        {
            String nodeText = "";

            if (ApplicationStaticData.focusedButton.Content == null)
            {
                // trying to visit a non customer node
                ShowMessage("This node cannot be visited", "Invalid Node Error");
                return;

            }else if (ApplicationStaticData.focusedButton.Content != null)
            {
                // get the node that was clicked
                nodeText = ((TextBlock)ApplicationStaticData.focusedButton.Content).Text.Trim();

                 // get the node
                 String customerText = nodeText.Split(' ')[0];

                 if (nodeText.ToLower().Equals("Start".ToLower()) || nodeText.ToLower().Equals("R".ToLower()) || customerText.ToLower().Equals("X".ToLower()))
                {
                    // trying to visit a non customer node
                    ShowMessage("This node cannot be visited", "Invalid Node Error");
                    return;

                }
            }
               
            // get the node that was clicked
            nodeText = ((TextBlock)ApplicationStaticData.focusedButton.Content).Text;

            bool visitedFlag = false;

            foreach(Button tempButton in ApplicationStaticData.listVisitedNodes)
            {
                if (tempButton.Name.Equals(ApplicationStaticData.focusedButton.Name))
                {
                    visitedFlag = true;
                    break;
                }
            }
            if (visitedFlag)
            {
                // trying to visit a non customer node
                ShowMessage("You have already visited this node", "Revisit Node Error");
                return;
            }
            else
            {
                if (_timer > 5)
                {
                    ApplicationStaticData.restorePreviousGrid = true;

                    // get the node
                    nodeText = nodeText.Trim();
                    nodeText = nodeText.Split(' ')[0];
                    ApplicationStaticData.currentNodeVisit = nodeText;

                    // change the style to visited
                    ApplicationStaticData.focusedButton.Style = Resources["QuestionNodeStyleVisited"] as Style;

                    // add it to visited list
                    ApplicationStaticData.listVisitedNodes.Add(ApplicationStaticData.focusedButton);

                    // udpate the timer value
                    UpdateTimer(10);
                    ApplicationStaticData.timeUsed = _timer;

                    NavigationService.Navigate(new Uri("/QuestionDescription.xaml", UriKind.RelativeOrAbsolute));

                }
                else
                {
                    ShowMessage("Your time is almost over, you atleast need 10 units to visit a node. End/Restart the game", "Time Up !!!!");
                }
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure? This will reset your current game progress", "Reset Game", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            // remove all the nodes first
           foreach (Button tempButton in _listMergedNodes){
                PlayAreaGrid.Children.Remove(tempButton);
            }

           ApplicationStaticData.restorePreviousGrid = false;

            // clear the merged node list
           _listMergedNodes.Clear();

            //clear the visited nodes
           DisableVisitedNodes();

            // now add them again
           AddQuestionNonQuestionNodesToGrid();

           //Populate Data 
           PopulateNodeData();

           // initialize the timer
           InitializeTimeMoney();

        }
        
        private void timeTextBox_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ApplicationStaticData.focusedButton.Focus();
        }

        private void timeTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ApplicationStaticData.focusedButton.Focus();
        }

        private void moneyTextBox_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ApplicationStaticData.focusedButton.Focus();
        }

        private void moneyTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ApplicationStaticData.focusedButton.Focus();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure? Going back will reset and exit game", "Exit Game", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                //base.OnBackKeyPress(e);
                ApplicationStaticData.restorePreviousGrid = false;
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                e.Cancel = true;
                ApplicationStaticData.focusedButton.Focus();
                NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
                ApplicationStaticData.focusedButton.Focus();
            }
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if(ApplicationStaticData.focusedButton != null)
                ApplicationStaticData.focusedButton.Focus();
            this.NavigationService.RemoveBackEntry();
        }


        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationStaticData.restorePreviousGrid = false;
            NavigationService.Navigate(new Uri("/OptimalSoultion.xaml", UriKind.RelativeOrAbsolute));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            FocusCorrectNode();
        }

    }
}