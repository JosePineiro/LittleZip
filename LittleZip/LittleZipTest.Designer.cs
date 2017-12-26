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
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.buttonSouce = new System.Windows.Forms.Button();
            this.textBoxZipFile = new System.Windows.Forms.TextBox();
            this.buttonZipFile = new System.Windows.Forms.Button();
            this.buttonZip = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textBoxSource
            // 
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSource.Location = new System.Drawing.Point(12, 12);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(539, 22);
            this.textBoxSource.TabIndex = 8;
            this.textBoxSource.Text = "Source";
            // 
            // buttonSouce
            // 
            this.buttonSouce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSouce.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSouce.Location = new System.Drawing.Point(557, 12);
            this.buttonSouce.Name = "buttonSouce";
            this.buttonSouce.Size = new System.Drawing.Size(46, 22);
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
            this.textBoxZipFile.Location = new System.Drawing.Point(12, 40);
            this.textBoxZipFile.Name = "textBoxZipFile";
            this.textBoxZipFile.Size = new System.Drawing.Size(539, 22);
            this.textBoxZipFile.TabIndex = 10;
            this.textBoxZipFile.Text = "ZipFile";
            // 
            // buttonZipFile
            // 
            this.buttonZipFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZipFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZipFile.Location = new System.Drawing.Point(557, 40);
            this.buttonZipFile.Name = "buttonZipFile";
            this.buttonZipFile.Size = new System.Drawing.Size(46, 22);
            this.buttonZipFile.TabIndex = 9;
            this.buttonZipFile.Text = ". . .";
            this.buttonZipFile.UseVisualStyleBackColor = true;
            this.buttonZipFile.Click += new System.EventHandler(this.buttonZipFile_Click);
            // 
            // buttonZip
            // 
            this.buttonZip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZip.Location = new System.Drawing.Point(276, 82);
            this.buttonZip.Name = "buttonZip";
            this.buttonZip.Size = new System.Drawing.Size(75, 30);
            this.buttonZip.TabIndex = 11;
            this.buttonZip.Text = "ZIP";
            this.buttonZip.UseVisualStyleBackColor = true;
            this.buttonZip.Click += new System.EventHandler(this.buttonZIP_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 136);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(615, 23);
            this.progressBar.TabIndex = 12;
            // 
            // LittleZipTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 159);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonZip);
            this.Controls.Add(this.textBoxZipFile);
            this.Controls.Add(this.buttonZipFile);
            this.Controls.Add(this.textBoxSource);
            this.Controls.Add(this.buttonSouce);
            this.Name = "LittleZipTest";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.LittleZipTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Button buttonSouce;
        private System.Windows.Forms.TextBox textBoxZipFile;
        private System.Windows.Forms.Button buttonZipFile;
        private System.Windows.Forms.Button buttonZip;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

