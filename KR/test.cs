using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KR
{
    public partial class test : Form
    {
        private DataBase database = new DataBase();
        private string searchMode = ""; // Текущий режим поиска
        public test()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            ConfigureUI();
        }
        private void ConfigureUI()
        {
            this.Text = "Выбор Запроса";
            this.Size = new Size(800, 600);
            this.BackColor = Color.WhiteSmoke;

            // Панель для кнопок
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Padding = new Padding(10);
            buttonPanel.AutoSize = true;
            buttonPanel.BackColor = Color.LightGray;

            // Добавление кнопок
            buttonPanel.Controls.Add(CreateStyledButton("Поиск по статусу", ButtonSearchByStatus_Click));
            buttonPanel.Controls.Add(CreateStyledButton("Поиск по клиенту", ButtonSearchByClient_Click));
            buttonPanel.Controls.Add(CreateStyledButton("Поиск по дате", ButtonSearchByDate_Click));
            buttonPanel.Controls.Add(CreateStyledButton("Поиск по оплате", ButtonSearchByPayment_Click));
            buttonPanel.Controls.Add(CreateStyledButton("Поиск по сотруднику", ButtonSearchByEmployee_Click));

            this.Controls.Add(buttonPanel);

            // DataGridView
            dataGridView1 = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.Navy,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    Padding = new Padding(5)
                },
                GridColor = Color.LightGray,
                RowTemplate = { Height = 40 },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.AliceBlue
                }
            };
            this.Controls.Add(dataGridView1);

            // Панель для дополнительных элементов
            panel1 = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                Visible = false
            };

            label1 = new Label
            {
                Text = "Введите значение:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel1.Controls.Add(label1);

            comboBox1 = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(5)
            };
            panel1.Controls.Add(comboBox1);

            dateTimePicker1 = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Left,
                Width = 120,
                Visible = false
            };
            dateTimePicker2 = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Right,
                Width = 120,
                Visible = false
            };
            panel1.Controls.Add(dateTimePicker1);
            panel1.Controls.Add(dateTimePicker2);

            this.Controls.Add(panel1);
        }

        private Button CreateStyledButton(string text, EventHandler onClick)
        {
            Button button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 50,
                Width = 200,
                Margin = new Padding(10)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += onClick;
            return button;
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

        private void ButtonSearchByStatus_Click(object sender, EventArgs e)
        {
            searchMode = "status";
            panel1.Visible = true;
            label1.Text = "Введите статус проекта:";
            comboBox1.Visible = true;
            LoadComboBoxData("SELECT DISTINCT Статус_проекта FROM Проект", "Статус_проекта");
        }

        private void ButtonSearchByClient_Click(object sender, EventArgs e)
        {
            searchMode = "client";
            panel1.Visible = true;
            label1.Text = "Введите ФИО клиента:";
            comboBox1.Visible = true;
            LoadComboBoxData("SELECT ФИО FROM Клиент", "ФИО");
        }

        private void ButtonSearchByDate_Click(object sender, EventArgs e)
        {
            searchMode = "date";
            panel1.Visible = true;
            label1.Text = "Выберите период:";
            comboBox1.Visible = false;
            dateTimePicker1.Visible = true;
            dateTimePicker2.Visible = true;
        }

        private void ButtonSearchByPayment_Click(object sender, EventArgs e)
        {
            searchMode = "payment";
            panel1.Visible = true;
            label1.Text = "Выберите тип оплаты:";
            comboBox1.Visible = true;
            LoadComboBoxData("SELECT DISTINCT Вид_оплаты FROM Оплата", "Вид_оплаты");
        }

        private void ButtonSearchByEmployee_Click(object sender, EventArgs e)
        {
            searchMode = "Worked";
            panel1.Visible = true;
            label1.Text = "Введите номер сотрудника:";
            comboBox1.Visible = true;
            LoadComboBoxData("SELECT Номер_сотрудника FROM Сотрудник", "Номер_сотрудника");
        }

        private void LoadComboBoxData(string query, string columnName)
        {
            comboBox1.Items.Clear();

            try
            {
                database.OpenConnection();
                SqlCommand command = new SqlCommand(query, database.getConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader[columnName].ToString());
                }
                reader.Close();
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

        private DataGridView dataGridView1;
        private Panel panel1;
        private Label label1;
        private ComboBox comboBox1;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;

        private void test_Load(object sender, EventArgs e)
        {

        }
    }
}
