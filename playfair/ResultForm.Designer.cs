using System;
using System.Windows.Forms;

namespace playfair
{
    public partial class ResultForm : Form
    {
        public ResultForm(string resultText)
        {
            InitializeComponent();

            // Hiển thị kết quả trong RichTextBox
            richTextBoxResult.Text = resultText;
        }

        private void InitializeComponent()
        {
            this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBoxResult
            // 
            this.richTextBoxResult.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxResult.Name = "richTextBoxResult";
            this.richTextBoxResult.Size = new System.Drawing.Size(625, 314);
            this.richTextBoxResult.TabIndex = 0;
            this.richTextBoxResult.Text = "";
            // 
            // ResultForm
            // 
            this.ClientSize = new System.Drawing.Size(649, 338);
            this.Controls.Add(this.richTextBoxResult);
            this.Name = "ResultForm";
            this.Text = "Result";
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.RichTextBox richTextBoxResult;
    }
}
