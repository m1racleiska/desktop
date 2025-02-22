using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR
{
    public partial class ReportForm : Form
    {
        DataBase database = new DataBase();
        private string reportText;

        public ReportForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            // Настройка формы
            this.Text = "Отчет";
            this.Width = 600;
            this.Height = 500;

            // Создание заголовка
            Label titleLabel = new Label
            {
                Text = "Официальный отчет компании \"Apex\"",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(100, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Метка для даты отчета
            Label dateLabel = new Label
            {
                Text = $"Дата формирования: {DateTime.Now.ToShortDateString()}",
                Font = new Font("Arial", 10, FontStyle.Italic),
                AutoSize = true,
                Location = new Point(350, 50)
            };
            this.Controls.Add(dateLabel);

            // Создание GroupBox для данных
            GroupBox groupBox = new GroupBox
            {
                Text = "Основные показатели",
                Font = new Font("Arial", 12, FontStyle.Regular),
                Size = new Size(500, 250),
                Location = new Point(50, 100)
            };
            this.Controls.Add(groupBox);

            // Метки для данных
            Label clientsLabel = new Label { Location = new Point(20, 40), AutoSize = true };
            Label employeesLabel = new Label { Location = new Point(20, 80), AutoSize = true };
            Label projectsLabel = new Label { Location = new Point(20, 120), AutoSize = true };

            groupBox.Controls.Add(clientsLabel);
            groupBox.Controls.Add(employeesLabel);
            groupBox.Controls.Add(projectsLabel);

            // Кнопка печати
            Button printButton = new Button
            {
                Text = "Печать",
                Font = new Font("Arial", 10),
                Size = new Size(100, 30),
                Location = new Point(250, 380)
            };
            printButton.Click += PrintButton_Click;
            this.Controls.Add(printButton);

            // Загрузка данных
            LoadData(clientsLabel, employeesLabel, projectsLabel);
        }

        private void LoadData(Label clientsLabel, Label employeesLabel, Label projectsLabel)
        {
            try
            {
                database.OpenConnection();

                // Загрузка данных
                SqlCommand cmdClients = new SqlCommand("SELECT COUNT(*) FROM Клиент", database.getConnection());
                int clientCount = (int)cmdClients.ExecuteScalar();
                clientsLabel.Text = $"Общее количество клиентов: {clientCount}";

                SqlCommand cmdEmployees = new SqlCommand("SELECT COUNT(*) FROM Сотрудник", database.getConnection());
                int employeeCount = (int)cmdEmployees.ExecuteScalar();
                employeesLabel.Text = $"Количество сотрудников: {employeeCount}";

                SqlCommand cmdProjects = new SqlCommand("SELECT COUNT(*) FROM Проект", database.getConnection());
                int projectCount = (int)cmdProjects.ExecuteScalar();
                projectsLabel.Text = $"Текущее количество проектов: {projectCount}";

                // Формирование текста отчета для печати
                reportText = "Официальный отчет компании \"Apex\"\n" +
                             $"Дата формирования: {DateTime.Now.ToShortDateString()}\n\n" +
                             "Основные показатели:\n" +
                             $"  - Общее количество клиентов: {clientCount}\n" +
                             $"  - Количество сотрудников: {employeeCount}\n" +
                             $"  - Текущее количество проектов: {projectCount}\n\n" +
                             "Данный отчет предоставляет сводную информацию о текущей деятельности компании \"Apex\".\n" +
                             "Если у вас возникли вопросы, свяжитесь с отделом аналитики.";
                database.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 800,
                Height = 600
            };

            if (previewDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Рисование текста отчета на странице
            e.Graphics.DrawString(reportText, new Font("Arial", 12), Brushes.Black, new PointF(50, 50));
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
