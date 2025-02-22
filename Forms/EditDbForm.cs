using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxFP_CLOCKLib;

namespace vlc_works
{
    public partial class EditDbForm: Form
    {
        private FaceForm faceForm;
        private AxFP_CLOCK axFP_CLOCK { get; set; }
        private int machineNumber = 1;

        public EditDbForm(FaceForm faceForm, AxFP_CLOCK axFP_CLOCK, int machineNumber)
        {
            InitializeComponent();

            this.faceForm = faceForm;
            this.axFP_CLOCK = axFP_CLOCK;
            this.machineNumber = machineNumber;
        }

        private void EditDbForm_SizeChanged(object sender, EventArgs e)
        {
            mainGrid.Size = new Size(Size.Width, Size.Height - 39); // 39 is form header size
        }
    }
}
