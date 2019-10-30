using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.Classes.AccountData
{
    /// <summary>
    /// Данные о доступе к накладным
    /// </summary>
    public class InvoiceData : INotifyPropertyChanged
    {
        private bool _purchaseInvoice = false;
        /// <summary>
        /// Признак доступности создания приходных накладных
        /// </summary>
        public bool PurchaseInvoice
        {
            get { return _purchaseInvoice; }
            set
            {
                if (_purchaseInvoice != value)
                {
                    _purchaseInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _salesInvoice = false;
        /// <summary>
        /// Признак доступности создания расходных накладных
        /// </summary>
        public bool SalesInvoice
        {
            get { return _salesInvoice; }
            set
            {
                if (_salesInvoice != value)
                {
                    _salesInvoice = value;
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
