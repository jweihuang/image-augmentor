﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class HighBoost : Form
    {

        public int fstxdim;
        public int fstydim;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;
        public int[,] G2Fgrid;

        public HighBoost(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            int[,] C2G = new int[xdim, ydim];
            Bitmap graymap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = graymap;
            fstxdim = xdim;
            fstydim = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G; 
        }

        private void HighBoost_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            int[,] G2F = new int[fstxdim, fstydim];
            float factor =9 * (float)numericUpDown1.Value - 1;     
            int boost;
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < fstydim; i++)
            {
                for (int j = 0; j < fstxdim; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = fstydim - 1; }
                    if (b == fstydim) { b = 0; }
                    if (c == -1) { c = fstxdim - 1; }
                    if (d == fstxdim) { d = 0; }

                    boost = (int)(((-1) * C2Ggrid[a, c] + (-1) * C2Ggrid[i, c] + (-1) * C2Ggrid[b, c] +
                                    (-1) * C2Ggrid[a, j] + (factor) * C2Ggrid[i, j] + (-1) * C2Ggrid[b, j] +
                                    (-1) * C2Ggrid[a, d] + (-1) * C2Ggrid[i, d] + (-1) * C2Ggrid[b, d]) / 9);
                    if (boost < 0) { boost = 0; }
                    if (boost > 255) { boost = 255; }
                    G2F[i, j] = boost;
                    filteredmap.SetPixel(j, i, Color.FromArgb(boost, boost, boost));
                    signal += (C2Ggrid[i, j] * C2Ggrid[i, j]);
                    noise += (C2Ggrid[i, j] - G2F[i, j]) * (C2Ggrid[i, j] - G2F[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            G2Fgrid = G2F;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;
        }
    }
}