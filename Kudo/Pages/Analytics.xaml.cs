using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kudo.Pages;

public partial class Analytics : ContentPage
{
    
    public Analytics()
    {
        InitializeComponent();
    }
    
    private async void nextPage(object sender, EventArgs e)
    {
        this.Navigation.PushAsync(new MainPage());
    }
}