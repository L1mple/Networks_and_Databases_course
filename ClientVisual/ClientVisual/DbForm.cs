using System;
using System.Data;
using System.Drawing;

using System.Windows.Forms;
using System.Data.SqlClient;

namespace Query
{
    public partial class DbForm : Form
    {
        private SqlConnection connection;
        private SqlCommand command;
        private DataTable table;
        private SqlDataAdapter adapter;        

        public DbForm()
        {
            InitializeComponent();

            connection = new SqlConnection();
            command = new SqlCommand();

            adapter = new SqlDataAdapter();
			adapter.SelectCommand = new SqlCommand();

            command.Connection = connection;
            adapter.SelectCommand.Connection = connection;

            table = new DataTable();

            ShowResult("Подключение не установлено", false);
        }

        private void ShowResult(string sRes, bool bNoError)
        {
            // Вывести сообщение в поле textBoxRes.
            textBoxRes.Text = "";
            textBoxRes.Text = sRes;
            // Подсветить текст зеленым или красным в зависимости от значения bNoError (атрибут ForeColor)
            if (bNoError == false)
            {
                textBoxRes.ForeColor = Color.Red;
            }
            else { textBoxRes.ForeColor = Color.Green;  }
            
        }

        private void buttonExec_Click(object sender, System.EventArgs e)
        {
            if (connection.State.ToString() != "Open") return;

            String sRes = "";
            bool bOk = false;
            try
            {
                command.CommandText = textBoxQuery.Text;
                int nRes = command.ExecuteNonQuery();
                sRes = "Изменено/добавлено " + nRes.ToString() + " строк";
                bOk = true;
            }
            catch (Exception ex)
            {
                sRes = "Ошибка выполнения команды:\r\n" + ex.Message;
            }
            ShowResult(sRes, bOk);
        }

        private void buttonSelect_Click(object sender, System.EventArgs e)
        {
            if (connection.State.ToString() != "Open") return;

            bool bOk = false;
            string sRes = "";
            try
            {
                table.Clear();         // очистить строки
                table.Columns.Clear(); // очистить столбцы
                                       
                dataGrid.DataSource = null;
                dataGrid.Refresh();

                adapter.SelectCommand.CommandText = textBoxQuery.Text;
                adapter.Fill(table);
                dataGrid.SetDataBinding(table, "");
                dataGrid.Refresh();

                bOk = true;
                sRes = "Выборка выполнена успешно";
            }
            catch (Exception ex)
            {
                sRes = "Ошибка выполнения команды:\r\n" + ex.Message;
            }
            ShowResult(sRes, bOk);
        }

        private void buttonConnect_Click(object sender, System.EventArgs e)
        {
            // Открыть подключение, используя строку подключения. См. 2-ю часть задания.
            // Выдать сообщение об успехе в ShowResult.
            String sConnBase = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\L1mpl\\Documents\\Shops.mdf;Integrated Security=True;Connect Timeout=30";
            bool bOk = false;
            try
            {
                connection.ConnectionString = sConnBase;
                connection.Open();
                bOk = true;
                ShowResult("Подключение установленно", true);
            }
            catch (Exception ex)
            {
                //sRes = "Ошибка подключения:\r\n" + ex.Message;
            }
        }

        private void buttonDisconnect_Click(object sender, System.EventArgs e)
        {
            // Закрыть подключение, сообщение в ShowResult.
            connection.Close();
            ShowResult("Подключение остановленно", false);
        }
    }

}
