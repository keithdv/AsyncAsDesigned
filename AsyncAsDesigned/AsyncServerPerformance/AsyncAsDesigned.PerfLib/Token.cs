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

        public Token(bool end)
        {
            End = true;
        }

        public readonly int ID;
        public readonly bool End = false;

        // Incremental value for the AppServer to keep track of
        public int AppServerID { get; set; }

    }
}
