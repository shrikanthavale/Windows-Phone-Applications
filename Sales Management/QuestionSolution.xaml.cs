using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sales_Management.StaticData;
using Sales_Management.Entities;
using Sales_Management.WebserviceAccess;

namespace Sales_Management
{
    public partial class QuestionSolution : PhoneApplicationPage
    {
        /// <summary>
        /// storing list of question optins like A,B,C,D
        /// </summary>
        List<QuestionOptionDetails> questionOptionDetails = null;


        public QuestionSolution()
        {
            // initalize the component
            InitializeComponent();

            // fetch the data using webservice call
            PopulateDataWebservice();

        }

        /// <summary>
        /// This method is used to populate data by using web service call. Captures the current visited node and does a webservice
        /// call to fetch all the options related to question. This method runs in the background thread and doesn't block UI thread.
        /// </summary>
        private async void PopulateDataWebservice()
        {
            // get the visited customer
            String visitedCustomer = ApplicationStaticData.currentNodeVisit;

            // progress indicatore visibility
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Loading Options Details ... Please Wait"
            };
            
            // show the progress indicator
            SystemTray.SetProgressIndicator(this, progressIndicator);

            // web service call
            questionOptionDetails = await WebServiceReadData.GetQuestionOptionDetails(visitedCustomer);

            // set the option details in UI
            foreach (QuestionOptionDetails questionOptions in questionOptionDetails)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = questionOptions.questionOptionDescription;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Width = 400;

                switch (questionOptions.caseStudyOptionNumber)
                {
                    case 1: optionA.Content = textBlock;
                        break;
                    case 2: optionB.Content = textBlock;
                        break;
                    case 3: optionC.Content = textBlock;
                        break;
                    case 4: optionD.Content = textBlock;
                        break;
                }
            }

            // set the first option selected
            optionA.IsChecked = true;

            // hide the progress indicator after successful loading
            progressIndicator.IsVisible = false;
        }

        /// <summary>
        /// This method handles the back button click on the UI, navigates to the previous page.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to previous data
            NavigationService.Navigate(new Uri("/QuestionDescription.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// This method handles the answer submit by user to a question. After selecting appropriate radio button users input is
        /// captured and compared with the solution and evaluation text is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitAnswerButton_Click(object sender, RoutedEventArgs e)
        {

            // option A checked
            if (optionA.IsChecked.Value)
            {
                foreach (QuestionOptionDetails questionOptionDetail in questionOptionDetails)
                {
                    if (questionOptionDetail.caseStudyOptionNumber == 1)
                    {
                        ApplicationStaticData.moneyEarned = ApplicationStaticData.moneyEarned + 
                            questionOptionDetail.questionOptionMoneyAssoicated;

                        String finalText = questionOptionDetail.questionOptionEvaluation + "\n\n" + "Your solution is worth $ "
                            + questionOptionDetail.questionOptionMoneyAssoicated;

                        QuestionSolutionExplanationText.Text = finalText;

                        break;
                    }

                }

            } 
            // option B checked
            else if(optionB.IsChecked.Value)
            {
                foreach (QuestionOptionDetails questionOptionDetail in questionOptionDetails)
                {
                    if (questionOptionDetail.caseStudyOptionNumber == 2)
                    {
                        ApplicationStaticData.moneyEarned = ApplicationStaticData.moneyEarned + 
                            questionOptionDetail.questionOptionMoneyAssoicated;


                        String finalText = questionOptionDetail.questionOptionEvaluation + "\n" + "Your solution is worth $ "
                            + questionOptionDetail.questionOptionMoneyAssoicated;

                        QuestionSolutionExplanationText.Text = finalText;

                        break;
                    }

                }

            }
            //option C checked
            else if (optionC.IsChecked.Value)
            {
                foreach (QuestionOptionDetails questionOptionDetail in questionOptionDetails)
                {
                    if (questionOptionDetail.caseStudyOptionNumber == 3)
                    {
                        ApplicationStaticData.moneyEarned = ApplicationStaticData.moneyEarned +
                            questionOptionDetail.questionOptionMoneyAssoicated;

                        String finalText = questionOptionDetail.questionOptionEvaluation + "\n" + "Your solution is worth $ "
                            + questionOptionDetail.questionOptionMoneyAssoicated;

                        QuestionSolutionExplanationText.Text = finalText;

                        break;
                    }

                }

            }
            // option D checked
            else if (optionD.IsChecked.Value)
            {
                foreach (QuestionOptionDetails questionOptionDetail in questionOptionDetails)
                {
                    if (questionOptionDetail.caseStudyOptionNumber == 4)
                    {
                        ApplicationStaticData.moneyEarned = ApplicationStaticData.moneyEarned +
                            questionOptionDetail.questionOptionMoneyAssoicated;

                        String finalText = questionOptionDetail.questionOptionEvaluation + "\n" + "Your solution is worth $ "
                                        + questionOptionDetail.questionOptionMoneyAssoicated;

                        QuestionSolutionExplanationText.Text = finalText;

                        break;
                    }

                }

            }

            // control the visibilities of grid
            ContinueButtonGrid.Visibility = System.Windows.Visibility.Visible;
            SubmitButtonGrid.Visibility = System.Windows.Visibility.Collapsed;
            
        }

        /// <summary>
        /// if the user has answered the question, this button will take him back to play area, where he has to continue ahead.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void continueToGame_Click(object sender, RoutedEventArgs e)
        {
            // navigate to play grid
            NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Handling of back pressed on the hardware device, if the user has already answered the question then take him back to play
        /// area else take him back to question, so that he can read again
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // based on visibilites of grid, it can be concluded if user has already answered or not
            if (ContinueButtonGrid.Visibility.Equals(System.Windows.Visibility.Visible))
            {
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/QuestionDescription.xaml", UriKind.RelativeOrAbsolute));
            }

        }

        /// <summary>
        /// As we are handling the back pressed event on our own , we have to make sure that back stack is cleared on navigation
        /// to every page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // clears back stack
            this.NavigationService.RemoveBackEntry();
        }
    }
}