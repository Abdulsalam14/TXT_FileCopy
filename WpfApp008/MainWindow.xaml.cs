using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp008
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {

        public Thread Thread { get; set; }
        public bool ThreadState { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Copy(string from, string to)
        {
            if (File.Exists(from))
            {
                Thread thread = new Thread(() =>
                {
                    using (FileStream fsread = new FileStream(from, FileMode.Open, FileAccess.Read))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            progress.Maximum = fsread.Length;
                        });

                        using (FileStream fswrite = new FileStream(to, FileMode.Create,FileAccess.Write))
                        {
                            int len = 1;
                            var filesize = fsread.Length;
                            byte[] buffer = new byte[len];
                            int size = 0;
                            do
                            {
                                len = fsread.Read(buffer, 0, buffer.Length);
                                fswrite.Write(buffer, 0, len);
                                filesize -= len;
                                size += len;
                                Dispatcher.Invoke(() =>
                                {
                                    progress.Value = size;
                                });
                                Thread.Sleep(50);
                                fswrite.Flush();
                            } while (len != 0);

                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Copy Succesfull");
                                progress.Value = 0;
                                Thread = null;
                                fromtextbox.Text = default;
                                totextbox.Text = default;
                                ThreadState = default;
                            });
                        }
                    }
                });
                Thread = thread;
                Thread.IsBackground = true;
                Thread.Start();
            }
            else
            {
                MessageBox.Show("Wrong file");
            }
        }

        private void startbutton_Click(object sender, RoutedEventArgs e)
        {
            if (fromtextbox.Text == "" || totextbox.Text == "")
            {
                MessageBox.Show("Enter Files");
                return;
            }
            if (Thread == null)
            {
                Copy(fromtextbox.Text, totextbox.Text);
            }
        }

        [Obsolete]
        private void suspendbutton_Click(object sender, RoutedEventArgs e)
        {
            if (ThreadState == false && Thread != null)
            {
                Thread.Suspend();
                ThreadState = true;
            }

        }

        [Obsolete]
        private void resumebutton_Click(object sender, RoutedEventArgs e)
        {
            if (ThreadState == true)
            {
                Thread.Resume();
                ThreadState = false;
            }
        }

        private void FromOpenbutton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.txt)|*.txt";
            fileDialog.ShowDialog();
            if (fileDialog.FileName != "" && Thread == null)
            {
                fromtextbox.Text = fileDialog.FileName;
            }
        }

        private void ToOpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "(*.txt)|*.txt";
            file.ShowDialog();
            if (file.FileName != null && Thread == null && file.FileName!=fromtextbox.Text)
            {
                totextbox.Text = file.FileName;
            }
        }
    }
}
