using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KR
{
    public partial class Payment : Form
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
            dataGridView1.Columns.Add("Номер_оплаты", "Номер_оплаты");
            dataGridView1.Columns.Add("Вид_оплаты", "Вид_оплаты");
            dataGridView1.Columns.Add("Дата_оплаты", "Дата_оплаты");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[3].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetDateTime(2), RowState.ModifiedNew);
        }
        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from Оплата";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        public Payment()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = $"select * from Оплата where concat (Номер_оплаты,Вид_оплаты,Дата_оплаты) like '%" + textBoxSeacrh.Text + "%'";

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

            int employeeId = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);

            if (IsEmployeeUsed(employeeId))
            {
                MessageBox.Show("Невозможно удалить оплату, так как он используется в других записях.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {

                dataGridView1.Rows[index].Cells[3].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[3].Value = RowState.Deleted;
        }
        private void Update()
        {
            database.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[3].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from Оплата where Номер_оплаты = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == RowState.ModifiedNew)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();

                    var Type = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var Date = dataGridView1.Rows[index].Cells[2].Value.ToString();

                    var ChangeQuery = $"update Оплата set Вид_оплаты = '{Type}', Дата_оплаты =  '{Date}' where Номер_оплаты = '{id}'";

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
                row.Cells[1].Value = comboBoxPaymentType.Text;
                row.Cells[2].Value = dateTimePicker2.Text;
                DateTime Date;
                if (DateTime.TryParse(dateTimePicker1.Text, out Date))
                {
                    row.Cells[2].Value = Date;
                }
                else
                {
                    MessageBox.Show("Неправильный формат даты начала проекта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

  
;

            }

        }
        private void ClearFields()
        {

            //textBoxType.Text = "";
            dateTimePicker2.Text = "";

        }


        private void textBoxName1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Payment_Load(object sender, EventArgs e)
        {
            comboBoxPaymentType.SelectedIndex = 0;
            CreateColums();
            RefresshDataGrid(dataGridView1);
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClearFields();
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
                comboBoxPaymentType.Text = row.Cells[1].Value.ToString();
                dateTimePicker2.Text = row.Cells[2].Value.ToString();
            }
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1);
        }

        private bool IsEmployeeUsed(int employeeId)
        {
            bool isUsed = false;

            string queryString = $"SELECT COUNT(*) FROM Проект WHERE Номер_оплаты = {employeeId}";

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
        private void InsertRow()
        {
            string type = comboBoxPaymentType.SelectedItem.ToString();
            DateTime date = dateTimePicker2.Value;

            // Получаем значения для новой строки из текстовых полей и элементов управления
          

            // Создаем строку SQL-запроса для вставки новой строки
            string insertQuery = $"INSERT INTO Оплата (Вид_оплаты, Дата_оплаты) VALUES ('{type}', '{date.ToString("yyyy-MM-dd")}');";

            // Выполняем запрос
            SqlCommand command = new SqlCommand(insertQuery, database.getConnection());
            database.OpenConnection();
            command.ExecuteNonQuery();
            database.CloseConnection();

            // Очищаем текстовые поля после добавления новой строки
            ClearFields();

            // Обновляем данные в DataGridView
            RefresshDataGrid(dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsertRow();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
