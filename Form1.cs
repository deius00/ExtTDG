using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using ExcelWorkbook = Microsoft.Office.Interop.Excel.Workbook;
using ExcelWorksheet = Microsoft.Office.Interop.Excel.Worksheet;
using ExcelRange = Microsoft.Office.Interop.Excel.Range;
using BackgroundWorker = System.ComponentModel.BackgroundWorker;

namespace ExtTDG
{
    public partial class Form1 : Form
    {
        private Dictionary<DataClassType, IGenerator> generatorTypes = new Dictionary<DataClassType, IGenerator>();
        private List<GeneratorParameters> m_generatorParameters = new List<GeneratorParameters>();
        private List<GeneratorStats> m_generatorStats = new List<GeneratorStats>();
        private List<List<string>> m_allResults = new List<List<string>>();
        private long m_generationDuration = 0;
        private long m_fileWriteDuration = 0;
        private bool m_isFileSelected = false;
        private bool m_saveToFileOk = false;
        private BackgroundWorker m_worker = new BackgroundWorker();

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

            public GeneratorStats(DataClassType type, long durationInMilliseconds)
            {
                this.type = type;
                this.durationInMilliseconds = durationInMilliseconds;
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

            // Initialize background worker
            m_worker.DoWork += new DoWorkEventHandler(StartBackgroundWork);
            m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FinishedBackgroundWork);
            tbFilePath.TextChanged += tbFilePath_TextChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
            DeactivateGenerateButton();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            m_generatorParameters = GetGeneratorParameters();
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

            if (sessionParametersOk)
            {
                m_allResults.Clear();
                m_generatorStats.Clear();

                // Initialize progress bar
                tsProgressBar.Minimum = 0;
                tsProgressBar.Maximum = m_generatorParameters.Count + 1; // One for 
                tsProgressBar.Step = 1;
                tsProgressBar.Value = 0;

                // Reset toolstrip status text and logs
                tsStatusDuration.Text = "Generating...";
                tbLogs.Text = "";

                // Cache generators key-value-pairs used in this session
                foreach (GeneratorParameters gp in m_generatorParameters)
                {
                    switch (gp.dataClassType)
                    {
                        case DataClassType.Name:
                            generatorTypes[gp.dataClassType] = new GeneratorName(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.Int32:
                            generatorTypes[gp.dataClassType] = new GeneratorInt32(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.Email:
                            generatorTypes[gp.dataClassType] = new GeneratorEmail(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.Date:
                            generatorTypes[gp.dataClassType] = new GeneratorDate(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.Address:
                            generatorTypes[gp.dataClassType] = new GeneratorAddress(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.Phone:
                            generatorTypes[gp.dataClassType] = new GeneratorPhone(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.URL:
                            generatorTypes[gp.dataClassType] = new GeneratorURL(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.ID:
                            generatorTypes[gp.dataClassType] = new GeneratorID(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        case DataClassType.String:
                            generatorTypes[gp.dataClassType] = new GeneratorString(gp.allowedCharacters, gp.anomalyCharacters, gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                            break;

                        default:
                            throw new Exception("Generator not implemented!");
                    }
                }

                // Call each subgenerator
                foreach (GeneratorParameters gp in m_generatorParameters)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    List<string> generatorResults = generatorTypes[gp.dataClassType].Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
                    m_generatorStats.Add(new GeneratorStats(gp.dataClassType, sw.ElapsedMilliseconds));
                    m_allResults.Add(generatorResults);
                    sw.Stop();
                    tsProgressBar.PerformStep();
                }

                // Log running times
                foreach(GeneratorStats gs in m_generatorStats)
                {
                    tbLogs.Text += "Generator" + gs.type.ToString() + " duration: " + gs.durationInMilliseconds + " ms" + "\r\n";
                }

                m_generationDuration = GetTotalDurationInMilliseconds(m_generatorStats);

                // Start background worker for saving results to file
                m_worker.RunWorkerAsync();

                // Disable Generate-button until saved results to file
                btnGenerate.Enabled = false;
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
            dgvGenerators.Rows[1].Cells[5].Value = "99999";
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
            dgvGenerators.Rows[6].Cells[3].Value = "!#()[]";
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
            dgvGenerators.Rows[8].Cells[3].Value = "!#()[]";
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

        private long GetTotalDurationInMilliseconds(List<GeneratorStats> stats)
        {
            long totalDurationInMilliseconds = 0;
            foreach (GeneratorStats gs in stats)
            {
                totalDurationInMilliseconds += gs.durationInMilliseconds;
            }
            return totalDurationInMilliseconds;
        }

        private void SaveResultsToFile()
        {
            object Nothing = System.Reflection.Missing.Value;
            ExcelApplication app = new ExcelApplication();
            app.Visible = false;
            app.DisplayAlerts = false;
            ExcelWorkbook workBook = app.Workbooks.Add(Nothing);
            ExcelWorksheet workSheet = workBook.Sheets[1];
            workSheet.Name = "Results";

            // Convert each generator results to array and write it in one go
            int numColumns = m_allResults.Count;
            int numRows = m_allResults[0].Count;
            object[,] arr = new object[numRows, numColumns];

            for (int colIndex = 0; colIndex < numColumns; colIndex++)
            {
                List<string> rows = m_allResults[colIndex];
                for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
                {
                    arr[rowIndex, colIndex] = rows[rowIndex];
                }
            }

            int topRow = 2;
            ExcelRange cellStart = workSheet.Cells[topRow, 1];
            ExcelRange cellEnd = workSheet.Cells[topRow + numRows - 1, numColumns];
            ExcelRange selectedRange = workSheet.Range[cellStart, cellEnd];
            selectedRange.NumberFormat = "@";
            selectedRange.Value = arr;
            selectedRange.Columns.AutoFit();

            // Write headers to columns
            for(int i = 0; i < m_allResults.Count; i++)
            {
                workSheet.Cells[1, 1 + i] = m_generatorParameters[i].dataClassTypeName;
            }

            // Try writing to file until operation is successful or aborted by user
            bool canContinue = false;
            while(!canContinue)
            {
                try
                {
                    workSheet.SaveAs(tbFilePath.Text, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
                    workBook.Close(false, Type.Missing, Type.Missing);
                    canContinue = true;
                    m_saveToFileOk = true;
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    // Catch 
                    string msg = "Result file open in Excel.\nClose Excel and press Retry.";
                    DialogResult result = MessageBox.Show(msg, "Error!", MessageBoxButtons.RetryCancel);
                    if (result == DialogResult.Cancel)
                    {
                        // Aborted by user, do not save results to file
                        canContinue = true;
                        m_saveToFileOk = false;
                    }
                }
            }

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
                    dgvGenerators.Rows[e.RowIndex].ErrorText = "Only integers greater than 0";
                }
            }

            // Check column maximum length
            if (e.ColumnIndex == 5)
            {
                int newValue;
                if (!int.TryParse(e.FormattedValue.ToString(), out newValue) || newValue < 0)
                {
                    e.Cancel = true;
                    dgvGenerators.Rows[e.RowIndex].ErrorText = "Only integers greater than 0";
                }
            }
        }

        // Validate text changed on tbFilePath
        private void tbFilePath_TextChanged(object sender, EventArgs e)
        {
            if(tbFilePath.TextLength > 0)
            {
                m_isFileSelected = true;
                ActivateGenerateButton();
            }
            else
            {
                m_isFileSelected = false;
                DeactivateGenerateButton();
            }
        }

        // Ask result file location from user
        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel file|*.xlsx";
            saveFileDialog1.Title = "Save results to Excel-file";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                tbFilePath.Text = filePath;
                m_isFileSelected = true;
                ActivateGenerateButton();
            }
        }

        private void ActivateGenerateButton()
        {
            btnGenerate.Enabled = true;
        }

        private void DeactivateGenerateButton()
        {
            btnGenerate.Enabled = false;
        }

        // Start background worker to save results to file
        private void StartBackgroundWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SaveResultsToFile();
            sw.Stop();
            m_fileWriteDuration = sw.ElapsedMilliseconds;
        }

        private void FinishedBackgroundWork(object sender, RunWorkerCompletedEventArgs e)
        {
            // Update toolstrip text
            tsProgressBar.PerformStep();
            string genText;
            string writeText;
            string message;
            if(m_saveToFileOk)
            {
                genText = "Done. Generation " + m_generationDuration + " ms";
                writeText = "file write " + m_fileWriteDuration + " ms.";
                message = genText + ", " + writeText;
            }
            else
            {
                genText = "Saving aborted by user.";
                message = genText;
            }

            tsStatusDuration.Text = message;
            ActivateGenerateButton();
        }
    }
}
