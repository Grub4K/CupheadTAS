using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using J2i.Net.XInputWrapper;

namespace TAS {
    class Manager {
        private InputRecorder recorder = new InputRecorder();
        private InputPlayer player = new InputPlayer();
    	public XboxController xbox;
        public Manager() {
            xbox = XboxController.RetrieveController(0);
            XboxController.UpdateFrequency = 30;
            XboxController.StartPolling();
        }

        public void doneMain() {
            player.Load("Cuphead.tas");
            while (player.MoveNext()) {
                if (player.Current.Slowdown) {
                    Console.WriteLine("Slowdown spotted, {0}", player.Current.Frames);
                    continue;
                }
                Console.WriteLine(player.Current.Actions);
            }

            XboxController.StopPolling();
        }

        public void doMain() {
            player.Load("Cuphead.tas");

            Console.WriteLine("START");
            recorder.Start();
            for (;;){
                while (!xbox.IsLeftShoulderPressed){}

                if (xbox.IsLeftStickPressed){break;}

                recorder.Add(GetActions());
                Console.WriteLine(new Input(1, GetActions()));

                while (xbox.IsLeftShoulderPressed){}
            }
            XboxController.StopPolling();
            Console.WriteLine("Saving to FILE");

            recorder.WriteFile("NEW_Cuphead.tas");

            Console.WriteLine("END");
        }

        private Actions GetActions(){
            Actions actions = Actions.None
                | (xbox.IsAPressed?         Actions.Jump   : 0)
                | (xbox.IsBPressed?         Actions.Ex     : 0)
                | (xbox.IsXPressed?         Actions.Attack : 0)
                | (xbox.IsYPressed?         Actions.Change : 0)
                | (xbox.IsDPadDownPressed?  Actions.Down   : 0)
                | (xbox.IsDPadLeftPressed?  Actions.Left   : 0)
                | (xbox.IsDPadRightPressed? Actions.Right  : 0)
                | (xbox.IsDPadUpPressed?    Actions.Up     : 0)
                | (xbox.IsStartPressed?     Actions.Start  : 0);
            //    | xbox.IsLeftStickPressed? Actions.Analog : 0;
            return actions;
        }

        [Flags]
        private enum State {
        	None,
        	Replay    = 1,
        	Record    = 2,
        	FrameStep = 4,
            Slowdown  = 8
        }
    }
    class Program {
        public static void Main(string[] args) {
            Manager manager = new Manager();
            void Separate() { System.Console.WriteLine("***********************"); }

            Separate();
            manager.doMain();
            Separate();

            return;
        }
    }
}
