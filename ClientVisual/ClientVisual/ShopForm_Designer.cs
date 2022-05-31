namespace SqlQuery
{
    partial class ShopForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxShops = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxPurchase = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPurchase = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxShops
            // 
            this.comboBoxShops.FormattingEnabled = true;
            this.comboBoxShops.Location = new System.Drawing.Point(39, 71);
            this.comboBoxShops.Name = "comboBoxShops";
            this.comboBoxShops.Size = new System.Drawing.Size(204, 32);
            this.comboBoxShops.TabIndex = 0;
            this.comboBoxShops.SelectedIndexChanged += new System.EventHandler(this.comboBoxShops_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Список магазинов";
            // 
            // listBoxPurchase
            // 
            this.listBoxPurchase.FormattingEnabled = true;
            this.listBoxPurchase.ItemHeight = 24;
            this.listBoxPurchase.Location = new System.Drawing.Point(274, 72);
            this.listBoxPurchase.Name = "listBoxPurchase";
            this.listBoxPurchase.Size = new System.Drawing.Size(318, 292);
            this.listBoxPurchase.TabIndex = 2;
            this.listBoxPurchase.SelectedIndexChanged += new System.EventHandler(this.listBoxPurchase_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Покупки";
            // 
            // textBoxPurchase
            // 
            this.textBoxPurchase.Location = new System.Drawing.Point(637, 72);
            this.textBoxPurchase.Multiline = true;
            this.textBoxPurchase.Name = "textBoxPurchase";
            this.textBoxPurchase.Size = new System.Drawing.Size(276, 292);
            this.textBoxPurchase.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(642, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Товары";
            // 
            // ShopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 404);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPurchase);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxPurchase);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxShops);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ShopForm";
            this.Text = "Подключение к БД";
            this.Load += new System.EventHandler(this.ShopForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxShops;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxPurchase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPurchase;
        private System.Windows.Forms.Label label3;
    }
}

