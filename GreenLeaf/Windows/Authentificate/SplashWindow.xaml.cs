using System;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace GreenLeaf.Windows.Authentificate
{
    /// <summary>
    /// Окно загрузки программы
    /// </summary>
    public partial class SplashWindow : Window
    {
        /// <summary>
        /// Окно загрузки программы
        /// </summary>
        /// <param name="department">Подразделение</param>
        /// <param name="developer">Разработчик</param>
        /// <param name="assembly">Сборка</param>
        public SplashWindow(string department, string developer, Assembly assembly)
        {
            InitializeComponent();

            tbDepartment.Text = department;
            tbDeveloper.Text = developer;
            AssemblyName asmName = assembly.GetName();
            tbVersion.Text = asmName.Version.Major + "." + asmName.Version.Minor + "." + asmName.Version.Revision;
            DateTime? dt = GetBuildDateTime(assembly);
            if (dt != null)
                tbDate.Text = ((DateTime)dt).Date.ToLongDateString();
        }

        /// <summary>
        /// Получить дату построения сборки.
        /// </summary>
        /// <param name="assembly">Сборка.</param>
        /// <returns>Дата-время сборки из COFF-заголовка.</returns>
        private DateTime GetBuildDateTime(Assembly assembly)
        {
            if (File.Exists(assembly.Location))
            {
                var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];
                using (var fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
                {
                    fileStream.Position = 0x3C;
                    fileStream.Read(buffer, 0, 4);
                    fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                    fileStream.Read(buffer, 0, 4); // "PE\0\0"
                    fileStream.Read(buffer, 0, buffer.Length);
                }

                var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));
                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond));
                }
                finally
                {
                    pinnedBuffer.Free();
                }
            }

            return new DateTime();
        }

        /// <summary>
        /// Structure that represents the COFF header format.
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms680313"/>.
        /// </summary>
        private struct _IMAGE_FILE_HEADER
        {
#pragma warning disable 0649

            /// <summary>
            /// The architecture type of the computer. An image file can only be run on the specified computer or a system 
            /// that emulates the specified computer.
            /// </summary>
            public ushort Machine;

            /// <summary>
            /// The number of sections. This indicates the size of the section table, which immediately follows the headers. 
            /// Note that the Windows loader limits the number of sections to 96.
            /// </summary>
            public ushort NumberOfSections;

            /// <summary>
            /// The low 32 bits of the time stamp of the image. This represents the date and time the image was created by 
            /// the linker. The value is represented in the number of seconds elapsed since midnight (00:00:00), January 1, 1970, 
            /// Universal Coordinated Time, according to the system clock.
            /// </summary>
            public uint TimeDateStamp;

            /// <summary>
            /// The offset of the symbol table, in bytes, or zero if no COFF symbol table exists.
            /// </summary>
            public uint PointerToSymbolTable;

            /// <summary>
            /// The number of symbols in the symbol table.
            /// </summary>
            public uint NumberOfSymbols;

            /// <summary>
            /// The size of the optional header, in bytes. This value should be 0 for object files.
            /// </summary>
            public ushort SizeOfOptionalHeader;

            /// <summary>
            /// The characteristics of the image. 
            /// </summary>
            public ushort Characteristics;

#pragma warning restore 0649
        }
    }
}
