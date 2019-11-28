using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SepiaGUI.Model
{
    class imageAndEnvironmentalDataModel
    {

     private byte[] arrayOfRGBValues;

     private Thread[] arrayOfThreads;

     private object[] arrayOfArguments;

        public byte[] getArrayOfRGBValues()
        {
            return arrayOfRGBValues;
        }

        public void setArrayOfRGBValues(byte[] arrayOfRGBValues)
        {
            this.arrayOfRGBValues = arrayOfRGBValues;
        }

        public Thread[] getArrayOfThreads()
        {
            return arrayOfThreads;
        }

        public object[] setArrayOfArguments()
        {
            return arrayOfArguments;
        }

        public void setArrayOfArguments(object [] arrayOfArguments)
        {
            this.arrayOfArguments = arrayOfArguments;
        }

        public void setArrayOfThreads(Thread[] arrayOfThreads)
        {
            this.arrayOfThreads = arrayOfThreads;
        }



        Stopwatch watch = new Stopwatch();
        public void setLogicalProcessors(Label logicalProcessorsLabel, String logicalProcessors)
        {

            logicalProcessorsLabel.Text = logicalProcessors;

        }
        public String countLogicalProcessors(bool optimal)
        {
            String numberOfLogicalProcessors = Environment.ProcessorCount.ToString();

            if (optimal == false)
                return numberOfLogicalProcessors;


            int logicalProcessors = Convert.ToInt32(numberOfLogicalProcessors) - 1;

            return (logicalProcessors.ToString());

        }
        public void countCores(Label CoresLabel)
        {

            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString()); // Cores
            }

            CoresLabel.Text = coreCount.ToString();

        }

        public void countPhysicalProcessors(Label physicalProcessorsLabel)
        {
            int physicalCores = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {

                physicalCores += int.Parse(item["NumberOfProcessors"].ToString());

            }

            setPhysicaProcessors(physicalProcessorsLabel, physicalCores);

        }

        public void setPhysicaProcessors(Label textLabel, int physicalCores)
        {
            textLabel.Text = physicalCores.ToString();
        }

        public void clearImage(PictureBox picture)
        {
            if (picture.Image != null)
            {
                picture.Image.Dispose();
                picture.Image = null;
            }
        }

        public int convertComboboxItemSelected(ComboBox combobox)
        {
            if (combobox.SelectedItem != null)
            {
                int value = Int32.Parse(combobox.SelectedItem.ToString());
                return value;
            }
            else
            {
                return 0;
            }
        }
        public void setImageFilter(OpenFileDialog openfile)
        {
            openfile.Filter = "Image Files( *.jpg; *.bmp;)| *.jpg; *.bmp;";

        }

        public async void DisableButton(int seconds, Button button)
        {
            button.Enabled = false;
            await Task.Delay(1000 * seconds);
            button.Enabled = true;

        }

        public static bool CheckFrameworkVersion()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    if (CheckFor45PlusVersion((int)ndpKey.GetValue("Release")))
                        return true;
                }
                else
                {
                    return false;
                }
                return false;
            }

            bool CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 528040)
                    return true;
                if (releaseKey >= 461808)
                    return true;


                return false;
            }


        }

        public void startWatch()
        {
            watch.Start();
        }
        public void stopWatch()
        {
            watch.Stop();
        }

        public string getWatchInMs()
        {
            string elapsedMs = watch.ElapsedMilliseconds.ToString();
            watch.Reset();
            return elapsedMs;
        }


        public void closeApp()
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
        }


    }   
}
