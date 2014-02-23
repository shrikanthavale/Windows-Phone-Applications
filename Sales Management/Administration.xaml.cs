using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sales_Management.Entities;
using Sales_Management.WebserviceAccess;

namespace Sales_Management
{
    public partial class Administration : PhoneApplicationPage
    {
        private AdministrationQuestionModel adminQuestionModel { get; set; }

        private QuestionDetails questionDetails { get; set; }

        private List<QuestionOptionDetails> questionOptionDetails { get; set; }

        private AdministrationQuestion question { get; set; }

        private ApplicationBar saveCancelApplicationBar = new ApplicationBar();

        private ApplicationBar optionABCDApplicationBar = new ApplicationBar();

        public Administration()
        {
            // initialize the component
            InitializeComponent();

            // create the application bars
            CreateSaveCancelApplicationBar();

            // create options button bar
            CreateOptionABCDApplicationBar();

            // new instance
            adminQuestionModel = new AdministrationQuestionModel();

            // populate data
            PopulateData();

        }



        private async void PopulateData()
        {
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Loading Complete List ... Please Wait"

            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            adminQuestionModel = await adminQuestionModel.LoadData();

            // set the data context
            DataContext = (adminQuestionModel);

            progressIndicator.IsVisible = false;

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
            //PopulateData();
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // long list selector
            LongListSelector longListSelector = sender as LongListSelector;

            // verifying the long list selector
            if (longListSelector == null)
                return;

            // question data
            question = longListSelector.SelectedItem as AdministrationQuestion;

            // check sound data
            if (question == null)
            {
                return;
            }

            // set the new pivot
            MainPivot.SelectedIndex = 1;

            // webservice call to fetch data
            PopulateDescriptionSolution(question.caseStudyNode);

            // visible the application bar
            ApplicationBar.IsVisible = true;

        }

        private async void PopulateDescriptionSolution(String selectedNode)
        {

            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Loading Node Specific Details ... Please Wait"

            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            questionDetails = await WebServiceReadData.GetSingleQuestionDetails(selectedNode);
            questionOptionDetails = await WebServiceReadData.GetQuestionOptionDetails(selectedNode);

            QuestionTitleTextBox.Text = questionDetails.caseStudyTitle;
            QuestionOrganizationText.Text = questionDetails.caseStudyOrganization;
            QuestionDescriptionText.Text = questionDetails.caseStudyDescription;

            foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
            {
                String questionOptionDescription = questionOptions.questionOptionDescription;
                String questionOptionEvaluation = questionOptions.questionOptionEvaluation;
                String questionOptionMoney = questionOptions.questionOptionMoneyAssoicated + "";

                switch (questionOptions.caseStudyOptionNumber)
                {
                    case 1: QuestionOptionATextBox.Text = questionOptionDescription;
                            QuestionOptionAEvaluationText.Text = questionOptionEvaluation;
                            QuestionOptionAAmountText.Text = questionOptionMoney;
                        break;
                    case 2: QuestionOptionBTextBox.Text = questionOptionDescription;
                        QuestionOptionBEvaluationText.Text = questionOptionEvaluation;
                        QuestionOptionBAmountText.Text = questionOptionMoney;
                        break;
                    case 3: QuestionOptionCTextBox.Text = questionOptionDescription;
                        QuestionOptionCEvaluationText.Text = questionOptionEvaluation;
                        QuestionOptionCAmountText.Text = questionOptionMoney;
                        break;
                    case 4: QuestionOptionDTextBox.Text = questionOptionDescription;
                        QuestionOptionDEvaluationText.Text = questionOptionEvaluation;
                        QuestionOptionDAmountText.Text = questionOptionMoney;
                        break;

                }
            }

            progressIndicator.IsVisible = false;

            MessageBoxResult result = MessageBox.Show("Data for selected node '" + selectedNode + "' loaded successfully. Details can be edited now."
                                               , "Loading Successful", MessageBoxButton.OK);
        }

        private void ApplicationBarIconButton_Cancel(object sender, EventArgs e)
        {
            QuestionTitleTextBox.Text = questionDetails.caseStudyTitle;
            QuestionOrganizationText.Text = questionDetails.caseStudyOrganization;
            QuestionDescriptionText.Text = questionDetails.caseStudyDescription;
        }

        private async void ApplicationBarIconButton_Save(object sender, EventArgs e)
        {
            QuestionDetails questionDetailsEntity = new QuestionDetails();

            questionDetailsEntity.caseStudyTitle = QuestionTitleTextBox.Text;
            questionDetailsEntity.caseStudyNode = question.caseStudyNode;
            questionDetailsEntity.caseStudyOrganization = QuestionOrganizationText.Text;
            questionDetailsEntity.caseStudyDescription = QuestionDescriptionText.Text;

            // web service call to save data
             ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Saving Details ... Please Wait"

            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            String response = await WebServiceWriteData.SaveQuestionDetails(questionDetailsEntity);

            MessageBoxResult result = MessageBox.Show("Details Saved Successfully"
                                , "Save Successful", MessageBoxButton.OK);

            questionDetails = questionDetailsEntity;
            
            progressIndicator.IsVisible = false;
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPivot.SelectedIndex == 0)
            {
                // invisible the application bar
                ApplicationBar.IsVisible = false;
            }

            if (MainPivot.SelectedIndex == 1)
            {
                // invisible the application bar
                ApplicationBar.IsVisible = false;
                ApplicationBar = saveCancelApplicationBar;
                ApplicationBar.IsVisible = true;
            }

            if (MainPivot.SelectedIndex == 2)
            {
                // invisible the application bar
                ApplicationBar.IsVisible = false;
                ApplicationBar = optionABCDApplicationBar;
                ApplicationBar.IsVisible = true;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
                PivotItem3Root1.Visibility = System.Windows.Visibility.Visible;
                PivotItem3Root2.Visibility = System.Windows.Visibility.Collapsed;
                PivotItem3Root3.Visibility = System.Windows.Visibility.Collapsed;
                PivotItem3Root4.Visibility = System.Windows.Visibility.Collapsed;
            }


        }

        private void CreateSaveCancelApplicationBar()
        {

            ApplicationBarIconButton cancelButton = new ApplicationBarIconButton();
            cancelButton.Text = "Cancel";
            cancelButton.IconUri = new Uri("/Assets/Images/cancel.png",UriKind.Relative);
            cancelButton.Click += new EventHandler(ApplicationBarIconButton_Cancel);
            saveCancelApplicationBar.Buttons.Add(cancelButton);

            ApplicationBarIconButton saveButton = new ApplicationBarIconButton();
            saveButton.Text = "Save";
            saveButton.IconUri = new Uri("/Assets/Images/save.png", UriKind.Relative);
            saveButton.Click += new EventHandler(ApplicationBarIconButton_Save);
            saveCancelApplicationBar.Buttons.Add(saveButton);

            ApplicationBar = saveCancelApplicationBar;
            ApplicationBar.IsVisible = false;

        }

        private void CreateOptionABCDApplicationBar()
        {

            ApplicationBarIconButton optionAButton = new ApplicationBarIconButton();
            optionAButton.Text = "A";
            optionAButton.IconUri = new Uri("/Assets/Images/A.png",UriKind.Relative);
            optionAButton.Click += new EventHandler(optionAClicked);
            optionABCDApplicationBar.Buttons.Add(optionAButton);

            ApplicationBarIconButton optionBButton = new ApplicationBarIconButton();
            optionBButton.Text = "B";
            optionBButton.IconUri = new Uri("/Assets/Images/B.png", UriKind.Relative);
            optionBButton.Click += new EventHandler(optionBClicked);
            optionABCDApplicationBar.Buttons.Add(optionBButton);

            ApplicationBarIconButton optionCButton = new ApplicationBarIconButton();
            optionCButton.Text = "C";
            optionCButton.IconUri = new Uri("/Assets/Images/C.png", UriKind.Relative);
            optionCButton.Click += new EventHandler(optionCClicked);
            optionABCDApplicationBar.Buttons.Add(optionCButton);

            ApplicationBarIconButton optionDButton = new ApplicationBarIconButton();
            optionDButton.Text = "D";
            optionDButton.IconUri = new Uri("/Assets/Images/D.png", UriKind.Relative);
            optionDButton.Click += new EventHandler(optionDClicked);
            optionABCDApplicationBar.Buttons.Add(optionDButton);

            ApplicationBarMenuItem cancelMenuItem = new ApplicationBarMenuItem();
            cancelMenuItem.Text = "Cancel Changes";
            cancelMenuItem.Click += cancelMenuItem_Click;

            ApplicationBarMenuItem saveMenuItem = new ApplicationBarMenuItem();
            saveMenuItem.Text = "Save All Changes";
            saveMenuItem.Click += saveMenuItem_Click;

            optionABCDApplicationBar.MenuItems.Add(cancelMenuItem);
            optionABCDApplicationBar.MenuItems.Add(saveMenuItem);

        }

        async void saveMenuItem_Click(object sender, EventArgs e)
        {
            List<QuestionOptionDetails> questionOptionDetailsNew = new List<QuestionOptionDetails>();

            foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
            {
                QuestionOptionDetails newQuestionOptionDetails = new QuestionOptionDetails();
                newQuestionOptionDetails.caseStudyNode = questionOptions.caseStudyNode;
                newQuestionOptionDetails.caseStudyOptionNumber = questionOptions.caseStudyOptionNumber;

                switch (questionOptions.caseStudyOptionNumber)
                {
                    case 1: 
                        newQuestionOptionDetails.questionOptionDescription = QuestionOptionATextBox.Text;
                        newQuestionOptionDetails.questionOptionEvaluation = QuestionOptionAEvaluationText.Text;
                        newQuestionOptionDetails.questionOptionMoneyAssoicated = Int16.Parse(QuestionOptionAAmountText.Text); 
                        break;
                    case 2: 
                        newQuestionOptionDetails.questionOptionDescription = QuestionOptionBTextBox.Text;
                        newQuestionOptionDetails.questionOptionEvaluation = QuestionOptionBEvaluationText.Text;
                        newQuestionOptionDetails.questionOptionMoneyAssoicated = Int16.Parse(QuestionOptionBAmountText.Text); 
                        break;
                    case 3: 
                        newQuestionOptionDetails.questionOptionDescription = QuestionOptionCTextBox.Text;
                        newQuestionOptionDetails.questionOptionEvaluation = QuestionOptionCEvaluationText.Text;
                        newQuestionOptionDetails.questionOptionMoneyAssoicated = Int16.Parse(QuestionOptionCAmountText.Text); 
                        break;
                    case 4: 
                        newQuestionOptionDetails.questionOptionDescription = QuestionOptionDTextBox.Text;
                        newQuestionOptionDetails.questionOptionEvaluation = QuestionOptionDEvaluationText.Text;
                        newQuestionOptionDetails.questionOptionMoneyAssoicated = Int16.Parse(QuestionOptionDAmountText.Text); 
                        break;

                }
                questionOptionDetailsNew.Add(newQuestionOptionDetails);
            }

            // web service call to save data
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Saving Details ... Please Wait"

            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            String response = await WebServiceWriteData.SaveQuestionOptionDetails(questionOptionDetailsNew);

            MessageBoxResult result = MessageBox.Show("Details Saved Successfully"
                                , "Save Successful", MessageBoxButton.OK);
            questionOptionDetails = questionOptionDetailsNew;

            progressIndicator.IsVisible = false;

        }

        void cancelMenuItem_Click(object sender, EventArgs e)
        {
            if (!((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled)
            {
                foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
                {
                    String questionOptionDescription = questionOptions.questionOptionDescription;
                    String questionOptionEvaluation = questionOptions.questionOptionEvaluation;
                    String questionOptionMoney = questionOptions.questionOptionMoneyAssoicated + "";

                    switch (questionOptions.caseStudyOptionNumber)
                    {
                        case 1: QuestionOptionATextBox.Text = questionOptionDescription;
                            QuestionOptionAEvaluationText.Text = questionOptionEvaluation;
                            QuestionOptionAAmountText.Text = questionOptionMoney;
                            break;
                    }

                }

            }
            else if (!((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled)
            {
                foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
                {
                    String questionOptionDescription = questionOptions.questionOptionDescription;
                    String questionOptionEvaluation = questionOptions.questionOptionEvaluation;
                    String questionOptionMoney = questionOptions.questionOptionMoneyAssoicated + "";

                    switch (questionOptions.caseStudyOptionNumber)
                    {
                        case 2: QuestionOptionBTextBox.Text = questionOptionDescription;
                            QuestionOptionBEvaluationText.Text = questionOptionEvaluation;
                            QuestionOptionBAmountText.Text = questionOptionMoney;
                            break;
                    }

                }

            }else if (!((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled)
            {
                foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
                {
                    String questionOptionDescription = questionOptions.questionOptionDescription;
                    String questionOptionEvaluation = questionOptions.questionOptionEvaluation;
                    String questionOptionMoney = questionOptions.questionOptionMoneyAssoicated + "";

                    switch (questionOptions.caseStudyOptionNumber)
                    {
                        case 3: QuestionOptionCTextBox.Text = questionOptionDescription;
                            QuestionOptionCEvaluationText.Text = questionOptionEvaluation;
                            QuestionOptionCAmountText.Text = questionOptionMoney;
                            break;
                    }

                }

            }
            else if (!((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled)
            {
                foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
                {
                    String questionOptionDescription = questionOptions.questionOptionDescription;
                    String questionOptionEvaluation = questionOptions.questionOptionEvaluation;
                    String questionOptionMoney = questionOptions.questionOptionMoneyAssoicated + "";

                    switch (questionOptions.caseStudyOptionNumber)
                    {
                        case 4: QuestionOptionDTextBox.Text = questionOptionDescription;
                            QuestionOptionDEvaluationText.Text = questionOptionEvaluation;
                            QuestionOptionDAmountText.Text = questionOptionMoney;
                            break;
                    }

                }

            }
        }

        private void optionAClicked(object sender, EventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
            PivotItem3Root1.Visibility = System.Windows.Visibility.Visible;
            PivotItem3Root2.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root3.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root4.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void optionBClicked(object sender, EventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
            PivotItem3Root1.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root2.Visibility = System.Windows.Visibility.Visible;
            PivotItem3Root3.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root4.Visibility = System.Windows.Visibility.Collapsed;

            
        }

        private void optionCClicked(object sender, EventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
            PivotItem3Root1.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root2.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root3.Visibility = System.Windows.Visibility.Visible;
            PivotItem3Root4.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void optionDClicked(object sender, EventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = false;
            PivotItem3Root1.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root2.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root3.Visibility = System.Windows.Visibility.Collapsed;
            PivotItem3Root4.Visibility = System.Windows.Visibility.Visible;
        }

    }
}