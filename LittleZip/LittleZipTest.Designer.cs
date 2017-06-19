namespace LittleZipTest
{
    partial class LittleZipTest
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxSouce = new System.Windows.Forms.TextBox();
            this.buttonSouce = new System.Windows.Forms.Button();
            this.textBoxZipFile = new System.Windows.Forms.TextBox();
            this.buttonZipFile = new System.Windows.Forms.Button();
            this.buttonZIP = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textBoxSouce
            // 
            this.textBoxSouce.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSouce.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSouce.Location = new System.Drawing.Point(12, 12);
            this.textBoxSouce.Name = "textBoxSouce";
            this.textBoxSouce.Size = new System.Drawing.Size(539, 22);
            this.textBoxSouce.TabIndex = 8;
            this.textBoxSouce.Text = "Souce";
            // 
            // buttonSouce
            // 
            this.buttonSouce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSouce.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSouce.Location = new System.Drawing.Point(557, 10);
            this.buttonSouce.Name = "buttonSouce";
            this.buttonSouce.Size = new System.Drawing.Size(46, 27);
            this.buttonSouce.TabIndex = 7;
            this.buttonSouce.Text = ". . .";
            this.buttonSouce.UseVisualStyleBackColor = true;
            this.buttonSouce.Click += new System.EventHandler(this.buttonSource_Click);
            // 
            // textBoxZipFile
            // 
            this.textBoxZipFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxZipFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxZipFile.Location = new System.Drawing.Point(12, 45);
            this.textBoxZipFile.Name = "textBoxZipFile";
            this.textBoxZipFile.Size = new System.Drawing.Size(539, 22);
            this.textBoxZipFile.TabIndex = 10;
            this.textBoxZipFile.Text = "ZipFile";
            // 
            // buttonZipFile
            // 
            this.buttonZipFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZipFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZipFile.Location = new System.Drawing.Point(557, 43);
            this.buttonZipFile.Name = "buttonZipFile";
            this.buttonZipFile.Size = new System.Drawing.Size(46, 27);
            this.buttonZipFile.TabIndex = 9;
            this.buttonZipFile.Text = ". . .";
            this.buttonZipFile.UseVisualStyleBackColor = true;
            this.buttonZipFile.Click += new System.EventHandler(this.buttonZipFile_Click);
            // 
            // buttonZIP
            // 
            this.buttonZIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZIP.Location = new System.Drawing.Point(265, 107);
            this.buttonZIP.Name = "buttonZIP";
            this.buttonZIP.Size = new System.Drawing.Size(75, 30);
            this.buttonZIP.TabIndex = 11;
            this.buttonZIP.Text = "ZIP";
            this.buttonZIP.UseVisualStyleBackColor = true;
            this.buttonZIP.Click += new System.EventHandler(this.buttonZIP_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 162);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(615, 23);
            this.progressBar.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 185);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonZIP);
            this.Controls.Add(this.textBoxZipFile);
            this.Controls.Add(this.buttonZipFile);
            this.Controls.Add(this.textBoxSouce);
            this.Controls.Add(this.buttonSouce);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSouce;
        private System.Windows.Forms.Button buttonSouce;
        private System.Windows.Forms.TextBox textBoxZipFile;
        private System.Windows.Forms.Button buttonZipFile;
        private System.Windows.Forms.Button buttonZIP;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

