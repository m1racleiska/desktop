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
    public partial class SearchPayment : Form
    {
        DataBase database = new DataBase();
        public SearchPayment()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
        private void LoadPaymentTypes()
        {
            // Очистка элементов ComboBox перед загрузкой новых данных
            comboBox1.Items.Clear();

            // SQL-запрос для получения уникальных типов оплаты из таблицы Оплата
            string queryString = "SELECT DISTINCT Вид_оплаты FROM Оплата";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());

            try
            {
                database.OpenConnection();
                SqlDataReader reader = command.ExecuteReader();

                // Заполнение ComboBox типами оплаты
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["Вид_оплаты"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов оплаты: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection();
            }
        }

        private void SearchPayment_Load(object sender, EventArgs e)
        {
            LoadPaymentTypes();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическое растягивание столбцов по ширине
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Получаем выбранный пользователем тип оплаты
            string selectedPaymentType = comboBox1.SelectedItem as string;

            // Получаем выбранные пользователем даты
            DateTime startDate = dateTimePicker2.Value;
            DateTime endDate = dateTimePicker1.Value;

            // Выполняем SQL-запрос для поиска проектов по выбранным параметрам
            string queryString = $"SELECT Проект.Название, Клиент.ФИО AS ФИО_клиента, Пакет_услуг.Номер_пакета_услуг " +
                                 $"FROM Проект " +
                                 $"INNER JOIN Клиент ON Проект.Номер_клиента = Клиент.Номер_клиента " +
                                 $"INNER JOIN Пакет_услуг ON Проект.Номер_пакета_услуг = Пакет_услуг.Номер_пакета_услуг " +
                                 $"INNER JOIN Оплата ON Проект.Номер_оплаты = Оплата.Номер_оплаты " +
                                 $"WHERE Оплата.Вид_оплаты = @PaymentType " +
                                 $"AND Оплата.Дата_оплаты BETWEEN @StartDate AND @EndDate";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            command.Parameters.AddWithValue("@PaymentType", selectedPaymentType);
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
                    MessageBox.Show("Нет проектов с выбранными параметрами", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
