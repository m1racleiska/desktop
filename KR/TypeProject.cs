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
    public partial class TypeProject : Form
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

        private void CreateColums()
        {
            dataGridView1.Columns.Add("Номер_типа_проекта", "Номер_типа_проекта");
            dataGridView1.Columns.Add("Название", "Название");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[2].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), RowState.ModifiedNew);
        }

        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from Тип_проекта";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        public TypeProject()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void TypeProject_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefresshDataGrid(dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBoxName1.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return; // Прерываем выполнение метода, так как поля не заполнены
            }

            // Подготовка SQL-запроса для вставки данных
            string query = "INSERT INTO Тип_проекта (Название) VALUES (@Name)";

            try
            {
                // Открытие соединения с базой данных
                database.OpenConnection();

                // Подготовка команды SQL
                SqlCommand cmd = new SqlCommand(query, database.getConnection());

                // Задание параметров для предотвращения SQL-инъекций
                cmd.Parameters.AddWithValue("@Name", textBoxName1.Text);


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

        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = $"select * from Тип_проекта where concat (Номер_типа_проекта,Название) like '%" + textBoxSeacrh.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, database.getConnection());

            database.OpenConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);

            }

            read.Close();
        }

        private void textBoxSeacrh_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            int employeeId = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);

            if (IsEmployeeUsed(employeeId))
            {
                MessageBox.Show("Невозможно удалить тип проекта, так как он используется в других записях.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {

                dataGridView1.Rows[index].Cells[2].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[2].Value = RowState.Deleted;
        }

        private void Update()
        {
            database.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[2].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from Тип_проекта where Номер_типа_проекта = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == RowState.ModifiedNew)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();

                    var Name = dataGridView1.Rows[index].Cells[1].Value.ToString();

                    var ChangeQuery = $"update Тип_проекта set Название = '{Name}' where Номер_типа_проекта = '{id}'";

                    var command = new SqlCommand(ChangeQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }


            }
            database.CloseConnection();
        }

        private void Change()
        {
            if (dataGridView1.Rows.Count > 0 && selectedRow >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                row.Cells[1].Value = textBoxName.Text;
            }

        }

        private void ClearFields()
        {
            textBoxID.Text = "";
            textBoxName.Text = "";

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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            textBoxID.ReadOnly = true;
            textBoxID.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0) // сортировка полей 
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxID.Text = row.Cells[0].Value.ToString();
                textBoxName.Text = row.Cells[1].Value.ToString();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1);
        }
        private bool IsEmployeeUsed(int employeeId)
        {
            bool isUsed = false;

            string queryString = $"SELECT COUNT(*) FROM Пакет_услуг WHERE Номер_типа_проекта = {employeeId}";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            int count = (int)command.ExecuteScalar();

            if (count > 0)
            {
                isUsed = true;
            }

            database.CloseConnection();

            return isUsed;
        }
    }
}
