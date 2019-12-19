

//Autor: Michał Jankowski
//Dzień: 13.12.2019r.
//Przedmiot: Języki Asemblerowe
//Temat: Efekt Sepii


//Changelog:
//

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SepiaGUI.Model
{
    //Klasa odpowiedzialna za przetrzymywanie  pomniejszych metod dla widoku
    // Pozwala na większą przejrzystość głównej klasy modelu
    class ImageAndEnvironmentalDataModel
    {
                                                                                                                     //Obiekt Stopwatch wykorzystywany do zliczania czasu
        readonly Stopwatch watch = new Stopwatch();
        /// <summary>
        /// Metoda ustawiająca ilość logicznych procesorów w labelu logicalProcessorsLabel
        /// </summary>
        /// <param name="logicalProcessorsLabel"> obiekt Label z widoku do ustawienia liczby logicznych procesorów</param>
        /// <param name="logicalProcessors"> string przechowujay informacje o libczie logicznych procesorow</param>
        
        public void SetLogicalProcessors(Label logicalProcessorsLabel, String logicalProcessors)
        {
                                                                                                                    // Ustawienie w labelu informacji o liczbie logicznych proceosrów
            logicalProcessorsLabel.Text = logicalProcessors;

        }
        /// <summary>
        /// Metoda zliczająca optymalną ilość logicznych procesorów wykorzystująca rejestry
        /// </summary>
        /// <param name="optimal"> wartość bool do ustawiania czy potrzbujemy optymalna czy nieoptymalna liczbie logicznych procesorów</param>
        /// <returns>String informujący o optymalnej liczbie logicznych procesorów</returns>
        
        public String CountLogicalProcessors(bool optimal)
        {
                                                                                                                      // String pobierający ze zmiennej środowiskowej informacje o liczbie logicznych procesorów
            String numberOfLogicalProcessors = Environment.ProcessorCount.ToString();
                                                                                                                      // niepotymalna liczba proceosorów (warunek)
            if (optimal == false)
                return numberOfLogicalProcessors;

                                                                                                                      // ustawienie optymalnej liczby procesorów
            int logicalProcessors = Convert.ToInt32(numberOfLogicalProcessors) - 1;
                                                                                                                      //zwrócenie danej liczby proceosrów w zależności od ustawienia boola
            return (logicalProcessors.ToString());

        }
        /// <summary>
        /// Metoda zliczająca liczbe rdzeni procesora i ustawiająca je w labelu CoresLabel
        /// </summary>
        /// <param name="CoresLabel"> Label odpowiadający za liczbę rdzeni w procesorze i ustawiający tę wartość w Labelu widoku</param>
        /// return void
        public void CountCores(Label CoresLabel)
        {

            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())   // wykorzystanie zapytania w celu uzyskania z rejestru liczby rdzeni
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());                                                 // liczba rdzenie w postaci int
            }

            CoresLabel.Text = coreCount.ToString();                                                                       // ustawienie wartości rdzeni w labelu 

        }

        
        /// <summary>
        /// /Metoda zliczjąca liczbę fizycznych rdzeni i  ich uwstawienie w setPhysicalProcessors
        /// </summary>
        /// <param name="physicalProcessorsLabel"> Label ustawiający liczbę fizycznych rdzenie w widoku</param>
        /// return void
        public void CountPhysicalProcessors(Label physicalProcessorsLabel)
        {
            int physicalCores = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get()) // wykorzystanie zapytania w celu uzyskania z rejestru liczby fizycznych rdzeni
            {

                physicalCores += int.Parse(item["NumberOfProcessors"].ToString());                                            // liczba fizycznych rdzeni w postaci int


            }

            SetPhysicaProcessors(physicalProcessorsLabel, physicalCores);                                                    // ustawienie fizycznych procesorów w metodzie SetPhysicalProcessors
            
        }
        /// <summary>
        ///  Mtoda ustawiająca liczbę fizycznych rdzeni w widoku
        /// </summary>
        /// <param name="textLabel"> Label do ustawienia fizycznych rdzeni w widoku</param>
        /// <param name="physicalCores"> int wartość fizycznych rdzeni</param>
        public void SetPhysicaProcessors(Label textLabel, int physicalCores)
        {
            textLabel.Text = physicalCores.ToString();                                                                       // ustawienie w labelu wartości fizycznych rdzeni
        }
        //
        /// <summary>
        /// Metoda czyszcząca pole załadowanego obrazka w GUI, aby zapobiec jego nawarstwianiu się.
        /// </summary>
        /// <param name="picture"> obiekt PictureBox zawierającego obrazek w GUI </param>
        public void ClearImage(PictureBox picture)
        {
            if (picture.Image != null)                                                                                        // warunek sprawdzajacy czy obrazek nie jest nullem
            {
                picture.Image.Dispose();                                                                                      // usuwanie zawartości obrazka z GUI
                picture.Image = null;                                                                                         // ustawienie pola  obrazka na null                                                              
            }
        }
        /// <summary>
        /// Metoda zwracająca aktualnie wybraną wartość combobox w postaci inta
        /// </summary>
        /// <param name="combobox"> dany combobox, który posiada wartość int</param>
        /// <returns> wartość combobox w int</returns>
        public int ConvertComboboxItemSelected(ComboBox combobox)
        {
            if (combobox.SelectedItem != null)                                                                                  // szukana wartość null
            {
                int value = Int32.Parse(combobox.SelectedItem.ToString());                                                      // wartość int combbox szukanej wartości
                return value;
            }
            else
            {
                return 0;                                                                                                        // jeśli nie znajdzie to ustawia 0
            }
        }
        /// <summary>
        /// Metoda ustawiająca aktualnie obsługiwany format obrazka jako jpg oraz bmp
        /// </summary>
        /// <param name="openfile"> OpenFileDialog obiekt otwieranego okienka GUI w widoku</param>
        /// return void
        public void SetImageFilter(OpenFileDialog openfile)
        {
            openfile.Filter = "Image Files( *.jpg; *.bmp;)| *.jpg; *.bmp;";                                                     // ustala obłsugę .jpg oraz .bmp plików

        }
        
        /// <summary>
        ///  Metoda wyłączająca obsługę obrazka na daną ilość sekund po jego kliknięciu
        /// </summary>
        /// <param name="seconds"> wartoś w sekundach opóxnienia</param>
        /// <param name="button"> danyc obiekt button który jest opóźniany</param>
        /// return void
        public async void DisableButton(int seconds, Button button)
        {
            button.Enabled = false;
            await Task.Delay(1000 * seconds);                                                                                    // metoda opóżniająca asynchronicznie button
            button.Enabled = true;

        }
        
        /// <summary>
        ///  Metoda sprawdzająca wersję frameworka
        /// </summary>
        /// <returns> wartość bool czy istnieje framework i odpowienia wersja frameworka</returns>
        public static bool CheckFrameworkVersion()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";                                                       // tworzenie stringa subkey w celu dotacia do danej wartości rejetstru, która przechowuję inforamcje o wersji .net frameorka

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))                  // pobranie wartościu wersji .net frameworka na podstawie rejestru i danego subkeya
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)                                                                       // sprawdzenie czy istnieje taki subkey i czy jest zainstalowany taki framework
                {
                    if (CheckFor45PlusVersion((int)ndpKey.GetValue("Release")))                                                                 // sprawdzenie czy jest to wersja 4.5> i pobranie wartości wersji frameworka
                        return true;
                }
                else
                {
                    return false;
                }
                return false;
            }
            /// <summary>
            ///  Metoda zwracająca infrormacje czy o obsługiwana wersja .net frameowrka jest odpowiednia dla programu
            /// </summary>
            /// <param name="releaseKey"> int wartość wyszukanej wersji frameworka</param>
            /// <returns> bool wartość czy obsługuję daną wersje frameworka </returns>
            bool CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 461808)                                                                                                   // warunek czy wersja frameworka wyższa niż 4.6 
                    return true;


                return false;
            }


        }
       
        /// <summary>
        /// Metoda rozpoczynająca zlicanie czasu dla Stopwatch zegara
        /// </summary>
        /// return void
        public void StartWatch()
        {
            watch.Start();
        }
       
        /// <summary>
        /// Metoda zatrzymująca zlicanie czasu dla Stopwatch zegara
        /// </summary>
        /// return void
        public void StopWatch()
        {
            watch.Stop();
        }
        
        /// <summary>
        /// Metoda pobierająca czas zegara oraz restartująca zegar
        /// </summary>
        /// <returns> String wartości zliczonego czasu</returns>
        public string GetWatchInMs()
        {
            string elapsedMs = watch.ElapsedMilliseconds.ToString();                        // pobranie wartości zliczonego czasu  do zmiennej
            watch.Reset();                                                                  // resetowanie wartości zegara i ustawianie na 0
            return elapsedMs;                                                               // zwrócenie wartości zegara
        }
        /// <summary>
        /// Metoda konwertująca dany format obrazka na bitmape
        /// </summary>
        /// <param name="fileName"> string lokalizacji obrazka pobrany z widoku</param>
        /// <returns> Bitmap obiekt skonwertowanego obrazka</returns>
        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using(Stream bmpStream = File.Open(fileName, FileMode.Open))                  // wykorzystanie obiektu Stream do otwarcia obrazka za pomocą metody File.open
            {
                Image image = Image.FromStream(bmpStream);                                // tworzenie skonwertowanego obrazka za pomocą obiektu Image

                bitmap = new Bitmap(image);                                              // tworzenie nowego obiektu bitmapy jako skonewertowanego obrazka
            }

            return bitmap;                                                               // zwrocenie skonwertowanego obrazka
        }
    }   
}
