using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    class SQLData : Helpers
    {
        public int Write_RCPT_IMPORT_Header(List<Rcpt> lstRcpt)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from RCPT_IMPORT_tbl_Header";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into IMPORT
                    cmd.CommandText = "INSERT INTO RCPT_IMPORT_tbl_Header (rcpt_no) VALUES (@arg1)";

                    cmd.Parameters.Add("@arg1", SqlDbType.Int);

                    foreach (Rcpt s in lstRcpt)
                    {
                        cmd.Parameters[0].Value = Convert.ToInt32(s.rcpt_no);                      

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't lave a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into IMPORT
                    cmd.CommandText = "SELECT COUNT(rcpt_no) from RCPT_IMPORT_tbl_Header";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        public string Get_New_Rcpts_String()
        {
            /* 
             * Get New Rcpts from IMPORT_tbl_Header that are NOT in WIP_tbl_Items.  They are new.
             * Build a string from the values in the recordset
             */ 
            string strRcpt = "";

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                cmd.CommandText = "RCPT_LKU_proc_New_Rcpts";

                SqlDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            strRcpt = strRcpt + rdr["rcpt_no"].ToString() + ",";
                        }
                    }                 
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            // Trim the comma on the end, if the string is not empty
            if (strRcpt.Length != 0)
                strRcpt = strRcpt.TrimEnd(strRcpt[strRcpt.Length - 1]);

            return strRcpt;
        }

        public int Write_Items_IMPORT(List<Items> lstItems)
        {
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from RCPT_IMPORT_tbl_Items";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into IMPORT
                    cmd.CommandText = @"INSERT INTO RCPT_IMPORT_tbl_Items(cus_ven_id,rcvg_whs,rcpt_dt,rcpt_no,rcpt_itm,
                                        ven_shp_ref,crr_vcl_desc,po_pfx,po_no,po_itm,wdth,bgt_as_part,
                                        tot_shpnt_wgt,pps_ctl_no,itm_ctl_no,tag_no,mill,mill_id,due_fm_dt,due_to_dt) " +
                                        "VALUES (@arg1,@arg2,@arg3,@arg4,@arg5,@arg6,@arg7,@arg8,@arg9,@arg10,@arg11," +
                                        "@arg12,@arg13,@arg14,@arg15,@arg16,@arg17,@arg18,@arg19,@arg20)";

                    cmd.Parameters.Add("@arg1", SqlDbType.Int);
                    cmd.Parameters.Add("@arg2", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg3", SqlDbType.DateTime);
                    cmd.Parameters.Add("@arg4", SqlDbType.Int);
                    cmd.Parameters.Add("@arg5", SqlDbType.Int);
                    cmd.Parameters.Add("@arg6", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg7", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg8", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg9", SqlDbType.Int);
                    cmd.Parameters.Add("@arg10", SqlDbType.Int);
                    cmd.Parameters.Add("@arg11", SqlDbType.Decimal);
                    cmd.Parameters.Add("@arg12", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg13", SqlDbType.Int);
                    cmd.Parameters.Add("@arg14", SqlDbType.Int);
                    cmd.Parameters.Add("@arg15", SqlDbType.Int);
                    cmd.Parameters.Add("@arg16", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg17", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg18", SqlDbType.VarChar);
                    cmd.Parameters.Add("@arg19", SqlDbType.DateTime);
                    cmd.Parameters.Add("@arg20", SqlDbType.DateTime);

                    foreach (Items s in lstItems)
                    {
                        cmd.Parameters[0].Value = Convert.ToInt32(s.cus_ven_id);
                        cmd.Parameters[1].Value = s.rcvg_whs.ToString();
                        cmd.Parameters[2].Value = Convert.ToDateTime(s.rcpt_dt);
                        cmd.Parameters[3].Value = Convert.ToInt32(s.rcpt_no);
                        cmd.Parameters[4].Value = Convert.ToInt32(s.rcpt_itm);
                        cmd.Parameters[5].Value = s.ven_shp_ref.ToString();
                        cmd.Parameters[6].Value = s.crr_vcl_desc.ToString();
                        cmd.Parameters[7].Value = s.po_pfx.ToString();
                        cmd.Parameters[8].Value = Convert.ToInt32(s.po_no);
                        cmd.Parameters[9].Value = Convert.ToInt32(s.po_itm);
                        cmd.Parameters[10].Value = Convert.ToDecimal(s.wdth);
                        cmd.Parameters[11].Value = s.bgt_as_part.ToString();
                        cmd.Parameters[12].Value = Convert.ToInt32(s.tot_shpnt_wgt);
                        cmd.Parameters[13].Value = Convert.ToInt32(s.pps_ctl_no);
                        cmd.Parameters[14].Value = Convert.ToInt32(s.itm_ctl_no);
                        cmd.Parameters[15].Value = s.tag_no.ToString();
                        cmd.Parameters[16].Value = s.mill.ToString();
                        cmd.Parameters[17].Value = s.mill_id.ToString();
                        cmd.Parameters[18].Value = s.due_fm_dt;
                        cmd.Parameters[19].Value = s.due_to_dt;

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't lave a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }

                /*
                 * Get numbers of rows in IMPORT.  This IMPORT table has an rID, otherwise
                 * each row would not contain a unique value to count
                 */
                try
                {
                    cmd.CommandText = "select count(rID) from RCPT_IMPORT_tbl_Items";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        // Insert values from IMPORT into WIP Items
        public int Write_IMPORT_to_Items()
        {
           // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy IMPORT to IPTFRC table.  Return rows inserted.
                cmd.CommandText = "RCPT_proc_IMPORT_to_Items";
              
                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        } 
    }
}
