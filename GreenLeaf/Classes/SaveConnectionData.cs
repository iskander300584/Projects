using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenLeaf.Classes
{
    [Serializable]
    public class SaveConnectionData
    {
        public string Server = string.Empty;

        public string DB = string.Empty;

        public string AdminLogin = string.Empty;

        public string AdminPassword = string.Empty;
    }
}
