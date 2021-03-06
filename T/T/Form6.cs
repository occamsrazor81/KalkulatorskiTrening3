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
using System.Security.Cryptography;
using System.IO;

namespace T
{
    public partial class Form6 : Form
    {
        OleDbConnection connection6 = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\stvar\OneDrive\Desktop\T\T\Zaposlenici.mdb");

        public Form6(Color bc, Font f)
        {
            InitializeComponent();

            this.BackColor = bc;
            this.Font = f;
            errorProvider1.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            passwirdTB.PasswordChar = '*';
            confirmPassTB.PasswordChar = '*';
            toolTip1.SetToolTip(passwirdTB, "Password je zaštićen znakom *");
            toolTip1.SetToolTip(confirmPassTB, "Ovaj password mora biti isti kao i onaj iznad");
            toolTip1.ToolTipIcon = ToolTipIcon.Warning;
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            // ovdje obaviti provjeru postoji li taj username vec i poklapaju li se passwordovi

            if(string.IsNullOrEmpty(usernameTB.Text) || string.IsNullOrEmpty(passwirdTB.Text) || string.IsNullOrEmpty(confirmPassTB.Text))
            {
                errorProvider1.SetError(Confirm, "Sva polja moraju biti popunjena");
                return;
            }

            errorProvider1.Clear();
            if(passwirdTB.Text.ToString().Trim().ToLower().CompareTo(confirmPassTB.Text.ToString().Trim().ToLower()) != 0)
            {
                errorProvider1.SetError(confirmPassTB, "Password se ne poklapa.");
                return;
            }

            errorProvider1.Clear();

            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select * from Zaposlenici";

            cmd.Connection = connection6;
            connection6.Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            bool kont = false;
            while(reader.Read())
            {
               // MessageBox.Show(reader.GetString(1).ToString(), reader.GetBoolean(3).ToString());
                if(reader.GetString(1) == usernameTB.Text)
                {
                    kont = true;
                    errorProvider1.SetError(usernameTB, "User već postoji");
                    break;
                }
            }
            reader.Close();
            connection6.Close();
            
            if(kont) return;
            
            else
            {
                

                OleDbCommand cmdInsert = new OleDbCommand();
                cmdInsert.CommandType = CommandType.Text;
                cmdInsert.CommandText = "INSERT INTO Zaposlenici ([Username], [Password], IsManager)" +
                    "VALUES (@Username, @Password, @IsManager)";

                cmdInsert.Parameters.AddWithValue("@Username", usernameTB.Text);
                cmdInsert.Parameters.AddWithValue("@Password", Encrypt_Decrypt.encrypt(passwirdTB.Text));
                cmdInsert.Parameters.AddWithValue("@IsManager", managerCheckBox.Checked);
                cmdInsert.Connection = connection6;


                connection6.Open();
                cmdInsert.ExecuteNonQuery();
                MessageBox.Show("A new member has been successfully added", "Hired", MessageBoxButtons.OK, MessageBoxIcon.Information);

                connection6.Close();

                this.Hide();
               

            }

            
        }

        private void ZatvoriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide(); 
           
        }

        private void BojuPozadineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = colorDialog1.ShowDialog();
            if (res == DialogResult.OK)
                this.BackColor = colorDialog1.Color;
        }

        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = fontDialog1.ShowDialog();
            if (res == DialogResult.OK)
                this.Font = fontDialog1.Font;
        }

        private void Form6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(Cursor.Position);
        }

        private void TB_MouseHover(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            string text = toolTip1.GetToolTip(tb);
            if (!string.IsNullOrEmpty(text))
                toolTip1.Show(text, tb, tb.PointToClient(new Point(Cursor.Position.X+5, Cursor.Position.Y)));
        }
        private void TB_MouseLeave(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;
            toolTip1.Hide(tb);
        }

      
    }
}
