using Ascon.Pilot.DataClasses;
using PilotMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin_HelloApp.AppContext;

namespace PilotMobile.AppContext
{
    /// <summary>
    /// Фабрика состояний
    /// </summary>
    static class StateFabrique
    {
        /// <summary>
        /// Список состояний
        /// </summary>
        private static List<PState> _states = new List<PState>();


        /// <summary>
        /// Получить состояние
        /// </summary>
        /// <param name="guid">GUID состояния</param>
        /// <returns>возвращает состояние или NULL, если состояние не найдено</returns>
        public static PState GetState(Guid guid)
        {
            PState state = _states.FirstOrDefault(s => s.Guid == guid);

            if (state != null)
                return state;

            List<MUserState> states = Global.DALContext.Repository.GetStates();
            MUserState mUserState = states.FirstOrDefault(s => s.Id == guid);
            if (mUserState == null)
            {
                return null;
            }

            state = new PState(mUserState);
            _states.Add(state);

            return state;
        }
    }
}