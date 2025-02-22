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

    
    public partial class Add_Client : Form
    {
        DataBase database = new DataBase();
        public Add_Client()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBoxFIO1.Text) || string.IsNullOrEmpty(textBoxNumb1.Text) || string.IsNullOrEmpty(textBoxEmail1.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return; // Прерываем выполнение метода, так как поля не заполнены
            }

            // Подготовка SQL-запроса для вставки данных
            string query = "INSERT INTO Клиент (ФИО, НОМЕР_ТЕЛЕФОНА, ЭЛЕКТРОННАЯ_ПОЧТА) VALUES (@FIO, @Numb, @Email)";

            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Подготовка команды SQL
                SqlCommand cmd = new SqlCommand(query, database.getConnection());

                // Задание параметров для предотвращения SQL-инъекций
                cmd.Parameters.AddWithValue("@FIO", textBoxFIO1.Text);
                cmd.Parameters.AddWithValue("@Numb", textBoxNumb1.Text);
                cmd.Parameters.AddWithValue("@Email", textBoxEmail1.Text);

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

        private void Add_Client_Load(object sender, EventArgs e)
        {

        }
    }
}
