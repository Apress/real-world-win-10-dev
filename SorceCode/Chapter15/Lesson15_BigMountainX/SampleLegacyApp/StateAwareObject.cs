using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace General.UWP.Library
{
    public class StateAwareObject<T> where T : StateAwareObject<T>, new()
    {
        string _file_path;

        public string AsSerializedString()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            dcs.WriteObject(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[ms.Length];
            ms.Read(buffer, 0, buffer.Length);
            string serialized_object = Encoding.ASCII.GetString(buffer);
            return serialized_object;
        }



        public void Save()
        {
            var f = File.CreateText(_file_path);
            var serialized = this.AsSerializedString();
            f.Write(serialized);
            f.Flush();
            f.Close();
        }

        public void Delete()
        {
            File.Delete(_file_path);
        }

        public static T FromSerializedString(string resume_string)
        {
            try
            {
                var buffer = Encoding.ASCII.GetBytes(resume_string);
                MemoryStream ms = new MemoryStream(buffer);
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                var retval = dcs.ReadObject(ms);
                var app_state = retval as T;
                return app_state;
            }
            catch
            {
                return null;
            }
        }

        public static T FromFileAsync(string file_path)
        {
            T app_state = null;
            if (File.Exists(file_path))
            {
                var serialized = File.ReadAllText(file_path);
                app_state = FromSerializedString(serialized);

            }

            if (app_state == null)
                app_state = new T();
            app_state._file_path = file_path;

            return app_state;
        }
    }
}
