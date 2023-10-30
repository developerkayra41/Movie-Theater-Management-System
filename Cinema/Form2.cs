using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinema
{
    public partial class Form2 : Form
    {
        System.Drawing.Image seat_png;

        public Form2()
        {
            InitializeComponent();
            seat_png = Properties.Resources.RedSeatPNG;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();

            this.Hide();
            form1.ShowDialog();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CreateButtons();

        }

        private void CreateButtons()
        {
            // a1 a2 a3
            // b1 b2 b3
            // c1 c2 c3
            
            string[] harfler = { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
            string[] sayilar = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            int locationNumber = 0;

            for (int j = 0; j < sayilar.Length; j++)
            {
                for (int i = 0; i < harfler.Length; i++)
                {
                    Button button = new Button();

                    button.Name = (harfler[j] + sayilar[i]);
                    button.Text = (harfler[j] + sayilar[i]);

                    button.Location = new System.Drawing.Point(20 + (locationNumber % 9) * 70, 20 + (locationNumber / 9) * 80);
                    button.Size = new System.Drawing.Size(65, 65);

                    button.Click += new EventHandler(Button_Click);
                    button.BackgroundImage = seat_png;
                    button.BackgroundImageLayout = ImageLayout.Zoom;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = Color.WhiteSmoke;
                    this.Controls.Add(button);
                    locationNumber++;
                }
                if (j == 4) break;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
       
            Button clickedButton = (Button)sender;
            MessageBox.Show("Tıklanan Buton: " + clickedButton.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }


    }
}
