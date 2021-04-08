using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ExtTDG
{
    public partial class Form1 : Form
    {
        private bool m_isFileSelected = false;
        private bool m_allowOverwrite = false;

        private enum DataClassType
        {
            Name, Int32, Email, Date, Address, Phone, URL, ID, String
        };

        // POD for generator parameters
        private class GeneratorParameters
        {
            public DataClassType dataClassType; // DataClass type
            public string dataClassTypeName;    // DataClass type name
            public string allowedCharacters;
            public string anomalyCharacters;
            public string minLength;            // Also minValue
            public string maxLength;            // Also maxValue
            public bool hasAnomalies;
            public bool isUnique;

            public GeneratorParameters()
            {
                this.dataClassTypeName = null;
                this.allowedCharacters = null;
                this.anomalyCharacters = null;
                this.minLength = null;
                this.maxLength = null;
                this.hasAnomalies = false;
                this.isUnique = false;
            }
        }

        private class GeneratorStats
        {
            public DataClassType type;
            public long durationInMilliseconds;

            public GeneratorStats()
            {
                durationInMilliseconds = 0;
            }

            public GeneratorStats(long durationInMilliseconds)
            {
                this.durationInMilliseconds = durationInMilliseconds;
            }

            public GeneratorStats(DataClassType type, Stopwatch sw)
            {
                this.type = type;
                this.durationInMilliseconds = sw.ElapsedMilliseconds;
            }
        }

        // Parameters for current session
        private class SessionParameters
        {
            public int numItems;                // Number of items to generate on this run
            public float anomalyChance;         // Anomaly chance (0.0 - 1.0)
            public Random rng;                  // Reference to Random-instance
            public string filePath;             // Path to result file
            public bool isNumItemsOk;           // Parameter ok flag
            public bool isAnomalyChanceOk;      // Parameter ok flag
            public bool isFileSelected;
            public bool allowOverwrite;

            public SessionParameters()
            {
                numItems = 0;
                anomalyChance = 0.0f;
                rng = null;
                filePath = null;
                isNumItemsOk = false;
                isAnomalyChanceOk = false;
                isFileSelected = false;
                allowOverwrite = false;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            List<GeneratorParameters> generatorParameters = GetGeneratorParameters();
            SessionParameters sessionParameters = GetSessionParameters();

            // Cache error messages and show them to user
            List<string> errorMessages = new List<string>();

            // Check that run parameters are ok
            bool sessionParametersOk = true;
            if (!sessionParameters.isNumItemsOk)
            {
                errorMessages.Add("Invalid number of items to generate.");
                sessionParametersOk = false;
            }

            if (!sessionParameters.isAnomalyChanceOk)
            {
                errorMessages.Add("Invalid anomaly chance.");
                sessionParametersOk = false;
            }

            if (!sessionParameters.isFileSelected)
            {
                errorMessages.Add("Select file for results.");
                sessionParametersOk = false;
            }
            else
            {
                // File is selected, check if file exists and overwrite allowed
                if (File.Exists(sessionParameters.filePath) && !sessionParameters.allowOverwrite)
                {
                    errorMessages.Add("File already exists and overwrite not allowed.");
                    sessionParametersOk = false;
                }
            }

            // 1) Tee lista ajettavista generaattoreista
            // 2) Parametrit täytetään jo konstruktorissa paitsi lukumäärä ja anomaliatodennäköisyyttä
            // 3) Aligeneraattori palauttaa Generate()-metodista listan (List<string>) tuloksista
            if (sessionParametersOk)
            {
                Stopwatch sw = new Stopwatch();
                List<List<string>> allResults = new List<List<string>>();
                List<string> generatorResults = new List<string>();
                List<GeneratorStats> generatorStats = new List<GeneratorStats>();


                // Initialize progress bar
                tsProgressBar.Maximum = generatorParameters.Count;
                tsProgressBar.Step = 1;
                tsProgressBar.Value = 0;

                // Reset toolstrip status text
                tsStatusLabel.Text = "Generating...";
                tsStatusDuration.Text = "Generating...";

                foreach (GeneratorParameters gp in generatorParameters)
                {
                    switch (gp.dataClassType)
                    {
                        case DataClassType.Name:
                            sw.Start();
                            GeneratorName generatorName = new GeneratorName(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorName.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.Int32:
                            sw.Start();
                            GeneratorInt32 generatorInt32 = new GeneratorInt32(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorInt32.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.Email:
                            sw.Start();
                            GeneratorEmail generatorEmail = new GeneratorEmail(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorEmail.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.Date:
                            sw.Start();
                            GeneratorDate generatorDate = new GeneratorDate(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength.ToString(), gp.maxLength.ToString(),
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorDate.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.Address:
                            sw.Start();
                            GeneratorAddress generatorAddress = new GeneratorAddress(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorAddress.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.Phone:
                            sw.Start();
                            GeneratorPhone generatorPhone = new GeneratorPhone(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorPhone.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.URL:
                            sw.Start();
                            GeneratorURL generatorURL = new GeneratorURL(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorURL.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.ID:
                            sw.Start();
                            GeneratorID generatorID = new GeneratorID(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorID.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        case DataClassType.String:
                            sw.Start();
                            GeneratorString generatorString = new GeneratorString(
                                gp.allowedCharacters, gp.anomalyCharacters,
                                gp.minLength, gp.maxLength,
                                gp.hasAnomalies, gp.isUnique);

                            generatorResults = generatorString.Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                            sw.Stop();
                            generatorStats.Add(new GeneratorStats(gp.dataClassType, sw));
                            allResults.Add(generatorResults);
                            break;

                        default:
                            break;
                    }

                    tsProgressBar.PerformStep();
                }

                // Display running times
                PrintStats(generatorStats);

                // Write results to Excel file
                SaveResultsToFile(allResults, generatorParameters);
                Console.WriteLine("Done");

                tsStatusLabel.Text = "Done.";
                tsStatusDuration.Text = "Completed in " + GetTotalDurationInMilliseconds(generatorStats) + " ms";
            }

            // Show error message to user at the end
            if (errorMessages.Count > 0)
            {
                string msg = "";
                foreach (string s in errorMessages)
                {
                    msg += s + "\n";
                }

                DialogResult result = MessageBox.Show(msg, "Error!", MessageBoxButtons.OK);
            }
        }

        // Populate data for DataGridView
        private void PopulateDataGridView()
        {
            dgvGenerators.Rows.Add(9);

            // Default values for Name
            dgvGenerators.Rows[0].Cells[0].Value = true;
            dgvGenerators.Rows[0].Cells[1].Value = DataClassType.Name;
            dgvGenerators.Rows[0].Cells[2].Value = "abcdefghijklmnopqrustuvwxyz";
            dgvGenerators.Rows[0].Cells[3].Value = "!#()";
            dgvGenerators.Rows[0].Cells[4].Value = "1";
            dgvGenerators.Rows[0].Cells[5].Value = "5";
            dgvGenerators.Rows[0].Cells[6].Value = true;
            dgvGenerators.Rows[0].Cells[7].Value = true;

            // Default values for Int32
            dgvGenerators.Rows[1].Cells[0].Value = true;
            dgvGenerators.Rows[1].Cells[1].Value = DataClassType.Int32;
            dgvGenerators.Rows[1].Cells[2].Value = "0123456789";
            dgvGenerators.Rows[1].Cells[3].Value = "-+";
            dgvGenerators.Rows[1].Cells[4].Value = "1";
            dgvGenerators.Rows[1].Cells[5].Value = "5";
            dgvGenerators.Rows[1].Cells[6].Value = true;
            dgvGenerators.Rows[1].Cells[7].Value = true;

            // Default values for Email
            dgvGenerators.Rows[2].Cells[0].Value = true;
            dgvGenerators.Rows[2].Cells[1].Value = DataClassType.Email;
            dgvGenerators.Rows[2].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz1234567890";
            dgvGenerators.Rows[2].Cells[3].Value = "!/#[]";
            dgvGenerators.Rows[2].Cells[4].Value = "3";
            dgvGenerators.Rows[2].Cells[5].Value = "30";
            dgvGenerators.Rows[2].Cells[6].Value = true;
            dgvGenerators.Rows[2].Cells[7].Value = true;

            // Default values for Date
            dgvGenerators.Rows[3].Cells[0].Value = true;
            dgvGenerators.Rows[3].Cells[1].Value = DataClassType.Date;
            dgvGenerators.Rows[3].Cells[2].Value = "0";
            dgvGenerators.Rows[3].Cells[3].Value = "abcdefghijklmnopqrstuvwxyz-/.";
            dgvGenerators.Rows[3].Cells[4].Value = "10000101";
            dgvGenerators.Rows[3].Cells[5].Value = "99991231";
            dgvGenerators.Rows[3].Cells[6].Value = true;
            dgvGenerators.Rows[3].Cells[7].Value = true;

            // Default values for Address
            dgvGenerators.Rows[4].Cells[0].Value = true;
            dgvGenerators.Rows[4].Cells[1].Value = DataClassType.Address;
            dgvGenerators.Rows[4].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz";
            dgvGenerators.Rows[4].Cells[3].Value = "!#&/";
            dgvGenerators.Rows[4].Cells[4].Value = "0";
            dgvGenerators.Rows[4].Cells[5].Value = "0";
            dgvGenerators.Rows[4].Cells[6].Value = true;
            dgvGenerators.Rows[4].Cells[7].Value = true;

            // Default values for Phone
            dgvGenerators.Rows[5].Cells[0].Value = true;
            dgvGenerators.Rows[5].Cells[1].Value = DataClassType.Phone;
            dgvGenerators.Rows[5].Cells[2].Value = "0123456789";
            dgvGenerators.Rows[5].Cells[3].Value = "abcdefghijklmnopqrstuvwxyz!";
            dgvGenerators.Rows[5].Cells[4].Value = "7";
            dgvGenerators.Rows[5].Cells[5].Value = "30";
            dgvGenerators.Rows[5].Cells[6].Value = true;
            dgvGenerators.Rows[5].Cells[7].Value = true;

            // Default values for URL
            dgvGenerators.Rows[6].Cells[0].Value = true;
            dgvGenerators.Rows[6].Cells[1].Value = DataClassType.URL;
            dgvGenerators.Rows[6].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz";
            dgvGenerators.Rows[6].Cells[3].Value = "!#()[]=";
            dgvGenerators.Rows[6].Cells[4].Value = "7";
            dgvGenerators.Rows[6].Cells[5].Value = "30";
            dgvGenerators.Rows[6].Cells[6].Value = true;
            dgvGenerators.Rows[6].Cells[7].Value = true;
            
            // Default values for Phone
            dgvGenerators.Rows[7].Cells[0].Value = true;
            dgvGenerators.Rows[7].Cells[1].Value = DataClassType.ID;
            dgvGenerators.Rows[7].Cells[2].Value = "0123456789";
            dgvGenerators.Rows[7].Cells[3].Value = "abcdefghijklmnopqrstuvwxyz!";
            dgvGenerators.Rows[7].Cells[4].Value = "3";
            dgvGenerators.Rows[7].Cells[5].Value = "6";
            dgvGenerators.Rows[7].Cells[6].Value = true;
            dgvGenerators.Rows[7].Cells[7].Value = true;

            // Default values for URL
            dgvGenerators.Rows[8].Cells[0].Value = true;
            dgvGenerators.Rows[8].Cells[1].Value = DataClassType.String;
            dgvGenerators.Rows[8].Cells[2].Value = "abcdefghijklmnopqrstuvwxyz";
            dgvGenerators.Rows[8].Cells[3].Value = "!#()[]=";
            dgvGenerators.Rows[8].Cells[4].Value = "1";
            dgvGenerators.Rows[8].Cells[5].Value = "10";
            dgvGenerators.Rows[8].Cells[6].Value = true;
            dgvGenerators.Rows[8].Cells[7].Value = true;
        }

        // Parse data for each row in DataGridView and return list of selected
        // parameters.
        private List<GeneratorParameters> GetGeneratorParameters()
        {
            List<GeneratorParameters> generatorParameters = new List<GeneratorParameters>();
            for (int i = 0; i < dgvGenerators.Rows.Count; i++)
            {
                DataGridViewRow row = dgvGenerators.Rows[i];
                bool rowIsSelected = (bool)row.Cells[0].Value;
                if (rowIsSelected)
                {
                    // Parse data from each row
                    GeneratorParameters gp = new GeneratorParameters();
                    gp.dataClassType = (DataClassType)row.Cells[1].Value;
                    gp.dataClassTypeName = gp.dataClassType.ToString();
                    gp.allowedCharacters = (string)row.Cells[2].Value;
                    gp.anomalyCharacters = (string)row.Cells[3].Value;

                    // Try to parse minimum and maximum length
                    gp.minLength = row.Cells[4].Value.ToString();
                    gp.maxLength = row.Cells[5].Value.ToString();

                    gp.hasAnomalies = (bool)row.Cells[6].Value;
                    gp.isUnique = (bool)row.Cells[7].Value;
                    generatorParameters.Add(gp);
                }
            }

            return generatorParameters;
        }

        // Parse parameters for this run
        private SessionParameters GetSessionParameters()
        {
            SessionParameters rp = new SessionParameters();
            rp.isNumItemsOk = int.TryParse(tbNumItems.Text, out rp.numItems);
            rp.isAnomalyChanceOk = float.TryParse(tbAnomalyChance.Text, out rp.anomalyChance);
            rp.rng = new Random(DateTime.Now.Millisecond);
            rp.filePath = tbFilePath.Text;
            rp.isFileSelected = m_isFileSelected;
            rp.allowOverwrite = cbAllowOverwrite.Checked;
            return rp;
        }

        private void PrintStats(List<GeneratorStats> stats)
        {
            long totalRunningTimeMilliseconds = 0;
            foreach (GeneratorStats gs in stats)
            {
                Console.WriteLine(gs.type.ToString() + " gen: " + gs.durationInMilliseconds + " ms");
                totalRunningTimeMilliseconds += gs.durationInMilliseconds;
            }

            Console.WriteLine("Total running time: " + totalRunningTimeMilliseconds + " ms");
        }

        private float GetTotalDurationInMilliseconds(List<GeneratorStats> stats)
        {
            long totalDurationInMilliseconds = 0;
            foreach (GeneratorStats gs in stats)
            {
                totalDurationInMilliseconds += gs.durationInMilliseconds;
            }
            return totalDurationInMilliseconds;
        }

        private void SaveResultsToFile(List<List<string>> results, List<GeneratorParameters> gp)
        {
            object Nothing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = false;
            app.DisplayAlerts = false;
            Microsoft.Office.Interop.Excel.Workbook workBook = app.Workbooks.Add(Nothing);
            Microsoft.Office.Interop.Excel.Worksheet workSheet = workBook.Sheets[1];
            workSheet.Name = "Results";

            // Write all results to file
            for (int colIndex = 0; colIndex < results.Count; colIndex++)
            {
                List<string> rows = results[colIndex];
                workSheet.Cells[1, colIndex + 1] = gp[colIndex].dataClassTypeName;

                for (int rowIndex = 0; rowIndex < results[colIndex].Count; rowIndex++)
                {
                    workSheet.Cells[rowIndex + 2, colIndex + 1] = rows[rowIndex];
                }
            }

            workSheet.SaveAs(tbFilePath.Text, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
            workBook.Close(false, Type.Missing, Type.Missing);
            app.Quit();
        }

        // Validate input to cells
        private void dgv_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Not used now, user cannot add rows
            if (dgvGenerators.Rows[e.RowIndex].IsNewRow)
            {
                return;
            }

            // Check column minimum length
            if (e.ColumnIndex == 4)
            {
                int newValue;
                if (!int.TryParse(e.FormattedValue.ToString(), out newValue) || newValue < 0)
                {
                    e.Cancel = true;
                    dgvGenerators.Rows[e.RowIndex].ErrorText = "Value must be greater than 0 and integer";
                }
            }

            // Check column maximum length
            if (e.ColumnIndex == 5)
            {
                int newValue;
                if (!int.TryParse(e.FormattedValue.ToString(), out newValue) || newValue < 0)
                {
                    e.Cancel = true;
                    dgvGenerators.Rows[e.RowIndex].ErrorText = "Value must be greater than 0 and integer";
                }
            }
        }

        // Ask result file location from user
        private void btnSelectResultLocation_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel file|*.xlsx";
            saveFileDialog1.Title = "Save results to Excel-file:";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                tbFilePath.Text = filePath;
                m_isFileSelected = true;
            }
        }

        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel file|*.xlsx";
            saveFileDialog1.Title = "Save results to Excel-file:";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                tbFilePath.Text = filePath;
                m_isFileSelected = true;
            }
        }
    }
}
