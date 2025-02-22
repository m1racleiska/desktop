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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Forms.ComboBox;

namespace KR
{
    public partial class Project : Form
    {
        DataBase database = new DataBase();
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }
        int selectedRow;

        private void CreateColums()
        {
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns.Add("Номер_проекта", "Номер_проекта");
            dataGridView1.Columns.Add("Название", "Название");
            dataGridView1.Columns.Add("Номер_клиента", "ФИО клиента");
            dataGridView1.Columns.Add("Номер_сотрудника", "ФИО сотрудника");
            dataGridView1.Columns.Add("Номер_пакета_услуг", "Пакет услуг");
            dataGridView1.Columns.Add("Номер_оплаты", "Номер_оплаты");
            dataGridView1.Columns.Add("Номер_рекламного_канала", "Рекламный канал");
            dataGridView1.Columns.Add("Бюджет_проекта", "Бюджет_проекта");
            dataGridView1.Columns.Add("Описание", "Описание");
            dataGridView1.Columns.Add("Дата_начала", "Дата_начала");
            dataGridView1.Columns.Add("Дата_окончания", "Дата_окончания");
            dataGridView1.Columns.Add("Статус_проекта", "Статус_проекта");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[12].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(
                record.IsDBNull(0) ? (object)DBNull.Value : record.GetInt32(0),      // Номер_проекта
                record.IsDBNull(1) ? (object)DBNull.Value : record.GetString(1),     // Название
                record.IsDBNull(2) ? (object)DBNull.Value : record.GetString(2),     // ФИО клиента
                record.IsDBNull(3) ? (object)DBNull.Value : record.GetString(3),     // ФИО сотрудника
                record.IsDBNull(4) ? (object)DBNull.Value : record.GetString(4),     // Название пакета услуг
                record.IsDBNull(5) ? (object)DBNull.Value : record.GetInt32(5),      // Номер оплаты
                record.IsDBNull(6) ? (object)DBNull.Value : record.GetString(6),     // Название рекламного канала
                record.IsDBNull(7) ? (object)DBNull.Value : record.GetInt32(7),      // Бюджет проекта
                record.IsDBNull(8) ? (object)DBNull.Value : record.GetString(8),     // Описание
                record.IsDBNull(9) ? (object)DBNull.Value : record.GetDateTime(9),   // Дата начала
                record.IsDBNull(10) ? (object)DBNull.Value : record.GetDateTime(10),
                record.IsDBNull(11) ? (object)"Неизвестно" : record.GetString(11),   // Статус проекта
                RowState.ModifiedNew                                              // Состояние строки
            );
        }
        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = @"SELECT 
                p.Номер_проекта,
                p.Название,
                ISNULL(c.ФИО, 'Неизвестен') AS ФИО_клиента,
                ISNULL(s.ФИО, 'Неизвестен') AS ФИО_сотрудника,
                ISNULL(pu.Название, 'Неизвестно') AS Название_пакета_услуг,
                ISNULL(p.Номер_оплаты, 0) AS Номер_оплаты,
                ISNULL(rk.Название, 'Не выбран') AS Название_рекламного_канала,
                ISNULL(p.Бюджет_проекта, 0) AS Бюджет_проекта,
                ISNULL(p.Описание, 'Нет описания') AS Описание,
                p.Дата_начала,
                ISNULL(p.Дата_окончания, '1900-01-01') AS Дата_окончания,
                ISNULL(p.Статус_проекта, 'Неизвестно') AS Статус_проекта
            FROM Проект p
            JOIN Клиент c ON p.Номер_клиента = c.Номер_клиента
            JOIN Сотрудник s ON p.Номер_сотрудника = s.Номер_сотрудника
            JOIN Пакет_услуг pu ON p.Номер_пакета_услуг = pu.Номер_пакета_услуг
            LEFT JOIN Рекламные_каналы rk ON p.Номер_рекламного_канала = rk.Номер_рекламного_канала;";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();

        }


        public Project()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            ScaleControls(this, 1.2f);
        }
        private void ScaleControls(Control control, float scaleFactor)
        {
            control.Font = new Font(control.Font.FontFamily, control.Font.Size * scaleFactor, control.Font.Style);
            control.Width = (int)(control.Width * scaleFactor);
            control.Height = (int)(control.Height * scaleFactor);
            control.Location = new Point((int)(control.Location.X * scaleFactor), (int)(control.Location.Y * scaleFactor));

            foreach (Control child in control.Controls)
            {
                ScaleControls(child, scaleFactor);
            }
        }

        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = @"SELECT 
           p.Номер_проекта,
           p.Название,
           c.ФИО AS ФИО_клиента,
           s.ФИО AS ФИО_сотрудника,
           pu.Название AS Название_пакета,
           p.Номер_оплаты,
           rk.Название AS Название_рекламного_канала,
           p.Бюджет_проекта,
           p.Описание,
           p.Дата_начала,
           p.Дата_окончания,
           p.Статус_проекта
       FROM Проект p
       JOIN Клиент c ON p.Номер_клиента = c.Номер_клиента
       JOIN Сотрудник s ON p.Номер_сотрудника = s.Номер_сотрудника
       JOIN Пакет_услуг pu ON p.Номер_пакета_услуг = pu.Номер_пакета_услуг
       JOIN Рекламные_каналы rk ON p.Номер_рекламного_канала = rk.Номер_рекламного_канала
       WHERE CONCAT(p.Номер_проекта, p.Название, c.ФИО, s.ФИО, pu.Название, rk.Название, p.Бюджет_проекта, p.Описание, p.Дата_начала, p.Дата_окончания, p.Статус_проекта) LIKE '%" + textBoxSeacrh.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, database.getConnection());

            database.OpenConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }

        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;


            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {

                dataGridView1.Rows[index].Cells[12].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[12].Value = RowState.Deleted;
        }

        private void Update()
        {
            database.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[12].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    // Обрабатываем удаление строки
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = "DELETE FROM Проект WHERE Номер_проекта = @id";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }

                if (rowState == RowState.ModifiedNew)
                {

                    // Получаем данные из ячеек DataGridView
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);  // Преобразуем id в int
                    var Name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var clientName = dataGridView1.Rows[index].Cells[2].Value.ToString();  // ФИО клиента
                    var workerName = dataGridView1.Rows[index].Cells[3].Value.ToString();  // ФИО сотрудника
                    var packageName = dataGridView1.Rows[index].Cells[4].Value.ToString();  // Название пакета
                    var paymentNumber = dataGridView1.Rows[index].Cells[5].Value.ToString();  // Номер оплаты
                    var channelName = dataGridView1.Rows[index].Cells[6].Value.ToString();  // Название рекламного канала
                    var price = dataGridView1.Rows[index].Cells[7].Value.ToString();  // Бюджет проекта
                    var description = dataGridView1.Rows[index].Cells[8].Value.ToString();  // Описание

                    // Проверяем, что значения для дат корректны
                    DateTime start = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;

                    if (DateTime.TryParse(dataGridView1.Rows[index].Cells[9].Value.ToString(), out start) &&
                        DateTime.TryParse(dataGridView1.Rows[index].Cells[10].Value.ToString(), out end))
                    {
                        // Выполняем запросы для получения ID по значениям (например, ФИО клиента или название пакета)
                        var clientIdQuery = "SELECT Номер_клиента FROM Клиент WHERE ФИО = @clientName";
                        var workerIdQuery = "SELECT Номер_сотрудника FROM Сотрудник WHERE ФИО = @workerName";
                        var packageIdQuery = "SELECT Номер_пакета_услуг FROM Пакет_услуг WHERE Название = @packageName";
                        var channelIdQuery = "SELECT Номер_рекламного_канала FROM Рекламные_каналы WHERE Название = @channelName";

                        // Получаем ID клиента, сотрудника, пакета и канала
                        var clientId = GetIdFromDatabase(clientIdQuery, new SqlParameter("@clientName", clientName));
                        var workerId = GetIdFromDatabase(workerIdQuery, new SqlParameter("@workerName", workerName));
                        var packageId = GetIdFromDatabase(packageIdQuery, new SqlParameter("@packageName", packageName));
                        var channelId = GetIdFromDatabase(channelIdQuery, new SqlParameter("@channelName", channelName));

                        if (clientId == -1 || workerId == -1 || packageId == -1 || channelId == -1)
                        {
                            MessageBox.Show("Один или несколько идентификаторов не найдены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Теперь обновляем запись в таблице Проект с полученными ID
                        var changeQuery = @"UPDATE Проект 
                                    SET Название = @Name, 
                                        Номер_клиента = @clientId, 
                                        Номер_сотрудника = @workerId, 
                                        Номер_пакета_услуг = @packageId, 
                                        Номер_оплаты = @paymentNumber, 
                                        Номер_рекламного_канала = @channelId, 
                                        Бюджет_проекта = @price, 
                                        Описание = @description, 
                                        Дата_начала = @start, 
                                        Дата_окончания = @end, 
                                        Статус_проекта = @status 
                                    WHERE Номер_проекта = @id";

                        var command = new SqlCommand(changeQuery, database.getConnection());
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@clientId", clientId);
                        command.Parameters.AddWithValue("@workerId", workerId);
                        command.Parameters.AddWithValue("@packageId", (object)packageId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@paymentNumber", paymentNumber);
                        command.Parameters.AddWithValue("@channelId", channelId);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@start", start);
                        command.Parameters.AddWithValue("@end", (object)end ?? DBNull.Value);
                        command.Parameters.AddWithValue("@status", dataGridView1.Rows[index].Cells[11].Value.ToString());

                        command.ExecuteNonQuery();
                    }
                    else
                    {

                        MessageBox.Show("Некорректные данные для даты начала или окончания.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            database.CloseConnection();
        }

        private int GetIdFromDatabase(string query, params SqlParameter[] parameters)
        {
            var command = new SqlCommand(query, database.getConnection());
            command.Parameters.AddRange(parameters);

            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
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

                comboBox.DataSource = null;


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


        private void Change()
        {
            if (dataGridView1.Rows.Count > 0 && selectedRow >= 0)
            {
                // Проверяем, заполнены ли поля
                if (!ValidateFields())
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                row.Cells[1].Value = textBoxName.Text;
                row.Cells[2].Value = comboBoxClient.Text;
                row.Cells[3].Value = comboBoxWorked.Text;
                row.Cells[4].Value = comboBoxPack.Text;
                row.Cells[5].Value = comboBoxPay.Text;
                row.Cells[6].Value = comboBoxRK.Text;
                row.Cells[7].Value = textBoxPrice.Text;
                row.Cells[8].Value = textBox1.Text;

                // Проверка правильности ввода даты начала проекта
                DateTime startDate;
                if (DateTime.TryParse(dateTimePicker1.Text, out startDate))
                {
                    row.Cells[9].Value = startDate;
                }
                else
                {
                    MessageBox.Show("Неправильный формат даты начала проекта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Проверка правильности ввода даты окончания проекта
                DateTime endDate;
                if (DateTime.TryParse(dateTimePicker2.Text, out endDate))
                {
                    row.Cells[10].Value = endDate;
                }
                else
                {
                    MessageBox.Show("Неправильный формат даты окончания проекта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                row.Cells[11].Value = comboBox1.Text;
            }

        }

        private void Project_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefresshDataGrid(dataGridView1);
            dataGridView1.AllowUserToAddRows = false;
            // Заполнение ComboBox остается без изменений, так как эти данные могут быть полезны для добавления проектов
            FillComboBoxFromTable("Клиент", comboBoxClient, "ФИО");
            FillComboBoxFromTable("Сотрудник", comboBoxWorked, "ФИО");
            FillComboBoxFromTable("Пакет_услуг", comboBoxPack, "Название");
            FillComboBoxFromTable("Рекламные_каналы", comboBoxRK, "Название");

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            selectedRow = e.RowIndex;

            if (selectedRow >= 0 && selectedRow < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                textBoxId.Text = row.Cells[0].Value.ToString();
                textBoxName.Text = row.Cells[1].Value.ToString();
                comboBoxClient.Text = row.Cells[2].Value.ToString();
                comboBoxWorked.Text = row.Cells[3].Value.ToString();
                comboBoxPack.Text = row.Cells[4].Value.ToString();
                comboBoxPay.Text = row.Cells[5].Value.ToString();
                comboBoxRK.Text = row.Cells[6].Value.ToString();
                textBoxPrice.Text = row.Cells[7].Value.ToString();
                textBox1.Text = row.Cells[8].Value.ToString();
                dateTimePicker1.Text = row.Cells[9].Value.ToString();
                dateTimePicker2.Text = row.Cells[10].Value.ToString();
                comboBox1.Text = row.Cells[11].Value.ToString();
            }
        }
        private void ClearFields()
        {
            textBoxId.Text = "";
            textBoxName.Text = "";
            comboBoxClient.Text = "";
            comboBoxWorked.Text = "";
            comboBoxPack.Text = "";
            comboBoxPay.Text = "";
            comboBoxRK.Text = "";
            textBoxPrice.Text = "";
            textBox1.Text = "";
            dateTimePicker1.Text = "";
            dateTimePicker2.Text = "";
            comboBox1.Text = "";

        }


        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            textBoxId.ReadOnly = true;
            textBoxId.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

     

        private void comboBoxClient_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSeacrh_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteRow();
            ClearFields();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Change();
            /*if (string.IsNullOrEmpty(textBoxId.Text) || string.IsNullOrEmpty(comboBoxClient.Text) || string.IsNullOrEmpty(comboBoxWorked.Text) || string.IsNullOrEmpty(comboBoxPack.Text) || string.IsNullOrEmpty(comboBoxPay.Text) || string.IsNullOrEmpty(comboBoxRK.Text) || string.IsNullOrEmpty(textBoxPrice.Text) || string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(textBoxName.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                database.CloseConnection();
                return; 
            }*/
        }

       

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_project add_Project = new Add_project();
            add_Project.Show();
        }

        private void textBoxId_TextChanged_1(object sender, EventArgs e)
        {
            textBoxId.ReadOnly = true;
            textBoxId.Enabled = false;
        }


        private bool ValidateFields()
        {
            bool isValid = true;

            if (!int.TryParse(textBoxPrice.Text, out int price))
            {
                textBoxPrice.BackColor = Color.Red;
                isValid = false;
                MessageBox.Show("Введите корректное числовое значение для поля 'Бюджет проекта'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBoxPrice.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            // Проверка заполнения ComboBox
            if (string.IsNullOrEmpty(comboBoxClient.Text))
            {
                comboBoxClient.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBoxClient.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            if (string.IsNullOrEmpty(comboBoxWorked.Text))
            {
                comboBoxWorked.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBoxWorked.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }


            if (string.IsNullOrEmpty(comboBoxPack.Text))
            {
                comboBoxPack.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBoxPack.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            if (string.IsNullOrEmpty(comboBoxPay.Text))
            {
                comboBoxPay.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBoxPay.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            if (string.IsNullOrEmpty(comboBoxRK.Text))
            {
                comboBoxRK.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBoxRK.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                comboBox1.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                comboBox1.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            // Проверка заполнения TextBox
            if (string.IsNullOrEmpty(textBoxName.Text))
            {
                textBoxName.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                textBoxName.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            if (string.IsNullOrEmpty(textBoxPrice.Text))
            {
                textBoxPrice.BackColor = Color.Red;
                isValid = false;
            }
            else
            {
                textBoxPrice.BackColor = SystemColors.Window; // Возвращаем стандартный цвет фона
            }

            // Добавьте аналогичные проверки для остальных ComboBox и TextBox

            return isValid;
        }

        private void textBoxPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Payment pay = new Payment();
            pay.Show();
        }

        private void рекламныеКаналыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AD aD = new AD();
            aD.Show();
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            applicationsClients applicationsClients = new applicationsClients();
            applicationsClients.Show();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
