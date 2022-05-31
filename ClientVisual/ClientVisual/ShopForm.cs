using System;
using System.Collections.Generic;
using System.Data;

using System.Windows.Forms;

using System.Data.SqlClient; // for MS SQL Server

/*
    Connection - подключение к БД (имя сервера, данные подключения: логин+пароль, БД)
 *  Command - для вызова любых SQL-запросов
 *  Adapter - для связи результатов SELECT с графическим интерфейсом через DataTable
 *  Reader  - для последовательной обработки результатов SELECT
 
 */

namespace SqlQuery
{
    public partial class ShopForm : Form
    {
        SqlConnection connection;
        SqlCommand command;

        DataTable table;
        SqlDataAdapter adapter;

        List<int> anPurchaseId;

        public ShopForm()
        {
            InitializeComponent();

            connection = new SqlConnection();
            command = new SqlCommand();
            
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand();
            
            command.Connection = connection;
            adapter.SelectCommand.Connection = connection;

            table = new System.Data.DataTable();

            anPurchaseId = new List<int>();
        }

        public void DbConnect()
        {
            String sConnBase = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\L1mpl\\Documents\\Shops.mdf;Integrated Security=True;Connect Timeout=30";
            bool bOk = false;
            try
            {
                connection.ConnectionString = sConnBase;
                connection.Open();
                bOk = true;
            }
            catch (Exception ex)
            {
                //sRes = "Ошибка подключения:\r\n" + ex.Message;
            }

        }

        private void RefreshShops()
        {
            table.Clear();

            adapter.SelectCommand.CommandText = "SELECT id, name FROM Shops";
            adapter.Fill(table); // неявный вызов SELECT и наполнение выборки table

            comboBoxShops.DataSource = table;
            comboBoxShops.ValueMember = "id";
            comboBoxShops.DisplayMember = "name";
        }

        bool bLoading;

        private void ShopForm_Load(object sender, EventArgs e)
        {
            bLoading = true;

            DbConnect();
            RefreshShops();

            bLoading = false;

            RefreshPurchase();
        }

        private void RefreshPurchase()
        {
            listBoxPurchase.Items.Clear();
            anPurchaseId.Clear();

            int nSelectedShopId = (int)comboBoxShops.SelectedValue;

            command.CommandText = "SELECT id, dt, summ FROM dbo.selectPurchase(" + nSelectedShopId.ToString() + ");";
            
            string s = "";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                DateTime dt = reader.GetDateTime(1);
                s = dt.ToString("dd.MM.yy hh:mm");
                double d = reader.GetDouble(2);
                s += " - " + d.ToString() + " рублей";

                listBoxPurchase.Items.Add(s);
                anPurchaseId.Add(id);
            }
            reader.Close();
            RefreshContent();
        }

        private void comboBoxShops_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bLoading) return;
            RefreshPurchase();
        }

        private void RefreshContent()
        {
            textBoxPurchase.Text = "";

            // Получить индекс выбранного элемента listBoxPurchase.
            // По этому индексу получить идентификатор покупки из anPurchaseId.

            int index = 0;
            index = listBoxPurchase.SelectedIndex;
            int id = 0;
            
            if (index != -1) {
                Console.WriteLine(anPurchaseId[index]);
                
                id = anPurchaseId[index];
            }
            
            // Использовать полученный идентификатор покупки для формирования запроса 3:
            //    Запрос должен использовать данные таблиц purchase, purchase_products и products.
            //    При составлении запроса использовать скрипт табличной функции selectPurchase в качестве примера.
            //    Использовать скалярную функцию getProductNameById для получения имени продукта по его идентификатору:
            //       SELECT dbo.getProductNameById(идентификатор_продукта) AS product_name,...
            
                command.CommandText = "SELECT name, number ,price FROM dbo.GetTableById(" + id.ToString() + ");";
            

            // Выполнить запрос 3.

            // Заполнить textBoxPurchase строками результата выполнения запроса 3:
            //    Каждая строка запроса содержит наименование, количество и цену товара.
            //    Обеспечить вывод строки в требуемом формате (см. рис. 1 и пример в RefreshPurchase()).
            
            string s = "";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                s = reader.GetString(0);
                int num = reader.GetInt32(1);
                
                double cost = reader.GetFloat(2);

                textBoxPurchase.Text += s + " " +  num.ToString() + " шт. х " + cost.ToString() + " руб." + Environment.NewLine;
            }
            reader.Close();
            
        }

        private void listBoxPurchase_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshContent();
        }
    }
}
