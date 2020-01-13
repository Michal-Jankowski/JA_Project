using System;

//Autor: Michał Jankowski
// Dzień: 13.12.2019r.
// Przedmiot: Języki Asemblerowe
// Temat: Efekt Sepii

//Changelog:
//
// v. 0.1  26/10/19r.
//  Dodanie pustej dllki. Połączenie komunikacji dllki z GUI C#.
//
// v. 0.2 27/10/19r.
//  Utworzenie metody CSharpDLLFUnc(object args), gdzie zostały przekazane argumenty z GUI.
//  Parsowanie obiektów na odpowiednie elementy do algorytmu, m.in. tablice bajtów obrazka
//
// v. 0.3  30/10/19r. 
// Dodanie algorytmu przetwarzania obrazka na odcienie szarości. Pierwsza część algorytmu działa poprawnie.
//
// v. 0.4 4/11/19r.
// Sprawdzenie poprawności  działania wykonywanego algorytmu dla odcienie szarości. Aktualnie przetwarzanie 1 piksela za
// jedną iteracją pętli.
//
// v. 0.5 12/11/19r. 
// Dodanie drugiej części algorytmu poprzez implementacje pętli z wypełnaniem współczynnika sepii do skłądowych piksela.
// Sprawdzenie poprawności wykonywanego algorytmu.
//
//v. 0.6 6/12/19r.
// Powiększenie ilości przetwarzanych pikseli w jednej iteracji dla odcieni szarości do 4 pikseli.
// Sprawdzenie poprawności przetwarzanych elementów dla zwiększonej ilości pikseli.
//
//v. 0.7 12/12/19r.
// Dodanie początkowych komentarzy dla pierwszej częsci algorytmu i wszystkich używanych zmiennych.
//
// v. 1.0 12/01/19r.
// Poprawa czytelności komentarzy i dodanie odpowiednich odstępów w kodzie.
// Ostateczne sprawdzenie zgodności algorytmu z założeniami 
//

namespace SepiaDll
{
    // klasa obsługująca efekt Sepii dla języka wysokiego poziomu.
    
    public class Sepia
    {

        /// <summary>
        ///  metoda odpowiedzialna za koloryzowanie danego obrazu na sepie
        /// wykonuje najpierw w jednej pętli algorytm odcieni szarości dla obrazu
        /// w drugiej pętli wykonuje algorytm wypełnienia sepią dla przetworzonego 
        /// wcześniej obrazu w pierwszej pętli
        /// </summary>
        /// <param name="args">
        /// object args -> Jest to obiekt 5 elementów typu Array, zadeklarowanych w GUI
        /// 
        /// parametry, które przechowuje ta kolekcja są następujące:
        /// RGBstart -> wartość typu int  początku przedziału tablicy bajtów obrazu
        /// RGBstop -> wartość typu int koniec przedziału tablicy bajtów obrazu
        /// sepiaValue -> wartość typu int wypełnienia sepii
        /// depthVallue -> wartość typu int głębii sepii
        /// </param>
        /// returns void 
        public static void  CSharpDllFunc(object args)
        {
                                                                 // tworzenie tablicy 5 obiektów typu Array, ponieważ takie elementy znajdują się w obiekie przekazywanym przez język wysokiego poziomu
            Array arguments = new object[5];
                                                                 // rzutowanie danego obiektu na Array
            arguments = (Array)args;
            
                                                                 //  zapisywanie do zmiennej początek przedziału tablicy bajtów dla danego waątku
            int RGBstart = (int)arguments.GetValue(0);
                                                                 // zapisywanie do zmiennej koniec przedziału tablicy bajtów dla danego wątku
            int RGBstop = (int)arguments.GetValue(1);
                                                                 // zapisywanie wskaźnika na tablice bajtów dla danego wątku
            byte[] rgbValues = (byte[])arguments.GetValue(2);
                                                                 // zapisywanie do zmiennej wartości wypełnienia sepią
            int sepiaValue = (int)arguments.GetValue(3);
                                                                 // zapisywanie do zmiennej wartości głębii sepii
            int depthValue = (int)arguments.GetValue(4);         
                                                                 // Początek obługi konwersji obrazka, otrzymanie efektu czarości
            for (int i = RGBstart; i <= RGBstop - 32; i += 32)
            {
                                                                // tworzenie zmiennej average i zapisanie danych tablicy i-tej do tej zmiennej
                int average = rgbValues[i];
                                                                // dodanie wartości i + 1 tablicy do average
                average += rgbValues[i + 1];
                                                                // dodanie wartości i + 2 tablicu do average
                average += rgbValues[i + 2];
                                                                // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i-tej wartości tablicy
                rgbValues[i] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 1 wartości tablicy
                rgbValues[i + 1] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 2 wartości tablicy
                rgbValues[i + 2] = (byte)average;

                                                                // przypisanie do zmiennej average  danych tablicy i + 4
                average = rgbValues[i + 4];
                                                                // dodanie wartości i + 5 tablicy do average
                average += rgbValues[i + 5];
                                                                // dodanie wartości i + 6 tablicu do average
                average += rgbValues[i + 6];
                                                                // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i + 4 wartości tablicy
                rgbValues[i + 4] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 5 wartości tablicy
                rgbValues[i + 5] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 6 wartości tablicy
                rgbValues[i + 6] = (byte)average;


                                                                // przypisanie do zmiennej average  danych tablicy i + 8
                average = rgbValues[i + 8];
                                                                // dodanie wartości i + 9 tablicy do average
                average += rgbValues[i + 9];
                                                                // dodanie wartości i + 10 tablicu do average
                average += rgbValues[i + 10];
                                                                // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i + 8 wartości tablicy
                rgbValues[i + 8] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 9 wartości tablicy
                rgbValues[i + 9] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 10 wartości tablicy
                rgbValues[i + 10] = (byte)average;



                                                                // przypisanie do zmiennej average  danych tablicy i + 12
                average = rgbValues[i + 12];
                                                                // dodanie wartości i + 13 tablicy do average
                average += rgbValues[i + 13];
                                                                // dodanie wartości i + 14 tablicu do average
                average += rgbValues[i + 14];
                                                                // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i + 12 wartości tablicy
                rgbValues[i + 12] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 13 wartości tablicy
                rgbValues[i + 13] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 14 wartości tablicy
                rgbValues[i + 14] = (byte)average;
                                                       
                                                                // przypisanie do zmiennej average  danych tablicy i + 16
                average = rgbValues[i + 16];
                                                                // dodanie wartości i + 17 tablicy do average
                average += rgbValues[i + 17];
                                                               // dodanie wartości i + 18 tablicu do average
                average += rgbValues[i + 18];
                                                               // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                               // wpisywanie jako bajt wartości średniej do i + 16 wartości tablicy
                rgbValues[i + 16] = (byte)average;
                                                               // wpisywanie jako bajt wartości średniej do i + 17 wartości tablicy
                rgbValues[i + 17] = (byte)average;
                                                               // wpisywanie jako bajt wartości średniej do i + 18 wartości tablicy
                rgbValues[i + 18] = (byte)average;


                                                              // przypisanie do zmiennej average  danych tablicy i + 20
                average = rgbValues[i + 20];
                                                              // dodanie wartości i + 20 tablicy do average
                average += rgbValues[i + 21];
                                                              // dodanie wartości i + 21 tablicu do average
                average += rgbValues[i + 22];
                                                             // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                             // wpisywanie jako bajt wartości średniej do i + 20 wartości tablicy
                rgbValues[i + 20] = (byte)average;
                                                             // wpisywanie jako bajt wartości średniej do i + 21 wartości tablicy
                rgbValues[i + 21] = (byte)average;
                                                             // wpisywanie jako bajt wartości średniej do i + 22 wartości tablicy
                rgbValues[i + 22] = (byte)average;

                                                             // przypisanie do zmiennej average  danych tablicy i + 24
                average = rgbValues[i + 24];
                                                             // dodanie wartości i + 25 tablicy do average
                average += rgbValues[i + 25];
                                                             // dodanie wartości i + 26 tablicu do average
                average += rgbValues[i + 26];
                                                             // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                             // wpisywanie jako bajt wartości średniej do i + 24 wartości tablicy
                rgbValues[i + 24] = (byte)average;
                                                             // wpisywanie jako bajt wartości średniej do i + 25 wartości tablicy
                rgbValues[i + 25] = (byte)average;
                                                             // wpisywanie jako bajt wartości średniej do i + 26 wartości tablicy
                rgbValues[i + 26] = (byte)average;


                                                            // przypisanie do zmiennej average  danych tablicy i + 28
                average = rgbValues[i + 28];
                                                            // dodanie wartości i + 29 tablicy do average
                average += rgbValues[i + 29];
                                                            // dodanie wartości i + 30 tablicu do average
                average += rgbValues[i + 30];
                                                            // dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                            // wpisywanie jako bajt wartości średniej do i + 28 wartości tablicy
                rgbValues[i + 28] = (byte)average;
                                                            // wpisywanie jako bajt wartości średniej do i + 29 wartości tablicy
                rgbValues[i + 29] = (byte)average;
                                                            // wpisywanie jako bajt wartości średniej do i + 30 wartości tablicy
                rgbValues[i + 30] = (byte)average;
                
            }

                                                                //Pętla tworząca efekt sepii dla przetworzonego obrazka odcienie czarości
            for (int i = RGBstart; i <= RGBstop - 4; i += 4)
            {
                if (rgbValues[i] < depthValue)                  // warunek sprawdzający czy podana wartość głębii sepii jest większa niż wartość składowej niebieskiej piksela

                    rgbValues[i] = 0;                           // jeżeli wartość składowej jest mniejsza to ustawiana jest ona na 0
                else
                   rgbValues[i] -= (byte)depthValue;            // jeżeli wartość głębii sepii jest mniejsza od składowej niebiesiej piksela to następuje przypisanie do składowej niebieskiej wartości pomniejszonej o wartość głębii
                                                                
                if (rgbValues[i + 1] > (255 - sepiaValue))      // warunek sprawdzający czy przekroczono wartość 255 dla koloru zielonego
                                                                
                    rgbValues[i + 1] = 255;                     // ustawienie maksymalnej dozwolonej wartości koloru, gdy przekroczono maskymalną wartość 255 dla składowej czerwonej piksela
                else                                            
                    rgbValues[i + 1] +=  (byte)sepiaValue;      // ustawienie dla wartości składowej czerwonej piksela  wypełenienia współczynnikiem sepii wynikającej z algorytmu
                                                            
                if (rgbValues[i + 2] > (255 - 2 * sepiaValue))  // warunek sprawdzający czy przekroczono wartość 255 dla koloru czerwonego
                                                               
                    rgbValues[i + 2] = 255;                     // ustawienie maksymalnej dozwolonej wartości koloru czerwonego, gdy przekroczono maksymalną wartość 255
                else
                    rgbValues[i + 2] += (byte)(2 * sepiaValue); // dodanie podwójnej wartości  składowej wypełnienia do wartości koloru czerwonego piksela, wynikające z algorytmu

            }


        }
            
        }
    }

