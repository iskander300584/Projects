using Ascon.Pilot.DataClasses;
using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin_HelloApp.Models;

namespace Xamarin_HelloApp.AppContext
{
    /// <summary>
    /// Фабрика типов Pilot
    /// </summary>
    static class TypeFabrique
    {
        /// <summary>
        /// Словарь типов Pilot
        /// </summary>
        private static Dictionary<int, PType> _types = new Dictionary<int, PType>();

        /// <summary>
        /// Словарь пиктограмм типов
        /// </summary>
        //private static Dictionary<int, SvgImageSource> _typeImages = new Dictionary<int, SvgImageSource>();


        /// <summary>
        /// Получить тип по ID
        /// </summary>
        /// <param name="id">ID типа</param>
        /// <returns>возвращает тип объекта или NULL в случае ошибки</returns>
        public static PType GetType(int id)
        {
            if (_types.Keys.Contains(id))
                return _types[id];
            else
            {
                try
                {
                    PType type = new PType(id);

                    _types.Add(id, type);

                    return type;
                }
                catch { }
            }

            return null;
        }


        /// <summary>
        /// Получение всех типов БД Pilot
        /// </summary>
        /// <returns>возвращает список всех типов</returns>
        public static List<PType> GetAllTypes()
        {
            List<PType> _list = new List<PType>();
            foreach (KeyValuePair<int, PType> pair in _types)
                _list.Add(pair.Value);

            return _list;
        }

        
        /// <summary>
        /// Очистить список типов
        /// </summary>
        public static void ClearTypes()
        {
            _types.Clear();
        }
    }
}