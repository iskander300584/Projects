using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.Classes.Account
{
    /// <summary>
    /// Данные о доступе к отчетам
    /// </summary>
    public class ReportsData : INotifyPropertyChanged
    {
        private bool _reports = false;
        /// <summary>
        /// Признак доступности отчетов
        /// </summary>
        public bool Reports
        {
            get { return _reports; }
            set
            {
                if (_reports != value)
                {
                    _reports = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reportPurchaseInvoice = false;
        /// <summary>
        /// Признак доступности отчетов по приходным накладным
        /// </summary>
        public bool ReportPurchaseInvoice
        {
            get { return _reportPurchaseInvoice; }
            set
            {
                if (_reportPurchaseInvoice != value)
                {
                    _reportPurchaseInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reportSalesInvoice = false;
        /// <summary>
        /// Признак доступности отчетов по расходным накладным
        /// </summary>
        public bool ReportSalesInvoice
        {
            get { return _reportSalesInvoice; }
            set
            {
                if (_reportSalesInvoice != value)
                {
                    _reportSalesInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _reportIncomeExpense = false;
        /// <summary>
        /// Признак доступности отчетов приход-расход
        /// </summary>
        public bool ReportIncomeExpense
        {
            get { return _reportIncomeExpense; }
            set
            {
                if (_reportIncomeExpense != value)
                {
                    _reportIncomeExpense = value;
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
