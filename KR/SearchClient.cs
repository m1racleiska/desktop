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
using System.Data.SqlClient;

namespace KR
{
    public partial class SearchClient : Form
    {
        DataBase database = new DataBase();
        public SearchClient()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void SearchClient_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическое растягивание столбцов по ширине
        }
        private void LoadComboBoxData()
        {
            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Запрос для получения данных из таблицы Клиент
                string query = "SELECT ФИО FROM Клиент";

                // Подготовка команды SQL
                SqlCommand cmd = new SqlCommand(query, database.getConnection());

                // Выполнение SQL-запроса и получение данных
                SqlDataReader reader = cmd.ExecuteReader();

                // Очистка ComboBox перед загрузкой новых данных
                comboBox1.Items.Clear();

                // Добавление данных из запроса в ComboBox
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["ФИО"].ToString());
                }

                // Закрытие SqlDataReader
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных ComboBox: " + ex.Message);
            }
            finally
            {
                // Закрытие соединения с базой данных
                database.CloseConnection();
            }
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Получаем выбранное ФИО клиента
            string clientName = comboBox1.Text;

            // SQL-запрос для поиска проектов клиента
            string query = "SELECT Проект.Название, Проект.Бюджет_проекта, Пакет_услуг.Консультация, Пакет_услуг.Брендинг, Пакет_услуг.Цифровой_маркетинг " +
                           "FROM Проект " +
                           "INNER JOIN Клиент ON Проект.Номер_клиента = Клиент.Номер_клиента " +
                           "INNER JOIN Пакет_услуг ON Проект.Номер_пакета_услуг = Пакет_услуг.Номер_пакета_услуг " +
                           "WHERE Клиент.ФИО = @ClientName";

            // Создание объекта команды SQL с параметром
            SqlCommand command = new SqlCommand(query, database.getConnection());
            command.Parameters.AddWithValue("@ClientName", clientName);

            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Создание адаптера данных и заполнение DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Отображение данных в DataGridView
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении запроса: " + ex.Message);
            }
            finally
            {
                // Закрытие соединения с базой данных
                database.CloseConnection();
            }
        }
    }
}
