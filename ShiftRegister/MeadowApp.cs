using Meadow;
using Meadow.Devices;
using System;
using System.Threading;

namespace ShiftRegister
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        Shift74HC595 shift;
        
        public MeadowApp()
        {
            Console.WriteLine("Initializing....");
            Initialize();
            Console.WriteLine("set first pin to 1");

            //shift.SendData(true);
            //shift.SendData(true);
            //shift.SendData(false);
            //shift.SendData(true);
            //shift.SendData(false);
            //shift.Update();

            //shift.SendData(11);
            //shift.Update();
            Loop();
            /*for(var index = 1; index < 256; ++index)
            {
                shift.SendData((byte)index);
                Thread.Sleep(2000);
            }
            Thread.Sleep(750);
            for (var index = 255; index >= 0; --index)
            {
                shift.SendData((byte)index);
                Thread.Sleep(750);
            }*/
        }

        void Initialize()
        {
            shift = new Shift74HC595(
                Device,
                Device.Pins.D15,
                Device.Pins.D14,
                Device.Pins.D13,
                Device.Pins.D12);

            Console.WriteLine("Clear Register");
            shift.Clear();
        }

        void Loop()
        {
            while(true)
            {
                for(int index=0; index < 9; ++index)
                {
                    shift.SendData((byte)Math.Pow(2,index));
                    shift.Update();
                    Thread.Sleep(100);
                }

                for(var index=8; index >=0; --index)
                {
                    shift.SendData((byte)(Math.Pow(2, index) - 1));
                    shift.Update();
                    Thread.Sleep(100);
                }
            }
        }        
    }
}
