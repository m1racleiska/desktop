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
using System.Windows.Forms.VisualStyles;

namespace KR
{
    public partial class SearchStatus : Form
    {
        DataBase database = new DataBase();
        public SearchStatus()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void LoadProjectStatuses()
        {
            // Очистка элементов ComboBox перед загрузкой новых данных
            comboBox1.Items.Clear();

            // SQL-запрос для получения уникальных статусов проектов из таблицы Проект
            string queryString = "SELECT DISTINCT Статус_проекта FROM Проект";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());

            try
            {
                database.OpenConnection();
                SqlDataReader reader = command.ExecuteReader();

                // Заполнение ComboBox статусами проектов
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["Статус_проекта"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статусов проектов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection();
            }
        }

        private void SearchStatus_Load(object sender, EventArgs e)
        {
            LoadProjectStatuses();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическое растягивание столбцов по ширине
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Получаем выбранный пользователем статус проекта
            string selectedStatus = comboBox1.SelectedItem as string;

            // Выполняем SQL-запрос для поиска проектов по выбранному статусу
            string queryString = $"SELECT Проект.Название, Сотрудник.ФИО AS ФИО_сотрудника, Клиент.ФИО AS ФИО_клиента " +
                                 $"FROM Проект " +
                                 $"INNER JOIN Сотрудник ON Проект.Номер_сотрудника = Сотрудник.Номер_сотрудника " +
                                 $"INNER JOIN Клиент ON Проект.Номер_клиента = Клиент.Номер_клиента " +
                                 $"WHERE Проект.Статус_проекта = @Status";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            command.Parameters.AddWithValue("@Status", selectedStatus);

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
                    MessageBox.Show("Нет проектов с выбранным статусом", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
