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
    public partial class applicationsClients : Form
    {
        DataBase database = new DataBase();

    

        public applicationsClients()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }
        int selectedRow;

        private void CreateColumns()
        {
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("FIO", "ФИО");
            dataGridView1.Columns.Add("Number", "Номер");
            dataGridView1.Columns.Add("Email", "Электронная почта");
            dataGridView1.Columns.Add("Package", "Пакет");
            dataGridView1.Columns.Add("Type_Project", "Тип проекта");
            dataGridView1.Columns.Add("Name_Project", "Название проекта");
            dataGridView1.Columns.Add("Description", "Описание");
            dataGridView1.Columns.Add("Budget", "Бюджет");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[9].Visible = false;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }


        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0),        // Номер_проекта
            record.GetString(1), // FIO
            record.GetString(2), // Number
            record.GetString(3), // Email
            record.GetString(4), // Package
            record.GetString(5), // Name_Project
            record.GetString(6), // Type_Project
            record.GetString(7), // Description
            record.GetDecimal(8), // Budget
            RowState.ModifiedNew       // Состояние строки
            );
        }
        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from OrderedServices";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }




        private void label8_Click(object sender, EventArgs e)
        {

        }

 
        private void applicationsClients_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefresshDataGrid(dataGridView1);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                {
                    int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                    string deleteQuery = "DELETE FROM OrderedServices WHERE ID = @ID";
                    SqlCommand command = new SqlCommand(deleteQuery, database.getConnection());
                    command.Parameters.AddWithValue("@ID", id);

                    database.OpenConnection();
                    command.ExecuteNonQuery();
                    database.CloseConnection();

                    // Удаляем строку из DataGridView
                    dataGridView1.Rows.Remove(selectedRow);
                }

                MessageBox.Show("Выбранные данные успешно удалены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                database.OpenConnection();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Id"].Value == null) continue;

                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    string fio = row.Cells["FIO"].Value?.ToString();
                    string number = row.Cells["Number"].Value?.ToString();
                    string email = row.Cells["Email"].Value?.ToString();
                    string package = row.Cells["Package"].Value?.ToString();
                    string typeProject = row.Cells["Type_Project"].Value?.ToString();
                    string nameProject = row.Cells["Name_Project"].Value?.ToString();
                    string description = row.Cells["Description"].Value?.ToString();
                    decimal budget = Convert.ToDecimal(row.Cells["Budget"].Value);

                    // Обновляем данные в базе данных
                    string updateQuery = @"
                UPDATE OrderedServices 
                SET FIO = @FIO, Number = @Number, Email = @Email, Package = @Package, 
                    Type_Project = @TypeProject, Name_Project = @NameProject, 
                    Description = @Description, Budget = @Budget 
                WHERE ID = @ID";
                    SqlCommand command = new SqlCommand(updateQuery, database.getConnection());
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@FIO", fio);
                    command.Parameters.AddWithValue("@Number", number);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Package", package);
                    command.Parameters.AddWithValue("@TypeProject", typeProject);
                    command.Parameters.AddWithValue("@NameProject", nameProject);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Budget", budget);

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection();
                RefresshDataGrid(dataGridView1); // Обновляем DataGridView
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для распределения данных!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string fio = selectedRow.Cells["FIO"].Value?.ToString();
            string number = selectedRow.Cells["Number"].Value?.ToString();
            string email = selectedRow.Cells["Email"].Value?.ToString();
            string package = selectedRow.Cells["Package"].Value?.ToString();
            string typeProject = selectedRow.Cells["Type_Project"].Value?.ToString();
            string nameProject = selectedRow.Cells["Name_Project"].Value?.ToString();
            string description = selectedRow.Cells["Description"].Value?.ToString();
            decimal budget = Convert.ToDecimal(selectedRow.Cells["Budget"].Value);

            try
            {
                database.OpenConnection();

                // Вызов окна выбора сотрудника
                using (var selectEmployeeForm = new SelectEmployeeForm())
                {
                    if (selectEmployeeForm.ShowDialog() == DialogResult.OK)
                    {
                        int employeeId = selectEmployeeForm.SelectedEmployeeId;

                        // Вставляем данные в таблицу Клиент
                        string insertClientQuery = "INSERT INTO Клиент (ФИО, Номер_телефона, Электронная_почта) VALUES (@FIO, @Number, @Email)";
                        SqlCommand clientCommand = new SqlCommand(insertClientQuery, database.getConnection());
                        clientCommand.Parameters.AddWithValue("@FIO", fio);
                        clientCommand.Parameters.AddWithValue("@Number", number);
                        clientCommand.Parameters.AddWithValue("@Email", email);
                        clientCommand.ExecuteNonQuery();

                        // Получаем ID нового клиента
                        string getClientIdQuery = "SELECT MAX(Номер_клиента) FROM Клиент";
                        SqlCommand getClientIdCommand = new SqlCommand(getClientIdQuery, database.getConnection());
                        int clientId = (int)getClientIdCommand.ExecuteScalar();

                        // Ищем Номер_пакета_услуг по названию пакета и типу проекта
                        string getPackageIdQuery = @"
                    SELECT Номер_пакета_услуг 
                    FROM Пакет_услуг 
                    WHERE Название = @PackageName AND Номер_типа_проекта = (
                        SELECT Номер_типа_проекта FROM Тип_проекта WHERE Название = @TypeName
                    )";
                        SqlCommand packageCommand = new SqlCommand(getPackageIdQuery, database.getConnection());
                        packageCommand.Parameters.AddWithValue("@PackageName", package);
                        packageCommand.Parameters.AddWithValue("@TypeName", typeProject);
                        object packageIdObj = packageCommand.ExecuteScalar();

                        if (packageIdObj == null)
                        {
                            MessageBox.Show("Не удалось найти подходящий пакет услуг или тип проекта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int packageId = (int)packageIdObj;

                        // Вставляем данные в таблицу Проект
                        string insertProjectQuery = @"
                    INSERT INTO Проект (Номер_клиента, Номер_сотрудника, Номер_пакета_услуг, Название, Описание, Бюджет_проекта, Номер_рекламного_канала, Статус_проекта, Дата_начала) 
                    VALUES (@ClientId, @EmployeeId, @PackageId, @NameProject, @Description, @Budget, @ModerationStatus, @ProjectStatus, @StartDate)";
                        SqlCommand projectCommand = new SqlCommand(insertProjectQuery, database.getConnection());
                        projectCommand.Parameters.AddWithValue("@ClientId", clientId);
                        projectCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                        projectCommand.Parameters.AddWithValue("@PackageId", packageId);
                        projectCommand.Parameters.AddWithValue("@NameProject", nameProject);
                        projectCommand.Parameters.AddWithValue("@Description", description);
                        projectCommand.Parameters.AddWithValue("@Budget", budget);
                        projectCommand.Parameters.AddWithValue("@ModerationStatus", DBNull.Value); // или конкретное числовое значение, если нужно
                        projectCommand.Parameters.AddWithValue("@ProjectStatus", "На модерации"); // Устанавливаем значение "На модерации"
                        projectCommand.Parameters.AddWithValue("@StartDate", DateTime.Now);
                        projectCommand.ExecuteNonQuery();

                        MessageBox.Show("Данные успешно распределены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            finally
            {
                // Закрываем соединение
                database.CloseConnection();
            }
        }

        private void textBoxSeacrh_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBoxSeacrh.Text.Trim().ToLower(); // Получаем текст из текстового поля
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                bool isVisible = false;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText))
                    {
                        isVisible = true;
                        break;
                    }
                }

                row.Visible = isVisible; // Показываем только строки, содержащие текст
            }
        }


    }
}
