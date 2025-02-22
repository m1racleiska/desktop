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
    public partial class Add_Ad : Form
    {

        DataBase database = new DataBase();
        public Add_Ad()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Add_Ad_Load(object sender, EventArgs e)
        {
            FillComboBoxFromTable("Тип_номера_рекламного_формата", comboBox1, "Номер_типа_рекламного_формата");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            database.OpenConnection();

            var Name = textBoxName1.Text;
            var price = textBoxPrice1.Text;
            var id_fr = comboBox1.Text;

            if (string.IsNullOrEmpty(textBoxName1.Text) || string.IsNullOrEmpty(textBoxPrice1.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                database.CloseConnection();
                return; // Прерываем выполнение метода, так как поля не заполнены
            }

            if (int.TryParse(textBoxPrice1.Text, out int parsedPrice))
            {
                var addQuerry = $"INSERT INTO Рекламные_каналы (Название, Цена_размещения, Номер_типа_рекламного_формата) VALUES ('{Name}', '{parsedPrice}', '{id_fr}')";

                try
                {
                    var command = new SqlCommand(addQuerry, database.getConnection());
                    command.ExecuteNonQuery();
                    MessageBox.Show("Запись успешно добавлена!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Цена должна иметь числовой формат!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            database.CloseConnection();

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
