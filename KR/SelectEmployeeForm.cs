using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR
{
    public partial class SelectEmployeeForm : Form
    {
        private DataBase database = new DataBase();
        public int SelectedEmployeeId { get; private set; }
        public string SelectedEmployeeName { get; private set; }

        public SelectEmployeeForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            // Загрузка данных при инициализации формы
            Load += SelectEmployeeForm_Load;

            // Добавление обработчика двойного щелчка
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
        }

        private void SelectEmployeeForm_Load(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                // Обновленный SQL-запрос для получения ФИО сотрудника и количества прикрепленных проектов
                string query = @"
            SELECT 
                Сотрудник.Номер_сотрудника, 
                Сотрудник.ФИО, 
                COUNT(Проект.Номер_проекта) AS Нагрузка
            FROM Сотрудник
            LEFT JOIN Проект ON Сотрудник.Номер_сотрудника = Проект.Номер_сотрудника
            GROUP BY Сотрудник.Номер_сотрудника, Сотрудник.ФИО";

                SqlDataAdapter adapter = new SqlDataAdapter(query, database.getConnection());
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = table;

                // Настройка DataGridView
                dataGridView1.Columns["Номер_сотрудника"].Visible = false; // Скрываем ID
                dataGridView1.Columns["ФИО"].HeaderText = "ФИО";
                dataGridView1.Columns["Нагрузка"].HeaderText = "Количество проектов";

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Проверяем, что строка выбрана
            {
                SelectedEmployeeId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Номер_сотрудника"].Value);
                SelectedEmployeeName = dataGridView1.Rows[e.RowIndex].Cells["ФИО"].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SelectEmployeeForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
