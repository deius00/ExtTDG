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
using ExtTDG.Data;

namespace ExtTDG
{
    public partial class Form1 : Form
    {
        // Contains minimum and maximum values for all DataClass types
        private DataClassRegistry m_dataClassRegistry = new DataClassRegistry();
        private Dictionary<DataClassType, IGenerator> m_generators = new Dictionary<DataClassType, IGenerator>();
        private List<GeneratorParameters> m_generatorParameters = new List<GeneratorParameters>();
        private List<GeneratorStats> m_generatorStats = new List<GeneratorStats>();
        private List<List<string>> m_allResults = new List<List<string>>();
        private RowParser m_rowParser = null;

        private long m_generationDuration = 0;
        private long m_fileWriteDuration = 0;
        private bool m_isFileSelected = false;
        private bool m_saveToFileOk = false;
        
        private BackgroundWorker m_worker = new BackgroundWorker();

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
            // Define default allowed and anomaly characters for DataClass type
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.Name,
                "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ-'",
                "!#¤%&()?/;.:,_<>|@£${[]}*");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.Int32,
                "0123456789",
                "!#¤%&/()=");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.Email,
                "abcdefghijklmnopqrstuvwxyz-@.",
                "!#¤%&()?/;:,_<>|£${[]}*");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.Date,
                "0",
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#¤%&()?;:,_<>|@£${[]}");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.Address,
                "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ-",
                "!#¤%&/()=?;:,_<>|@£${[]}");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.Phone,
                "0123456789",
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#¤%&/()=?;:,_<>|@£${[]}");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.URL,
                "abcdefghijklmnopqrstuvwxyz1234567890",
                "!#¤%&/()=?;:,_<>|@£${[]}");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.ID,
                "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                "!#¤%&/()=?`@£$€{[]}^¨'*~,.;:<>|§½");
            
            m_dataClassRegistry.SetDefaultCharacters(DataClassType.String,
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                "!#¤%&()?/;.:,_-<>|@£${[]}*");

            // Set default min/maxvalues for each DataClassType
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.Name, "1", "50");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.Int32, "0", "1500");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.Email, "6", "50");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.Date, "10000101", "99991231");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.Address, "0", "50");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.Phone, "7", "12");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.URL, "0", "50");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.ID, "1", "6");
            m_dataClassRegistry.SetDefaultMinMaxValues(DataClassType.String, "1", "15");

            // Parses rows and converts them into list of generator parameters
            // Only active and supported rows (determined by DataClassType)
            // are added to list.
            m_rowParser = new RowParser(dgvGenerators, m_dataClassRegistry);

            // Insert default rows to DataGridView
            Utility.SetDefaultRowData(dgvGenerators, m_dataClassRegistry);
            DeactivateGenerateButton();
            ValidateTest();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            List<string> errorMessages = new List<string>();
            m_generatorParameters = m_rowParser.ParseRows();
            SessionParameters sessionParameters = BuildSessionParameters();

            if(m_generatorParameters.Count == 0)
            {
                // No generators active, do nothing
                return;
            }

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
                tbLogs.Text = "";

                // Get new list of generators
                m_generators = BuildGenerators();

                // Validate generators
                bool isValidationOk = true;
                List<ValidationResult> validationResults = new List<ValidationResult>();
                foreach (GeneratorParameters gp in m_generatorParameters)
                {
                    ValidationResult result;
                    bool isGeneratorValid = m_generators[gp.dataClassType].Validate(sessionParameters.numItems, out result);
                    result.generatorName = "Generator" + gp.dataClassTypeName;
                    validationResults.Add(result);
                    isValidationOk &= isGeneratorValid;
                }

                if (!isValidationOk)
                {
                    // Output validation errors to log
                    tbLogs.Text = "";
                    tsStatusDuration.Text = "Validation errors. Check logs.";
                    foreach(ValidationResult res in validationResults)
                    {
                        if(!res.isValid)
                        {

                            tbLogs.AppendText(res.generatorName + System.Environment.NewLine);
                            foreach (string msg in res.messages)
                            {
                                tbLogs.AppendText("- " + msg + System.Environment.NewLine);
                            }
                            tbLogs.AppendText(System.Environment.NewLine);
                        }
                    }
                    return;
                }

                // Run subgenerators
                tsStatusDuration.Text = "Generating...";
                foreach (GeneratorParameters gp in m_generatorParameters)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    List<string> generatorResults = m_generators[gp.dataClassType].Generate(sessionParameters.numItems, sessionParameters.anomalyChance, sessionParameters.rng);
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

                // Calculate total generation duration
                m_generationDuration = 0;
                foreach (GeneratorStats gs in m_generatorStats)
                {
                    m_generationDuration += gs.durationInMilliseconds;
                }

                // Start background worker for saving results to file
                // disable Generate-button until saving is done
                m_worker.RunWorkerAsync();
                DeactivateGenerateButton();
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

        // Parse parameters for this run
        private SessionParameters BuildSessionParameters()
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

        private Dictionary<DataClassType, IGenerator> BuildGenerators()
        {
            Dictionary<DataClassType, IGenerator> generators = new Dictionary<DataClassType, IGenerator>();
            foreach (GeneratorParameters gp in m_generatorParameters)
            {
                switch (gp.dataClassType)
                {
                    case DataClassType.Name:
                        generators[gp.dataClassType] = new GeneratorName(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.Int32:
                        generators[gp.dataClassType] = new GeneratorInt32(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.Email:
                        generators[gp.dataClassType] = new GeneratorEmail(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.Date:
                        generators[gp.dataClassType] = new GeneratorDate(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.Address:
                        generators[gp.dataClassType] = new GeneratorAddress(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.Phone:
                        generators[gp.dataClassType] = new GeneratorPhone(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.URL:
                        generators[gp.dataClassType] = new GeneratorURL(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.ID:
                        generators[gp.dataClassType] = new GeneratorID(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    case DataClassType.String:
                        generators[gp.dataClassType] = new GeneratorString(gp.allowedCharacters, gp.anomalyCharacters,
                            gp.minLength, gp.maxLength, gp.hasAnomalies, gp.isUnique);
                        break;

                    default:
                        throw new Exception("Generator not implemented!");
                }
            }

            return generators;
        }

        private void SaveResultsToFile()
        {
            ExcelApplication app = new ExcelApplication();
            ExcelWorkbook workBook = app.Workbooks.Add();
            ExcelWorksheet workSheet = app.ActiveSheet;
            app.Visible = false;
            app.DisplayAlerts = false;
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
                genText = "Saving aborted by user. Results not saved file.";
                message = genText;
            }

            tsStatusDuration.Text = message;
            ActivateGenerateButton();
        }

        private void ValidateTest()
        {
            dgvGenerators.Rows[0].Cells[0].Value = false;
            dgvGenerators.Rows[1].Cells[0].Value = false;
            dgvGenerators.Rows[2].Cells[0].Value = false;
            dgvGenerators.Rows[3].Cells[0].Value = false;
            dgvGenerators.Rows[4].Cells[0].Value = false;
            dgvGenerators.Rows[5].Cells[0].Value = false;
            dgvGenerators.Rows[6].Cells[0].Value = false;
            dgvGenerators.Rows[7].Cells[0].Value = false;
            dgvGenerators.Rows[8].Cells[0].Value = false;
            tbFilePath.Text = "D:\\yliopisto\\tkt\\OHTU II\\R09-UiPath\\TestData\\results.xlsx";
            cbAllowOverwrite.Checked = true;
        }
    }
}
