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
    public partial class Add_Service : Form
    {
        DataBase database = new DataBase();
        public Add_Service()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {

        }
        private void LoadComboBoxData()
        {
            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Запрос для получения данных из другой таблицы
                string query = "SELECT Номер_типа_проекта FROM Тип_проекта";

                // Подготовка команды SQL
                SqlCommand cmd = new SqlCommand(query, database.getConnection());

                // Выполнение SQL-запроса и получение данных
                SqlDataReader reader = cmd.ExecuteReader();

                // Очистка ComboBox перед загрузкой новых данных
                comboBoxType.Items.Clear();

                // Добавление данных из запроса в ComboBox
                while (reader.Read())
                {
                    comboBoxType.Items.Add(reader["Номер_типа_проекта"].ToString());
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxName.Text) || string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(comboBox3.Text) || string.IsNullOrEmpty(comboBoxType.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return; // Прерываем выполнение метода, так как поля не заполнены
            }

            // Подготовка SQL-запроса для вставки данных
            string query = "INSERT INTO Пакет_услуг (Название, Консультация, Брендинг,Цифровой_маркетинг,Номер_типа_проекта) VALUES (@Name, @Cons, @Brend, @Mark, @id_pr)";

            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Подготовка команды SQL
                SqlCommand cmd = new SqlCommand(query, database.getConnection());

                // Задание параметров для предотвращения SQL-инъекций
                cmd.Parameters.AddWithValue("@Name", textBoxName.Text);
                cmd.Parameters.AddWithValue("@Cons", comboBox1.Text);
                cmd.Parameters.AddWithValue("@Brend", comboBox2.Text);
                cmd.Parameters.AddWithValue("@Mark", comboBox3.Text);
                cmd.Parameters.AddWithValue("@id_pr", comboBoxType.Text);

                // Выполнение SQL-запроса
                int rowsAffected = cmd.ExecuteNonQuery();

                // Проверка успешности выполнения запроса
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Запись успешно добавлена!");
                }
                else
                {
                    MessageBox.Show("Не удалось добавить запись. Пожалуйста, проверьте данные и попробуйте снова.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                // Закрытие соединения с базой данных после завершения операции
                database.CloseConnection();
            }

        }

        private void Add_Service_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }
    }
}
