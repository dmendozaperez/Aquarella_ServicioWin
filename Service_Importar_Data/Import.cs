using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Basico;
namespace Service_Importar_Data
{
    public partial class Import : ServiceBase
    {
        Timer tmservicio = null;
        private Int32 _valida_service = 0;
        public Import()
        {
            InitializeComponent();
            tmservicio = new Timer(10000);
            tmservicio.Elapsed += new ElapsedEventHandler(tmpServicio_Elapsed);
        }
        void tmpServicio_Elapsed(object sender,ElapsedEventArgs e)
        {
            string _error_tarea = "";
            Int32 _valor = 0;
            try
            {
                //verificarsi es el servicio se esta ejeutando
                 if (_valida_service==0)
                //(Importar_Data._get_estado_servicio()==0)
                {
                    _valor = 1;
                    _valida_service = 1;
                    //activar servicio
                    //Importar_Data.actualiza_servicio(1);                   
                    string _error = "";
                    //ejecutar la importacion de data
                    Importar_Data.ejecutatarea(ref _error,ref _error_tarea);
                    //********************
                    //si es que hay un error entonces grabamos el error en tabla del sql
                    if (_error_tarea.Trim().Length>0)
                    {
                        Importar_Data.insertar_error_service(_error_tarea);
                    }

                    //una vez se haya realizado las importaciones
                    //setear la tabla en cero
                    _valida_service = 0;
                    //Importar_Data.actualiza_servicio(0);                   
                }                                
                //****************************************************************************

            }
            catch (Exception ex)
            {
                _valida_service = 0;
                _error_tarea += "===>>" + ex.Message;
                if (_error_tarea.Trim().Length > 0)
                {
                    Importar_Data.insertar_error_service(_error_tarea);
                }
                _valida_service = 0;
                //Importar_Data.actualiza_servicio(0);                   
            }
            if (_valor == 1)
            {
                //if (System.IO.File.Exists(varchivov))
                //{
                _valida_service = 0;
                //System.IO.File.Delete(varchivov);
                //}   
            }
        }
        protected override void OnStart(string[] args)
        {
            tmservicio.Start();
        }

        protected override void OnStop()
        {
            tmservicio.Stop();
        }
    }
}
