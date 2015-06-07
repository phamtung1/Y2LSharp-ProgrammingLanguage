using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Y2LCore.SyntaxTree;

namespace Y2LCore
{
    public class CodeIO
    {
        #region Save/Load

        public static void Save(Module module, string fileName)
        {

            Stream stream = File.Open(fileName, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, module);
            stream.Close();

        }
        public static Module Load(string fileName)
        {
            try
            {
                Module data = null;

                Stream stream = File.Open(fileName, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();

                data = (Module)bformatter.Deserialize(stream);
                stream.Close();
                return data;
            }
            catch { return null; }

        }

        #endregion
    }
}
