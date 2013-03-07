using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace Rememo
{
    class EarconManager
    {

        public MediaPlayer mp;

        public void Playback(String filename){
            mp = new MediaPlayer();
            mp.Open(new Uri(filename));
            Console.WriteLine("This was the filename: " + filename);
            mp.Play();
        }
    }

    
}
