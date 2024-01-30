namespace TS4DefaultEyesFixer
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Shine_checkBox = new System.Windows.Forms.CheckBox();
            this.Copy_checkBox = new System.Windows.Forms.CheckBox();
            this.TextureOnly_radioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CASPs_radioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(116, 136);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(309, 51);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select Default Replacement Package";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(113, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 60);
            this.label1.TabIndex = 1;
            this.label1.Text = "Fixes default replacements broken by patches, including\r\neyes, eyebrows, makeup, " +
    "and skins.\r\n\r\nDefault skins must use the texture-only option.";
            // 
            // Shine_checkBox
            // 
            this.Shine_checkBox.AutoSize = true;
            this.Shine_checkBox.Location = new System.Drawing.Point(122, 293);
            this.Shine_checkBox.Name = "Shine_checkBox";
            this.Shine_checkBox.Size = new System.Drawing.Size(211, 17);
            this.Shine_checkBox.TabIndex = 4;
            this.Shine_checkBox.Text = "Remove shine (works only with CASPs)";
            this.Shine_checkBox.UseVisualStyleBackColor = true;
            // 
            // Copy_checkBox
            // 
            this.Copy_checkBox.AutoSize = true;
            this.Copy_checkBox.Location = new System.Drawing.Point(122, 317);
            this.Copy_checkBox.Name = "Copy_checkBox";
            this.Copy_checkBox.Size = new System.Drawing.Size(246, 17);
            this.Copy_checkBox.TabIndex = 5;
            this.Copy_checkBox.Text = "Make a new package with only converted files";
            this.Copy_checkBox.UseVisualStyleBackColor = true;
            // 
            // TextureOnly_radioButton
            // 
            this.TextureOnly_radioButton.AutoSize = true;
            this.TextureOnly_radioButton.Location = new System.Drawing.Point(6, 19);
            this.TextureOnly_radioButton.Name = "TextureOnly_radioButton";
            this.TextureOnly_radioButton.Size = new System.Drawing.Size(182, 17);
            this.TextureOnly_radioButton.TabIndex = 0;
            this.TextureOnly_radioButton.TabStop = true;
            this.TextureOnly_radioButton.Text = "Texture-only (no CC wrench icon)";
            this.TextureOnly_radioButton.UseVisualStyleBackColor = true;
            this.TextureOnly_radioButton.CheckedChanged += new System.EventHandler(this.TextureOnly_radioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CASPs_radioButton);
            this.groupBox1.Controls.Add(this.TextureOnly_radioButton);
            this.groupBox1.Location = new System.Drawing.Point(116, 193);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 84);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Convert to:";
            // 
            // CASPs_radioButton
            // 
            this.CASPs_radioButton.AutoSize = true;
            this.CASPs_radioButton.Location = new System.Drawing.Point(6, 42);
            this.CASPs_radioButton.Name = "CASPs_radioButton";
            this.CASPs_radioButton.Size = new System.Drawing.Size(211, 17);
            this.CASPs_radioButton.TabIndex = 1;
            this.CASPs_radioButton.TabStop = true;
            this.CASPs_radioButton.Text = "With CASPs (will have CC wrench icon)";
            this.CASPs_radioButton.UseVisualStyleBackColor = true;
            this.CASPs_radioButton.CheckedChanged += new System.EventHandler(this.CASPs_radioButton_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 388);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Copy_checkBox);
            this.Controls.Add(this.Shine_checkBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "TS4 Defaults Fixer 3.0 by cmar";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox Shine_checkBox;
        private System.Windows.Forms.CheckBox Copy_checkBox;
        private System.Windows.Forms.RadioButton TextureOnly_radioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton CASPs_radioButton;
    }
}

