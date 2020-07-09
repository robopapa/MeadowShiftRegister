using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Distance;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowApplication2
{
    public class HCSR_04 : IRangeFinder
    {
        public float SpeedOfSoundFactor => 580.0f;
        private float _CurrentDistance = -1;

        public float CurrentDistance
        {
            get
            {
                if (_CurrentDistance == -1) return _CurrentDistance;
                switch (MeasureUnit)
                {
                    case Unit.Inches:
                        return _CurrentDistance / 2.54f;
                    case Unit.Meter:
                        return _CurrentDistance * 100;
                    case Unit.Feet:
                        return _CurrentDistance / 30.48f;
                    default:
                        return _CurrentDistance;
                }
            }
            private set { _CurrentDistance = value; }
        }

        public float MinimumDistance => 2;

        public float MaximumDistance => 400;

        public Unit MeasureUnit { get; private set; } = Unit.Centimeter;

        public event EventHandler<DistanceEventArgs> DistanceDetected = delegate { };

        protected IDigitalInputPort echo;
        protected IDigitalOutputPort trigger;
        protected long duration = 0;
        protected bool _IsStarted = false;
        private HCSR_04() { }
        public HCSR_04(IIODevice device, IPin echo, IPin trigger, Unit measureUnit) : this(device.CreateDigitalInputPort(echo, InterruptMode.EdgeBoth), device.CreateDigitalOutputPort(trigger, false), measureUnit) { }
        public HCSR_04(IDigitalInputPort e, IDigitalOutputPort t, Unit measureUnit)
        {
            echo = e;
            trigger = t;
            //echo.Changed += EchoChanged;
            MeasureUnit = measureUnit;
        }

        public void Start()
        {
            //Console.WriteLine("Start._IsStarted={0}", _IsStarted);
            if (_IsStarted) return;

            CurrentDistance = -1; // reset currentDistance.
            var startTicks = DateTime.Now.Ticks;
            trigger.State = false;
            //Thread.Sleep(1);
            while (DateTime.Now.Ticks - startTicks < 10) ;
            startTicks = DateTime.Now.Ticks;
            trigger.State = true;
            //Thread.Sleep(1);
            while (DateTime.Now.Ticks - startTicks < 80) ;

            trigger.State = false; // now we need to wait for the echo.
            //Console.WriteLine("Start: value of echo:{0}", echo.State);
            _IsStarted = true;
            duration = DateTime.Now.Ticks; // Start the clock.
        }

        private void EchoChanged(object sender, DigitalInputPortEventArgs e)
        {
            //Console.WriteLine("value of echo:{0}", echo.State);
            if (!_IsStarted)
            {
                //Console.WriteLine("Start is false");
                return;
            }
            
            if (e.Value)
            {
                //Console.WriteLine("Echo value is true, reset duration");
                //duration = DateTime.Now.Ticks; // Start the clock.
                return;
            }

            // Echo got something back. We can calculate the duration.
            var endDate = DateTime.Now.Ticks;
            var delta = endDate - duration;
            //Console.WriteLine("delta {0}", delta);
            CurrentDistance = delta / SpeedOfSoundFactor;
            DistanceDetected?.Invoke(this, new DistanceEventArgs(CurrentDistance));
            _IsStarted = false;
        }
    }


    public enum Unit
    {
        Centimeter,
        Inches,
        Meter,
        Feet
    }


}
