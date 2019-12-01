using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SepiaDll
{
    public class Sepia
    {
      public static void  CSharpDllFunc(object args)
        {
            Array arguments = new object[4];
            arguments = (Array)args;
            

            int RGBstart = (int)arguments.GetValue(0);
            int RGBstop = (int)arguments.GetValue(1);
            byte[] rgbValues = (byte[])arguments.GetValue(2);
            int ToneValue = (int)arguments.GetValue(3);

            for (int i = RGBstart; i <= RGBstop - 4; i += 4)
            {

                int average = rgbValues[i];
                average += rgbValues[i + 1];
                average += rgbValues[i + 2];

                average /= 3;

                rgbValues[i] = (byte)average;
                rgbValues[i + 1] = (byte)average;
                rgbValues[i + 2] = (byte)average;

            }


            for (int i = RGBstart; i <= RGBstop - 4; i += 4)
            {
                if ((rgbValues[i + 1] + ToneValue) > 255) // zielony
                    rgbValues[i + 1] = 255;
                else
                    rgbValues[i + 1] = (byte)(rgbValues[i + 1] + ToneValue);

                if ((rgbValues[i + 2] + 2 * ToneValue) > 255) //czerwony
                    rgbValues[i + 2] = 255;
                else
                    rgbValues[i + 2] = (byte)(rgbValues[i + 2] + 2 * ToneValue);

            }


        }
            
        }
    }

