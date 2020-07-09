using Meadow.Hardware;
using System;
using System.Threading;


namespace ShiftRegister
{
    public class Shift74HC595
    {
        protected IDigitalOutputPort data;
        protected IDigitalOutputPort latch;
        protected IDigitalOutputPort clock;
        protected IDigitalOutputPort clear;
        protected IDigitalOutputPort outputEnable;
        private Shift74HC595() { }
        /// <summary>
        /// This will explain the pins for 74HC595
        /// </summary>
        /// <param name="device">Board Device.</param>
        /// <param name="data">Pin 14 on the 74. That will indicate what value of bit we want to push to the shift register. Also known as SER</param>
        /// <param name="latch">Pin 12 on the 74. This will push to Qa - Qh when set to HIGH all of the values we included thus far. Also known as RCLK.</param>
        /// <param name="clock">Pin 11 on the 74. Thi swill push one bit from <see cref="data"/> to the shift register temp storage when set to HIGH.</param>
        /// <param name="clear">Pin 10 on the 74. Inverse pin. Normally should be set to LOW. When clearing need to be set to HIGH. use <see cref="Clear"/> function </param>
        /// <param name="outputEnable">Pin 13 on the 74. Invese pin just like <see cref="clear"/> pin. Normally want to be set to LOW. When High output pins are disabled. <see cref="Disable"/> and <see cref="Enable"/> functions.</param>
        public Shift74HC595(IIODevice device, IPin data, IPin latch, IPin clock, IPin clear = null, IPin outputEnable = null) :
            this(
                device.CreateDigitalOutputPort(data),
                device.CreateDigitalOutputPort(latch),
                device.CreateDigitalOutputPort(clock),
                clear == null ? null : device.CreateDigitalOutputPort(clear, true),
                outputEnable == null ? null : device.CreateDigitalOutputPort(outputEnable)
                )
        { }
        /// <summary>
        /// This will explain the pins for 74HC595
        /// </summary>
        /// <param name="device">Board Device.</param>
        /// <param name="data">Pin 14 on the 74. That will indicate what value of bit we want to push to the shift register. Also known as SER</param>
        /// <param name="latch">Pin 12 on the 74. This will push to Qa - Qh when set to HIGH all of the values we included thus far. Also known as RCLK.</param>
        /// <param name="clock">Pin 11 on the 74. Thi swill push one bit from <see cref="data"/> to the shift register temp storage when set to HIGH.</param>
        /// <param name="clear">Pin 10 on the 74. Inverse pin. Normally should be set to HIGHT. When clearing need to be set to LOW. use <see cref="Clear"/> function </param>
        /// <param name="outputEnable">Pin 13 on the 74. Invese pin just like <see cref="clear"/> pin. Normally want to be set to LOW. When High output pins are disabled. <see cref="Disable"/> and <see cref="Enable"/> functions.</param>
        public Shift74HC595(IDigitalOutputPort data, IDigitalOutputPort latch, IDigitalOutputPort clock, IDigitalOutputPort clear = null, IDigitalOutputPort outputEnable = null)
        {
            this.data = data;
            this.latch = latch;
            this.clock = clock;
            this.clear = clear;
            this.outputEnable = outputEnable;
        }
        public void SendData(bool bit)
        {
            //Console.Write(bit);
            data.State = bit;
            clock.State = true;
            clock.State = false;

        }
        public void SendData(byte data, bool mostSignificantBit = false)
        {
            var length = sizeof(byte) * 8;
            latch.State = false;
            //Console.Write("{0} = ", data);
            for (var index = 0; index < length; ++index)
            {
                //Console.Write("{0} = {1}", 1 << (length -index), data & (1 << (length - index)));
                SendData(mostSignificantBit ? 
                    ((data & (1 << index)) != 0) :
                    ((data & (1 << (length - index))) != 0));   
            }
            //Console.WriteLine();
            latch.State = true;
            latch.State = false;
        }

        /// <summary>
        /// send binary values represeted as a string. This function is good if you have multiple shift registers 
        /// </summary>
        /// <param name="data">string represending binary values to send to shift register/s</param>
        /// <param name="mostSignificantBit">Start from most significant bit (i.e. end of binary values) or beginning.</param>
        public void SendData(string data, bool mostSignificantBit = false)
        {
            if (mostSignificantBit)
            {
                var str = data.ToCharArray();
                Array.Reverse(str);
                data = new string(str);
            }

            for (var index = data.Length - 1; index >= 0; --index)
            {
                SendData(data[index] == '1');
            }

        }
        public bool Update()
        {

            Console.WriteLine("Setting Latch to hight.... get ready");
            latch.State = true;
            latch.State = false;
            return true;
        }
        public bool Clear()
        {
            if (clear == null)
                throw new Exception("Clear pin wasn't set");
            Console.WriteLine("Clearing resister");
            clear.State = false;
            latch.State = true;
            Thread.Sleep(1);
            latch.State = false;
            clear.State = true;

            return true;
        }
        public bool Disable()
        {
            if (outputEnable == null)
                throw new Exception("Output Enable pin wasn't set");

            outputEnable.State = true;

            return true;
        }
        public bool Enable()
        {
            if (outputEnable == null)
                throw new Exception("Output Enable pin wasn't set");

            outputEnable.State = false;
            return true;
        }
    }
}
