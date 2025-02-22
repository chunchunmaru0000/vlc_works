﻿using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace vlc_works
{
    public partial class PhotoForm: Form
    {
        public PhotoForm(byte[] photoBytes)
        {
            InitializeComponent();

            pictureBox.Image = new Bitmap(new MemoryStream(photoBytes)).Clone() as Bitmap;
        }
    }
}
