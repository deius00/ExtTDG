using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExtTDG.Data
{
    public class Utility
    {
        public static void SetDefaultRowData(DataGridView dgv, DataClassRegistry dcr)
        {
            DataClassType[] defaultGenerators = new DataClassType[]
            {
                DataClassType.Name,
                DataClassType.Int32,
                DataClassType.Email,
                DataClassType.Date,
                DataClassType.Address,
                DataClassType.Phone,
                DataClassType.URL,
                DataClassType.ID,
                DataClassType.String,
            };

            dgv.Rows.Add(defaultGenerators.Length);
            for(int i = 0; i < defaultGenerators.Length; i++)
            {
                dgv.Rows[i].Cells[0].Value = true;
                dgv.Rows[i].Cells[1].Value = defaultGenerators[i];
                dgv.Rows[i].Cells[2].Value = dcr.GetDefaultAllowedChars(defaultGenerators[i]);
                dgv.Rows[i].Cells[3].Value = dcr.GetDefaultAnomalyChars(defaultGenerators[i]);
                dgv.Rows[i].Cells[4].Value = dcr.GetDefaultMinValue(defaultGenerators[i]);
                dgv.Rows[i].Cells[5].Value = dcr.GetDefaultMaxValue(defaultGenerators[i]);
                dgv.Rows[i].Cells[6].Value = dcr.GetDefaultHasAnomalies(defaultGenerators[i]);
                dgv.Rows[i].Cells[7].Value = dcr.GetDefaultIsUnique(defaultGenerators[i]);
            }
        }
    }
}
