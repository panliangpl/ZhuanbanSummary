
namespace ALLCheck
{
    partial class frmTool
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
            this.hebing_btn = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // hebing_btn
            // 
            this.hebing_btn.Location = new System.Drawing.Point(392, 304);
            this.hebing_btn.Margin = new System.Windows.Forms.Padding(2);
            this.hebing_btn.Name = "hebing_btn";
            this.hebing_btn.Size = new System.Drawing.Size(44, 27);
            this.hebing_btn.TabIndex = 1;
            this.hebing_btn.Text = "计算";
            this.hebing_btn.UseVisualStyleBackColor = true;
            this.hebing_btn.Click += new System.EventHandler(this.hebing_btn_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(424, 285);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // frmTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 342);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.hebing_btn);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmTool";
            this.Text = "闭环专班";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button hebing_btn;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}