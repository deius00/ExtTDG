using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtTDG
{
    /// <summary>
    /// RowParser parses rows in DataGridView and returns list of parameteros for 
    /// all active and supported generator.
    /// </summary>
    public class RowParser
    {
        private DataGridView m_dgv;
        private DataClassRegistry m_dgr;

        public RowParser(DataGridView dgv, DataClassRegistry dgr)
        {
            this.m_dgv = dgv;
            this.m_dgr = dgr;
        }

        public List<GeneratorParameters> ParseRows()
        {
            List<GeneratorParameters> generatorParameters = new List<GeneratorParameters>();
            foreach(DataGridViewRow row in m_dgv.Rows)
            {
                bool rowIsSelected = (bool)row.Cells[0].Value;
                if (rowIsSelected)
                {
                    GeneratorParameters gp = new GeneratorParameters();
                    gp.dataClassType = (DataClassType)row.Cells[1].Value;
                    gp.dataClassTypeName = gp.dataClassType.ToString();
                    gp.allowedCharacters = (string)row.Cells[2].Value;
                    gp.anomalyCharacters = (string)row.Cells[3].Value;

                    // Parse minimum length
                    if (row.Cells[4].Value == null)
                    {
                        // Use default min value for generator and update cell
                        gp.minLength = m_dgr.GetDefaultMinValue(gp.dataClassType);
                        row.Cells[4].Value = gp.minLength;
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(row.Cells[4].Value.ToString()))
                        {
                            gp.minLength = m_dgr.GetDefaultMinValue(gp.dataClassType);
                            row.Cells[4].Value = gp.minLength;
                        }
                        else
                        {
                            gp.minLength = row.Cells[4].Value.ToString();
                        }
                    }

                    // Parse maximum length
                    if (row.Cells[5].Value == null)
                    {
                        // Use default max value for generator and update cell
                        gp.maxLength = m_dgr.GetDefaultMaxValue(gp.dataClassType);
                        row.Cells[5].Value = gp.maxLength;
                        Console.WriteLine(row.Cells[5].Value.ToString());
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(row.Cells[5].Value.ToString()))
                        {
                            gp.maxLength = m_dgr.GetDefaultMaxValue(gp.dataClassType);
                            row.Cells[5].Value = gp.maxLength;
                        }
                        else
                        {
                            gp.maxLength = row.Cells[5].Value.ToString();
                        }
                    }

                    gp.hasAnomalies = (bool)row.Cells[6].Value;
                    gp.isUnique = (bool)row.Cells[7].Value;
                    generatorParameters.Add(gp);
                }
            }

            return generatorParameters;
        }
    }
}
