using System;
using System.Windows.Forms;

namespace ClientVisual {
    public partial class MainForm : Form {
        Client client;

        public MainForm() {
            InitializeComponent();

            client = new Client();
        }

        // Обработчик события загрузки формы Load. Подключение - в MainForm::InitializeComponent().
        private void MainForm_Load(object sender, EventArgs e) {
            client.Connect("127.0.0.1", 2020);
            client.Start();

            LoginForm form = new LoginForm(client);
            // Закрыть форму (и приложение).
            if (form.ShowDialog() != DialogResult.OK) Close();
            // Успех авторизации.
            // ...
        }
    }
}
