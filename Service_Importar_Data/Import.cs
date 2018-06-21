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
using PrestaShopUpd;

namespace Service_Importar_Data
{
    public partial class Import : ServiceBase
    {
        Timer tmservicio = null;

        Timer tmservivio_ecc = null;
        Timer tmservivio_pres = null;

        private Int32 _valida_service = 0;

        private Int32 _valida_service_ecc = 0;

        private Int32 _valida_service_pres = 0;
        public Import()
        {
            InitializeComponent();
            /*servicio de aquarella*/
            tmservicio = new Timer(10000);
            tmservicio.Elapsed += new ElapsedEventHandler(tmpServicio_Elapsed);

            /*servicio para ecommerce*/
            tmservivio_ecc = new Timer(10000);
            tmservivio_ecc.Elapsed += new ElapsedEventHandler(tmservivio_ecc_Elapsed);

            /*servivio de prestashop*/
            //tmservivio_pres = new Timer(10000);
            //tmservivio_pres.Elapsed += new ElapsedEventHandler(tmservivio_pres_Elapsed);

        }
        #region<evendo de prestashop>
        void tmservivio_pres_Elapsed(object sender, ElapsedEventArgs e)
        {
            string _error_tarea = "";
            Int32 _valor = 0;
            try
            {
                //verificarsi es el servicio se esta ejeutando
                if (_valida_service_pres == 0)
                {
                    _valor = 1;
                    _valida_service_pres = 1;
                    string _error = "";
                    //ejecutar prstashop

                    ActStock eje = new ActStock();
                    _error = eje.EjecutaStock();

                    //Importar_Data.ejecutatarea_ecc(ref _error, ref _error_tarea);
                    //********************
                    //si es que hay un error entonces grabamos el error en tabla del sql
                    if (_error.Trim().Length > 0)
                    {
                        Importar_Data.insertar_error_service_ec(_error);
                    }

                    //una vez se haya realizado las importaciones
                    //setear la tabla en cero
                    _valida_service_pres = 0;

                }
                //****************************************************************************

            }
            catch (Exception ex)
            {
                _valida_service_pres = 0;
                _error_tarea += "===>>" + ex.Message;
                if (_error_tarea.Trim().Length > 0)
                {
                    Importar_Data.insertar_error_service_ec(_error_tarea);
                }
                _valida_service_pres = 0;

            }
            if (_valor == 1)
            {
                _valida_service_pres = 0;
            }
        }
        #endregion
        #region<evento de ecommerce>
        void tmservivio_ecc_Elapsed(object sender, ElapsedEventArgs e)
        {
            string _error_tarea = "";
            Int32 _valor = 0;
            try
            {
                //verificarsi es el servicio se esta ejeutando
                if (_valida_service_ecc == 0)               
                {
                    _valor = 1;
                    _valida_service_ecc = 1;                                 
                    string _error = "";
                    //ejecutar la importacion de data
                    Importar_Data.ejecutatarea_ecc(ref _error, ref _error_tarea);
                    //********************
                    //si es que hay un error entonces grabamos el error en tabla del sql
                    if (_error_tarea.Trim().Length > 0)
                    {
                        Importar_Data.insertar_error_service_ec(_error_tarea);
                    }

                    //una vez se haya realizado las importaciones
                    //setear la tabla en cero
                    _valida_service_ecc = 0;
                            
                }
                //****************************************************************************

            }
            catch (Exception ex)
            {
                _valida_service_ecc = 0;
                _error_tarea += "===>>" + ex.Message;
                if (_error_tarea.Trim().Length > 0)
                {
                    Importar_Data.insertar_error_service(_error_tarea);
                }
                _valida_service_ecc = 0;
                             
            }
            if (_valor == 1)
            {
                _valida_service_ecc = 0;              
            }
        }
        #endregion

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
            tmservivio_ecc.Start();
            //tmservivio_pres.Start();
        }

        protected override void OnStop()
        {
            tmservicio.Stop();
            tmservivio_ecc.Stop();
            //tmservivio_pres.Stop();
        }
    }
}
