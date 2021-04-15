using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExtTDG.Data
{
    public class Utility
    {
        public static void SetDefaultRowData(DataGridView dgv, DataClassRegistry dcr)
        {
            dgv.Rows.Add(9);

            // Default values for Name
            dgv.Rows[0].Cells[0].Value = true;
            dgv.Rows[0].Cells[1].Value = DataClassType.Name;
            dgv.Rows[0].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.Name);
            dgv.Rows[0].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.Name);
            dgv.Rows[0].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.Name);
            dgv.Rows[0].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.Name);
            dgv.Rows[0].Cells[6].Value = true;
            dgv.Rows[0].Cells[7].Value = true;

            // Default values for Int32
            dgv.Rows[1].Cells[0].Value = true;
            dgv.Rows[1].Cells[1].Value = DataClassType.Int32;
            dgv.Rows[1].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.Int32);
            dgv.Rows[1].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.Int32);
            dgv.Rows[1].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.Int32);
            dgv.Rows[1].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.Int32);
            dgv.Rows[1].Cells[6].Value = true;
            dgv.Rows[1].Cells[7].Value = true;

            // Default values for Email
            dgv.Rows[2].Cells[0].Value = true;
            dgv.Rows[2].Cells[1].Value = DataClassType.Email;
            dgv.Rows[2].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.Email);
            dgv.Rows[2].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.Email);
            dgv.Rows[2].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.Email);
            dgv.Rows[2].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.Email);
            dgv.Rows[2].Cells[6].Value = true;
            dgv.Rows[2].Cells[7].Value = true;

            // Default values for Date
            dgv.Rows[3].Cells[0].Value = true;
            dgv.Rows[3].Cells[1].Value = DataClassType.Date;
            dgv.Rows[3].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.Date);
            dgv.Rows[3].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.Date);
            dgv.Rows[3].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.Date);
            dgv.Rows[3].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.Date);
            dgv.Rows[3].Cells[6].Value = true;
            dgv.Rows[3].Cells[7].Value = true;

            // Default values for Address
            dgv.Rows[4].Cells[0].Value = true;
            dgv.Rows[4].Cells[1].Value = DataClassType.Address;
            dgv.Rows[4].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.Address);
            dgv.Rows[4].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.Address);
            dgv.Rows[4].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.Address);
            dgv.Rows[4].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.Address);
            dgv.Rows[4].Cells[6].Value = true;
            dgv.Rows[4].Cells[7].Value = true;

            // Default values for Phone
            dgv.Rows[5].Cells[0].Value = true;
            dgv.Rows[5].Cells[1].Value = DataClassType.Phone;
            dgv.Rows[5].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.Phone);
            dgv.Rows[5].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.Phone);
            dgv.Rows[5].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.Phone);
            dgv.Rows[5].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.Phone);
            dgv.Rows[5].Cells[6].Value = true;
            dgv.Rows[5].Cells[7].Value = true;

            // Default values for URL
            dgv.Rows[6].Cells[0].Value = true;
            dgv.Rows[6].Cells[1].Value = DataClassType.URL;
            dgv.Rows[6].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.URL);
            dgv.Rows[6].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.URL);
            dgv.Rows[6].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.URL);
            dgv.Rows[6].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.URL);
            dgv.Rows[6].Cells[6].Value = true;
            dgv.Rows[6].Cells[7].Value = true;

            // Default values for Phone
            dgv.Rows[7].Cells[0].Value = true;
            dgv.Rows[7].Cells[1].Value = DataClassType.ID;
            dgv.Rows[7].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.ID);
            dgv.Rows[7].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.ID);
            dgv.Rows[7].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.ID);
            dgv.Rows[7].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.ID);
            dgv.Rows[7].Cells[6].Value = true;
            dgv.Rows[7].Cells[7].Value = true;

            // Default values for URL
            dgv.Rows[8].Cells[0].Value = true;
            dgv.Rows[8].Cells[1].Value = DataClassType.String;
            dgv.Rows[8].Cells[2].Value = dcr.GetDefaultAllowedChars(DataClassType.String);
            dgv.Rows[8].Cells[3].Value = dcr.GetDefaultAnomalyChars(DataClassType.String);
            dgv.Rows[8].Cells[4].Value = dcr.GetDefaultMinValue(DataClassType.String);
            dgv.Rows[8].Cells[5].Value = dcr.GetDefaultMaxValue(DataClassType.String);
            dgv.Rows[8].Cells[6].Value = true;
            dgv.Rows[8].Cells[7].Value = true;
        }
    }
}
