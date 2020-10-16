using PilotMobile.ViewContexts;
using PilotMobile.ViewModels;
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
    /// Страница карточки объекта
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardPage : ContentPage
    {
        /// <summary>
        /// Контекст данных страницы
        /// </summary>
        private CardPage_Context context;


        /// <summary>
        /// Страница карточки объекта
        /// </summary>
        /// <param name="pilotItem">элемент Pilot</param>
        public CardPage(IPilotObject pilotItem)
        {
            InitializeComponent();

            context = new CardPage_Context(pilotItem, this);

            this.BindingContext = context;
        }


        /// <summary>
        /// Переход на основную страницу приложения
        /// </summary>
        public void NavigateToMainPage()
        {
            Navigation.PopModalAsync();
        }
    }
}