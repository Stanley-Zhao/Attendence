namespace AutomateGenerator
{
    partial class GenerateData
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerateData));
			this.btnGenerateData = new System.Windows.Forms.Button();
			this.btnClearDatabase = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtCountEmployee = new System.Windows.Forms.TextBox();
			this.txtCountLeave = new System.Windows.Forms.TextBox();
			this.btnGenerateTestData = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnGenerateData
			// 
			this.btnGenerateData.Enabled = false;
			this.btnGenerateData.Location = new System.Drawing.Point(80, 139);
			this.btnGenerateData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnGenerateData.Name = "btnGenerateData";
			this.btnGenerateData.Size = new System.Drawing.Size(154, 28);
			this.btnGenerateData.TabIndex = 0;
			this.btnGenerateData.Text = "Generate Data";
			this.btnGenerateData.UseVisualStyleBackColor = true;
			this.btnGenerateData.Click += new System.EventHandler(this.btnGenerateData_Click);
			// 
			// btnClearDatabase
			// 
			this.btnClearDatabase.Location = new System.Drawing.Point(267, 139);
			this.btnClearDatabase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnClearDatabase.Name = "btnClearDatabase";
			this.btnClearDatabase.Size = new System.Drawing.Size(154, 28);
			this.btnClearDatabase.TabIndex = 1;
			this.btnClearDatabase.Text = "Clear Database";
			this.btnClearDatabase.UseVisualStyleBackColor = true;
			this.btnClearDatabase.Click += new System.EventHandler(this.btnClearDatabase_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(139, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Count of employees";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(286, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Count of leave records for each employee";
			// 
			// txtCountEmployee
			// 
			this.txtCountEmployee.Location = new System.Drawing.Point(352, 7);
			this.txtCountEmployee.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCountEmployee.Name = "txtCountEmployee";
			this.txtCountEmployee.Size = new System.Drawing.Size(132, 23);
			this.txtCountEmployee.TabIndex = 4;
			this.txtCountEmployee.Text = "50";
			// 
			// txtCountLeave
			// 
			this.txtCountLeave.Location = new System.Drawing.Point(352, 44);
			this.txtCountLeave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCountLeave.Name = "txtCountLeave";
			this.txtCountLeave.Size = new System.Drawing.Size(132, 23);
			this.txtCountLeave.TabIndex = 5;
			this.txtCountLeave.Text = "5";
			// 
			// btnGenerateTestData
			// 
			this.btnGenerateTestData.BackColor = System.Drawing.Color.GreenYellow;
			this.btnGenerateTestData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGenerateTestData.Location = new System.Drawing.Point(80, 90);
			this.btnGenerateTestData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnGenerateTestData.Name = "btnGenerateTestData";
			this.btnGenerateTestData.Size = new System.Drawing.Size(154, 28);
			this.btnGenerateTestData.TabIndex = 6;
			this.btnGenerateTestData.Text = "Generate Test Data";
			this.btnGenerateTestData.UseVisualStyleBackColor = false;
			this.btnGenerateTestData.Click += new System.EventHandler(this.btnGenerateTestData_Click);
			// 
			// GenerateData
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 182);
			this.Controls.Add(this.btnGenerateTestData);
			this.Controls.Add(this.txtCountLeave);
			this.Controls.Add(this.txtCountEmployee);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnClearDatabase);
			this.Controls.Add(this.btnGenerateData);
			this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GenerateData";
			this.Text = "GenerateData";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateData;
        private System.Windows.Forms.Button btnClearDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCountEmployee;
        private System.Windows.Forms.TextBox txtCountLeave;
		private System.Windows.Forms.Button btnGenerateTestData;
    }
}