using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAsDesigned.PerfLib
{
    [Serializable]
    public class Token
    {
        public Token(int uniqueID, int total)
        {
            this.UniqueID = uniqueID;
            this.Total = total;
        }
        public readonly int UniqueID;
        public readonly int Total;
        public readonly string AppServerToClient = Guid.NewGuid().ToString();
        public readonly string DataServerToAppServer = Guid.NewGuid().ToString();

    }
}
