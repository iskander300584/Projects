using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.Classes.Account
{
    /// <summary>
    /// Данные о доступе к административной панели
    /// </summary>
    public class AdminPanelData : INotifyPropertyChanged
    {
        private bool _adminPanel = false;
        /// <summary>
        /// Признак доступности панели администратора
        /// </summary>
        public bool AdminPanel
        {
            get { return _adminPanel; }
            set
            {
                if (_adminPanel != value)
                {
                    _adminPanel = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelAddAccount = false;
        /// <summary>
        /// Признак доступности добавления пользователя
        /// </summary>
        public bool AdminPanelAddAccount
        {
            get { return _adminPanelAddAccount; }
            set
            {
                if (_adminPanelAddAccount != value)
                {
                    _adminPanelAddAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelEditAccount = false;
        /// <summary>
        /// Признак доступности редактирования пользователя
        /// </summary>
        public bool AdminPanelEditAccount
        {
            get { return _adminPanelEditAccount; }
            set
            {
                if (_adminPanelEditAccount != value)
                {
                    _adminPanelEditAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelDeleteAccount = false;
        /// <summary>
        /// Признак доступности удаления пользователя
        /// </summary>
        public bool AdminPanelDeleteAccount
        {
            get { return _adminPanelDeleteAccount; }
            set
            {
                if (_adminPanelDeleteAccount != value)
                {
                    _adminPanelDeleteAccount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelSetNumerator = false;
        /// <summary>
        /// Признак доступности установки значения нумератора
        /// </summary>
        public bool AdminPanelSetNumerator
        {
            get { return _adminPanelSetNumerator; }
            set
            {
                if (_adminPanelSetNumerator != value)
                {
                    _adminPanelSetNumerator = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _adminPanelJournal = false;
        /// <summary>
        /// Признак доступности просмотра журнала событий
        /// </summary>
        public bool AdminPanelJournal
        {
            get { return _adminPanelJournal; }
            set
            {
                if (_adminPanelJournal != value)
                {
                    _adminPanelJournal = value;
                    OnPropertyChanged();
                }
            }
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
