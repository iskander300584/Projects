using System;
using System.Collections.Generic;
using System.Text;

namespace PilotMobile.AppContext
{
    /// <summary>
    /// Константы заданий Pilot
    /// </summary>
    public static class TaskConstants
    {
        /// <summary>
        /// Имя атрибута "Заголовок"
        /// </summary>
        public const string TitleAttribute = "title";


        /// <summary>
        /// Имя атрибута "Состояние"
        /// </summary>
        public const string StateAttribute = "state";


        /// <summary>
        /// Имя атрибута "Ответственные"
        /// </summary>
        public const string ResponsibleAttribute = "responsible";


        /// <summary>
        /// Имя атрибута "Инициатор"
        /// </summary>
        public const string InitiatorAttribute = "initiator";


        /// <summary>
        /// Имя атрибута "Исполнитель"
        /// </summary>
        public const string ExecutorAttribute = "executor";


        /// <summary>
        /// Имя атрибута "Срок до"
        /// </summary>
        public const string DeadlineAttribute = "deadlineDate";


        /// <summary>
        /// Имя атрибута "Аудиторы"
        /// </summary>
        public const string AuditorsAttribute = "auditors";


        /// <summary>
        /// Имя атрибута "Актуально для"
        /// </summary>
        public const string ActualAttribute = "actualFor";


        /// <summary>
        /// Наименование состояния "Отозвано"
        /// </summary>
        public const string RevokedState = "revoked";
    }
}