using Ascon.Pilot.DataClasses;
using FFImageLoading.Svg.Forms;
using System.Collections.Generic;
using System.IO;
using Xamarin_HelloApp.AppContext;

namespace Xamarin_HelloApp.Models
{
    /// <summary>
    /// Тип Pilot
    /// </summary>
    public class PType
    {
        private int id = 0;
        /// <summary>
        /// ID типа
        /// </summary>
        public int ID
        {
            get => id;
        }


        private MType mType;
        /// <summary>
        /// Тип объекта Pilot
        /// </summary>
        public MType MType
        {
            get => mType;
        }


        private string name = string.Empty;
        /// <summary>
        /// Системное имя атрибута
        /// </summary>
        public string Name
        {
            get => name;
        }


        private string visibleName = string.Empty;
        /// <summary>
        /// Отображаемое имя атрибута
        /// </summary>
        public string VisibleName
        {
            get => visibleName;
        }


        private SvgImageSource imageSource = null;
        /// <summary>
        /// Источник изображения
        /// </summary>
        public SvgImageSource ImageSource
        {
            get => imageSource;
        }


        private bool isSystem = false;
        /// <summary>
        /// Является служебным
        /// </summary>
        public bool IsSystem
        {
            get => isSystem;
        }


        private bool isDocument = false;
        /// <summary>
        /// Является документом
        /// </summary>
        public bool IsDocument
        {
            get => isDocument;
        }


        private PAttribute[] attributes;
        /// <summary>
        /// Атрибуты типа
        /// </summary>
        public PAttribute[] Attributes
        {
            get => attributes;
        }


        /// <summary>
        /// Тип Pilot
        /// </summary>
        /// <param name="id">ID типа</param>
        public PType(int id)
        {
            this.id = id;

            MType type = Global.DALContext.Repository.GetType(id);

            mType = type;

            // Получение пиктограммы типа
            if (type.Icon != null)
            {
                imageSource = SvgImageSource.FromStream(() => new MemoryStream(type.Icon));
            }

            name = type.Name;

            visibleName = type.Title;

            isSystem = (type.Kind != TypeKind.User);

            isDocument = type.HasFiles;

            // Получение списка атрибутов
            List<PAttribute> _attrs = new List<PAttribute>();
            foreach(MAttribute attr in type.Attributes)
            {
                _attrs.Add(new PAttribute(attr, this));
            }
            attributes = _attrs.ToArray();
        }
    }
}