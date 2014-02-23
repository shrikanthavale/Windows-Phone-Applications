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
using Sales_Management.WebserviceAccess;
using Sales_Management.Entities;

namespace Sales_Management
{
    public partial class QuestionDescription : PhoneApplicationPage
    {
        public QuestionDescription()
        {
            // initalize the components
            InitializeComponent();

            // fetch the data using webservice call
            PopulateDataWebservice();

        }

        private async void PopulateDataWebservice()
        {
            // get the visited customer
            String visitedCustomer = ApplicationStaticData.currentNodeVisit;

            // web service call 
            ProgressIndicator progressIndicator = new ProgressIndicator()
            {
                IsVisible = true,
                IsIndeterminate = false,
                Text = "Loading Question Details ... Please Wait"
            };

            SystemTray.SetProgressIndicator(this, progressIndicator);

            QuestionDetails questionDetails = await WebServiceReadData.GetSingleQuestionDetails(visitedCustomer);

            questionName.Text = questionDetails.caseStudyNode;
            questionTitle.Text = questionDetails.caseStudyTitle;
            questionOrganization.Text = questionDetails.caseStudyOrganization;
            questionDescription.Text = questionDetails.caseStudyDescription;

            progressIndicator.IsVisible = false;

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure? Going back without answering will fetch you no points", "No Answer", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
            }

        }

        private void solutionsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/QuestionSolution.xaml", UriKind.RelativeOrAbsolute));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure? Going back without answering will fetch you no points", "No Answer", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
        }

    }
}