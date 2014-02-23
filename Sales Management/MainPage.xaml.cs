using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sales_Management.Resources;

namespace Sales_Management
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PlayGrid.xaml", UriKind.RelativeOrAbsolute));
        }

         //Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/ApplicationIcon.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
        }

        private void adminButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Administration.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}