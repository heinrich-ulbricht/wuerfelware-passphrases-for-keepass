using PwGenWuerfelware.Classes;
using PwGenWuerfelware.Configuration;
using System.Windows.Forms;

namespace PwGenWuerfelware.Forms
{
    public partial class FormConfiguration : Form
    {
        public ConfigurationModel Configuration { get; set; }

        public FormConfiguration()
        {
            InitializeComponent();
        }

        public FormConfiguration(ConfigurationModel config): this()
        {
            Configuration = config;
            numericUpDown1.Minimum = Constants.MinWordsPerPassphrase;
            numericUpDown1.Maximum = Constants.MaxWordsPerPassphrase;
            numericUpDown1.Value = Configuration.WordCount;
        }

        private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
        {
            Configuration.WordCount = decimal.ToInt32(((NumericUpDown)sender).Value);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        public void setData(string filePath, int wordCount)
        {
            tbFilePath.Text = filePath;
            lblNumberOfWords.Text = wordCount.ToString();
        }
    }
}
