﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T
{
    public partial class Form4 : Form
    {
        //OleDbConnection connection4 = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\student\Desktop\T\T\Artikli.mdb");
        OleDbConnection connection4 = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\stvar\OneDrive\Desktop\T\T\Artikli.mdb");

        string item = "Prazno";

        public Form4(Color bc, Font f)
        {
            InitializeComponent();

            this.Font = f;
            this.BackColor = bc;
            KodoviArtikala.Items.Clear();
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            

            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT KodArtikla from Artikli";

            cmd.Connection = connection4;
            connection4.Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                KodoviArtikala.Items.Add(reader.GetString(0));
            }
                reader.Close();
                connection4.Close();
            
        }

        private void KodoviArtikala_SelectedIndexChanged(object sender, EventArgs e)
        {

            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT Ime from Artikli where KodArtikla = @KodArtikla"; 
            cmd.Parameters.AddWithValue("@KodArtikla", KodoviArtikala.GetItemText(KodoviArtikala.SelectedItem));
            cmd.Connection = connection4;
            connection4.Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item = reader.GetString(0);
            }
            reader.Close();
            connection4.Close();
            imeSelektiranogLabel.Text = item;
            
        }

        private void Spremi_Click(object sender, EventArgs e)
        {
            if (KodoviArtikala.SelectedIndex < 0)
            {
                errorProvider1.SetError(Spremi, "Treba odabrati artikl kojem želimo promijeniti popust.");
                return; 
            }

            errorProvider1.Clear();
            DialogResult uvjet = MessageBox.Show("Primjena popusta će utjecati na stanje u dućanu", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (uvjet == DialogResult.Yes)
            {

                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Artikli set Popust = @Popust where KodArtikla = @KodArtikla";

                cmd.Parameters.AddWithValue("@Popust", Convert.ToInt32(popustNUD.Value));
                cmd.Parameters.AddWithValue("@KodArtikla", KodoviArtikala.GetItemText(KodoviArtikala.SelectedItem));


                cmd.Connection = connection4;
                connection4.Open();
                cmd.ExecuteNonQuery();
                DialogResult result = MessageBox.Show("Popust promijenjen.", "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                connection4.Close();

                if (result == DialogResult.OK)
                    this.Hide();

            }

            else this.Hide();

        }

        private void zatvoriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void bojuPozadineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = colorDialog1.ShowDialog();
            if (res == DialogResult.OK)
                this.BackColor = colorDialog1.Color;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = fontDialog1.ShowDialog();
            if (res == DialogResult.OK)
                this.Font = fontDialog1.Font;
        }

        private void Form4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(Cursor.Position);
        }

     
    }
}
