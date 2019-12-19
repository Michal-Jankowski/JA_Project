
//Autor: Michał Jankowski
//Dzień: 13.12.2019r.
//Przedmiot: Języki Asemblerowe
//Temat: Efekt Sepii



//Changelog:

//version 0.1 :




using SepiaGUI.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SepiaApp
{
                                                                                                             // Klasa główna programu. Wykonuję wszystkie operacje związane z widokiem oraz przetwarzaniem modelu.
                                                                                                             // Tutaj wywoływane są dllki do wykonania algorytmu Sepii
    public partial class SepiaProgram : Form
    {   
                                                                                                             // zmienna dla talicy bajtów obrazka
        byte[] RGBValuesOfImage;
                                                                                                            // obiekt tablicy wątków
        Thread[] arrayOfThreads;
                                                                                                            // obiekt tablicy obiektów danych do przekazania jako parametry dllki
        object[] arrayOfArguments;

                                                                                                            // obiekt klasy imageAndEnvironmentalDataModel do tworzenia niezbędnych metod dla modelu i widoku
        readonly ImageAndEnvironmentalDataModel model = new ImageAndEnvironmentalDataModel();
        
                                                                                                            // import asm dllki przetwarzania efektu sepii obrazka
       [DllImport("SepiaAsmDll.dll")]
        /// <summary>
        ///  import asm dllki do wykonania efektu sepii
        /// </summary>
        /// <param name="tab"> wskaźnik byte* tablicy bajtów</param>
        /// <param name="start"> wartość int zmiennej wartości początka przedziału</param>
        /// <param name="stop"> wartość int zmiennej wartości końca przedziału</param>
        /// <param name="toneValue"> wartość  int zmiennej współczynnika wypełnienia sepii</param>
        /// return void
        unsafe public static extern void Sepia(byte* tab, int start,int stop, int toneValue);
                                                                                                            
        [DllImport("SepiaAsmDll.dll")]
        /// <summary>
        /// import asm dllki do sprawdzenie kompatybilności obsługi instrukcji AVX w rejestrze ecx
        /// </summary>
        /// <param name="check"> Wartość szukanej funkcjonalności w procesorze</param>
        /// <returns> wartość bool prawda lub fałsz danej funkcjonalności</returns>
        public static extern bool DetectFeatureECX(int check);
                                                                                                           
        [DllImport("SepiaAsmDll.dll")]
        /// <summary>
        /// import Asm dllki do sprawdzenie kompatybilności obsługi instrukcji MMX w rejestrze edx 
        /// </summary>
        /// <param name="check"> Wartość szukanej funkcjonalności w procesorze</param>
        /// <returns>wartość bool prawda lub fałsz danej funkcjonalności</returns> 
        public static extern bool DetectFeatureEDX(int check);
                  
        /// <summary>
        /// Konsturktor klasy SepiaProgram() wykonujący metodę inicjalizującą główne komponenty widoku
        /// </summary>
        public SepiaProgram()
        {
         
            InitializeComponent();
           
        }

        /// <summary>
        ///  metoda wykonywana podczas włączenia okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoad(object sender, EventArgs e)
        {
                                                                                                             // nazwa głównego okna 
                                                                                                            
                                                                                                             // metoda, która zlicza ilość fizycznych procesorów
            model.CountPhysicalProcessors(physicalProcessorsLabel);
                                                                                                             // metoda, która zlicza liczbę rdzeni w CPU
            model.CountCores(coresLabel);
                                                                                                             // metoda, któr zlicza liczbę logicznych procesorów
            model.SetLogicalProcessors(logicalProcessorsLabel, model.CountLogicalProcessors(false));
                                                                                                             // ustawia liczbę optymalnych wątków dla aplikacji
            ActiveThreadsComboBox.SelectedItem = model.CountLogicalProcessors(true);
                                                                                                             // ustawia domyślną wartość sepii na 0
            SepiaComboBox.SelectedItem = "0";
                                                                                                             // ustawia domyślną wartość radiobutton
            CsharpRadioButton.Checked = true;
                                                                                                             // sprawdza obsługę MMX i AVX oraz w przypadku braku obsługi instrukcji uniemożliwia jej wykonanie
            if(CheckCompabilites() == false)
            {
                                                                                                             // informacja w przypadku braku obsługi asemblera
                AsmRadioButton.Enabled = false;
                MessageBox.Show("Processor do not support MMX or AVX instructions set", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                                                                                                             // sprawdza wersję frameworku i zamyka aplikacje gdy jej brakuje
            if(ImageAndEnvironmentalDataModel.CheckFrameworkVersion() == false)
            {
                                                                                                             // obsługa odpowiedniej wersji frameworka dla programu
                MessageBox.Show("Outdated version of .NET framework. Version 4.7.2 or newer is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();                                                    // zamknięcie okna aplikacji
            }

        }

        /// <summary>
        ///  metoda sprawdzająca kompatybilność MMX oraz AVX wykorzystując asemblerową dllke
        /// </summary>
        /// <returns> bool wartość danej funkcjonalności</returns>
        private bool CheckCompabilites()
        {
            if (DetectFeatureEDX(23) == true && DetectFeatureEDX(25) == true && DetectFeatureEDX(26) == true)                                                              // metoda asemblerowa sprawdzająca czy są obsługiwane instrukcje MMX 
            {

                if (DetectFeatureECX(28) == true && DetectFeatureECX(19) == true && DetectFeatureECX(20) == true)                                                          // metoda asemblerowa sprawdzająca czy są obsługiwane instrukcje AVX
                {

                    return true;
                }

            }
            return false;


        }

        /// <summary>
        ///  metoda do ładowania obrazka za pomocą przycisku upload image
        /// </summary>
        /// <param name="sender"> obiekt wysyłanej akcji w przypadku wciśnięcia przycisku</param>
        /// <param name="e"> Obiekt przekazujący informacje o wciśniętym przycisku</param>
        private void UploadImageButton(object sender, EventArgs e)
        {
                                                                                                            // obiekt wykorzystywany do utworzenia list rozwijanej przy wybieraniu obrazka
            using (OpenFileDialog openFile = new OpenFileDialog())
            {

                                                                                                            // ustawienie obsługiwanych formatów obrazków. Są to: jpg,bmp. 
                model.SetImageFilter(openFile);
                try
                {
                                                                                                            // jeżeli otworzono listę rozwijaną, wybierz obrazek i zainicjalizuj go w UI jako Image oraz dla dalszej obługi jako Bitmap
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        uploadImageTextBox.Text = openFile.FileName;

                        InsertImage.SizeMode = PictureBoxSizeMode.StretchImage;


                        InsertImage.Image = model.ConvertToBitmap(openFile.FileName);

                        Bitmap imageFile = new Bitmap(InsertImage.Image);

                      
                        imageFile.Dispose();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error while loading the image !", exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                }
            }

        }

        /// <summary>
        ///  metoda obsługująca konwersję obrazka do Sepii za pomocą przycisku convert// metoda obsługująca konwersję obrazka do Sepii za pomocą przycisku convert
        /// </summary>
        /// <param name="sender"> obiekt wysyłanej akcji w przypadku wciśnięcia przycisku</param>
        /// <param name="e"> Obiekt przekazujący informacje o wciśniętym przycisku </param>
        /// return void
        private void ConvertImageButton(object sender, EventArgs e)
        {

            int filterValue = 0;

            Bitmap convertedImage;

                                                                                                               // czyszczenie za każdym załadowaniem obrazka poprzedniego starego obrazka
            model.ClearImage(ConvertImage);

            try
            {
                                                                                                               // przepisywanie obiektu zdjęcia
                convertedImage = new Bitmap(InsertImage.Image);

                                                                                                               // wybranie wartości sepii w comboboxie
                if (SepiaComboBox.SelectedItem != null)
                {
                                                                                                                // przepisywanie wartości filtru sepii do zmiennej
                    filterValue = int.Parse(SepiaComboBox.SelectedItem.ToString());
                }
                                                                                                                // metoda wyciągająca z obrazka niezbędne dane
                SepiaBitmap(convertedImage, filterValue);
                                                                                                                // ustawienie rozmiaru okna obrazka do "rozciągnietego"
                ConvertImage.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                                                // ustawienie nowego obrazka w UI
                ConvertImage.Image = convertedImage;

            } catch(Exception)
            {
                MessageBox.Show("Error while trying to convert image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                                                                                                                // zablokuj możliwość konwersji obrazka na 2 sekundy
            model.DisableButton(3, ConvertButton);

        }
       

        /// <summary>
        /// metoda wyciągająca tablice bajtów z obrazka oraz przekazująca je do obróbki w dllce
        /// </summary>
        /// <param name="InputImage">  przekazany obraz do przetworzenia Sepii </param>
        /// <param name="toneValue"> wartość zmiennej dla współczynnika wypełnienia </param>
        /// return void
        private void SepiaBitmap(Bitmap InputImage, int toneValue)
        {
            bool smallImage = false;
                                                                                                                // rozmiar obrazka jako x * y, gdzie x to szerokość i y to wysokość
            var rectangle = new Rectangle(0, 0, InputImage.Width, InputImage.Height);
                                                                                                                // dane o stosowanej konwecji ARGB
            var data = InputImage.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                                                                                                                // głębokość obrazka, czyli ilość bajtów na piksel
            var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;
                                                                                                                // tworzenie tablicy bajtów obrazka

            int smallImageSizeMultiplier = 256;                                                                 // zmienna do powiększania obrazka, aby poprawnie wykonać algorytm
             
            RGBValuesOfImage = new byte[data.Width * data.Height * depth];                                      // tworzenie tablicy bajtó obrazka na podstawie jego rozmiaru oraz ilości bajtów na piksel

            if(RGBValuesOfImage.Length < smallImageSizeMultiplier)                                              // warunek sprawdzający czy obrazek jest za mały
            {
                smallImage = true;                                                                              // tworzenie nowej tablicy, powiększonej o podany rozmiar zmiennej powiększającej
                RGBValuesOfImage = new byte[data.Width * data.Height * depth  + smallImageSizeMultiplier];
            }

            try {
                                                                                                                  // wykorzystywanie Marshal.Copy do zablokowanie na wyłączność podanej tablicy i skopiowanie danych z zdjęcia do niej   
             Marshal.Copy(data.Scan0, RGBValuesOfImage, 0, RGBValuesOfImage.Length);
                                                                                                                  // metoda rozpoczynająca konwersje obrazka w dll
             InvokeDll(CsharpRadioButton, AsmRadioButton, model.ConvertComboboxItemSelected(ActiveThreadsComboBox), toneValue);
                                                                                                                  // zakończenie pracy zegara
                model.StopWatch();
                                                                                                                   // wpisanie do logRichBoxa długości konwersji w Ms
             LogRichTextBox.Text = ("Conversion time: " + model.GetWatchInMs() + " Ms");
                                                                                                                   // odblokowanie tablicy bajtów
                if(smallImage == false)
            Marshal.Copy(RGBValuesOfImage, 0, data.Scan0, RGBValuesOfImage.Length);
                else
            Marshal.Copy(RGBValuesOfImage, 0, data.Scan0, RGBValuesOfImage.Length  - smallImageSizeMultiplier);
                                                                                                                    // informowanie oprogramowania do powrócenia przepływu sterowania danych
                this.Invalidate();
                                                                                                                    // odblokowanie bitów tablicy danych obrazka
            InputImage.UnlockBits(data);
                                                                                                                    // zapisywanie obrazka o formacie Bmp jako "ConvertedImage"
            InputImage.Save("Converted Image.bmp", ImageFormat.Bmp);
            }
            catch (Exception)                                                                                       // wyjątek w przypadku błedu podczas konwersji obrazka
            {
            
                MessageBox.Show("There was an error during image conversion!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

       
        /// <summary>
        /// Metoda przygotowująca delegaty i interwały wątków
        /// </summary>
        /// <param name="csharp"> radio button dla dllki C# w GUI </param>
        /// <param name="asm"> radio button dla dllki asm w GUI</param>
        /// <param name="threads"> wartość zmiennej int ilości wybranych wątków z comboboxa</param>
        /// <param name="toneValue">  wartość zmiennej współczynnika wypełnienia sepii </param>
        /// return void
        public void PrepareDelegates(RadioButton csharp,RadioButton asm, int threads, int toneValue) 
        {
                                                                                                                      // nie przekazano żadnej tablicy albo tablica jest pusta
            if (RGBValuesOfImage == null)
                return;
                                                                                                                      // wątek 1 dla  Asm, szczególny przypadek
            if (AppThreadAsm(asm, threads, toneValue) == true)
                return;
                                                                                                                      // wątek 1 dla C#, sczególny przypadek
            if (AppThreadCsharp(csharp, threads, toneValue) == true)
                return;
            arrayOfThreads = new Thread[threads];
            arrayOfArguments = new object[threads];
                                                                                                                       // podział tablice na wątki
            int interval = RGBValuesOfImage.GetLength(0) / threads;
                                                                                                                       // pętla tworząca interwały
            for(int i = 0; i< arrayOfThreads.GetLength(0); ++i)
            {
                int start = i * interval;
                                                                                                                       // uzupełenienie do pełnego piksela dla początku przedziału

                while(start %16 != 0)
                {
                    start += 1;   
                }
                int stop = (i + 1) * interval;
                                                                                                                       // uzupełenienie do pełnego piksela dla końcu przedziału
                while (stop %16 != 0)
                { 
                    stop += 1;
                }
                if (stop > RGBValuesOfImage.Length ||( (i == threads - 1) && stop != RGBValuesOfImage.Length))
                    stop = RGBValuesOfImage.Length;
                                                                                                                       // tablica obiektów przekazywana dla każdego wątku z odpowiednią wartością sepii
                arrayOfArguments[i] = new object[4] { start, stop, RGBValuesOfImage, toneValue };
                                                                                                                       // nowy wątek dla C#
                if (csharp.Checked) arrayOfThreads[i] = new Thread(new ParameterizedThreadStart(SepiaDll.Sepia.CSharpDllFunc));
                                                                                                                       // nowy wątek dla asmeblera
                else if (asm.Checked) arrayOfThreads[i] = new Thread(new ParameterizedThreadStart(AssemblerFunction));

            }  
        }
       
        /// <summary>
        /// Metoda tworząca i rozpoczynająca pracę nowych wątków
        /// </summary>
        /// return void
        private void CreateThreads()
        {
                                                                                                                      // brak tablicy wątków lub tablica jest pusta
            if (arrayOfThreads == null)
                return;
                                                                                                                      // początek zliczania czasu
            model.StartWatch();
            for (int i = 0; i< arrayOfThreads.GetLength(0); ++i)
            {
                                                                                                                      // start wątku
                arrayOfThreads[i].Start(arrayOfArguments[i]);
              
            }
        }
        /// <summary>
        /// Metoda czekająca i sprawdzająca czy wątek się zakończył
        /// </summary>
        /// return void
        private void WaitForThreads()
        {
                                                                                                                       // pusta tablica wątków 
            if (arrayOfThreads == null) return;

            bool done = false;
                                                                                                                      // sprawdzenie czy wątek się zakończył
            while (!done)
            {
                done = true;

                for (int i = 0; i < arrayOfThreads.GetLength(0); ++i)
                                                                                                                     // wartość bool sprawdzająca czy tablica wątków jest pusta lub czy dany wątek "nie żyje" 
                    done &= (arrayOfThreads[i] == null || !arrayOfThreads[i].IsAlive);
            }   
        }

        /// <summary>
        ///  Metoda przechowująca 3 metody: 1 metoda tworzenia delegatów, 2 metoda tworzenia wątkow oraz 3 metoda czekająca na wątki
        /// Proces przetwarzania danych do dllki, kontrola tworzenia i zakończenia trwania wątków
        /// </summary>
        /// <param name="csharpRadioButton"> radio button dla dllki C# w GUI</param>
        /// <param name="asmRadioButton"> radio button dla dllki asm w GUI</param>
        /// <param name="threads">wartość zmiennej int ilości wybranych wątków z comboboxa</param>
        /// <param name="toneValue"> wartość zmiennej współczynnika wypełnienia sepii</param>
        public void InvokeDll( RadioButton csharpRadioButton,RadioButton asmRadioButton, int threads, int toneValue)
        {
                                                                                                                        // metoda wykonująca przygytowoania na podział na wątki dla wykonania dllki
            PrepareDelegates(csharpRadioButton, asmRadioButton, threads, toneValue);
                                                                                                                        // metoda tworzenia wątków
            CreateThreads();
                                                                                                                       // metoda czekająca na wykonanie wątków
            WaitForThreads();

        }
       
        /// <summary>
        ///  // Metoda specjalna dla asemblera, aby rozłożyć danych obiekt na typy proste dla asemblera
        /// Wykorzystano klauzule unsafe, aby móc wykorzystać wątki
        /// </summary>
        /// <param name="argum"> obiekt 4 elementów zawierających informacje o początku oraz końcu przedziału i zarówno tablicy bajtów obrazu i wartości wypełnienia sepii</param>
        unsafe private void AssemblerFunction(object argum)
        {
                                                                                                                      // tworzenie oiektu typu Array, aby przechować niebędne elementy do wykonania dla asm dllki
             Array args = new object[4];
                                                                                                                      // rzutowanie parametru metody na obiekt typu Array utworzony wcześniej
             args = (Array)argum;
                                                                                                                      // zmienna int przechowująca wartość początku przedziału tablicy bajtów
             int start = (int)args.GetValue(0);
                                                                                                                      // zmienna int przechowująca wartość końca przedziału tablicy bajtów
             int stop  = (int)args.GetValue(1);
                                                                                                                      // wskaźnik na tablice bajtów obrazka
             byte[] table = (byte[])args.GetValue(2);
                                                                                                                      // zmienna int przechowująca wartość współczynnika wypełnienia sepii
             int toneValue = (int)args.GetValue(3);
                                                                                                                      // stały rozmiar tablicy, aby można było ją przekazać do rejestru i wykonania dla asm dllki
            fixed (byte* ptr = &table[0])
            {
                                                                                                                      // Start wykonywania dllki dla asemblera
                Sepia(ptr, start, stop, toneValue);
            }
        }
      
        /// <summary>
        ///   Metoda specjalna w przypadku jednego wątku visualowego dla C#
        /// Zasada działania podobna jak dla wielu wątków
        /// </summary>
        /// <param name="csharp">  radio button dla dllki C# w GUI</param>
        /// <param name="threads"> wartość zmiennej int ilości wybranych wątków z comboboxa</param>
        /// <param name="toneValue">wartość zmiennej współczynnika wypełnienia sepii</param>
        /// <returns> wartość bool czy użytkownik wybrał wykonanie algorytmu dla 1 wątku w c#</returns>
        private bool AppThreadCsharp(RadioButton csharp, int threads, int toneValue)
        {
            if (threads == 1 && csharp.Checked)                                                                             // warunek sprawdzający czy użytkownik wybrał 1 wątek
            {
                arrayOfThreads = null;
                int start = 0;
                int stop = RGBValuesOfImage.Length;                                                                         // uzupełenienie początku zliczania przedziału na 0 i końca na cały rozmiar tablicy bajtów obrazka

                arrayOfArguments = new object[4] { start, stop, RGBValuesOfImage, toneValue };                              // uzupełenie danych do tablicy obiektów przekazywanych do dllki wykonującej efekt Sepii

                model.StartWatch();                                                                                         // wykonanie zliczania  czasu wykonania algorytmu oraz  wejście do porcedury zliaczającej czas jego wykonania

                SepiaDll.Sepia.CSharpDllFunc(arrayOfArguments);
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Metoda specjalna w przypadku jednego wątku asemlerowego dla asemblera
        /// Zasada działania podobna jak dla wielu wątków
        /// </summary>
        /// <param name="asm">radio button dla dllki ASM w GUI</param>
        /// <param name="threads">wartość zmiennej int ilości wybranych wątków z comboboxa</param>
        /// <param name="toneValue">>wartość zmiennej współczynnika wypełnienia sepii</param>
        /// <returns>wartość bool czy użytkownik wybrał wykonanie algorytmu dla 1 wątku w asm</returns>
        private bool AppThreadAsm(RadioButton asm ,int threads ,int toneValue)
        {

            if (threads == 1 && asm.Checked)                                                                                // warunek sprawdzający czy użytkownik wybrał 1 wątek
            {
                arrayOfThreads = null;
                int start = 0;
                int stop = RGBValuesOfImage.Length;                                                                         // uzupełenienie początku zliczania przedziału na 0 i końca na cały rozmiar tablicy bajtów obrazka

                arrayOfArguments = new object[4] { start, stop, RGBValuesOfImage, toneValue };                              // uzupełenie danych do tablicy obiektów przekazywanych do dllki wykonującej efekt Sepii

                model.StartWatch();                                                                                          // wykonanie zliczania  czasu wykonania algorytmu oraz  wejście do porcedury zliaczającej czas jego wykonania

                AssemblerFunction(arrayOfArguments);
                return true;
            }
            return false;

        }

        private void LogRichTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
