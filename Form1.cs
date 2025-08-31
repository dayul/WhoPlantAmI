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
    public partial class Form1 : Form
    {
        List<PlantResult> results;
        public Form1()
        {
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {
            try
            {
                string filename = "results.csv";
                var lines = File.ReadAllLines(filename).Skip(1); // 첫 줄 제외
                results = lines.Select(line =>
                {
                    var parts = line.Split(',');    // , 로 구분
                    return new PlantResult          // 객체 생성해서 리스트에 추가
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Tags = parts[2],
                        FlowerMeaning = parts[3],
                        Description = parts[4]
                    };
                }).ToList();
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

        public PlantResult GetRandomResult()
        {
            Random rand = new Random();
            int index = rand.Next(results.Count);
            return results[index];
        }

        private void 내역불러오기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHistory form = Application.OpenForms["FormHistory"] as FormHistory;
            if (form != null)
            {
                form.UpdateHistory();
                form.Activate();
            }
            else
            {
                form = new FormHistory(this);
                form.Show();
            }
        }

        private void 끝내기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        private void buttonResult_Click(object sender, EventArgs e)
        {
            string season = comboBoxSeason.SelectedItem?.ToString();
            string value = comboBoxValue.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(season) || string.IsNullOrEmpty(value))
            {
                MessageBox.Show("계절과 중요하게 생각하는 것을 선택해주세요!");
                return;
            }

            PlantResult selected = GetRandomResult();

            textBoxResult.Text = $"당신은 {selected.Name} 입니다!" 
                       + Environment.NewLine + Environment.NewLine +
                       $"꽃말: {selected.FlowerMeaning}" 
                       + Environment.NewLine + Environment.NewLine +
                       $"{selected.Description}";

            string historyLine = $"{DateTime.Now},{season},{value},{selected.Name}";
            SaveHistory(historyLine);
        }

        private void SaveHistory(string history)
        {
            try
            {
                string filename = "history.csv";
                File.AppendAllText(filename, history + Environment.NewLine);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"권한이 없어요. \n{ex.Message}", "파일 권한 오류!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"상담 내역 저장 중 오류가 발생했어요. \n{ex.Message}", "알 수 없는 오류!");
            }
        }

        internal void LoadHistory(string historyLine)
        {
            var parts = historyLine.Split(',');
            if (parts.Length < 4) return;

            string datetime = parts[0];
            string season = parts[1];
            string value = parts[2];
            string plantName = parts[3];

            var plant = results.FirstOrDefault(r => r.Name == plantName);
            if (plant != null)
            {
                textBoxResult.Text = $"{plant.Name} ({datetime})"
                                     + Environment.NewLine + Environment.NewLine +
                                     $"계절: {season}"
                                     + Environment.NewLine + Environment.NewLine +
                                     $"중요 가치: {value}"
                                     + Environment.NewLine + Environment.NewLine +
                                     $"꽃말: {plant.FlowerMeaning}"
                                     + Environment.NewLine + Environment.NewLine +
                                     $"{plant.Description}";
            }
            else
            {
                // 식물 정보를 찾지 못한 경우
                textBoxResult.Text = $"선택한 기록: {datetime}, {season}, {value}, {plantName}\n" +
                                     "(식물 정보 없음)";
            }
        }

    }
}
