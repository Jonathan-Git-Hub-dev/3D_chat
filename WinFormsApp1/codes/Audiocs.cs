using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.codes
{
    internal class Audiocs
    {
    }
}
/*
//audio communications code
using (var waveIn = new WaveInEvent())
        {
            waveIn.DeviceNumber = 0;
            //(samples per second )
            waveIn.WaveFormat = new WaveFormat(4000,16, 1);
            waveIn.BufferMilliseconds = 100;//every half second
            //100
            //try some stuff
            //var waveOut = new WaveOutEvent();
            //var audioFileReader = new AudioFileReader(filePath);
            //waveOut.Init(e.Buffer);
            //waveOut.Play();
            WaveOutEvent _waveOut;
            BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
            bufferedWaveProvider.BufferLength = (int)(waveIn.WaveFormat.AverageBytesPerSecond * 0.5); // 2 seconds buffer
            bufferedWaveProvider.DiscardOnBufferOverflow = true; // Discard old data if buffer overflows

            // Initialize WaveOut (speaker output)
            _waveOut = new WaveOutEvent();
            _waveOut.Init(bufferedWaveProvider);

            // Start recording and playback
            //_waveIn.StartRecording();
            Byte[] last= new Byte[800];

            waveIn.DataAvailable += (sender, e) =>
            {
                Console.WriteLine(e.Buffer.Length.ToString() + " " + e.BytesRecorded.ToString());
                //maybe 2 bytes per sample

                //e.buffer
                //e.bytesrecorded
                //Console.WriteLine("got data");
                Byte[] temp = new Byte[800];
                Array.Copy(e.Buffer, temp, 800);
                //combine with last
                for (int i=0; i<last.Length; i+=2)
                {
                    //do for 2byes at a time

                    byte[] byteArray = { e.Buffer[i], e.Buffer[i+1] };
                    // Convert the byte array to a short using BitConverter.ToInt16
                    short resultShort = BitConverter.ToInt16(byteArray, 0);
                    resultShort /= 2;
                    //byte[] bytes = BitConverter.GetBytes(resultShort);
                    //e.Buffer[i] = bytes[0];
                    //e.Buffer[i+1] = bytes[1];

                    byte[] byteArray2 = { last[i], last[i + 1] };
                    // Convert the byte array to a short using BitConverter.ToInt16
                    short resultShort2 = BitConverter.ToInt16(byteArray2, 0);
                    resultShort2 /= 2;
                    resultShort += resultShort2;
                    byte[] bytes = BitConverter.GetBytes(resultShort);
                    e.Buffer[i] = bytes[0];
                    e.Buffer[i + 1] = bytes[1];


                    //e.Buffer[i] = (Byte)((int)e.Buffer[i] / 2);
                    //e.Buffer[i] = (Byte)(((int)e.Buffer[i]/2 + (int)last[i]/2));
                    //e.Buffer[i] = 0;// (Byte)(((int)e.Buffer[i] + (int)last[i]) / 2);
                }

                Array.Copy(temp, last, 1600);
                bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);



            };

            _waveOut.Play();
            waveIn.StartRecording();
            Console.ReadKey();
            waveIn.StopRecording();
        }



 
*/

