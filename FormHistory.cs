using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhoPlantAmI
{
    public partial class FormHistory : Form
    {
        private List<string> history;
        private Form1 form;
        public FormHistory(Form1 form)
        {
            this.form = form;
            InitializeComponent();
            UpdateHistory();
        }

        private void LoadHistory()
        {
            try
            {
                string filename = "history.csv";
                history = File.ReadAllLines(filename).ToList();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"파일이 없어요. \n{ex.Message}", "파일이 없는 오류!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"권한이 없어요. \n{ex.Message}", "파일 권한 오류!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"알 수 없는 오류가 발생했어요 \n{ex.Message}", "알 수 없는 오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            UpdateHistory();
        }

        public void UpdateHistory()
        {
            LoadHistory();
            listBoxHistory.Items.Clear();
            listBoxHistory.Items.AddRange(history.ToArray());
        }

        private void listBoxHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLine = history[listBoxHistory.SelectedIndex];
            form.LoadHistory(selectedLine);
        }
    }
}
