using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TourismAppV2.Dialogs.CustomDialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingDialog : PopupPage
    {
        public LoadingDialog(string message = null)
        {
            InitializeComponent();
            loadingText.Text = string.IsNullOrEmpty(message) ? "Processing..." : message;
        }
    }
}