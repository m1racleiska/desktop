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
    public partial class AD : Form
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

            dataGridView1.Columns.Add("Номер_рекламного_канала", "Номер_рекламного_канала");
            dataGridView1.Columns.Add("Название", "Название");
            dataGridView1.Columns.Add("Цена_размещения", "Цена_размещения");
            dataGridView1.Columns.Add("Номер_типа_рекламного_формата", "Номер_типа_рекламного_формата");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[4].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
           
        }
        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt32(2), record.GetInt32(3), RowState.ModifiedNew);
        }

        private void RefresshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from Рекламные_каналы";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }


        public AD()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AD_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefresshDataGrid(dataGridView1);
            ClearFields();
            LoadComboBoxValues();
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0) // сортировка полей 
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBoxID.Text = row.Cells[0].Value.ToString();
                textBoxName.Text = row.Cells[1].Value.ToString();
                textBoxPrice.Text = row.Cells[2].Value.ToString();
                comboBox1.Text = row.Cells[3].Value.ToString();
            }
        }

        private void Search(DataGridView dgw)// поиск по записям
        {
            dgw.Rows.Clear();

            string searchString = $"select * from Рекламные_каналы where concat (Номер_рекламного_канала,Название, Цена_размещения, Номер_типа_рекламного_формата) like '%" + textBoxSeacrh.Text + "%'";

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
                MessageBox.Show("Невозможно удалить канал, так как он используется в других записях.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var Name = textBoxName.Text;
            int price;
            var id_fr = comboBox1.Text;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBoxPrice.Text, out price))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id, Name, price, id_fr);
                    dataGridView1.Rows[selectedRowIndex].Cells[4].Value = RowState.Modified;
                }
                else
                {
                    MessageBox.Show("Цена должна иметь числовой формат!");
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
                    var deleteQuery = $"delete from Рекламные_каналы where Номер_рекламного_канала = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == RowState.Modified)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();

                    var Name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var price = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var id_fr = dataGridView1.Rows[index].Cells[3].Value.ToString();

                    var ChangeQuery = $"update Рекламные_каналы set Название = '{Name}', Цена_размещения = '{price}', Номер_типа_рекламного_формата = '{id_fr}' where Номер_рекламного_канала = '{id}'";

                    var command = new SqlCommand(ChangeQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }


            }
            database.CloseConnection();
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
            RefresshDataGrid(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add_Ad add_ad = new Add_Ad();
            add_ad.Show();
        }
        private void ClearFields()
        {
            textBoxID.Text = "";
            textBoxName.Text = "";
            textBoxPrice.Text = "";
            comboBox1.Text = "";
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TypeFormat typeFormat = new TypeFormat();
            typeFormat.Show();
        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void LoadComboBoxValues()
        {
            string queryString = "SELECT Номер_типа_рекламного_формата FROM Тип_номера_рекламного_формата";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetInt32(0)); // Добавляем только номера в comboBox
            }

            reader.Close();
            database.CloseConnection();
        }
        private bool IsEmployeeUsed(int employeeId)
        {
            bool isUsed = false;

            string queryString = $"SELECT COUNT(*) FROM Проект WHERE Номер_рекламного_канала = {employeeId}";

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