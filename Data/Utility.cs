using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExtTDG.Data
{
    public class Utility
    {
        public static void SetDefaultRowData(DataGridView dgv)
        {
            dgv.Rows.Add(9);

            // Default values for Name
            dgv.Rows[0].Cells[0].Value = true;
            dgv.Rows[0].Cells[1].Value = DataClassType.Name;
            dgv.Rows[0].Cells[2].Value = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ-'";
            dgv.Rows[0].Cells[3].Value = "!#¤%&()?/;.:,_<>|@£${[]}*";
            dgv.Rows[0].Cells[4].Value = "1";
            dgv.Rows[0].Cells[5].Value = "8";
            dgv.Rows[0].Cells[6].Value = true;
            dgv.Rows[0].Cells[7].Value = true;

            // Default values for Int32
            dgv.Rows[1].Cells[0].Value = true;
            dgv.Rows[1].Cells[1].Value = DataClassType.Int32;
            dgv.Rows[1].Cells[2].Value = "0123456789";
            dgv.Rows[1].Cells[3].Value = "-+";
            dgv.Rows[1].Cells[4].Value = "1";
            dgv.Rows[1].Cells[5].Value = "99999";
            dgv.Rows[1].Cells[6].Value = true;
            dgv.Rows[1].Cells[7].Value = true;

            // Default values for Email
            dgv.Rows[2].Cells[0].Value = true;
            dgv.Rows[2].Cells[1].Value = DataClassType.Email;
            dgv.Rows[2].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz-@.";
            dgv.Rows[2].Cells[3].Value = "!#¤%&()?/;:,_<>|£${[]}*";
            dgv.Rows[2].Cells[4].Value = "3";
            dgv.Rows[2].Cells[5].Value = "30";
            dgv.Rows[2].Cells[6].Value = true;
            dgv.Rows[2].Cells[7].Value = true;

            // Default values for Date
            dgv.Rows[3].Cells[0].Value = true;
            dgv.Rows[3].Cells[1].Value = DataClassType.Date;
            dgv.Rows[3].Cells[2].Value = "0";
            dgv.Rows[3].Cells[3].Value = "abcdefghijklmnopqrstuvwxyz-/.";
            dgv.Rows[3].Cells[4].Value = "10000101";
            dgv.Rows[3].Cells[5].Value = "99991231";
            dgv.Rows[3].Cells[6].Value = true;
            dgv.Rows[3].Cells[7].Value = true;

            // Default values for Address
            dgv.Rows[4].Cells[0].Value = true;
            dgv.Rows[4].Cells[1].Value = DataClassType.Address;
            dgv.Rows[4].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz";
            dgv.Rows[4].Cells[3].Value = "!#&/";
            dgv.Rows[4].Cells[4].Value = "0";
            dgv.Rows[4].Cells[5].Value = "50";
            dgv.Rows[4].Cells[6].Value = true;
            dgv.Rows[4].Cells[7].Value = true;

            // Default values for Phone
            dgv.Rows[5].Cells[0].Value = true;
            dgv.Rows[5].Cells[1].Value = DataClassType.Phone;
            dgv.Rows[5].Cells[2].Value = "0123456789";
            dgv.Rows[5].Cells[3].Value = "abcdefghijklmnopqrstuvwxyz!";
            dgv.Rows[5].Cells[4].Value = "7";
            dgv.Rows[5].Cells[5].Value = "30";
            dgv.Rows[5].Cells[6].Value = true;
            dgv.Rows[5].Cells[7].Value = true;

            // Default values for URL
            dgv.Rows[6].Cells[0].Value = true;
            dgv.Rows[6].Cells[1].Value = DataClassType.URL;
            dgv.Rows[6].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz";
            dgv.Rows[6].Cells[3].Value = "!#()[]";
            dgv.Rows[6].Cells[4].Value = "7";
            dgv.Rows[6].Cells[5].Value = "30";
            dgv.Rows[6].Cells[6].Value = true;
            dgv.Rows[6].Cells[7].Value = true;

            // Default values for Phone
            dgv.Rows[7].Cells[0].Value = true;
            dgv.Rows[7].Cells[1].Value = DataClassType.ID;
            dgv.Rows[7].Cells[2].Value = "0123456789";
            dgv.Rows[7].Cells[3].Value = "abcdefghijklmnopqrstuvwxyz!";
            dgv.Rows[7].Cells[4].Value = "3";
            dgv.Rows[7].Cells[5].Value = "6";
            dgv.Rows[7].Cells[6].Value = true;
            dgv.Rows[7].Cells[7].Value = true;

            // Default values for URL
            dgv.Rows[8].Cells[0].Value = true;
            dgv.Rows[8].Cells[1].Value = DataClassType.String;
            dgv.Rows[8].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz";
            dgv.Rows[8].Cells[3].Value = "!#()[]";
            dgv.Rows[8].Cells[4].Value = "1";
            dgv.Rows[8].Cells[5].Value = "10";
            dgv.Rows[8].Cells[6].Value = true;
            dgv.Rows[8].Cells[7].Value = true;
        }
    }
}
