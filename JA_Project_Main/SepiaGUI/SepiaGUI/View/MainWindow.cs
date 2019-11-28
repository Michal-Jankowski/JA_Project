using SepiaGUI.Model;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainApp : Form
    {   
        byte[] RGBValuesOfImage;

        Thread[] arrayOfThreads;

        object[] arrayOfArguments;


        imageAndEnvironmentalDataModel model = new imageAndEnvironmentalDataModel();
        

       [DllImport("SepiaAsmDll.dll")]
       unsafe  public static extern void Sepia(byte* tab, int start,int stop, int toneValue);
        [DllImport("SepiaAsmDll.dll")]
        public static extern bool DetectFeatureECX(int check);
        [DllImport("SepiaAsmDll.dll")]
        public static extern bool DetectFeatureEDX(int check);

        public MainApp()
        {
         
            InitializeComponent();
           
        }

        private void windowLoad(object sender, EventArgs e)
        {
            //Main Window text
            this.Text = "SepiaProgram";
            // Method that counts number of physical processors
            model.countPhysicalProcessors(physicalProcessorsLabel);
            //Method that counts number of Cores in the CPU
            model.countCores(coresLabel);
            //Method that counts number of Logical Processors
            model.setLogicalProcessors(logicalProcessorsLabel, model.countLogicalProcessors(false));
            // setLogicalProcessors(label3, countLogicalProcessors());
            // Sets deafut value for number of threads
            ActiveThreadsComboBox.SelectedItem = model.countLogicalProcessors(true);
            //Sets number of default sepia value 
            SepiaComboBox.SelectedItem = "0";

            if(CheckCompabilites() == true)
            {
                AsmRadioButton.Enabled = false;
                MessageBox.Show("Processor do not support MMX or AVX instructions set", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if(imageAndEnvironmentalDataModel.CheckFrameworkVersion() == false)
            {
                MessageBox.Show("Outdated version of .NET framework. Version 4.7.2 or newer is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        private bool CheckCompabilites()
        {
            if (DetectFeatureEDX(23) == false) // MMX
            {

                if (DetectFeatureECX(28) == false) // AVX
                {

                    return true;
                }

            }
            return false;


        }
        private void uploadImageButton(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            model.setImageFilter(openFile);
            try
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    uploadImageTextBox.Text = openFile.FileName;

                    InsertImage.SizeMode = PictureBoxSizeMode.StretchImage;

                    InsertImage.Image = new Bitmap(openFile.FileName);

                    Bitmap imageFile = new Bitmap(InsertImage.Image);

                }
            }catch (OutOfMemoryException exception)
            {
                MessageBox.Show("Image was to big to load !", exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }
        private void convertImageButton(object sender, EventArgs e)
        {

            int filterValue = 0;

            Bitmap convertedImage;


            model.clearImage(ConvertImage);

            try
            {
                convertedImage = new Bitmap(InsertImage.Image);

                if (SepiaComboBox.SelectedItem != null)
                {
                    filterValue = int.Parse(SepiaComboBox.SelectedItem.ToString());
                }

                SepiaBitmap(convertedImage, filterValue);
                convertedImage.Save("ConvertedImage.bmp", ImageFormat.Bmp);

                ConvertImage.SizeMode = PictureBoxSizeMode.StretchImage;
                ConvertImage.Image = convertedImage;
            } catch(Exception)
            {
                MessageBox.Show("Error while trying to load image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            model.DisableButton(2, ConvertButton);

        }
        private void SepiaBitmap(Bitmap InputImage, int toneValue)
        {
            
            var rectangle = new Rectangle(0, 0, InputImage.Width, InputImage.Height);

            var data = InputImage.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            RGBValuesOfImage = new byte[data.Width * data.Height * depth];

            try {

             Marshal.Copy(data.Scan0, RGBValuesOfImage, 0, RGBValuesOfImage.Length);

             InvokeDll(CsharpRadioButton, AsmRadioButton, model.convertComboboxItemSelected(ActiveThreadsComboBox), toneValue);

             model.startWatch();

             LogRichTextBox.Text = (model.getWatchInMs() + " Ms");

            Marshal.Copy(RGBValuesOfImage, 0, data.Scan0, RGBValuesOfImage.Length);

             this.Invalidate();

            InputImage.UnlockBits(data);

            InputImage.Save("ConvertedFile.bmp", ImageFormat.Bmp);
            }
            catch (Exception)
            {
            
                MessageBox.Show("There was an error during image conversion!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public void PrepareDelegates(RadioButton csharp,RadioButton asm, int threads, int toneValue) 
        {
            if (RGBValuesOfImage == null)
                return;

            if (appThreadAsm(asm, threads, toneValue) == true)
                return;


            if (appThreadCsharp(csharp, threads, toneValue) == true)
                return;

            arrayOfThreads = new Thread[threads];

            arrayOfArguments = new object[threads];

            int interval = RGBValuesOfImage.GetLength(0) / threads;
           
            for(int i = 0; i< arrayOfThreads.GetLength(0); ++i)
            {
             
                int start = i * interval;

                while(start %4 != 0)
                {
                    start += 1;
                    
                }

                int stop = (i + 1) * interval;

                while(stop %4 != 0)
                { 
                    stop += 1;
                }


                arrayOfArguments[i] = new object[4] { start, stop, RGBValuesOfImage, toneValue };

                if (csharp.Checked) arrayOfThreads[i] = new Thread(new ParameterizedThreadStart(SepiaDll.Sepia.CSharpDllFunc));

                else if (asm.Checked) arrayOfThreads[i] = new Thread(new ParameterizedThreadStart(assemblerFunction));


            }
        }
        private  void createThreads()
        {
            if (arrayOfThreads == null)
                return;
            model.startWatch();
            for (int i = 0; i< arrayOfThreads.GetLength(0); ++i)
            {
                arrayOfThreads[i].Start(arrayOfArguments[i]);
              
            }
            
        }
        private void waitForThreads()
        {

            if (arrayOfThreads == null) return;

            bool done = false;

            while (!done)
            {
                done = true;

                for (int i = 0; i < arrayOfThreads.GetLength(0); ++i)
                    done &= (arrayOfThreads[i] == null || !arrayOfThreads[i].IsAlive);
            }
            
        }
        public void InvokeDll( RadioButton csharpRadioButton,RadioButton asmRadioButton, int threads, int toneValue)
        {
  
            PrepareDelegates(csharpRadioButton, asmRadioButton, threads, toneValue);
            
            createThreads();
            
            waitForThreads();

        }
        unsafe private void assemblerFunction(object argum)
        {
             Array args = new object[4];

             args = (Array)argum;
             
             int start = (int)args.GetValue(0);
            
             int stop  = (int)args.GetValue(1);
            
             byte[] table = (byte[])args.GetValue(2);

             int toneValue = (int)args.GetValue(3);

            fixed (byte* ptr = &table[0])
            {
                Sepia(ptr, start, stop, toneValue);
            }
        }
        private bool appThreadCsharp(RadioButton csharp, int threads, int toneValue)
        {
            if (threads == 0 && csharp.Checked)
            {
                arrayOfThreads = null;
                int start = 0;
                int stop = RGBValuesOfImage.Length;

                arrayOfArguments = new object[4] { start, stop, RGBValuesOfImage, toneValue };

                model.startWatch();

                SepiaDll.Sepia.CSharpDllFunc(arrayOfArguments);
                return true;
            }
            return false;
        }
        private bool appThreadAsm(RadioButton asm ,int threads ,int toneValue)
        {

            if (threads == 0 && asm.Checked)
            {
                arrayOfThreads = null;
                int start = 0;
                int stop = RGBValuesOfImage.Length;

                arrayOfArguments = new object[4] { start, stop, RGBValuesOfImage, toneValue };

                model.startWatch();

                assemblerFunction(arrayOfArguments);
                return true;
            }
            return false;

        }
        private void LogRichTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
