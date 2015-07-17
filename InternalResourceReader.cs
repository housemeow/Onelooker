using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kelly
{
    class InternalResourceReader
    {
        private Assembly _assembly;
        public InternalResourceReader(Assembly assembly)
        {
            this._assembly = assembly;
        }

        public String ReadAllText(string resourceName)
        {
            using (Stream stream = _assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
