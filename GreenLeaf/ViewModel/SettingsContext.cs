using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.ViewModel
{
    public class SettingsContext : INotifyPropertyChanged
    {
        private int _numeratorPurchase_ID = 0;
        /// <summary>
        /// ID нумератора приходных накладных
        /// </summary>
        public int NumeratorPurchase_ID
        {
            get { return _numeratorPurchase_ID; }
            set
            {
                if(_numeratorPurchase_ID != value)
                {
                    _numeratorPurchase_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _numeratorPurchase_Value = 0;
        /// <summary>
        /// Значение нумератора приходных накладных
        /// </summary>
        public int NumeratorPurchase_Value
        {
            get { return _numeratorPurchase_Value; }
            set
            {
                if(_numeratorPurchase_Value != value)
                {
                    _numeratorPurchase_Value = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _numeratorSales_ID = 0;
        /// <summary>
        /// ID нумератора расходных накладных
        /// </summary>
        public int NumeratorSales_ID
        {
            get { return _numeratorSales_ID; }
            set
            {
                if(_numeratorSales_ID != value)
                {
                    _numeratorSales_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _numeratorSales_Value = 0;
        /// <summary>
        /// Значение нумератора расходных накладных
        /// </summary>
        public int NumeratorSales_Value
        {
            get { return _numeratorSales_Value; }
            set
            {
                if(_numeratorSales_Value != value)
                {
                    _numeratorSales_Value = value;
                    OnPropertyChanged();
                }
            }
        }

        private IDictionary<string, string> _settingsCollection = null;
        /// <summary>
        /// Коллекция настроек программы
        /// </summary>
        public IDictionary<string, string> SettingsCollection
        {
            get { return _settingsCollection; }
            set
            {
                if(_settingsCollection != value)
                {
                    _settingsCollection = value;
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