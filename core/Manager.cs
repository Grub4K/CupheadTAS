using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using J2i.Net.XInputWrapper;

namespace TAS {
    class Manager {
        private InputRecorder recorder = new InputRecorder();
        private InputPlayer player = new InputPlayer();
        private State state = State.None;
    	public XboxController xbox;
        public Manager() {
            xbox = XboxController.RetrieveController(0);
            XboxController.UpdateFrequency = 30;
            XboxController.StartPolling();
            new Task(UpdateStateAsync).Start();
        }

        private void UpdateStateAsync() {
            for (;;) {
                if (xbox.IsRightStickPressed && state != State.Recording) {
                    state = (state == State.Replaying)? State.None : State.Replaying;
                    while (xbox.IsRightStickPressed) {}
                }
                if (xbox.IsLeftStickPressed && state != State.Replaying) {
                    state = (state == State.Recording)? State.None : State.Recording;
                    while (xbox.IsLeftStickPressed) {}
                }
            }
        }

        private void UpdateModeAsync() {
            for (;;) {
                if (xbox.IsRightStickPressed && state != State.Recording) {
                    state = (state == State.Replaying)? State.None : State.Replaying;
                    while (xbox.IsRightStickPressed) {}
                }
                if (xbox.IsLeftStickPressed && state != State.Replaying) {
                    state = (state == State.Recording)? State.None : State.Recording;
                    while (xbox.IsLeftStickPressed) {}
                }
            }
        }

        public void Testing() {
            for (;;) {
                if (state == State.Replaying) {
                    player.Load("Cuphead.tas");
                    player.Reset();
                    Console.WriteLine("Replaying");
                    while (player.MoveNext() && state == State.Replaying) {
                        Console.WriteLine(player.Current);
                        System.Threading.Thread.Sleep(100);
                    }
                    state = State.None;
                    Console.WriteLine("Ended Replay");
                } else if (state == State.Recording) {
                    recorder.Reset();
                    Console.WriteLine("Recording");
                    while (state == State.Recording) {
                        while (!xbox.IsLeftShoulderPressed){
                            if (state != State.Recording) {
                                goto end_recording;
                            }
                        }

                        recorder.Add(GetActions());

                        while (xbox.IsLeftShoulderPressed && state == State.Recording){}
                    }
                    end_recording:
                    recorder.WriteFile("new_Cuphead.tas");
                    Console.WriteLine("Ended Recording");
                }
            }
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
            //    | (xbox.IsLeftStickPressed? Actions.Analog : 0);
            return actions;
        }

        private enum State {
        	None,
        	Replaying,
        	Recording
        }

        private enum Mode {
            None,
        	FrameStep,
            Slowdown
        }
    }

    class Program {
        public static void Main(string[] args) {
            Manager manager = new Manager();
            void Separate() { System.Console.WriteLine("***********************"); }

            Separate();
            manager.Testing();
            Separate();

            return;
        }
    }
}
