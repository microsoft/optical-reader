using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace OpticalReaderApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private OpticalReaderLib.OpticalReaderTask _task = new OpticalReaderLib.OpticalReaderTask();

        public MainPage()
        {
            InitializeComponent();

            _task.Completed += OpticalReaderTask_Completed;
        }

        private void ReadCodeButton_Click(object sender, EventArgs e)
        {
            TypeTextBlock.Text = "";
            DescriptionTextBlock.Text = "";

            _task.Show();
        }

        private void OpticalReaderTask_Completed(object sender, OpticalReaderLib.OpticalReaderResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                System.Diagnostics.Debug.WriteLine("Code read successfully");

                TypeTextBlock.Text = String.Format("{0} ({1} bytes)", e.Format, e.Data.Length);
                DescriptionTextBlock.Text = e.Text;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Code reading aborted");
            }
        }
    }
}