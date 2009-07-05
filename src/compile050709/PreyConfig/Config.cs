using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Prey;

namespace PreyConfig
{
    public partial class Config : Form
    {
        Configuracion configuracionPrey;
        public Config()
        {
            InitializeComponent();
            this.Load += new EventHandler(Config_Load);
        }

        void btnActivar_Click(object sender, EventArgs e)
        {
            configuracionPrey.URLActivacion = tbURLActivacion.Text;
            configuracionPrey.IntervaloMonitoreo = (int)nudMonitoreo.Value;
            configuracionPrey.CorreoElectronico = tbCorreoElectronico.Text;
            configuracionPrey.ServidorSMTP = tbServSMTP.Text;
            configuracionPrey.UsuarioSMTP = tbUsuario.Text;
            configuracionPrey.PuertoSMTP = int.Parse(tbPuertoSMTP.Text);
            configuracionPrey.EsSSL = cbSSL.Checked;
            if (tbClave.Text != "")
                configuracionPrey.ClaveSMTP = tbClave.Text;
            configuracionPrey.GuardarConfiguracion();
            MessageBox.Show("La configuración de Prey se ha realizado correctamente.", "Configuración de Prey", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        void Config_Load(object sender, EventArgs e)
        {
            configuracionPrey = Configuracion.ObtenerConfiguracionActual();
            tbURLActivacion.Text = configuracionPrey.URLActivacion;
            nudMonitoreo.Value = (decimal)configuracionPrey.IntervaloMonitoreo;
            if (configuracionPrey.RutaPreyAgent != "")
                cbActivarPrey.Checked = true;
            tbCorreoElectronico.Text = configuracionPrey.CorreoElectronico;
            tbServSMTP.Text = configuracionPrey.ServidorSMTP;
            tbUsuario.Text = configuracionPrey.UsuarioSMTP;
            cbSSL.Checked = configuracionPrey.EsSSL;
            tbPuertoSMTP.Text = configuracionPrey.PuertoSMTP.ToString();
            cbActivarPrey.CheckedChanged += new EventHandler(cbActivarPrey_CheckedChanged);
            btnActivar.Click += new EventHandler(btnActivar_Click);
        }

        private void cbActivarPrey_CheckedChanged(object sender, EventArgs e)
        {
            if (cbActivarPrey.Checked)
            {
                this.serviceController1.Start();
            }
            else
            {
                this.serviceController1.Pause();
            }
        }
    }
}
