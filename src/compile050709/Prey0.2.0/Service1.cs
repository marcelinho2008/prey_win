using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Net.Mail;
using WebCamera;
using Prey;


namespace PreyPW
{
    public partial class Service1 : ServiceBase
    {
               
        PictureBox pbWebCam;
        bool camaraCapturada = false;
        WebCam webC = new WebCam();
        Configuracion configuracionPrey;
        System.Timers.Timer monitor;
        
        public Service1()
        {
            InitializeComponent();
            //t = new System.Timers.Timer();
            //t.Interval = 10000;
            //t.Elapsed += new ElapsedEventHandler(t_Elapsed);



            pbWebCam = new PictureBox();
            monitor = new System.Timers.Timer();
            pbWebCam.Width = 640;
            pbWebCam.Height = 480;
            monitor.Elapsed += new ElapsedEventHandler(monitor_Tick);
            configuracionPrey = Configuracion.ObtenerConfiguracionActual();
            monitor.Interval = (int)configuracionPrey.IntervaloMonitoreo * 60000;
            
        }

        protected override void OnStart(string[] args)
        {
            //t.Start();

            monitor.Start();
        }

        protected override void OnStop()
        {

        }

        void monitor_Tick(object sender, EventArgs e)
        {
            monitor.Stop();
            try
            {
                if (SenalActivacion())
                {
                    //Show();
                    capturarCamaraWeb();
                    string logPrey;
                    string rutaLog = String.Format("{0}\\{1}", Environment.GetEnvironmentVariable("TEMP"), "prey.txt");
                    string rutaImgScr = String.Format("{0}\\{1}", Environment.GetEnvironmentVariable("TEMP"), "prey-screenshot.jpg");
                    string rutaWebScr = String.Format("{0}\\{1}", Environment.GetEnvironmentVariable("TEMP"), "prey-webcam.jpg");
                    logPrey = Prey.Prey.ObtenerInformacionSistema();
                    logPrey += Prey.Prey.ObtenerTareasActivasEquipo();
                    logPrey += Prey.Prey.ObtenerInformacionRed();
                    logPrey += Prey.Prey.ObternetInformacionWifi();
                    Prey.Prey.CapturarPantalla(rutaImgScr);
                    webC.SaveImage(rutaWebScr);
                    //Hide();
                    enviarPorCorreo(logPrey, rutaImgScr, rutaWebScr);
                }
            }
            catch { }
            finally
            {
                monitor.Start();
            }
        }

        private void enviarPorCorreo(string Log, string RutaScrImg, string RutaWebImg)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.To.Add(new MailAddress(configuracionPrey.CorreoElectronico));
                correo.From = new MailAddress(configuracionPrey.CorreoElectronico);
                correo.Subject = "Información Prey";
                correo.SubjectEncoding = Encoding.UTF8;
                correo.Body = Log;
                correo.BodyEncoding = Encoding.UTF8;
                correo.IsBodyHtml = false;
                correo.Attachments.Add(new Attachment(RutaScrImg));
                correo.Attachments.Add(new Attachment(RutaWebImg));
                SmtpClient smtpServ = new SmtpClient();
                smtpServ.Host = configuracionPrey.ServidorSMTP;
                smtpServ.Credentials = configuracionPrey.ObtenerCredenciales();
                smtpServ.EnableSsl = configuracionPrey.EsSSL;
                smtpServ.Port = configuracionPrey.PuertoSMTP;
                smtpServ.Send(correo);
            }
            catch { }
        }

        private bool SenalActivacion()
        {
            bool activador = false;
            string activacion;
            try
            {
                WebClient wc = new WebClient();
                activacion = Encoding.ASCII.GetString(wc.DownloadData(configuracionPrey.URLActivacion));
            }
            catch
            {
                activacion = "";
            }
            activador = (activacion != "") ? true : false;
            return activador;
        }

        private void capturarCamaraWeb()
        {
            try
            {
                if (!camaraCapturada)
                {
                    //this.Controls.Add(pbWebCam);
                    //webC.Container = (PictureBox)this.Controls[0];
                    webC.OpenConnection();
                    camaraCapturada = true;
                }
            }
            catch { }
        }

    }
}
