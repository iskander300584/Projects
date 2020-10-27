using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PilotMobile.Models.SearchQuery
{
    /// <summary>
    /// Элемент поискового запроса
    /// </summary>
    abstract class ISearchQueryItem : INotifyPropertyChanged
    {
        protected string name;
        /// <summary>
        /// Наименование типа/атрибута
        /// </summary>
        public string Name
        {
            get => name;
            protected set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged();

                    GetStringValue();
                }
            }
        }


        protected string value;
        /// <summary>
        /// Значение типа/атрибута
        /// </summary>
        public string Value
        {
            get => value;
            set
            {
                if(this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged();

                    GetStringValue();
                }
            }
        }


        protected string stringValue;
        /// <summary>
        /// Строковое значение поискового запроса
        /// </summary>
        public string StringValue
        {
            get => stringValue;
        }


        protected bool isType = false;
        /// <summary>
        /// Признак, что элемент является типом
        /// </summary>
        public bool IsType
        {
            get => isType;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// Получение строкового значения поискового запроса
        /// </summary>
        protected abstract void GetStringValue();


        /// <summary>
        /// Получение строкового значения поискового запроса
        /// </summary>
        public override string ToString()
        {
            return StringValue;
        }
    }
}