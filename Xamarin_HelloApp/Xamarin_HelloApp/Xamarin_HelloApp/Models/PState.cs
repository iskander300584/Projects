using Ascon.Pilot.DataClasses;
using FFImageLoading.Svg.Forms;
using System;
using System.IO;

namespace PilotMobile.Models
{
    /// <summary>
    /// Состояние
    /// </summary>
    public class PState
    {
        private Guid guid;
        /// <summary>
        /// GUID состояния
        /// </summary>
        public Guid Guid
        {
            get => guid;
        }


        private MUserState mUserState;
        /// <summary>
        /// Состояние Pilot
        /// </summary>
        public MUserState MUserState
        {
            get => mUserState;
        }


        private string name;
        /// <summary>
        /// Системное наименование состояния
        /// </summary>
        public string Name
        {
            get => name;
        }


        private SvgImageSource imageSource = null;
        /// <summary>
        /// Источник изображения
        /// </summary>
        public SvgImageSource ImageSource
        {
            get => imageSource;
        }


        /// <summary>
        /// Состояние
        /// </summary>
        /// <param name="state">состояние Pilot</param>
        public PState(MUserState state)
        {
            mUserState = state;

            guid = state.Id;

            name = state.Name;

            // Получение пиктограммы состояния
            if (state.Icon != null)
            {
                imageSource = SvgImageSource.FromStream(() => new MemoryStream(state.Icon));
            }
        }
    }
}