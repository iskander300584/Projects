﻿using Ascon.Pilot.DataClasses;
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
        private static Dictionary<int, SvgImageSource> _typeImages = new Dictionary<int, SvgImageSource>();


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
        /// Получить пиктограмму типа по ID типа
        /// </summary>
        /// <param name="id">ID типа</param>
        /// <returns></returns>
        public static SvgImageSource GetImageSource(int id)
        {
            if (_typeImages.Keys.Contains(id))
                return _typeImages[id];
            else
            {
                MType type = Global.DALContext.Repository.GetType(id);

                if (type.Icon != null)
                {
                    SvgImageSource imageSource = SvgImageSource.FromStream(() => new MemoryStream(type.Icon));

                    _typeImages.Add(id, imageSource);

                    return imageSource;
                }

                return null;
            }
        }
    }
}