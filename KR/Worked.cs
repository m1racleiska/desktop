using Mysqlx.Crud;
using MySqlX.XDevAPI.Relational;
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
    public partial class Worked : Form
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

        //создание таблицы под бд
        private void CreateColums()
        {

            dataGridView1.Columns.Add("Номер_сотрудника", "Номер_сотрудника");
            dataGridView1.Columns.Add("ФИО", "ФИО");
            dataGridView1.Columns.Add("Должность", "Должность");
            dataGridView1.Columns.Add("Зарплата", "Зарплата");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[4].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        // считывание данных с бд
        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetInt32(3), RowState.ModifiedNew);
        }

        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from Сотрудник";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        public Worked()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Worked_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefresshDataGrid(dataGridView1);
            ClearFields();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0) // сортировка полей 
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxID.Text = row.Cells[0].Value.ToString();
                textBoxFIO.Text = row.Cells[1].Value.ToString();
                textBoxPost.Text = row.Cells[2].Value.ToString();
                textBoxZP.Text = row.Cells[3].Value.ToString();
            }
        }

        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = $"select * from Сотрудник where concat (Номер_сотрудника,ФИО, Должность, Зарплата) like '%" + textBoxSeacrh.Text + "%'";

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
                MessageBox.Show("Невозможно удалить сотрудника, так как он используется в других записях.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;  

            var id = textBoxID.Text;
            var FIO = textBoxFIO.Text;
            var Post = textBoxPost.Text;
            int ZP;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBoxZP.Text, out ZP))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id, FIO,Post, ZP);
                    dataGridView1.Rows[selectedRowIndex].Cells[4].Value = RowState.Modified;
                }
                else
                {
                    MessageBox.Show("Зарплата должна иметь числовой формат!");
                }
            }

        }

        // метод обновление таблицы и сохранение результатов в бд
        private void Update()
        {
            database.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[4].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from Сотрудник where Номер_сотрудника = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == RowState.Modified)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();

                    var FIO = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var Post = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var ZP = dataGridView1.Rows[index].Cells[3].Value.ToString();

                    var ChangeQuery = $"update Сотрудник set ФИО = '{FIO}', Должность = '{Post}', Зарплата = '{ZP}' where Номер_сотрудника = '{id}'";

                    var command = new SqlCommand(ChangeQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }


            }
            database.CloseConnection();
        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            textBoxID.ReadOnly = true;
            textBoxID.Enabled= false;
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

        private void button2_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1 );
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Worked add_Worked = new Add_Worked();
            add_Worked.Show();  
        }

        private void ClearFields()
        {
            textBoxID.Text = "";
            textBoxFIO.Text = "";
            textBoxPost.Text = "";
            textBoxZP.Text = "";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private bool IsEmployeeUsed(int employeeId)
        {
            bool isUsed = false;

            string queryString = $"SELECT COUNT(*) FROM Проект WHERE Номер_сотрудника = {employeeId}";

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
