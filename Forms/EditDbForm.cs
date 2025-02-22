using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_works
{
    public partial class EditDbForm: Form
    {
        private FaceForm faceForm;

        public EditDbForm(FaceForm faceForm)
        {
            InitializeComponent();
            this.faceForm = faceForm;
        }
    }
}
