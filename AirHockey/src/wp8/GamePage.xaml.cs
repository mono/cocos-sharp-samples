using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;

namespace AirHockey.WP8
{
    public partial class GamePage : PhoneApplicationPage
    {

        // Constructor
        public GamePage()
        {
            InitializeComponent();
            CocosSharp.CCApplication.Create(new AppDelegate(), "", this);
           
        }
     
    }
}