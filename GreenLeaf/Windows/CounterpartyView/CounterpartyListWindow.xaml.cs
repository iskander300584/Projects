using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenLeaf.ViewModel;
using GreenLeaf.Classes;

namespace GreenLeaf.Windows.CounterpartyView
{
    /// <summary>
    /// Список контрагентов
    /// </summary>
    public partial class CounterpartyListWindow : Window
    {
        #region Объявление команд

        /// <summary>
        /// Обновить
        /// </summary>
        public static RoutedUICommand Refresh = new RoutedUICommand("Обновить", "Refresh", typeof(CounterpartyListWindow));

        /// <summary>
        /// Открыть карточку
        /// </summary>
        public static RoutedUICommand OpenCard = new RoutedUICommand("Открыть", "OpenCard", typeof(CounterpartyListWindow));

        /// <summary>
        /// Редактировать контрагента
        /// </summary>
        public static RoutedUICommand EditCounterparty = new RoutedUICommand("Редактировать", "EditCounterparty", typeof(CounterpartyListWindow));

        /// <summary>
        /// Аннулировать контрагента
        /// </summary>
        public static RoutedUICommand AnnulateCounterparty = new RoutedUICommand("Аннулировать", "AnnulateCounterparty", typeof(CounterpartyListWindow));

        /// <summary>
        /// Сброс фильтров поиска
        /// </summary>
        public static RoutedUICommand ResetFilter = new RoutedUICommand("Сбросить фильтры поиска", "ResetFilter", typeof(CounterpartyListWindow));

        #endregion

        #region Поля класса

        /// <summary>
        /// Признак отображения списка поставщиков
        /// </summary>
        private bool ShowProviders = false;

        #endregion

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

            if(showProviders)
            {
                btnAddCounterparty.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderAdd;
            }
            else
            {
                btnAddCounterparty.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerAdd;
            }

            GetData();

            dataGrid.ItemsSource = CounterpartyList;
        }

        #region Поиск

        /// <summary>
        /// Получение данных
        /// </summary>
        private void GetData()
        {
            dataGrid.ItemsSource = null;

            CounterpartyList = (ShowProviders) ? Counterparty.GetProviderList() : Counterparty.GetCustomerList();
        }

        /// <summary>
        /// Ввод Enter в фильтры поиска
        /// </summary>
        private void SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                SearchText_Changed(null, null);
        }

        /// <summary>
        /// Ввод текста в фильтр поиска
        /// </summary>
        private void SearchText_Changed(object sender, TextChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            GetData();

            string nomination = tbSearchNomination.Text.Trim();
            string code = tbSearchCode.Text.Trim();

            for (int i = 0; i < CounterpartyList.Count;)
            {
                if (nomination != "" && code == "")
                {
                    if ((!CounterpartyList[i].Nomination.Contains(nomination) && !CounterpartyList[i].Surname.Contains(nomination) && !CounterpartyList[i].Name.Contains(nomination)))
                        CounterpartyList.Remove(CounterpartyList[i]);
                    else
                        i++;
                }
                else if (nomination == "" && code != "")
                {
                    if (!CounterpartyList[i].Code.Contains(code))
                        CounterpartyList.Remove(CounterpartyList[i]);
                    else
                        i++;
                }
                else if (nomination != "" && code != "")
                {
                    if ((!CounterpartyList[i].Nomination.Contains(nomination) && !CounterpartyList[i].Surname.Contains(nomination) && !CounterpartyList[i].Name.Contains(nomination)) || !CounterpartyList[i].Code.Contains(code))
                        CounterpartyList.Remove(CounterpartyList[i]);
                    else
                        i++;
                }
                else
                    break;
            }

            dataGrid.ItemsSource = CounterpartyList;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Проверка возможности сброса фильтров поиска
        /// </summary>
        private void ResetFilter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tbSearchCode != null && tbSearchNomination != null && tbSearchCode.Text != "" && tbSearchNomination.Text != "") ? true : false;
        }

        /// <summary>
        /// Сброс фильтров поиска
        /// </summary>
        private void ResetFilter_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            tbSearchCode.TextChanged -= SearchText_Changed;
            tbSearchNomination.TextChanged -= SearchText_Changed;

            tbSearchCode.Text = "";
            tbSearchNomination.Text = "";

            tbSearchCode.TextChanged += SearchText_Changed;
            tbSearchNomination.TextChanged += SearchText_Changed;

            GetData();

            dataGrid.ItemsSource = CounterpartyList;

            Mouse.OverrideCursor = null;
        }

        #endregion

        /// <summary>
        /// Кнопка "Обновить данные"
        /// </summary>
        private void Refresh_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SearchText_Changed(null, null);
        }

        /// <summary>
        /// Создание контрагента
        /// </summary>
        private void AddCounterparty_Click(object sender, RoutedEventArgs e)
        {
            CounterpartyWindow view = new CounterpartyWindow(ShowProviders);
            view.Owner = this;

            view.ShowDialog();

            SearchText_Changed(null, null);
        }

        #region Открыть карточку

        /// <summary>
        /// Проверка возможности открытия карточки
        /// </summary>
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (dataGrid != null && dataGrid.SelectedItem != null) ? true : false;
        }

        /// <summary>
        /// Открыть карточку
        /// </summary>
        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Counterparty counterparty = (Counterparty)dataGrid.SelectedItem;

            CounterpartyWindow view = new CounterpartyWindow(WindowMode.Read, counterparty);
            view.Owner = this;

            view.ShowDialog();

            dataGrid.Items.Refresh();
        }

        /// <summary>
        /// Двойной клик по контрагенту
        /// </summary>
        private void dgCounterparties_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Counterparty counterparty = (Counterparty)dataGrid.SelectedItem;

            CounterpartyWindow view = new CounterpartyWindow(WindowMode.Read, counterparty);
            view.Owner = this;

            view.ShowDialog();

            dataGrid.Items.Refresh();
        }

        #endregion

        /// <summary>
        /// Проверка возможности редактировать контрагента
        /// </summary>
        private void EditCounterparty_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataGrid == null || dataGrid.SelectedItem == null)
                e.CanExecute = false;
            else
            {
                if (ShowProviders)
                    e.CanExecute = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderEdit;
                else
                    e.CanExecute = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerEdit;
            }
        }

        /// <summary>
        /// Редактирование контрагента
        /// </summary>
        private void EditCounterparty_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Counterparty counterparty = (Counterparty)dataGrid.SelectedItem;

            CounterpartyWindow view = new CounterpartyWindow(WindowMode.Edit, counterparty);
            view.Owner = this;

            view.ShowDialog();

            dataGrid.Items.Refresh();
        }

        /// <summary>
        /// Проверка возможности аннулирования контрагента
        /// </summary>
        private void AnnulateCounterparty_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataGrid == null || dataGrid.SelectedItem == null)
                e.CanExecute = false;
            else
            {
                if (ShowProviders)
                    e.CanExecute = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderDelete;
                else
                    e.CanExecute = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerDelete;
            }
        }

        /// <summary>
        /// Аннулирование контрагента
        /// </summary>
        private void AnnulateCounterparty_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dialog.QuestionMessage(this, "Аннулировать контрагента\nВ дальнейшем его использование будет невозможно") == MessageBoxResult.Yes)
            {
                Counterparty counterparty = (Counterparty)dataGrid.SelectedItem;

                if (counterparty.AnnuateCounterparty())
                {
                    Dialog.TransparentMessage(this, "Контрагент аннулирован");

                    SearchText_Changed(null, null);
                }
            }
        }
    }
}