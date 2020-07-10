using Meadow;
using Meadow.Devices;
using System;
using System.Threading;
// Uncomment below if you want to use Meadow.Foundation Schema.
//using Meadow.Foundation.ICs.IOExpanders;
//using System.Linq;

namespace ShiftRegister
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        // comment below variable if you want to use Meadow.Foundation Schema
        Shift74HC595 shift;
        // Uncomment below variable to use Meadow.Foundation Schema
        //x74595 shiftRegister;
        
        public MeadowApp()
        {
            Console.WriteLine("Initializing....");
            Initialize();
            Loop();
        }

        void Initialize()
        {
            // comment below code if you want to use Meadow.Foundation Schema.
            shift = new Shift74HC595(
                Device,
                Device.Pins.D15,
                Device.Pins.D14,
                Device.Pins.D13,
                Device.Pins.D12);
            
            Console.WriteLine("Clear Register");
            shift.Clear();
            // Uncomment below code if you want to use Meadow.Foundation Schema.
            /*
            shiftRegister = new x74595(Device, Device.CreateSpiBus(), Device.Pins.D03, 8);
            */
        }

        void Loop()
        {
            while(true)
            {
                // comment this code if you want to use Meadow.Foundation schema
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
                /* Uncomment if you want to use Meadow.Foundation schema
                foreach(var pin in shiftRegister.Pins.AllPins)
                {
                    shiftRegister.WriteToPin(pin, true);
                    Thread.Sleep(100);
                    shiftRegister.WriteToPin(pin, false);
                }
                foreach(var pin in shiftRegister.Pins.AllPins.Reverse())
                {
                    shiftRegister.WriteToPin(pin, true);
                    Thread.Sleep(100);
                    shiftRegister.WriteToPin(pin, false);
                }
                */
            }
        }        
    }
}
