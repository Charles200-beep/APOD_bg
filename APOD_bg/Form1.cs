using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APOD_bg
{
    public partial class Form1 : Form
    { 
        public Form1()
        {
            InitializeComponent();
            setSettings();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            //Wallpaper.downloadSetWallpaper(Constants.APIKey);
            backgroundWorker1.RunWorkerAsync();
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           Wallpaper.downloadSetWallpaper();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            string date = dateTimePicker1.Value.Date.ToString();
            Wallpaper.URL = "https://api.nasa.gov/planetary/apod?api_key=" + Properties.Settings.Default.API_Key + "&date=" + date.Substring(0, 10);
            Properties.Settings.Default.Selected_date_full = dateTimePicker1.Value.Date;
            Properties.Settings.Default.Selected_date = date.Substring(0, 10);
            Debug.WriteLine(Wallpaper.URL);
        }

        private void currentDateCheckbox_Click(object sender, EventArgs e)
        {
            if (currentDateCheckbox.Checked)
            {
                string date = DateTime.Now.ToString();
                dateTimePicker1.Enabled = false;
                Wallpaper.URL = "https://api.nasa.gov/planetary/apod?api_key=" + Properties.Settings.Default.API_Key + "&date=" + date.Substring(0, 10);
                Debug.WriteLine(Wallpaper.URL);
                Properties.Settings.Default.Use_Current_Date = true;
            } else
            {
                dateTimePicker1.Enabled = true;
                Properties.Settings.Default.Use_Current_Date = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString();
            if (currentDateCheckbox.Checked)
            {
                Wallpaper.URL = "https://api.nasa.gov/planetary/apod?api_key=" + Properties.Settings.Default.API_Key + "&date=" + date.Substring(0, 10);
            }
            this.backgroundWorker1.RunWorkerAsync();
            Debug.WriteLine("timer triggered!");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)numericUpDown1.Value * 3600000;
            Properties.Settings.Default.Delay = (int)numericUpDown1.Value;
            Debug.WriteLine(timer1.Interval);
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer1.Enabled = true;
                Properties.Settings.Default.Enable_Disable = true;
            }
            else
            {
                timer1.Enabled = false;
                Properties.Settings.Default.Enable_Disable = false;
            }
            Debug.WriteLine(timer1.Enabled);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void TiledRadio_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Display_mode = 0;
            Wallpaper.style = 0;
        }

        private void CenteredRadio_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Display_mode = 1;
            Wallpaper.style = 1;
        }

        private void StretchedRadio_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Display_mode = 2;
            Wallpaper.style = 2;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.API_Key = textBox1.Text;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://api.nasa.gov/",
                UseShellExecute = true
            });
        }

        private void setSettings()
        {
            switch (Properties.Settings.Default.Display_mode)
            {
                case 0:
                    TiledRadio.Checked = true;
                    Wallpaper.style = 0;
                    break;
                case 1:
                    CenteredRadio.Checked = true;
                    Wallpaper.style = 1;
                    break;
                case 2:
                    StretchedRadio.Checked = true;
                    Wallpaper.style = 2;
                    break;
                default:
                    CenteredRadio.Checked = true;
                    Wallpaper.style = 1;
                    break;
            }
            dateTimePicker1.Enabled = false;
            dateTimePicker1.Value = Properties.Settings.Default.Selected_date_full;
            currentDateCheckbox.Checked = Properties.Settings.Default.Use_Current_Date;
            if (Properties.Settings.Default.Use_Current_Date)
            {
                dateTimePicker1.Enabled = false;
            }
            else
            {
                dateTimePicker1.Enabled = true;
            }
            timer1.Interval = Properties.Settings.Default.Delay * 3600000;
            numericUpDown1.Value = Properties.Settings.Default.Delay;
            timer1.Enabled = Properties.Settings.Default.Enable_Disable;
            checkBox1.Checked = Properties.Settings.Default.Enable_Disable;
            textBox1.Text = Properties.Settings.Default.API_Key;
            if (Properties.Settings.Default.First_Run)
            {
                Properties.Settings.Default.Selected_date = DateTime.Now.ToString();
                Properties.Settings.Default.First_Run = false;
            }
            if (Properties.Settings.Default.Use_Current_Date)
            {
                string date = DateTime.Now.ToString();
                Wallpaper.URL = "https://api.nasa.gov/planetary/apod?api_key=" + Properties.Settings.Default.API_Key + "&date=" + date.Substring(0, 10);
            }
            else
            {
                Wallpaper.URL = "https://api.nasa.gov/planetary/apod?api_key=" + Properties.Settings.Default.API_Key + "&date=" + Properties.Settings.Default.Selected_date;
            }
        }
    }
}
