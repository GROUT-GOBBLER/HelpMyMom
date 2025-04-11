using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        //string URL = $"http://localhost:5124/api";
        string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

        public MainPage()
        {
            InitializeComponent();
        }

       

       

        private void LoginButton_Clicked(object sender, EventArgs e)
        {

        }
    }

}
