using PilotMobile.ViewContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PilotMobile.Pages
{
    /// <summary>
    /// Окно списка заданий
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        private TasksPage_Context context;

        /// <summary>
        /// Окно списка заданий
        /// </summary>
        public TasksPage()
        {
            InitializeComponent();

            context = new TasksPage_Context();

            this.BindingContext = context;
        }
    }
}