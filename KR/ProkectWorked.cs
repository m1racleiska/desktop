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
    public partial class ProkectWorked : Form
    {
        DataBase database = new DataBase();

        int selectedRow;
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

       
        public ProkectWorked()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void ProkectWorked_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Автоматическое растягивание столбцов по ширине
        }

        private void textBoxSeacrh_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void LoadProjectData(int employeeId)
        {
            string queryString = $"SELECT Сотрудник.ФИО, Сотрудник.Должность, Проект.Название, Проект.Дата_окончания " +
                                 $"FROM Проект " +
                                 $"INNER JOIN Сотрудник ON Проект.Номер_сотрудника = Сотрудник.Номер_сотрудника " +
                                 $"WHERE Проект.Номер_сотрудника = {employeeId}";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());


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
                    MessageBox.Show("Для указанного сотрудника нет проектов", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void LoadComboBoxData()
        {
            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Запрос для получения данных из другой таблицы
                string query = "SELECT Номер_сотрудника FROM Сотрудник";

                // Подготовка команды SQL
                SqlCommand cmd = new SqlCommand(query, database.getConnection());

                // Выполнение SQL-запроса и получение данных
                SqlDataReader reader = cmd.ExecuteReader();

                // Очистка ComboBox перед загрузкой новых данных
                comboBox1.Items.Clear();

                // Добавление данных из запроса в ComboBox
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["Номер_сотрудника"].ToString());
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

            int employeeId;
            if (int.TryParse(comboBox1.Text, out employeeId))
            {
                LoadProjectData(employeeId);
            }
            else
            {
                MessageBox.Show("Введите корректный номер сотрудника", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
    
}    

