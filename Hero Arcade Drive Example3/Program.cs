using System;
using System.Threading;
using Microsoft.SPOT;
using System.Text;


using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
//`using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Gadgeteer.Module;

namespace Hero_Arcade_Drive_Example3
{
    public class Program
    {
        /* create PWM controllers 
         note the PWM output are sent to two controller on each side by splitting the PWM data cable.*/
        static PWMSpeedController victor_right = new PWMSpeedController(CTRE.HERO.IO.Port3.PWM_Pin8);
        static PWMSpeedController victor_left = new PWMSpeedController(CTRE.HERO.IO.Port3.PWM_Pin9);
        static StringBuilder stringBuilder = new StringBuilder();
        static PneumaticControlModule my_PCM = new PneumaticControlModule(0);
        static CTRE.Phoenix.Controller.GameController m_gamepad = null;
        // IO module stuff
           CTRE.Gadgeteer.Module.DriverModule driver = new DriverModule(CTRE.HERO.IO.Port5);
        //   bool drivelow = DriverModule.OutputState.driveLow;
        //   bool pullup = DriverModule.OutputState.pullUp;
//        private bool running = false;

       // public bool Running { get => running; set => running = value; }

        public static void Main()
        {

            // IO module stuff
         //   CTRE.Gadgeteer.Module.DriverModule driver = new DriverModule(CTRE.HERO.IO.Port5);
         //   bool drivelow = DriverModule.OutputState.driveLow;
         //   bool pullup = DriverModule.OutputState.pullUp;
         //  bool running = false;

            /* loop forever */
            while (true)
            {
                /* drive robot using gamepad */
                Drive();
                /* control the pneumatic stuff */
                Pneumatic();

                //Control digital outputs
                Dout();

                /* print whatever is in our string builder */
                Debug.Print(stringBuilder.ToString());
                stringBuilder.Clear();

                /* run this task every 20ms */
                Thread.Sleep(20);
            }
        }
        /**
         * If value is within 10% of center, clear it.
         * @param value [out] floating point value to deadband.
         */
        static void Deadband(ref float value)
        {
            if (value < -0.10)
            {
                /* outside of deadband */
            }
            else if (value > +0.10)
            {
                /* outside of deadband */
            }
            else
            {
                /* within 10% so zero it */
                value = 0;
            }
        }
        static void Drive()
        {
            // tests for gamepad. assigns it if found
            if (null == m_gamepad)
                m_gamepad = new GameController(UsbHostDevice.GetInstance());
            /* If the game loses connection the watchdog is not fed and the hero board disables*/
            if (m_gamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
            { /* print the axis value */
                stringBuilder.Append("true ");
                /* allow motor control */
                CTRE.Phoenix.Watchdog.Feed();
              //  running.Equals = true;

            }
            /* The drive controller run at a maxium of 1/2 speed unless the "Turbo" button is pressed which 
             give full speed to the motor controllers*/
            bool turbo = m_gamepad.GetButton(6);

            float x = -1 * m_gamepad.GetAxis(1);
            float y = m_gamepad.GetAxis(5);
           if (turbo)
            {//full power
            }
            else
            {
              x = x/2;
             y =  y/2;
             }

            Deadband(ref x);
            Deadband(ref y);
  
            float leftThrot = x;
            float rightThrot = y;

            victor_left.Set(leftThrot);
            victor_right.Set(rightThrot);

            stringBuilder.Append(y + "  ");
            stringBuilder.Append(x);
            stringBuilder.Append("hi");


        }
        static void Pneumatic()
        {
            // get button for low speed Left bumper button This shifts the gear box
            bool LowGear = m_gamepad.GetButton(5);
            if(LowGear)
            {
                //Actuate low gear SOL
                my_PCM.SetSolenoidOutput(0, false);
            }
            else
            {
                my_PCM.SetSolenoidOutput(0, true);
            }
            // enable/disable compressor The actual logic to run the compressor is controlled by the PCM
            // and a pressure switch This just enables to compressor to run.
            //button A disables the compressor
            bool buttonA = m_gamepad.GetButton(2);
            if (buttonA)
            {
                my_PCM.StopCompressor();
                // Button Y enables the compressor
                stringBuilder.Append("stop");
                }
            bool buttonY = m_gamepad.GetButton(4);
            if (buttonY)
            {
                my_PCM.StartCompressor();
               stringBuilder.Append("runcompressor");
                   }
        }
        static void Dout()
        {

        }

    }
}