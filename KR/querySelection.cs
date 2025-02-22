using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR
{
    public partial class querySelection : Form
    {
        DataBase database = new DataBase();

        private string searchMode = ""; // Текущий режим поиска


        public querySelection()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            panel1.Visible = false;


        }

        private void SetupDataGridView()
        {
            // Устанавливаем стиль для DataGridView
            dataGridView1.BorderStyle = BorderStyle.FixedSingle; // Добавляем рамку для аккуратности
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245); // Светло-серый фон для чередующихся строк
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Тонкие горизонтальные границы
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 228, 255); // Мягкий синий цвет при выделении
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black; // Чёрный текст при выделении
            dataGridView1.BackgroundColor = Color.White; // Белый фон таблицы
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230); // Светло-серый фон заголовков
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black; // Чёрный текст заголовков
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); // Уменьшенный шрифт заголовков
            dataGridView1.RowTemplate.Height = 30; // Оптимальная высота строк
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular); // Уменьшенный шрифт ячеек
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black; // Чёрный текст в строках

            // Устанавливаем авторазмер колонок
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Выравнивание заголовков и ячеек
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Отключаем видимые границы по умолчанию для улучшенного вида
            dataGridView1.GridColor = Color.FromArgb(220, 220, 220); // Светло-серые границы

            // Убираем возможность редактирования ячеек по умолчанию
            dataGridView1.ReadOnly = true;
        }




        private void LoadComboBoxData()
        {
            comboBox1.Items.Clear();

            if (searchMode == "status")
            {
                string query = "SELECT DISTINCT Статус_проекта FROM Проект";
                SqlCommand command = new SqlCommand(query, database.getConnection());

                try
                {
                    database.OpenConnection();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["Статус_проекта"].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке статусов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    database.CloseConnection();
                }
            }
            else if (searchMode == "client")
            {
                string query = "SELECT ФИО FROM Клиент";
                SqlCommand command = new SqlCommand(query, database.getConnection());

                try
                {
                    database.OpenConnection();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["ФИО"].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    database.CloseConnection();
                }
            }
            else if (searchMode == "payment")
            {
                string queryString = "SELECT DISTINCT Вид_оплаты FROM Оплата";

                SqlCommand command = new SqlCommand(queryString, database.getConnection());

                try
                {
                    database.OpenConnection();
                    SqlDataReader reader = command.ExecuteReader();

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
            else if (searchMode == "Worked")
            {
                try
                {
                    database.OpenConnection();

                    string query = "SELECT Номер_сотрудника FROM Сотрудник";
                    SqlCommand cmd = new SqlCommand(query, database.getConnection());
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["Номер_сотрудника"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при загрузке данных ComboBox: " + ex.Message);
                }
                finally
                {
                    database.CloseConnection();
                }
            }
        }

        private void ExecuteSearch(string query, SqlParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(query, database.getConnection());
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            try
            {
                database.OpenConnection();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dataTable;
                }
                else
                {
                    MessageBox.Show("Данные не найдены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection();
            }
        }

        private void LoadProjectData(int employeeId)
        {
            string queryString = "SELECT Сотрудник.ФИО, Сотрудник.Должность, Проект.Название, Проект.Дата_окончания " +
                                 "FROM Проект " +
                                 "INNER JOIN Сотрудник ON Проект.Номер_сотрудника = Сотрудник.Номер_сотрудника " +
                                 "WHERE Проект.Номер_сотрудника = @EmployeeId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmployeeId", employeeId)
            };

            ExecuteSearch(queryString, parameters);
        }

        private void querySelection_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SetupDataGridView();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            searchMode = "status";
            panel1.Visible = false; // Скрываем панель выбора дат
            label1.Text = "Введите статус проекта:"; // Меняем текст подсказки
            comboBox1.Visible = true; // Показываем ComboBox
            LoadComboBoxData(); // Загружаем данные в ComboBox
        }


        private void button5_Click(object sender, EventArgs e)
        {
            searchMode = "client";
            panel1.Visible = false; // Скрываем панель выбора дат
            label1.Text = "Введите ФИО клиента:"; // Меняем текст подсказки
            comboBox1.Visible = true; // Показываем ComboBox
            LoadComboBoxData(); // Загружаем данные в ComboBox
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрано ли значение в ComboBox (для поиска по статусу, клиенту или типу оплаты)
            if ((searchMode == "status" || searchMode == "client" || searchMode == "payment") && string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Выберите значение из списка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (searchMode == "status")
            {
                string query = "SELECT Проект.Название, Сотрудник.ФИО AS Сотрудник, Клиент.ФИО AS Клиент " +
                               "FROM Проект " +
                               "INNER JOIN Сотрудник ON Проект.Номер_сотрудника = Сотрудник.Номер_сотрудника " +
                               "INNER JOIN Клиент ON Проект.Номер_клиента = Клиент.Номер_клиента " +
                               "WHERE Проект.Статус_проекта = @Status";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@Status", comboBox1.Text)
                };

                ExecuteSearch(query, parameters);
            }
            else if (searchMode == "client")
            {
                string query = "SELECT Проект.Название, Проект.Бюджет_проекта AS Бюджет, Пакет_услуг.Консультация, Пакет_услуг.Брендинг, Пакет_услуг.Цифровой_маркетинг AS Маркетинг " +
                               "FROM Проект " +
                               "INNER JOIN Клиент ON Проект.Номер_клиента = Клиент.Номер_клиента " +
                               "INNER JOIN Пакет_услуг ON Проект.Номер_пакета_услуг = Пакет_услуг.Номер_пакета_услуг " +
                               "WHERE Клиент.ФИО = @ClientName";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@ClientName", comboBox1.Text)
                };

                ExecuteSearch(query, parameters);
            }
            else if (searchMode == "date")
            {
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date;

                string query = "SELECT Проект.Название, Проект.Дата_начала AS Начало, Проект.Дата_окончания AS Начало, Проект.Бюджет_проекта AS Бюджет  " +
                               "FROM Проект " +
                               "WHERE Проект.Дата_начала >= @StartDate AND Проект.Дата_окончания <= @EndDate";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate)
                };

                ExecuteSearch(query, parameters);
            }
            else if (searchMode == "payment")
            {
                string selectedPaymentType = comboBox1.SelectedItem as string;
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date;

                string query = "SELECT Проект.Название, Клиент.ФИО AS Клиент, Пакет_услуг.Номер_пакета_услуг AS Пакет " +
                               "FROM Проект " +
                               "INNER JOIN Клиент ON Проект.Номер_клиента = Клиент.Номер_клиента " +
                               "INNER JOIN Пакет_услуг ON Проект.Номер_пакета_услуг = Пакет_услуг.Номер_пакета_услуг " +
                               "INNER JOIN Оплата ON Проект.Номер_оплаты = Оплата.Номер_оплаты " +
                               "WHERE Оплата.Вид_оплаты = @PaymentType " +
                               "AND Оплата.Дата_оплаты BETWEEN @StartDate AND @EndDate";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@PaymentType", selectedPaymentType),
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate)
                };

                ExecuteSearch(query, parameters);
            }
            else if (searchMode == "Worked")
            {
                if (int.TryParse(comboBox1.Text, out int employeeId))
                {
                    LoadProjectData(employeeId);
                }
                else
                {
                    MessageBox.Show("Введите корректный номер сотрудника", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите режим поиска!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            searchMode = "date"; // Устанавливаем режим поиска по дате
            panel1.Visible = true; // Показываем панель выбора дат
            label1.Text = "Выберите период:"; // Меняем текст подсказки
            comboBox1.Visible = false; // Скрываем ComboBox
        }

        private void button4_Click(object sender, EventArgs e)
        {
        searchMode = "payment"; // Устанавливаем режим поиска по типу оплаты
        panel1.Visible = true; // Показываем панель выбора дат
        label1.Text = "Выберите тип оплаты и период:"; // Меняем текст подсказки
        comboBox1.Visible = true; // Показываем ComboBox для типов оплаты
        LoadComboBoxData(); // Загружаем типы оплаты в ComboBox
        }

        private void button1_Click(object sender, EventArgs e)
        {
            searchMode = "Worked";
            panel1.Visible = false; // Скрываем панель выбора дат
            label1.Text = "Введите номер сотрудника:"; // Меняем текст подсказки
            comboBox1.Visible = true; // Показываем ComboBox
            LoadComboBoxData(); // Загружаем данные в ComboBox
        }
    }
    
    
}
