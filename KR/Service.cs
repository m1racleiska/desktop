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

namespace KR
{
    public partial class Service : Form
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

            dataGridView1.Columns.Add("Номер_пакета_услуг", "Номер_пакета_услуг");
            dataGridView1.Columns.Add("Название", "Название");
            dataGridView1.Columns.Add("Консультация", "Консультация");
            dataGridView1.Columns.Add("Брендинг", "Брендинг");
            dataGridView1.Columns.Add("Цифровой_маркетинг", "Цифровой_маркетинг");
            dataGridView1.Columns.Add("Номер_типа_проекта", "Номер_типа_проекта");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[6].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4),record.GetInt32(5), RowState.ModifiedNew);
        }
        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from Пакет_услуг";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        public Service()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Service_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefresshDataGrid(dataGridView1);
            ClearFields();
            LoadComboBoxData();
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

        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = $"select * from Пакет_услуг where concat (Номер_пакета_услуг,Название, Брендинг, Цифровой_маркетинг,Номер_типа_проекта, Консультация) like '%" + textBoxSeacrh.Text + "%'";

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
                MessageBox.Show("Невозможно удалить пакет услуг, так как он используется в других записях.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {

                dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
        }

        private void Change()
        {

            if (dataGridView1.Rows.Count > 0 && selectedRow >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                row.Cells[1].Value = textBoxName.Text;
                row.Cells[2].Value = comboBox1.Text;
                row.Cells[3].Value = comboBox2.Text;
                row.Cells[4].Value = comboBox3.Text;
                row.Cells[5].Value = comboBoxType.Text;
            }

        }

        private void Update()
        {
            database.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[6].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from Пакет_услуг where Номер_пакета_услуг = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == RowState.ModifiedNew)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();

                    var Name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var Cons = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var Brend = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var Mark = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var id_pr = dataGridView1.Rows[index].Cells[5].Value.ToString();


                    var ChangeQuery = $"update Пакет_услуг set Название = '{Name}',Консультация = '{Cons}', Брендинг = '{Brend}', Цифровой_маркетинг = '{Mark}', Номер_типа_проекта = '{id_pr}' where Номер_пакета_услуг = '{id}'";

                    var command = new SqlCommand(ChangeQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }


            }
            database.CloseConnection();
        }

        private void ClearFields()
        {
            textBoxID.Text = "";
            textBoxName.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBoxType.Text = "";
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

        private void textBoxSeacrh_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0) // сортировка полей 
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxID.Text = row.Cells[0].Value.ToString();
                textBoxName.Text = row.Cells[1].Value.ToString();
                comboBox1.Text = row.Cells[2].Value.ToString();
                comboBox2.Text = row.Cells[3].Value.ToString();
                comboBox3.Text = row.Cells[4].Value.ToString();
                comboBoxType.Text = row.Cells[5].Value.ToString();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Service add_Service = new Add_Service();
            add_Service.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TypeProject typeProject = new TypeProject();
            typeProject.Show();
        }
        private bool IsEmployeeUsed(int employeeId)
        {
            bool isUsed = false;

            string queryString = $"SELECT COUNT(*) FROM Проект WHERE Номер_пакета_услуг = {employeeId}";

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
