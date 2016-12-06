namespace V3_Trader_Project
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.findIndicators_btn = new System.Windows.Forms.Button();
            this.backtest_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // findIndicators_btn
            // 
            this.findIndicators_btn.Location = new System.Drawing.Point(30, 97);
            this.findIndicators_btn.Name = "findIndicators_btn";
            this.findIndicators_btn.Size = new System.Drawing.Size(156, 72);
            this.findIndicators_btn.TabIndex = 0;
            this.findIndicators_btn.Text = "Find Indicators";
            this.findIndicators_btn.UseVisualStyleBackColor = true;
            this.findIndicators_btn.Click += new System.EventHandler(this.findIndicators_btn_Click);
            // 
            // backtest_btn
            // 
            this.backtest_btn.Location = new System.Drawing.Point(221, 97);
            this.backtest_btn.Name = "backtest_btn";
            this.backtest_btn.Size = new System.Drawing.Size(156, 72);
            this.backtest_btn.TabIndex = 1;
            this.backtest_btn.Text = "Backtest";
            this.backtest_btn.UseVisualStyleBackColor = true;
            this.backtest_btn.Click += new System.EventHandler(this.backtest_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 287);
            this.Controls.Add(this.backtest_btn);
            this.Controls.Add(this.findIndicators_btn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button findIndicators_btn;
        private System.Windows.Forms.Button backtest_btn;
    }
}

