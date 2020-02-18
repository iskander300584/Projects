using System.Windows;
using GreenLeaf.Classes;
using GreenLeaf.ViewModel;

namespace GreenLeaf.Windows.CounterpartyView
{
    /// <summary>
    /// Окно данных контрагента
    /// </summary>
    public partial class CounterpartyWindow : Window
    {
        /// <summary>
        /// Режим запуска окна
        /// </summary>
        private WindowMode Mode = WindowMode.Read;

        /// <summary>
        /// Отображаемый контрагент
        /// </summary>
        private Counterparty CurrentCounterparty = null;

        /// <summary>
        /// Запуск окна создания контрагента
        /// </summary>
        /// <param name="isProvider">поставщик</param>
        public CounterpartyWindow(bool isProvider)
        {
            InitializeComponent();

            this.Mode = WindowMode.Create;

            this.CurrentCounterparty = new Counterparty();

            this.CurrentCounterparty.IsProvider = this.CurrentCounterparty.IsProvider;

            this.Title = (this.CurrentCounterparty.IsProvider) ? "Создание поставщика" : "Создание клиента";

            tbType.Text = (this.CurrentCounterparty.IsProvider) ? "Поставщик" : "Клиент";

            tbNomination.IsEnabled = this.CurrentCounterparty.IsProvider;

            if(this.CurrentCounterparty.IsProvider)
            {
                btnEdit.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderEdit;
                btnAnnulate.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderDelete;
            }
            else
            {
                btnEdit.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerEdit;
                btnAnnulate.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerDelete;
            }

            // Настройка доступности объектов интерфейса
            SetControlsEnabled();
        }

        /// <summary>
        /// Запуск окна контрагента для чтения или редактирования данных 
        /// </summary>
        public CounterpartyWindow(WindowMode mode, Counterparty counterparty)
        {
            InitializeComponent();

            this.Mode = mode;

            this.CurrentCounterparty = counterparty;

            if (this.CurrentCounterparty.IsProvider)
                this.Title = (this.Mode == WindowMode.Read) ? "Данные поставщика" : "Редактирование данных поставщика";
            else
                this.Title = (this.Mode == WindowMode.Read) ? "Данные клиента" : "Редактирование данных клиента";

            tbType.Text = (this.CurrentCounterparty.IsProvider) ? "Поставщик" : "Клиент";

            tbNomination.IsEnabled = this.CurrentCounterparty.IsProvider;

            if (this.CurrentCounterparty.IsProvider)
            {
                btnEdit.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderEdit;
                btnAnnulate.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyProviderDelete;
            }
            else
            {
                btnEdit.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerEdit;
                btnAnnulate.IsEnabled = ProgramSettings.CurrentUser.CounterpartyData.CounterpartyCustomerDelete;
            }

            // Настройка доступности объектов интерфейса
            SetControlsEnabled();

            // Заполнение данных
            LoadData();
        }

        /// <summary>
        /// Настройка доступности элементов интерфейса
        /// </summary>
        private void SetControlsEnabled()
        {
            if (this.Mode == WindowMode.Read)
            {
                btnCancel.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Collapsed;

                btnEdit.Visibility = Visibility.Visible;
                btnAnnulate.Visibility = Visibility.Visible;

                tbCode.IsReadOnly = true;
                tbNomination.IsReadOnly = true;
                tbSurname.IsReadOnly = true;
                tbName.IsReadOnly = true;
                tbPatronymic.IsReadOnly = true;
                tbAdress.IsReadOnly = true;
                tbPhone.IsReadOnly = true;
            }
            else
            {
                btnCancel.Visibility = Visibility.Visible;
                btnApply.Visibility = Visibility.Visible;

                btnEdit.Visibility = Visibility.Collapsed;
                btnAnnulate.Visibility = Visibility.Collapsed;

                tbCode.IsReadOnly = false;
                tbNomination.IsReadOnly = false;
                tbSurname.IsReadOnly = false;
                tbName.IsReadOnly = false;
                tbPatronymic.IsReadOnly = false;
                tbAdress.IsReadOnly = false;
                tbPhone.IsReadOnly = false;
            }
        }

        /// <summary>
        /// Загрузка данных контрагента на форму
        /// </summary>
        private void LoadData()
        {
            this.CurrentCounterparty.GetFullDataByID();

            tbCode.Text = this.CurrentCounterparty.Code;
            tbNomination.Text = this.CurrentCounterparty.Nomination;
            tbSurname.Text = this.CurrentCounterparty.Surname;
            tbName.Text = this.CurrentCounterparty.Name;
            tbPatronymic.Text = this.CurrentCounterparty.Patronymic;
            tbAdress.Text = this.CurrentCounterparty.Adress;
            tbPhone.Text = this.CurrentCounterparty.Phone;
        }

        /// <summary>
        /// Кнопка Применить
        /// </summary>
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentCounterparty.Code = tbCode.Text.Trim();
            this.CurrentCounterparty.Nomination = tbNomination.Text.Trim();
            this.CurrentCounterparty.Surname = tbSurname.Text.Trim();
            this.CurrentCounterparty.Name = tbName.Text.Trim();
            this.CurrentCounterparty.Patronymic = tbPatronymic.Text.Trim();
            this.CurrentCounterparty.Adress = tbAdress.Text.Trim();
            this.CurrentCounterparty.Phone = tbPhone.Text.Trim();

            if (this.Mode == WindowMode.Create)
            {
                if (this.CurrentCounterparty.CreateCounterparty())
                {
                    Dialog.TransparentMessage(this, "Данные успешно сохранены");

                    this.Mode = WindowMode.Read;

                    SetControlsEnabled();
                }
            }
            else
            {
                if (this.CurrentCounterparty.EditCounterparty())
                {
                    Dialog.TransparentMessage(this, "Данные успешно сохранены");

                    this.Mode = WindowMode.Read;

                    SetControlsEnabled();
                }
            }
        }

        /// <summary>
        /// Кнопка Отмена
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Mode == WindowMode.Create)
                this.Close();
            else
            {
                this.Mode = WindowMode.Read;

                SetControlsEnabled();

                LoadData();
            }
        }

        /// <summary>
        /// Кнопка Редактировать
        /// </summary>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            this.Mode = WindowMode.Edit;

            SetControlsEnabled();
        }

        /// <summary>
        /// Кнопка Аннулировать
        /// </summary>
        private void btnAnnulate_Click(object sender, RoutedEventArgs e)
        {
            if(Dialog.QuestionMessage(this, "Аннулировать контрагента\nВ дальнейшем его использование будет невозможно") == MessageBoxResult.Yes)
            {
                if(this.CurrentCounterparty.AnnuateCounterparty())
                {
                    Dialog.TransparentMessage(this, "Контрагент аннулирован");

                    btnEdit.IsEnabled = false;
                    btnAnnulate.IsEnabled = false;
                }
            }
        }
    }
}