using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using System.Data;
using System.Text;

namespace BatchMailer
{
    public partial class MainForm : Form
    {
        private string _dataPath = string.Empty;
        private string _attachmentsFolder = string.Empty;
        private CancellationTokenSource? _cts;

        public MainForm()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void BtnChooseData_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "CSV or Excel (*.csv;*.xlsx)|*.csv;*.xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _dataPath = ofd.FileName;
                txtDataPath.Text = _dataPath;
            }
        }

        private void BtnChooseFolder_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _attachmentsFolder = fbd.SelectedPath;
                txtFolderPath.Text = _attachmentsFolder;
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            var rows = ReadRecipients(_dataPath);
            MessageBox.Show($"Found {rows.Count} recipients.\nFirst 5 preview:\n" +
                string.Join("\n", rows.Take(5).Select(r => $"{r.FirstName} {r.LastName} <{r.Email}> => {r.Attachment1}, {r.Attachment2}")),
                "Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            btnSend.Enabled = false;
            btnPreview.Enabled = false;
            _cts = new CancellationTokenSource();

            var recipients = ReadRecipients(_dataPath);
            if (recipients.Count == 0)
            {
                MessageBox.Show("No recipients found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetUi();
                return;
            }

            // SMTP settings
            string host = txtSmtpHost.Text.Trim();
            if (!int.TryParse(txtSmtpPort.Text.Trim(), out int port)) port = 587;
            string user = txtSenderEmail.Text.Trim();
            string pass = txtSenderPass.Text;
            string subjectTemplate = txtSubject.Text.Trim();
            string bodyTemplate = txtBody.Text.Trim();

            int total = recipients.Count;
            int success = 0, failed = 0;
            progressBar.Value = 0;
            lblStatus.Text = "Sending...";
            lblSuccess.Text = "Success: 0";
            lblFailed.Text = "Failed: 0";

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string logFolder = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logFolder);
            string successLog = Path.Combine(logFolder, $"success_{timestamp}.csv");
            string failLog = Path.Combine(logFolder, $"failed_{timestamp}.csv");

            await Task.Run(async () =>
            {
                using var successWriter = new StreamWriter(successLog, false);
                using var failWriter = new StreamWriter(failLog, false);

                await successWriter.WriteLineAsync("FirstName,LastName,Email,Attachment1,Attachment2");
                await failWriter.WriteLineAsync("FirstName,LastName,Email,Attachment1,Attachment2,Error");

                for (int i = 0; i < recipients.Count; i++)
                {
                    var r = recipients[i];
                    try
                    {
                        string subject = ReplacePlaceholders(subjectTemplate, r);
                        string body = ReplacePlaceholders(bodyTemplate, r);
                        await SendSingleEmail(host, port, user, pass, r, subject, body);
                        Interlocked.Increment(ref success);
                        await successWriter.WriteLineAsync($"{Quote(r.FirstName)},{Quote(r.LastName)},{Quote(r.Email)},{Quote(r.Attachment1)},{Quote(r.Attachment2)}");
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref failed);
                        await failWriter.WriteLineAsync($"{Quote(r.FirstName)},{Quote(r.LastName)},{Quote(r.Email)},{Quote(r.Attachment1)},{Quote(r.Attachment2)},{Quote(ex.Message)}");
                    }

                    int done = i + 1;
                    this.Invoke(() =>
                    {
                        progressBar.Value = (int)((done / (double)total) * 100);
                        lblSuccess.Text = $"Success: {success}";
                        lblFailed.Text = $"Failed: {failed}";
                        lblStatus.Text = $"Sent {done} / {total}";
                    });

                    await Task.Delay(300, _cts.Token);
                }

                await successWriter.FlushAsync();
                await failWriter.FlushAsync();
            });

            MessageBox.Show($"Done. Success: {success}, Failed: {failed}\nLogs at: {Path.Combine(AppContext.BaseDirectory, "logs")}",
                "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ResetUi();
        }

        private void ResetUi()
        {
            btnSend.Enabled = true;
            btnPreview.Enabled = true;
            lblStatus.Text = "Ready";
        }

        private static string Quote(string s) => $"\"{s?.Replace("\"", "\"\"")}\"";

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(_dataPath) || !File.Exists(_dataPath))
            {
                MessageBox.Show("Select a valid CSV or XLSX file.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(_attachmentsFolder) || !Directory.Exists(_attachmentsFolder))
            {
                MessageBox.Show("Select a valid attachments folder.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSenderEmail.Text) || string.IsNullOrWhiteSpace(txtSenderPass.Text))
            {
                MessageBox.Show("Enter sender SMTP credentials.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Enter a subject line.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtBody.Text))
            {
                MessageBox.Show("Enter a message body.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private List<Recipient> ReadRecipients(string path)
        {
            var list = new List<Recipient>();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".csv")
            {
                var lines = File.ReadAllLines(path);
                int start = 0;
                if (lines.Length > 0 && lines[0].ToLower().Contains("first")) start = 1;
                for (int i = start; i < lines.Length; i++)
                {
                    var parts = SplitCsvLine(lines[i]);
                    if (parts.Length < 3) continue;
                    var r = new Recipient
                    {
                        FirstName = parts.ElementAtOrDefault(0) ?? "",
                        LastName = parts.ElementAtOrDefault(1) ?? "",
                        Email = parts.ElementAtOrDefault(2) ?? "",
                        Attachment1 = parts.ElementAtOrDefault(3) ?? "",
                        Attachment2 = parts.ElementAtOrDefault(4) ?? ""
                    };
                    if (!Path.IsPathRooted(r.Attachment1) && !string.IsNullOrWhiteSpace(r.Attachment1))
                        r.Attachment1 = Path.Combine(_attachmentsFolder, r.Attachment1);
                    if (!Path.IsPathRooted(r.Attachment2) && !string.IsNullOrWhiteSpace(r.Attachment2))
                        r.Attachment2 = Path.Combine(_attachmentsFolder, r.Attachment2);
                    list.Add(r);
                }
            }
            else if (ext == ".xlsx" || ext == ".xls")
            {
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration { UseHeaderRow = true }
                };
                var ds = reader.AsDataSet(conf);
                if (ds.Tables.Count == 0) return list;
                var dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    var r = new Recipient
                    {
                        FirstName = row.Table.Columns.Contains("FirstName") ? row["FirstName"].ToString() ?? "" : "",
                        LastName = row.Table.Columns.Contains("LastName") ? row["LastName"].ToString() ?? "" : "",
                        Email = row.Table.Columns.Contains("Email") ? row["Email"].ToString() ?? "" : "",
                        Attachment1 = row.Table.Columns.Contains("Attachment1") ? row["Attachment1"].ToString() ?? "" : "",
                        Attachment2 = row.Table.Columns.Contains("Attachment2") ? row["Attachment2"].ToString() ?? "" : ""
                    };
                    if (!Path.IsPathRooted(r.Attachment1) && !string.IsNullOrWhiteSpace(r.Attachment1))
                        r.Attachment1 = Path.Combine(_attachmentsFolder, r.Attachment1);
                    if (!Path.IsPathRooted(r.Attachment2) && !string.IsNullOrWhiteSpace(r.Attachment2))
                        r.Attachment2 = Path.Combine(_attachmentsFolder, r.Attachment2);
                    list.Add(r);
                }
            }
            return list;
        }

        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var cur = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        cur.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(cur.ToString().Trim());
                    cur.Clear();
                }
                else
                {
                    cur.Append(c);
                }
            }
            result.Add(cur.ToString().Trim());
            return result.ToArray();
        }

        private static string ReplacePlaceholders(string template, Recipient r)
        {
            return template
                .Replace("{FirstName}", r.FirstName ?? "", StringComparison.OrdinalIgnoreCase)
                .Replace("{LastName}", r.LastName ?? "", StringComparison.OrdinalIgnoreCase)
                .Replace("{Email}", r.Email ?? "", StringComparison.OrdinalIgnoreCase);
        }

        private async Task SendSingleEmail(string host, int port, string user, string pass, Recipient r, string subject, string body)
        {
            using var message = new MailMessage();
            message.From = new MailAddress(user);
            message.To.Add(new MailAddress(r.Email, $"{r.FirstName} {r.LastName}"));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            if (!string.IsNullOrWhiteSpace(r.Attachment1) && File.Exists(r.Attachment1))
                message.Attachments.Add(new Attachment(r.Attachment1));
            if (!string.IsNullOrWhiteSpace(r.Attachment2) && File.Exists(r.Attachment2))
                message.Attachments.Add(new Attachment(r.Attachment2));

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pass),
                Timeout = 20000
            };
            await client.SendMailAsync(message);
        }
    }
}
