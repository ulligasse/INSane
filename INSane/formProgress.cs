using System.Windows.Forms;

namespace INSane
{
    public partial class formProgress : Form
    {
        public formProgress()
        {
            InitializeComponent();
        }

        public void SetPages(int pages)
        {
            lbl_pages.Text = "Gescannte Seite(n): " + pages;
        }
    }
}
