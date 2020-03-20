using GreenLeaf.Classes;
using GreenLeaf.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GreenLeaf.Windows.AdminPanel
{
    /// <summary>
    /// Interaction logic for AdminAccountWindow.xaml
    /// </summary>
    public partial class AdminAccountWindow : Window
    {
        #region Поля класса

        /// <summary>
        /// Контекст данных окна
        /// </summary>
        private AdminContext context = new AdminContext();

        #endregion

        /// <summary>
        /// Окно добавления пользователя
        /// </summary>
        public AdminAccountWindow()
        {
            InitializeComponent();

            context.CurrentAccount = new Account();

            this.DataContext = context;
        }

        /// <summary>
        /// Окно отображения данных пользователя
        /// </summary>
        /// <param name="mode">режим работы окна</param>
        /// <param name="account">пользователь</param>
        public AdminAccountWindow(WindowMode mode, Account account)
        {
            InitializeComponent();

            context.CurrentAccount = account;

            context.CurrentAccount.GetFullDataByID();

            context.Mode = mode;

            this.DataContext = context;
        }

        
    }
}
