using System;
using System.Windows.Forms;

//Autor: Michał Jankowski
//Dzień: 13.12.2019r.
//Przedmiot: Języki Asemblerowe
//Temat: Efekt Sepii


// Główna klasa programu
// Generowana autmoatycznie w przypadku towrzenia aplikacji okienka w WidowsForms
namespace SepiaApp
{
                                                     // statyczna klasa Programu tworzona przez WindowsForms
    static class Program
    {
       

       
        [STAThread]
                                                     //Główna metoda program w której tworzony jest program
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SepiaProgram());

          
           

        }
    }
}
