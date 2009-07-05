using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Prey
{
    class CryptoRSA
    {
        //private string unid = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
        private static string dirDatos = Directory.GetCurrentDirectory();
        private static string ficKeys = Path.Combine(dirDatos+"\\.prey", "lib_rsa.priv.xml");


        private static void crearXMLClaves(String fichero)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string xmlKey = rsa.ToXmlString(true);

            //Si no existe el directorio, crearlo
            string dirDatos = Path.GetDirectoryName(fichero);

            if (Directory.Exists(dirDatos) == false)
            {
                Directory.CreateDirectory(dirDatos);
            }

            //textBox3.Text = xmlKey;


            using (StreamWriter sw = new StreamWriter(fichero, false, Encoding.UTF8))
            {
                sw.WriteLine(xmlKey);
                sw.Close();
            }
        }

        private static string clavesXML(String ficher)
        {
            string s;

            using (StreamReader sr = new StreamReader(ficher, Encoding.UTF8))
            {
                s = sr.ReadToEnd();
                sr.Close();
            }

            return s;
        }

        private static byte[] cifrar(String texto, string xmlKys)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlKys);

            byte[] datosEnc = rsa.Encrypt(Encoding.Default.GetBytes(texto), false);

            return datosEnc;

        }

        private static string descifrar(byte[] datosEnc, string xmlKeys)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlKeys);
            byte[] datos = rsa.Decrypt(datosEnc, false);

            string res = Encoding.Default.GetString(datos);

            return res;
        }
    }
}
