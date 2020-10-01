using Ascon.Pilot.DataClasses;
using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamarin_HelloApp.AppContext
{
    /// <summary>
    /// Фабрика типов Pilot
    /// </summary>
    static class TypeImages
    {
        /// <summary>
        /// Словарь пиктограмм типов
        /// </summary>
        private static Dictionary<int, SvgImageSource> _typeImages = new Dictionary<int, SvgImageSource>();


        /// <summary>
        /// Получить пиктограмму типа по ID типа
        /// </summary>
        /// <param name="id"></param>
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