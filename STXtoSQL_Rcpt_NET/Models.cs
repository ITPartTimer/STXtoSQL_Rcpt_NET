using System;
using System.Collections.Generic;
using System.Text;


namespace STXtoSQL.Models
{
    public class Rcpt
    {
        public int rcpt_no { get; set; }
    }

    public class Items
    {

        public int cus_ven_id { get; set; }
        public string rcvg_whs { get; set; }
        public DateTime rcpt_dt { get; set; }
        public int rcpt_no { get; set; }
        public int rcpt_itm { get; set; }
        public string ven_shp_ref { get; set; }
        public string crr_vcl_desc { get; set; }
        public string po_pfx { get; set; }
        public int po_no { get; set; }
        public int po_itm { get; set; }
        public decimal wdth { get; set; }
        public string bgt_as_part { get; set; }
        public int tot_shpnt_wgt { get; set; }
        public int pps_ctl_no { get; set; }
        public int itm_ctl_no {get; set;}
        public string tag_no { get; set; }
        public string mill { get; set; }
        public string mill_id { get; set; }
        public DateTime due_fm_dt { get; set; }
        public DateTime due_to_dt { get; set; }
    }
}
