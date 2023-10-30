using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection.Emit;
//using System.Threading.Tasks;

namespace Cinema
{
    public partial class Form1 : Form
    {
        ListViewClass listViewClass = new ListViewClass();

 

        static Random random = new Random();

        static string constring = "Data Source=DESKTOP-ANF5MEQ;Initial Catalog=cinema_db;Integrated Security=True";
        SqlConnection connect = new SqlConnection(constring);
        
        //tek seferlik işlem
        int eklenenKoltuk = 0;
        int eklendiKoltuk = 0;
        int added_Seat;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("", 20);
            listView1.Columns.Add("Product Type", 100);
            listView1.Columns.Add("Price", 67);
            listView1.Width = 195;

            listView2.FullRowSelect = true; // Tam satır seçimini etkinleştirin
            listView2.GridLines = true; // Izgara çizgilerini etkinleştirin
            
            dataGridView2.Rows.Add("", "");

            listViewTemizle();

        }

        public void listViewControl(int again)
        {
            int totalPrice = 0;
            if (again == 0) { label1.Text = "0 ₺";  label2.Text = "0"; }
          
            for (int i = 0; i < again; i++)
            {
                string[] bilgiler = { "1", "Koltuk", "50" };
                ListViewItem kayit = new ListViewItem(bilgiler);
                listView1.Items.Add(kayit);
                label2.Text = Convert.ToString(i + 1);

                totalPrice += int.Parse(bilgiler[2]);
                label1.Text = Convert.ToString(totalPrice + "₺");
            }
        }

        static string GenerateRandomCode()
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // Hem sayı hem harf karakterleri
            int length = 4; // 4 haneli kod

            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = characters[random.Next(characters.Length)];
            }

            return new string(code);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            label6.Text = string.Empty;
            label7.Text = string.Empty;

            string ticket_code = GenerateRandomCode();
            try
            {
                if (connect.State == ConnectionState.Closed) connect.Open();
                string sameControl = "select count(*) from tickets where ticket_no = '@randomTicketCode'";
                SqlCommand komut = new SqlCommand(sameControl, connect);

                komut.Parameters.AddWithValue("@randomTicketCode", ticket_code);

                int result = Convert.ToInt32(komut.ExecuteScalar());

                if (result == 0)
                {
                    string ticketAdd = "insert into tickets values (@ticketCode)";
                    SqlCommand komut1 = new SqlCommand(ticketAdd, connect);

                    komut1.Parameters.AddWithValue("@ticketCode", ticket_code);

                    komut1.ExecuteNonQuery();
                    eklenenKoltuk++;
                    // BURAYA KOLTUK EKLE

                    dataGridView1.Rows.Add(ticket_code, "");

                    button2.Enabled = false;
                }
                else
                {
                    ticket_code = GenerateRandomCode();
                }
                connect.Close();
            }
            catch (Exception hata)
            {

                MessageBox.Show("Hata meydana geldi", hata.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            this.Hide();
            form2.ShowDialog();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            void dataGridClear(int girenIslem)
            {
                if (girenIslem == eklenenKoltuk)
                    dataGridView1.Rows.Clear();

            }
            int islem = 0;

            label6.Text = string.Empty;
            label7.Text = string.Empty;

            dataGridView1.SelectAll();

            string[,] ticket_owner1 = new string[eklenenKoltuk, 1];

            int counter = 0;
            for (int i = 0; i < ticket_owner1.GetLength(0); i++)
            {
                for (int j = 0; j <= ticket_owner1.GetLength(1); j++)
                {
                    string text = dataGridView1.SelectedRows[i].Cells[j].Value.ToString();
                    // Boşsa False döndürür, Değilse True
                    if (string.IsNullOrEmpty(text))
                    {
                        counter++;
                    }
                }
            }


            if (counter == 0)
            {
                try
                {
                    if (connect.State == ConnectionState.Closed) connect.Open();

                    string[,] ticket_owner = new string[eklenenKoltuk, 1];

                    for (int i = 0; i < ticket_owner.GetLength(0); i++)
                    {
                        for (int j = 0; j < ticket_owner.GetLength(1); j++)
                        {
                            try
                            {
                                string tradeEkleTicket = "insert into ticket_trade values(@ticket_no, @owner_name)";
                                SqlCommand komut3 = new SqlCommand(tradeEkleTicket, connect);
                                komut3.Parameters.AddWithValue("@ticket_no", dataGridView1.SelectedRows[i].Cells[j].Value.ToString());
                                komut3.Parameters.AddWithValue("@owner_name", dataGridView1.SelectedRows[i].Cells[j + 1].Value.ToString());
                                komut3.ExecuteNonQuery();

                                string[] bilgiler = { dataGridView1.SelectedRows[i].Cells[j].Value.ToString(), dataGridView1.SelectedRows[i].Cells[j + 1].Value.ToString() };
                                ListViewItem kayit = new ListViewItem(bilgiler);
                                listView2.Items.Add(kayit);

                                label6.Text = "Biletler Kaydedildi !";
                                label6.ForeColor = Color.Green;

                                //data grid view temizle

                                eklendiKoltuk = eklenenKoltuk;

                                string sil_seat = "delete from added_seat";
                                SqlCommand komut10 = new SqlCommand(sil_seat, connect);
                                komut10.ExecuteNonQuery();

                                string add_seat = "insert into added_seat values (@eklendiKoltuk);";
                                SqlCommand komut9 = new SqlCommand(add_seat, connect);
                                komut9.Parameters.AddWithValue("@eklendiKoltuk", eklendiKoltuk.ToString());
                                komut9.ExecuteNonQuery();



                                islem++;
                                dataGridClear(islem);

                                button2.Enabled = true;
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                    connect.Close();
                    listViewControl(eklenenKoltuk);

                }
                catch (Exception hata)
                {

                    MessageBox.Show("Bir hata oluştu", hata.Message);
                }
            }
            else
            {
                label6.Text = "Lütfen boş kutu bırakmayın";
                label6.ForeColor = Color.Red;
            }
        }




        private void button4_Click(object sender, EventArgs e)
        {
            label7.Text = string.Empty;
            label7.ForeColor = Color.Black;
            // BİLET SİL
            string cell1 = dataGridView2.Rows[0].Cells[0].Value as string;
            string cell2 = dataGridView2.Rows[0].Cells[1].Value as string;

            if (string.IsNullOrEmpty(cell1) && string.IsNullOrEmpty(cell2))
            {
                label7.Text = "En az 1 hucreyi doldurun";
                label7.ForeColor = Color.Red;
            }
            else if (!string.IsNullOrEmpty(cell1))
            {
                try
                {

                    if (connect.State == ConnectionState.Closed) connect.Open();

                    string isTrueTicket = "select count(*) from ticket_trade where ticket_no = @ticket_no";
                    SqlCommand ticketControl1 = new SqlCommand(isTrueTicket, connect);
                    ticketControl1.Parameters.AddWithValue("@ticket_no", cell1);
                    int haveTicket = Convert.ToInt32(ticketControl1.ExecuteScalar());
 

                    if (haveTicket != 0)
                    {
                        string komut = "delete from ticket_trade where ticket_no = @ticket_no";
                        SqlCommand komut11 = new SqlCommand(komut, connect);
                        komut11.Parameters.AddWithValue("ticket_no", cell1);
                        komut11.ExecuteNonQuery();

                        string komut1 = "delete from tickets where ticket_no = @ticket_no";
                        SqlCommand komut13 = new SqlCommand(komut1, connect);
                        komut13.Parameters.AddWithValue("@ticket_no", cell1);
                        komut13.ExecuteNonQuery();

                        label7.Text = string.Format("Başarıyla {0} silindi", cell1); 
                        label7.ForeColor = Color.Green;

                        dataGridView2.Rows[0].Cells[0].Value = string.Empty;
                    }
                    else
                    {
                        label7.Text = string.Format("{0} numaralı bilet bulunamadı !",cell1);
                        label7.ForeColor = Color.Red;
                        dataGridView2.Rows[0].Cells[0].Value = string.Empty;
                    }
                    connect.Close();



                }
                catch (Exception hata)
                {
                    MessageBox.Show("Bir hata meydana gedi --", hata.Message);
                }
            }
            else if (!string.IsNullOrEmpty(cell2))
            {
                try
                {
                    if (connect.State == ConnectionState.Closed) connect.Open();

                    string isTrueName = "select count(*) from ticket_trade where ownar_name = @name";
                    SqlCommand ticketControl2 = new SqlCommand(isTrueName, connect);
                    ticketControl2.Parameters.AddWithValue("@name", cell2);
                    int haveName = Convert.ToInt32(ticketControl2.ExecuteScalar());

                    if(haveName > 0)
                    {
                        string komut = "delete from ticket_trade where ownar_name = @name";
                        SqlCommand komut12 = new SqlCommand(komut, connect);
                        komut12.Parameters.AddWithValue("@name", cell2);
                        

                        string findNameKomut = "select ticket_no from ticket_trade where ownar_name = @name";
                        SqlCommand komutFindName = new SqlCommand(findNameKomut, connect);
                        komutFindName.Parameters.AddWithValue("@name", cell2);
                        string fondTicket = Convert.ToString(komutFindName.ExecuteScalar());

                        string biletSil = "delete from tickets where ticket_no = @ticket_no";
                        SqlCommand biletSilKomut = new SqlCommand(biletSil, connect);
                        biletSilKomut.Parameters.AddWithValue("@ticket_no", fondTicket);


                        biletSilKomut.ExecuteNonQuery();
                        komut12.ExecuteNonQuery();



                        label7.Text = string.Format("Başarıyla {0} silindi", cell2);
                        label7.ForeColor = Color.Green;

                        dataGridView2.Rows[0].Cells[1].Value = string.Empty;
                    }

                    else
                    {
                        label7.Text = string.Format("{0} isimli müşteri bulunamadı !", cell2);
                        label7.ForeColor = Color.Red;
                        dataGridView2.Rows[0].Cells[1].Value = string.Empty;
                    }
                    connect.Close();



                }
                catch (Exception hata)
                {

                    MessageBox.Show("Bir hata meydana geldi", hata.Message);

                }
            }
            listViewTemizle();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_TickAsync(object sender, EventArgs e)
        {


        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listViewTemizle()
        {
            listView1.Items.Clear();

            listView2.Items.Clear();

            try
            {
                if (connect.State == ConnectionState.Closed) connect.Open();


                string koltukSayy = "select count(*) from ticket_trade";
                SqlCommand komut11 = new SqlCommand(koltukSayy, connect);
                int koltukSay = Convert.ToInt32(komut11.ExecuteScalar());
                listViewControl(koltukSay);


                string counterQuery = "select * from ticket_trade";
                using (SqlCommand komut8 = new SqlCommand(counterQuery, connect))


                using (SqlDataReader reader = komut8.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sutun1 = reader.GetString(0);
                        string sutun2 = reader.GetString(1);

                        string[] bilgiler = new string[2];
                        bilgiler[0] = sutun1;
                        bilgiler[1] = sutun2;

                        ListViewItem kayit = new ListViewItem(bilgiler);
                        listView2.Items.Add(kayit);

                    }
                }
                connect.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show("Bir hata oluştu", hata.Message);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            listViewTemizle();
            label6.Text = string.Empty;
            label7.Text = string.Empty;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (connect.State == ConnectionState.Closed) connect.Open();
                string[] hepsiniSil = { "delete from ticket_trade", "delete from tickets", "delete from added_seat" };
                for (int i = 0; i < hepsiniSil.Length; i++)
                {
                    using (SqlCommand komutHepsiniSil = new SqlCommand(hepsiniSil[i], connect)) 
                    komutHepsiniSil.ExecuteNonQuery();
                }
                connect.Close();
                button6.Text = "Temizlendi";
            }
            catch (Exception hata)
            {
                MessageBox.Show("Bir hata meydana geldi", hata.Message);
            }
        }
    }
}
