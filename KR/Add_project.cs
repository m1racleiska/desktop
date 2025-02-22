using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KR
{
    public partial class Add_project : Form
    {
        DataBase database = new DataBase();
        public Add_project()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Add_project_Load(object sender, EventArgs e)
        {
            FillComboBoxFromTable("Клиент", comboBoxClient, "Номер_клиента");
            FillComboBoxFromTable("Сотрудник", comboBoxWorked, "Номер_сотрудника");
            FillComboBoxFromTable("Пакет_услуг", comboBoxPack, "Номер_пакета_услуг");
            FillComboBoxFromTable("Оплата", comboBoxPay, "Номер_оплаты");
            FillComboBoxFromTable("Рекламные_каналы", comboBoxRK, "Номер_рекламного_канала");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            database.OpenConnection();

            var id_client = comboBoxClient.Text;
            var Name = textBoxName.Text;
            var id_worked = comboBoxWorked.Text;
            var id_py = comboBoxPack.Text;
            var id_plate = comboBoxPay.Text;
            var id_rk = comboBoxRK.Text;
            var price = textBoxPrice.Text;
            var des = textBox1.Text;
            var start = dateTimePicker1.Value; // Здесь используем Value, чтобы получить DateTime
            var end = dateTimePicker2.Value;   // Здесь также используем Value
            var status = comboBox1.Text;

            // Создание SQL-запроса для вставки данных
            string insertQuery = "INSERT INTO Проект (Номер_клиента,Название, Номер_сотрудника, Номер_пакета_услуг, Номер_оплаты, Номер_рекламного_канала, Бюджет_проекта, Описание, Дата_начала, Дата_окончания, Статус_проекта) " +
                                 $"VALUES (@id_client,@Name, @id_worked, @id_py, @id_plate, @id_rk, @price, @des, @start, @end, @status)";

            try
            {
                // Создание команды SQL и добавление параметров
                SqlCommand command = new SqlCommand(insertQuery, database.getConnection());
                command.Parameters.AddWithValue("@id_client", id_client);
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@id_worked", id_worked);
                command.Parameters.AddWithValue("@id_py", id_py);
                command.Parameters.AddWithValue("@id_plate", id_plate);
                command.Parameters.AddWithValue("@id_rk", id_rk);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@des", des);
                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);
                command.Parameters.AddWithValue("@status", status);

                // Выполнение команды
                command.ExecuteNonQuery();

                // Вывод сообщения об успешном добавлении
                MessageBox.Show("Данные успешно добавлены в таблицу.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Обработка исключений при ошибке выполнения запроса
                MessageBox.Show("Ошибка при добавлении данных в таблицу: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Закрытие соединения с базой данных
                database.CloseConnection();
            }
        }
        private void FillComboBoxFromTable(string tableName, ComboBox comboBox, string valueMember)
        {
            // Создание SQL-запроса для выборки только номеров из таблицы
            string selectQuery = $"SELECT {valueMember} FROM {tableName}";

            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Создание команды SQL
                SqlCommand command = new SqlCommand(selectQuery, database.getConnection());

                // Выполнение команды и получение данных
                SqlDataReader reader = command.ExecuteReader();

                // Очистка элементов ComboBox перед добавлением новых данных
                comboBox.Items.Clear();

                // Чтение данных и добавление их в ComboBox
                while (reader.Read())
                {
                    // Добавление номеров в ComboBox
                    comboBox.Items.Add(reader[valueMember].ToString());
                }

                // Закрытие чтения данных
                reader.Close();
            }
            catch (Exception ex)
            {
                // Обработка исключений при ошибке выполнения запроса
                MessageBox.Show("Ошибка при заполнении ComboBox: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Закрытие соединения с базой данных
                database.CloseConnection();
            }
        }


        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
