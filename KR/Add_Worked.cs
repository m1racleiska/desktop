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

namespace KR
{


    public partial class Add_Worked : Form
    {

        DataBase database = new DataBase();

        public Add_Worked()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            database.OpenConnection();

            var FIO = textBoxFIO1.Text;
            var Post = textBoxPost1.Text;
            int ZP;

            if (string.IsNullOrEmpty(textBoxFIO1.Text) || string.IsNullOrEmpty(textBoxPost1.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return; // Прерываем выполнение метода, так как поля не заполнены
            }

            if (int.TryParse(textBoxZP1.Text, out ZP))
            {
                var addQuerry = $"insert into Сотрудник (ФИО, Должность, Зарплата) values ('{FIO}', '{Post}', '{ZP}') ";

                var command = new SqlCommand(addQuerry, database.getConnection());
                command.ExecuteNonQuery();

                MessageBox.Show("Запись успешно добавлена!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                MessageBox.Show("Зарплта должна иметь числовой формат!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            database.CloseConnection();



        }

        private void Add_Worked_Load(object sender, EventArgs e)
        {

        }
    }
}
