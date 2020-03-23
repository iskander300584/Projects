using GreenLeaf.Classes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GreenLeaf.ViewModel
{
    /// <summary>
    /// Контекст данных окна администрирования пользователя
    /// </summary>
    public class AdminContext : INotifyPropertyChanged
    {
        private Account _currentAccount;
        /// <summary>
        /// Пользователь
        /// </summary>
        public Account CurrentAccount
        {
            get { return _currentAccount; }
            set
            {
                if(_currentAccount != value)
                {
                    _currentAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private WindowMode _mode = WindowMode.Create;
        /// <summary>
        /// Режим работы окна
        /// </summary>
        public WindowMode Mode
        {
            get { return _mode; }
            set
            {
                if(_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged();

                    SetControlsEnabled();
                }
            }
        }

        private bool _isReadOnly = false;
        /// <summary>
        /// Доступность объектов
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        private bool _isEnabled = true;
        /// <summary>
        /// Доступность ComboBox
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
        }

        private Visibility _editVisibility = Visibility.Collapsed;
        /// <summary>
        /// Доступность кнопки "Редактировать"
        /// </summary>
        public Visibility EditVisibility
        {
            get { return _editVisibility; }
        }

        private Visibility _annulateVisibility = Visibility.Collapsed;
        /// <summary>
        /// Доступность кнопки "Аннулировать"
        /// </summary>
        public Visibility AnnulateVisibility
        {
            get { return _annulateVisibility; }
        }

        private Visibility _applyVisibility = Visibility.Visible;
        /// <summary>
        /// Доступность кнопки "Применить"
        /// </summary>
        public Visibility ApplyVisibility
        {
            get { return _applyVisibility; }
        }

        private Visibility _cancelVisibility = Visibility.Visible;
        /// <summary>
        /// Доступность кнопки "Отмена"
        /// </summary>
        public Visibility CancelVisibility
        {
            get { return _cancelVisibility; }
        }

        /// <summary>
        /// Настроить свойства доступа к элементам интерфейса
        /// </summary>
        private void SetControlsEnabled()
        {
            switch(_mode)
            {
                case WindowMode.Edit:
                case WindowMode.Create:
                    _isEnabled = true;
                    _isReadOnly = false;
                    _applyVisibility = Visibility.Visible;
                    _cancelVisibility = Visibility.Visible;
                    _annulateVisibility = Visibility.Collapsed;
                    _editVisibility = Visibility.Collapsed;
                    break;

                case WindowMode.Read:
                    _isEnabled = false;
                    _isReadOnly = true;
                    _applyVisibility = Visibility.Collapsed;
                    _cancelVisibility = Visibility.Collapsed;
                    _annulateVisibility = Visibility.Visible;
                    _editVisibility = Visibility.Visible;
                    break;
            }

            OnPropertyChanged("IsReadOnly");
            OnPropertyChanged("IsEnabled");
            OnPropertyChanged("EditVisibility");
            OnPropertyChanged("AnnulateVisibility");
            OnPropertyChanged("ApplyVisibility");
            OnPropertyChanged("CancelVisibility");
        }

        // Изменение свойств объекта
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}