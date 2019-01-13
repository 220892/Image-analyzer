﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Morphological_image_analyzer
{
    /// <summary>
    /// Logika interakcji dla klasy ModalWindowAnaliseResult.xaml
    /// </summary>
    public partial class ModalWindowAnaliseResult : Window
    {

        ImageAnalyzer imageAnalyzer = new ImageAnalyzer();

        Bitmap bitmap;

        public ModalWindowAnaliseResult(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            InitializeComponent();
            calculateParameters();
        }

        public void calculateParameters()
        {
            imageAnalyzer.countObjects(bitmap);
            int cos = 0;
        }
    }
}
