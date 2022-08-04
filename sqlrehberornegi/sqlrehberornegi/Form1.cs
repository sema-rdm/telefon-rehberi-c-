using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;//sql server bağlantısı yapmak için 
using System.Text.RegularExpressions;
namespace sqlrehberornegi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        SqlConnection baglanti;              //tüm kod boyunca kullanılacak atamalar public olaarak 
        SqlCommand komut;                            //global alanda yapılır 
        SqlDataAdapter da;
        DataSet ds;
        int eklenen = 1;
        ErrorProvider provider = new ErrorProvider();
        void KisiGetir()
        {
            baglanti = new SqlConnection("server=.; Initial Catalog=DBrehber;Integrated Security=True");
            da = new SqlDataAdapter("Select *From TLBkisiler", baglanti);
            ds = new DataSet();
            baglanti.Open();
            da.Fill(ds, "TLBkisiler");
            dataGridView1.DataSource = ds.Tables["TLBkisiler"];
            baglanti.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //  Bu kod satırı 'dBrehberDataSet.TLBkisiler' tablosuna veri yükler.
            //  Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            // this.tLBkisilerTableAdapter.Fill(this.dBrehberDataSet.TLBkisiler);
            KisiGetir();
        }
        public static bool TelefonFormatKontrol(string tel)
        {
            string RegexDesen = @"^(0(\d{1})(\d{9}))$";
            Match Eslesme = Regex.Match(tel, RegexDesen, RegexOptions.IgnoreCase);
            return Eslesme.Success;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            baglanti.Open();
            string sorgu = "Insert into TLBkisiler (ad,soyad,telefon) values (@ad,@soyad,@tel)";
            komut = new SqlCommand(sorgu, baglanti);
            bool TelefonDogruMu = TelefonFormatKontrol(textBox4.Text);
            if (TelefonDogruMu == true)
            {
                KisiGetir();
            }
            else
            {
                MessageBox.Show("Telefon numarası hatalıdır. Lütfen kontrol ediniz.");
                eklenen = 0;
            }
            if (eklenen != 0)
            {
                komut.Parameters.AddWithValue("@ad", textBox2.Text);
                komut.Parameters.AddWithValue("@soyad", textBox3.Text);
                komut.Parameters.AddWithValue("@tel", textBox4.Text);
                baglanti.Open();
                eklenen = komut.ExecuteNonQuery();
                KisiGetir();
                baglanti.Close();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
            }
            else
            {
                MessageBox.Show("kayıt işlemi başarısız");
            }
            //kullanıcıdan gerekli bilgiler (ad, soyad, tel..) alınarak yeni kişi eklemesi yapılır 
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
        }  //textbox a girilen değerlerin datagridview ile sistemde görünmesi sağlanır 

        void KayıtSil(String ad)
        {
            string sql = "DELETE FROM TLBkisiler WHERE ad=@ad";
            komut = new SqlCommand(sql, baglanti);
            komut.Parameters.AddWithValue("@ad", ad);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow drow in dataGridView1.SelectedRows)
            {
                string ad = Convert.ToString(drow.Cells[1].Value);
                KayıtSil(ad);
                KisiGetir();
            }
        }
        //datagridview de önceden eklenmiş bir kişinin bilgileri girilerek sistemden silinmesi sağlanır 
        private void button3_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("update TLBkisiler(ad,soyad,telefon)values(@ad,@soyad,@tel)", baglanti);
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            KisiGetir();
        }
        //kişiler güncellenir 
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter("SElect *from TLBkisiler where ad like '" + textBox5.Text + "%'", baglanti);
            DataSet ds = new DataSet();
            baglanti.Open();
            da.Fill(ds, "TLBkisiler");
            dataGridView1.DataSource = ds.Tables["TLBkisiler"];
            baglanti.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void ad_validating(object sender, CancelEventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                provider.SetError(textBox2, "Ad değerini girmelisiniz");
                eklenen = 0;
            }
            else
            {
                provider.SetError(textBox2, "");
                eklenen = 1;
            }
        }

        private void soyad_validated(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "")
            {
                provider.SetError(textBox3, "Soyad değerini girmelisiniz");
                eklenen = 0;
            }
            else
            {
                provider.SetError(textBox3, "");
                eklenen = 1;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        // kişilerin ismine göre listede arama yapılır
    }
}



     