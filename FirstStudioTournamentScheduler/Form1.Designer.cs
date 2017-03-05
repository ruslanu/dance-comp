namespace FirstStudioTournamentScheduler
{
    partial class Form1
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
            this.btn_ReadParticipants = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_ReadParticipants
            // 
            this.btn_ReadParticipants.Location = new System.Drawing.Point(221, 132);
            this.btn_ReadParticipants.Name = "btn_ReadParticipants";
            this.btn_ReadParticipants.Size = new System.Drawing.Size(166, 23);
            this.btn_ReadParticipants.TabIndex = 0;
            this.btn_ReadParticipants.Text = "Read Participants";
            this.btn_ReadParticipants.UseVisualStyleBackColor = true;
            this.btn_ReadParticipants.Click += new System.EventHandler(this.btn_ReadParticipants_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 286);
            this.Controls.Add(this.btn_ReadParticipants);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_ReadParticipants;
    }
}

