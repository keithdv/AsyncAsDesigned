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
        public Token(int id)
        {
            this.ID = id;
        }

        public readonly int ID;
        public readonly string AppServerToClient = Guid.NewGuid().ToString();
        public readonly string DataServerToAppServer = Guid.NewGuid().ToString();

        public int AppServerID { get; set; }

    }
}
