using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.Classes.AccountData
{
    /// <summary>
    /// Данные о доступе к контрагентам
    /// </summary>
    public class CounterpartyData : INotifyPropertyChanged
    {
        private bool _counterparty = false;
        /// <summary>
        /// Признак доступности управления контрагентами
        /// </summary>
        public bool Counterparty
        {
            get { return _counterparty; }
            set
            {
                if (_counterparty != value)
                {
                    _counterparty = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProvider = false;
        /// <summary>
        /// Признак доступности управления поставщиками
        /// </summary>
        public bool CounterpartyProvider
        {
            get { return _counterpartyProvider; }
            set
            {
                if (_counterpartyProvider != value)
                {
                    _counterpartyProvider = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProviderAdd = false;
        /// <summary>
        /// Признак доступности добавления поставщика
        /// </summary>
        public bool CounterpartyProviderAdd
        {
            get { return _counterpartyProviderAdd; }
            set
            {
                if (_counterpartyProviderAdd != value)
                {
                    _counterpartyProviderAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProviderEdit = false;
        /// <summary>
        /// Признак доступности редактирования поставщика
        /// </summary>
        public bool CounterpartyProviderEdit
        {
            get { return _counterpartyProviderEdit; }
            set
            {
                if (_counterpartyProviderEdit != value)
                {
                    _counterpartyProviderEdit = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyProviderDelete = false;
        /// <summary>
        /// Признак доступности удаления поставщика
        /// </summary>
        public bool CounterpartyProviderDelete
        {
            get { return _counterpartyProviderDelete; }
            set
            {
                if (_counterpartyProviderDelete != value)
                {
                    _counterpartyProviderDelete = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomer = false;
        /// <summary>
        /// Признак доступности управления покупателями
        /// </summary>
        public bool CounterpartyCustomer
        {
            get { return _counterpartyCustomer; }
            set
            {
                if (_counterpartyCustomer != value)
                {
                    _counterpartyCustomer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomerAdd = false;
        /// <summary>
        /// Признак доступности добавления покупателя
        /// </summary>
        public bool CounterpartyCustomerAdd
        {
            get { return _counterpartyCustomerAdd; }
            set
            {
                if (_counterpartyCustomerAdd != value)
                {
                    _counterpartyCustomerAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomerEdit = false;
        /// <summary>
        /// Признак доступности редактирования покупателя
        /// </summary>
        public bool CounterpartyCustomerEdit
        {
            get { return _counterpartyCustomerEdit; }
            set
            {
                if (_counterpartyCustomerEdit != value)
                {
                    _counterpartyCustomerEdit = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _counterpartyCustomerDelete = false;
        /// <summary>
        /// Признак доступности удаления покупателя
        /// </summary>
        public bool CounterpartyCustomerDelete
        {
            get { return _counterpartyCustomerDelete; }
            set
            {
                if (_counterpartyCustomerDelete != value)
                {
                    _counterpartyCustomerDelete = value;
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
