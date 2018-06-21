using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basico;
using PrestaShopUpd;

namespace Aplication_Import
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string _error_tarea = "";
            try
            {
                //verificarsi es el servicio se esta ejeutando
                //if (Importar_Data._get_estado_servicio() == 0)
                //{
                //activar servicio
                //Importar_Data.actualiza_servicio(1);
                string _error = "";
                //ActStock eje = new ActStock();
                //string _error = eje.EjecutaStock();// ActStock.EjecutaStock();

                //MessageBox.Show(_error);

                //ejecutar la importacion de data
                //Importar_Data.ejecutatarea(ref _error, ref _error_tarea);
                Importar_Data.ejecutatarea_ecc(ref _error, ref _error_tarea);
                //Importar_Data.ejecutatarea(ref _error, ref _error_tarea);
                //********************
                //si es que hay un error entonces grabamos el error en tabla del sql
                //if (_error_tarea.Trim().Length > 0)
                //{
                //    Importar_Data.insertar_error_service(_error_tarea);
                //}

                //una vez se haya realizado las importaciones
                //setear la tabla en cero
                //Importar_Data.actualiza_servicio(0);
                //}
                //****************************************************************************

            }
            catch (Exception ex)
            {
                _error_tarea += "===>>" + ex.Message;
                //if (_error_tarea.Trim().Length > 0)
                //{
                //    Importar_Data.insertar_error_service(_error_tarea);
                //}
                //Importar_Data.actualiza_servicio(0);
            }
        }
    }
}
