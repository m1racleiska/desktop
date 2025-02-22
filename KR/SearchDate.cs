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

    

    public partial class SearchDate : Form
    {
        DataBase database = new DataBase();
        public SearchDate()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void SearchDate_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now.Date;
            dateTimePicker2.Value = DateTime.Now.Date;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическое растягивание столбцов по ширине
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Получаем выбранные пользователем даты начала и окончания
            DateTime startDate = dateTimePicker1.Value.Date;
            DateTime endDate = dateTimePicker2.Value.Date;

            // Выполняем SQL-запрос для поиска проектов по датам
            string queryString = $"SELECT Проект.Название, Проект.Дата_начала, Проект.Дата_окончания, Проект.Бюджет_проекта " +
                                 $"FROM Проект " +
                                 $"WHERE Проект.Дата_начала >= @StartDate AND Проект.Дата_окончания <= @EndDate";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            try
            {
                database.OpenConnection();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Проверяем, есть ли данные для отображения
                if (dataTable.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dataTable;
                }
                else
                {
                    MessageBox.Show("Нет проектов в указанный период", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection();
            }
        }
    }
}
