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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KR
{

    public partial class Client : Form
    {

        DataBase database = new DataBase();

        int selectedRow;

        enum RowState
        {
            Existed,
            New,
            ModifiedNew,
            Deleted
        }

        //создание таблицы под бд
        private void CreateColums()
        {

            dataGridView1.Columns.Add("Номер_клиента", "Номер_клиента");
            dataGridView1.Columns.Add("ФИО", "ФИО");
            dataGridView1.Columns.Add("Номер_телефона", "Номер_телефона");
            dataGridView1.Columns.Add("Электронная_почта", "Электронная_почта");
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
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), RowState.ModifiedNew);
        }

        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from Клиент";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }


        public Client()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Client_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefresshDataGrid(dataGridView1);
            ClearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if(e.RowIndex >= 0) // сортировка полей 
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxID.Text = row.Cells[0].Value.ToString();
                textBoxFIO.Text = row.Cells[1].Value.ToString();
                textBoxNumb.Text = row.Cells[2].Value.ToString();
                textBoxEmail.Text = row.Cells[3].Value.ToString();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Change();
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefresshDataGrid(dataGridView1);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Add_Client addclient = new Add_Client();
            addclient.Show();
        }


        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = $"select * from Клиент where concat (Номер_клиента,ФИО, НОМЕР_ТЕЛЕФОНА, ЭЛЕКТРОННАЯ_ПОЧТА) like '%" + textBoxSeacrh.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, database.getConnection());

            database.OpenConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read()) { 
                ReadSingleRow(dgw, read);

            }

            read.Close();
        }



        private void textBoxSeacrh_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }
        //УДАЛЕНИЕ СТРОКИ 
        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            int employeeId = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);

            if (IsEmployeeUsed(employeeId))
            {
                MessageBox.Show("Невозможно удалить клиента, так как он используется в других записях.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dataGridView1.Rows[index].Visible = false;

            if( dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty) {

                dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;  
                return;
            }

            dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;
        }
        // метод обновление таблицы и сохранение результатов в бд
        private void Update()
        {
            database.OpenConnection();

            for(int index = 0 ; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[4].Value;

                if( rowState == RowState.Existed) 
                    continue;

                if(rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from Клиент where Номер_клиента = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if(rowState == RowState.ModifiedNew) { 
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
         
                    var FIO = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var Numb = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var Email = dataGridView1.Rows[index].Cells[3].Value.ToString();

                    var ChangeQuery = $"update Клиент set ФИО = '{FIO}', Номер_телефона = '{Numb}', Электронная_почта = '{Email}' where Номер_клиента = '{id}'";

                    var command = new SqlCommand(ChangeQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                
            }
            database.CloseConnection();
        }

        private void ClearFields()
        {
            textBoxID.Text = "";
            textBoxFIO.Text = "";
            textBoxNumb.Text = "";
            textBoxEmail.Text = "";
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


        private void Change()
        {
            if (dataGridView1.Rows.Count > 0 && selectedRow >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                row.Cells[1].Value = textBoxFIO.Text;
                row.Cells[2].Value = textBoxNumb.Text;
                row.Cells[3].Value = textBoxEmail.Text;
            }

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
        private bool IsEmployeeUsed(int employeeId)
        {
            bool isUsed = false;

            string queryString = $"SELECT COUNT(*) FROM Проект WHERE Номер_клиента = {employeeId}";

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
