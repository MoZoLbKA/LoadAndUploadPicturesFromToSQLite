using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LoadAndUploadPicturesFromToSQLite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Image> images = new List<Image>();
        private void button1_Click(object sender, EventArgs e)
        {
            Array buf = null;
            //метод подгружает в БД
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                buf = ms.ToArray();
            }
            SQLiteConnection con = new SQLiteConnection("Data Source=pictures.db;Version=3;");
            con.Open();
            SQLiteCommand com = new SQLiteCommand("insert into images(images) values(@photo)", con);
            com.Parameters.Add("@photo", DbType.Binary, 10000).Value = buf;
            com.ExecuteNonQuery();
            con.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //метод выгружает из дб и добавляет в лист картинок
            SQLiteConnection con = new SQLiteConnection("Data Source=pictures.db;Version=3;");
            SQLiteCommand cmd = new SQLiteCommand("SELECT images FROM images", con);
            con.Open();
            SQLiteDataReader dr;
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                {
                    byte[] im = (byte[])dr["images"];
                    MemoryStream ms = new MemoryStream(im);
                    
                    images.Add(Image.FromStream(ms));
                    //pictureBox1.Image = Image.FromStream(ms);
                    //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            con.Close();
        }
    }
}
