using System;



namespace SepiaDll
{
    //Klasa obsługująca efekt Sepii dla języka wysokiego poziomu.
    
    public class Sepia
    {
        //Metoda odpowiedzialna za koloryzowanie danego obrazu na sepie
        //Wykonuje najpierw w jednej pętli algorytm odcieni szarości dla obrazu
        // W drugiej pętli wykonuje algorytm wypełnienia sepią dla przetworzonego 
        // wcześniej obrazu w pierwszej pętli
        // parametry wejściowe procedury:
        // object args -> Jest to obiekt 4 elementów typu Array, zadeklarowanych w GUI
        // parametry, które przechowuje ta kolekcja są następujące:
        // RGBstart -> wartość typu int  początku przedziału tablicy bajtów obrazu
        // RGBstop -> wartość typu int koniec przedziału tablicy bajtów obrazu
        // sepiaValue -> wartość typu int wypełnienia sepii
      public static void  CSharpDllFunc(object args)
        {
                                                                 // tworzenie tablicy 4 obiektów typu Array, ponieważ takie elementy znajdują się w obiekie przekazywanym przez język wysokiego poziomu
            Array arguments = new object[4];
                                                                 // rzutowanie danego obiektu na Array
            arguments = (Array)args;
            
                                                                 //  zapisywanie do zmiennej początek przedziału tablicy bajtów dla danego waątku
            int RGBstart = (int)arguments.GetValue(0);
                                                                // zapisywanie do zmiennej koniec przedziału tablicy bajtów dla danego wątku
            int RGBstop = (int)arguments.GetValue(1);
                                                                // zapisywanie wskaźnika na tablice bajtów dla danego wątku
            byte[] rgbValues = (byte[])arguments.GetValue(2);
                                                                 // zapisywanie do zmiennej wartość efektu sepii
            int sepiaValue = (int)arguments.GetValue(3);

                                                                 // Początek obługi konwersji obrazka, otrzymanie efektu czarości
            for (int i = RGBstart; i <= RGBstop - 16; i += 16)
            {
                                                                //tworzenie zmiennej average i zapisanie danych tablicy i-tej do tej zmiennej
                int average = rgbValues[i];
                                                                //dodanie wartości i + 1 tablicy do average
                average += rgbValues[i + 1];
                                                                //dodanie wartości i + 2 tablicu do average
                average += rgbValues[i + 2];
                                                                //dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i-tej wartości tablicy
                rgbValues[i] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 1 wartości tablicy
                rgbValues[i + 1] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 2 wartości tablicy
                rgbValues[i + 2] = (byte)average;

                                                                //tworzenie zmiennej average i zapisanie danych tablicy i + 4 do tej zmiennej
                average = rgbValues[i + 4];
                                                                //dodanie wartości i + 5 tablicy do average
                average += rgbValues[i + 5];
                                                                //dodanie wartości i + 6 tablicu do average
                average += rgbValues[i + 6];
                                                                //dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i + 4 wartości tablicy
                rgbValues[i + 4] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 5 wartości tablicy
                rgbValues[i + 5] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 6 wartości tablicy
                rgbValues[i + 6] = (byte)average;
                

                                                                // tworzenie zmiennej average i zapisanie danych tablicy i + 8 do tej zmiennej
                average = rgbValues[i + 8];
                                                                // dodanie wartości i + 9 tablicy do average
                average += rgbValues[i + 9];
                                                                //dodanie wartości i + 10 tablicu do average
                average += rgbValues[i + 10];
                                                                //dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i + 8 wartości tablicy
                rgbValues[i + 8] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 9 wartości tablicy
                rgbValues[i + 9] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 10 wartości tablicy
                rgbValues[i + 10] = (byte)average;



                                                                //tworzenie zmiennej average i zapisanie danych tablicy i + 12 do tej zmiennej
                average = rgbValues[i + 12];
                                                                //dodanie wartości i + 13 tablicy do average
                average += rgbValues[i + 13];
                                                                //dodanie wartości i + 14 tablicu do average
                average += rgbValues[i + 14];
                                                                //dzielenie wartości average przez 3, aby otrzymać średnią
                average /= 3;
                                                                // wpisywanie jako bajt wartości średniej do i + 12 wartości tablicy
                rgbValues[i + 12] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 13 wartości tablicy
                rgbValues[i + 13] = (byte)average;
                                                                // wpisywanie jako bajt wartości średniej do i + 14 wartości tablicy
                rgbValues[i + 14] = (byte)average;

            }

                                                                //Pętla tworząca efekt sepii dla przetworzonego obrazka odcienie czarości
            for (int i = RGBstart; i <= RGBstop - 4; i += 4)
            {
                                                                //warunek sprawdzający czy przekroczono wartość 255 dla koloru zielonego
                if ((rgbValues[i + 1] + sepiaValue) > 255)
                                                                //Ustawienie maksymalnej dozwolonej wartości koloru, gdy przekroczono maskymalną wartość 255 dla składowej czerwonej piksela
                    rgbValues[i + 1] = 255;
                else
                    rgbValues[i + 1] = (byte)(rgbValues[i + 1] + sepiaValue);
                                                                                  // warunek sprawdzający czy przekroczono wartość 255 dla koloru czerwonego
                if ((rgbValues[i + 2] + 2 * sepiaValue) > 255) 
                                                                                 // ustawienie maksymalnej dozwolonej wartości koloru czerwonego, gdy przekroczono maksymalną wartość 255
                    rgbValues[i + 2] = 255;
                else
                    rgbValues[i + 2] = (byte)(rgbValues[i + 2] + 2 * sepiaValue); // dodanie podwójnej wartości  składowej wypełnienia do wartości koloru czerwonego piksela, wynikające z algorytmu

            }


        }
            
        }
    }

