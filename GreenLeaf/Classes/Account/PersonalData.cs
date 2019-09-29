using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenLeaf.Classes.Account
{
    /// <summary>
    /// Персональные данные пользователя
    /// </summary>
    public class PersonalData : INotifyPropertyChanged
    {
        private string _surname = string.Empty;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname
        {
            get { return _surname; }
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }

        private string _name = string.Empty;
        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }

        private string _patronymic = string.Empty;
        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {
                if (_patronymic != value)
                {
                    _patronymic = value;
                    OnPropertyChanged();

                    GetVisibleName();
                }
            }
        }

        private string _visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string VisibleName
        {
            get { return _visibleName; }
        }

        private string _adress = string.Empty;
        /// <summary>
        /// Адрес
        /// </summary>
        public string Adress
        {
            get { return _adress; }
            set
            {
                if (_adress != value)
                {
                    _adress = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _phone = string.Empty;
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _sex = false;
        /// <summary>
        /// Пол (TRUE - мужской; FALSE - женский)
        /// </summary>
        public bool Sex
        {
            get { return _sex; }
            set
            {
                if (_sex != value)
                {
                    _sex = value;
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

        /// <summary>
        /// Получение отображаемого имени
        /// </summary>
        private void GetVisibleName()
        {
            if (_surname.Trim() != string.Empty)
            {
                _visibleName = _surname;

                if (_name.Trim() != "")
                {
                    _visibleName += " " + _name[0] + ".";

                    if (_patronymic.Trim() != "")
                        _visibleName += " " + _patronymic[0] + ".";
                }
            }
            else if (_name.Trim() != string.Empty)
            {
                _visibleName = _name;

                if (_patronymic.Trim() != string.Empty)
                    _visibleName += " " + _patronymic;
            }

            OnPropertyChanged("VisibleName");
        }
    }
}
