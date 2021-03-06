﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestaShopUpd
{
    public class ActStock
    {
        SqlConnection sql;
        Conexion oConexion = new Conexion();

        MySqlConnection mysql;
        Conexion oConexionMySql = new Conexion();


        private string tienda = "11";
        private string error = "";
        private Int32 cant_reg=0;

        private DataTable ListaStocks(string tienda)
        {
            SqlDataAdapter da = new SqlDataAdapter("USP_ECOM_LISTASTOCK", sql);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
            DataTable dt = new DataTable();

            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public void ActualizaOrigen(string tienda, string mov_id, string detmov_id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "USP_ECOM_ACTSTOCK";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sql;

            cmd.Parameters.Add("@tienda", SqlDbType.VarChar).Value = tienda;
            cmd.Parameters.Add("@mov_id", SqlDbType.VarChar).Value = mov_id;
            cmd.Parameters.Add("@det_mov_id", SqlDbType.VarChar).Value = detmov_id;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ControlaTrans(int estado)
        {
            try
            {
                if (estado == 0)    // INICIA
                {
                    SqlCommand tranSQL = new SqlCommand("BEGIN TRAN STOCK;", sql);
                    tranSQL.ExecuteNonQuery();

                    MySqlCommand tranMySQL = mysql.CreateCommand();
                    tranMySQL.CommandText = "BEGIN;";
                    tranMySQL.ExecuteNonQuery();
                }
                if (estado == 1)    // FINALIZA OK 
                {
                    SqlCommand tranSQL = new SqlCommand("COMMIT TRAN STOCK;", sql);
                    tranSQL.ExecuteNonQuery();

                    MySqlCommand tranMySQL = mysql.CreateCommand();
                    tranMySQL.CommandText = "COMMIT;";
                    tranMySQL.ExecuteNonQuery();
                }
                if (estado == 2)    // ERROR ENCONTRADO
                {
                    SqlCommand tranSQL = new SqlCommand("ROLLBACK TRAN STOCK;", sql);
                    tranSQL.ExecuteNonQuery();

                    MySqlCommand tranMySQL = mysql.CreateCommand();
                    tranMySQL.CommandText = "ROLLBACK;";
                    tranMySQL.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizaStocks(DataTable precios)
        {
            //Valida con Productos Existentes
            //string queryvalida = "Select Distinct replace(replace(reference,'-',''),'_','') as reference From ps_product_attribute";
            string queryvalida = "CALL USP_EXISTE_PROD();";
            MySqlCommand cmd = new MySqlCommand(queryvalida, mysql);
            MySqlDataAdapter returnVal = new MySqlDataAdapter(queryvalida, mysql);
            DataTable Productos = new DataTable();
            try
            {
                returnVal.Fill(Productos);
            }
            catch (Exception ex)
            {
                error = "No se pudo obtener datos de productos de Prestashop.";
                throw ex;
            }

            //Crea Datatable con datos Finales
            DataTable Final = new DataTable();
            Final = precios.Clone();
            Final.Clear();

            //Validacion
            string val;
            foreach (DataRow row1 in precios.Rows)
            {
                val = "no";
                foreach (DataRow row2 in Productos.Rows)
                {
                    if (row1["product_id"].ToString() == row2["reference"].ToString())
                    {
                        val = "si";
                        Final.ImportRow(row1);
                        Productos.Rows.Remove(row2);
                        break;
                    }
                }
                if (val == "no")
                {
                    error = "No se encontró producto " + row1["product_id"] + " en Prestashop.";
                    //throw new System.ArgumentException("Código de Producto Inválido", "reference");
                }
            }

            //Elimino datos anteriores
            MySqlCommand comdel = mysql.CreateCommand();
            comdel.CommandText = "Delete From ps_erp;";
            try
            {
                comdel.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                error = "No se pudo limpiar la tabla ps_erp";
                throw ex;
            }

            string id_mov = "";
            //recorro datatable final
            foreach (DataRow row in Final.Rows)
            {
                id_mov = row["mov_id"].ToString();
                //Actualizar en Prestashop
                MySqlCommand comm = mysql.CreateCommand();
                comm.CommandText = "Insert into ps_erp (ref_product, stock) values ('" + row["product_id"] + "'," + row["cantidad"] + ");";

                try
                {
                    //ejecucion
                    comm.ExecuteNonQuery();
                    //Actualiza estado para no repetir
                    ActualizaOrigen(tienda, row["mov_id"].ToString(), row["det_mov_id"].ToString());
                }
                catch (Exception ex)
                {
                    error = "No se pudo insertar datos en la tabla ps_erp.";
                    throw ex;
                }
                cant_reg = cant_reg + 1;
            }

        }


        public string EjecutaStock()
        {
            try
            {
                sql = oConexion.getConexionSQL();
                sql.Open();

                mysql = oConexionMySql.getConexionMySQL();
                mysql.Open();

                ControlaTrans(0);
            }
            catch (Exception ex)
            {
                return "No se pudo abrir Transaccion. // " + ex.Message;
            }

            try
            {
                DataTable tabla = new DataTable();

                tabla = ListaStocks(tienda);

                ActualizaStocks(tabla);
            }
            catch (Exception ex)
            {
                ControlaTrans(2);
                sql.Close();
                mysql.Close();
                return "Error: " + error + " // " + ex.Message;
            }
          
            ControlaTrans(1);
            sql.Close();
            mysql.Close();
            return "";
        }

        //public static void Main()
        //{
        //    ActStock exe = new ActStock();
        //    exe.EjecutaStock();
        //}


    }
}
