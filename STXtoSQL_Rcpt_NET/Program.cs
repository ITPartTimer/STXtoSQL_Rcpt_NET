using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.Log;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;

namespace STXtoSQL_Rcpt_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * COPY IN CODE FROM IPTFRC PROJECT
             * 1. Put all Rcpts in IMPORT_tbl_Header
             * 2. Compare (1) to Rcts in WIP_tbl_Items
             * 3. ODBC query for Items using Rcpts from (2)
             * 4. Call SP to stamp and put new IMPORT items into WIP
             */
            Logger.LogWrite("MSG", "Start: " + DateTime.Now.ToString());

            // Declare and defaults
            int odbcCnt = 0;
            int importCnt = 0;
            int insertCnt = 0;
            int hdrCnt = 0;

            #region FromSTRATIX
            ODBCData objODBC = new ODBCData();

            List<Rcpt> lstRcpt = new List<Rcpt>();

            /*
             * Get data from Stratix
             * Returns list of receipt numbers
             */
            try
            {
                lstRcpt = objODBC.Get_Rcpts();
            }
            catch (Exception ex)
            {
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                return;
            }
            #endregion

            #region ToSQL
            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstRcpt.Count != 0)
            {
                odbcCnt = lstRcpt.Count;

                // Put receipts in lstRcpt into IMPORT Rcpt table
                try
                {
                    hdrCnt = objSQL.Write_RCPT_IMPORT_Header(lstRcpt);
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                /*
                 * Compare receipts in IMPORT_Header to WIP_Items.
                 * Return a string of receipts in Header NOT found 
                 * in Items.  Those are new.
                 */
                string strRcpts = "";

                try
                {
                    strRcpts = objSQL.Get_New_Rcpts_String();
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                // Only execute, if there are new pre-receipts
                if(strRcpts.Length != 0)
                {
                    /*
                * Get Receipt items from Stratix using strRcpts
                */
                    List<Items> lstItems = new List<Items>();

                    try
                    {
                        lstItems = objODBC.Get_Items(strRcpts);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWrite("EXC", ex);
                        Logger.LogWrite("MSG", "Return");
                        return;
                    }

                    // Write lstItems to IMPORT
                    try
                    {
                        importCnt = objSQL.Write_Items_IMPORT(lstItems);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWrite("EXC", ex);
                        Logger.LogWrite("MSG", "Return");
                        return;
                    }

                    // Call SP to put IMPORT Items data into WIP Items table
                    try
                    {
                        insertCnt = objSQL.Write_IMPORT_to_Items();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWrite("EXC", ex);
                        Logger.LogWrite("MSG", "Return");
                        return;
                    }
                }
               
                Logger.LogWrite("MSG", "ODBC/IMPORT/ITEMS=" + odbcCnt.ToString() + ":" + importCnt.ToString() + ":" + insertCnt.ToString());
            }
            else
                Logger.LogWrite("MSG", "No data");

            Logger.LogWrite("MSG", "End: " + DateTime.Now.ToString());
            #endregion

            // Testing
            //Console.WriteLine("Press key to exit");
            //Console.ReadKey();
        }
    }
}
