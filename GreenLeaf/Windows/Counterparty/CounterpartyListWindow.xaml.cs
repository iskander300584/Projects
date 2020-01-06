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
using GreenLeaf.ViewModel;

namespace GreenLeaf.Windows.CounterpartyView
{
    /// <summary>
    /// Список контрагентов
    /// </summary>
    public partial class CounterpartyListWindow : Window
    {
        /// <summary>
        /// Признак отображения списка поставщиков
        /// </summary>
        private bool ShowProviders = false;

        /// <summary>
        /// Список отображаемых контрагентов
        /// </summary>
        private List<Counterparty> CounterpartyList = new List<Counterparty>();

        /// <summary>
        /// Список контрагентов
        /// </summary>
        /// <param name="showProviders">отображать поставщиков</param>
        public CounterpartyListWindow(bool showProviders)
        {
            InitializeComponent();

            ShowProviders = showProviders;

            this.Title = (ShowProviders) ? "Список поставщиков" : "Список клиентов";

            RefreshData();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        private void RefreshData()
        {
            dataGrid.ItemsSource = null;

            // TODO

            dataGrid.ItemsSource = CounterpartyList;
        }

        //TODO
        private void SearchText_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
