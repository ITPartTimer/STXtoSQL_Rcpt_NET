using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<Rcpt> Get_Rcpts()
        {

            List<Rcpt> lstRcpt = new List<Rcpt>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);

            try
            {
                conn.Open();

                OdbcCommand cmd = conn.CreateCommand();

                // Get receipt no for all purchase pre-receipt records
                cmd.CommandText = "select rch_rcpt_no from trtrch_rec where rch_rcpt_pfx='PR' and rch_rcpt_typ='P'";

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        Rcpt b = new Rcpt();

                        b.rcpt_no = Convert.ToInt32(rdr["rch_rcpt_no"]);                    

                        lstRcpt.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return lstRcpt;
        }

        public List<Items> Get_Items(string strRcpt)
        {

            List<Items> lstItems = new List<Items>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);

            try
            {
                conn.Open();

                OdbcCommand cmd = conn.CreateCommand();

                // Get receipt no for all PO pre-receipt records where rcpt_no is in a list
                cmd.CommandText = @"select rch_cus_ven_id,rch_rcvg_whs,rch_rcpt_dt,rci_rcpt_pfx,rci_rcpt_no,rci_rcpt_itm,rch_ven_shp_ref,
                                    rch_crr_vcl_desc,rci_po_pfx,rci_po_no,rci_po_itm,rci_wdth,rci_bgt_as_part,rcd_shpnt_wgt,rci_tot_shpnt_wgt,
                                    rci_pps_ctl_no,rcd_itm_ctl_no,prd_tag_no,prd_mill,pcr_mill_id,por_due_fm_dt,por_due_to_dt
                                    from trtrci_rec rci
                                    inner join trtrch_rec rch
                                    on rci.rci_rcpt_no = rch.rch_rcpt_no
                                    inner join trtrcd_rec rcd
                                    on rci.rci_rcpt_no + rci.rci_rcpt_itm = rcd.rcd_rcpt_no + rcd.rcd_rcpt_itm
                                    inner join intprd_rec prd
                                    on rcd.rcd_itm_ctl_no = prd.prd_itm_ctl_no
                                    inner join intpcr_rec pcr
                                    on prd.prd_itm_ctl_no = pcr.pcr_itm_ctl_no
                                    inner join potpor_rec por
                                    on rci.rci_po_no + rci.rci_po_itm = por.por_po_no + por.por_po_itm
                                    where rci_rcpt_no in (" + strRcpt + ")";

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        Items b = new Items();

                        b.cus_ven_id = Convert.ToInt32(rdr["rch_cus_ven_id"]);
                        b.rcvg_whs = rdr["rch_rcvg_whs"].ToString();
                        b.rcpt_dt = Convert.ToDateTime(rdr["rch_rcpt_dt"]);
                        b.rcpt_no = Convert.ToInt32(rdr["rci_rcpt_no"]);
                        b.rcpt_itm = Convert.ToInt32(rdr["rci_rcpt_itm"]);
                        b.ven_shp_ref = rdr["rch_ven_shp_ref"].ToString();
                        b.crr_vcl_desc = rdr["rch_crr_vcl_desc"].ToString();
                        b.po_pfx = rdr["rci_po_pfx"].ToString();
                        b.po_no = Convert.ToInt32(rdr["rci_po_no"]);
                        b.po_itm = Convert.ToInt32(rdr["rci_po_itm"]);
                        b.wdth = Convert.ToDecimal(rdr["rci_wdth"]);
                        b.bgt_as_part = rdr["rci_bgt_as_part"].ToString();
                        b.shpnt_wgt = Convert.ToInt32(rdr["rcd_shpnt_wgt"]);
                        b.tot_shpnt_wgt = Convert.ToInt32(rdr["rci_tot_shpnt_wgt"]);
                        b.pps_ctl_no = Convert.ToInt32(rdr["rci_pps_ctl_no"]);
                        b.itm_ctl_no = Convert.ToInt32(rdr["rcd_itm_ctl_no"]);
                        b.tag_no = rdr["prd_tag_no"].ToString();
                        b.mill = rdr["prd_mill"].ToString();
                        b.mill_id = rdr["pcr_mill_id"].ToString();
                        b.due_fm_dt = Convert.ToDateTime(rdr["por_due_fm_dt"]);
                        b.due_to_dt = Convert.ToDateTime(rdr["por_due_to_dt"]);

                        lstItems.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return lstItems;
        }
    }
}
