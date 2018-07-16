using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using J2i.Net.XInputWrapper;

namespace Cuphead {
    void SendInputs(string Inputs) {
        Console.WriteLine(Inputs);
    }
}

namespace TAS {
    [Flags]
	public enum State {
		None,
		Replay    = 1,
		Record    = 2,
		FrameStep = 4
	}
    class Manager {
        private InputController controller = new InputController();
    	XboxController xbox = XboxController.RetrieveController(0);
        XboxController.UpdateFrequency = 30;
        XboxController.StartPolling();



        private Actions GetActions(){
            Actions actions = Actions.None
                | xbox.IsAPressed?         Actions.Jump   : 0
                | xbox.IsBPressed?         Actions.Ex     : 0
                | xbox.IsXPressed?         Actions.Attack : 0
                | xbox.IsYPressed?         Actions.Change : 0
                | xbox.IsDPadDownPressed?  Actions.Down   : 0
                | xbox.IsDPadLeftPressed?  Actions.Left   : 0
                | xbox.IsDPadRightPressed? Actions.Right  : 0
                | xbox.IsDPadUpPressed?    Actions.Up     : 0
                | xbox.IsStartPressed?     Actions.Start  : 0;
            //    | xbox.IsLeftStickPressed? Actions.Analog : 0;
            return actions;
        }
    }
    class Program {
        public static void Main(string[] args) {
            Actions actions;

            Console.WriteLine("Input started");
            for (;;){
                while (!xbox.IsLeftShoulderPressed){}

                Input input = new Input(0, 1, actions);
                if (input.HasActions(Actions.Analog)){
                    Point point = xbox.LeftThumbStick;
                    float angle = (Math.Atan2((double)point.X, (double)point.Y)+Math.PI)*180D/Math.PI;
                    input.Angle = angle;
                }
                Console.WriteLine(input);

                while (xbox.IsLeftShoulderPressed){}
            }
        }
    }
}
