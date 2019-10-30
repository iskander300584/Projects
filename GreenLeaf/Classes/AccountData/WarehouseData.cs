using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.Classes.AccountData
{
    /// <summary>
    /// Данные о доступе к управлению складом
    /// </summary>
    public class WarehouseData : INotifyPropertyChanged
    {
        private bool _warehouse = false;
        /// <summary>
        /// Признак доступности управления складом
        /// </summary>
        public bool Warehouse
        {
            get { return _warehouse; }
            set
            {
                if (_warehouse != value)
                {
                    _warehouse = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseAddProduct = false;
        /// <summary>
        /// Признак доступности добавления вида товара
        /// </summary>
        public bool WarehouseAddProduct
        {
            get { return _warehouseAddProduct; }
            set
            {
                if (_warehouseAddProduct != value)
                {
                    _warehouseAddProduct = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseEditProduct = false;
        /// <summary>
        /// Признак доступности редактирования вида товара
        /// </summary>
        public bool WarehouseEditProduct
        {
            get { return _warehouseEditProduct; }
            set
            {
                if (_warehouseEditProduct != value)
                {
                    _warehouseEditProduct = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseAnnulateProduct = false;
        /// <summary>
        /// Признак доступности аннулирования вида товара
        /// </summary>
        public bool WarehouseAnnulateProduct
        {
            get { return _warehouseAnnulateProduct; }
            set
            {
                if (_warehouseAnnulateProduct != value)
                {
                    _warehouseAnnulateProduct = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _warehouseEditCount = false;
        /// <summary>
        /// Признак доступности редактирования количества товара
        /// </summary>
        public bool WarehouseEditCount
        {
            get { return _warehouseEditCount; }
            set
            {
                if (_warehouseEditCount != value)
                {
                    _warehouseEditCount = value;
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
