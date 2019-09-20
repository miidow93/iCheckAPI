using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI
{
    public class Settings: ISettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ISettings
    {
         string ConnectionString { get; set; }

         string DatabaseName { get; set; }
    }
}
