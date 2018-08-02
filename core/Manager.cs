using System;
using System.IO;
using XInput.Wrapper;
using System.Collections.Generic;

namespace TAS {
    static class Manager {
        private enum Modes {
            AdvanceSlow,
            AdvanceFrame,
            Advance
        }
        private static X.Gamepad gamepad;
        private static X.Gamepad.GamepadButtons input;
        private static bool LTriggerHeld, RTriggerHeld, isStopped;
        private static Modes Mode;
        //static GamepadState state;
        static Manager() {
            if (!X.IsAvailable) {
                throw new FileNotFoundException("XInput1_4.dll cannot be loaded.");
            }
            gamepad = X.Gamepad_1;
            X.Gamepad.Capability caps = gamepad.Capabilities;

            gamepad.ConnectionChanged += (o, i) => {
                // Give short haptical feedback to signal initialization
                gamepad.FFB_RightMotor(1f, 250);
            };

            gamepad.StateChanged += (o, i) => {
                if (!LTriggerHeld && (gamepad.LTrigger_N != 0)) {
                    LTriggerHeld = true;
                    if (Mode != Modes.Advance) {
                        Mode = Modes.Advance;
                        isStopped = false;
                    } else {
                        Mode = Modes.AdvanceFrame;
                        isStopped = true;
                    }
                } else if (gamepad.LTrigger_N == 0){
                    LTriggerHeld = false;
                }
                if (!RTriggerHeld && (gamepad.RTrigger_N != 0)) {
                    RTriggerHeld = true;
                    Mode = Modes.AdvanceFrame;
                    isStopped = false;
                } else if (gamepad.RTrigger_N == 0){
                    RTriggerHeld = false;
                }
                if (gamepad.LStick_up) {
                    event_Switch(Controller.Modes.Recording);
                    isStopped = true;
                }
                if (gamepad.RStick_up) {
                    event_Switch(Controller.Modes.Replaying);
                    isStopped = false;
                }
                ProcessInputs();
            };

            Mode = Modes.Advance;
            X.StartPolling(gamepad);
            // End of init
        }

        private static void event_Switch(Controller.Modes mode) {
            Controller.Mode = (Controller.Mode == mode)? Controller.Modes.None : mode;
        }

        private static void ProcessInputs() {
            input = (X.Gamepad.GamepadButtons)0
                | (gamepad.A_down          ? X.Gamepad.GamepadButtons.A          : 0)
                | (gamepad.B_down          ? X.Gamepad.GamepadButtons.B          : 0)
                | (gamepad.X_down          ? X.Gamepad.GamepadButtons.X          : 0)
                | (gamepad.Y_down          ? X.Gamepad.GamepadButtons.Y          : 0)
                | (gamepad.Dpad_Down_down  ? X.Gamepad.GamepadButtons.Dpad_Down  : 0)
                | (gamepad.Dpad_Up_down    ? X.Gamepad.GamepadButtons.Dpad_Up    : 0)
                | (gamepad.Dpad_Left_down  ? X.Gamepad.GamepadButtons.Dpad_Left  : 0)
                | (gamepad.Dpad_Right_down ? X.Gamepad.GamepadButtons.Dpad_Right : 0)
                | (gamepad.Start_down      ? X.Gamepad.GamepadButtons.Start      : 0)
                | (gamepad.LBumper_down    ? X.Gamepad.GamepadButtons.LBumper    : 0)
                | (gamepad.RBumper_down    ? X.Gamepad.GamepadButtons.RBumper    : 0);
        }

        public static X.Gamepad.GamepadButtons GetInput(){
            while (isStopped) {
                System.Threading.Thread.Sleep(20);
            }
            if (Mode == Modes.AdvanceFrame) {
                isStopped = true;
            }
            if (Mode == Modes.AdvanceSlow) {
                System.Threading.Thread.Sleep(100);
            }
            // Advance
            switch (Controller.Mode) {
                case Controller.Modes.Recording:
                    Controller.Add(input);
                    break;
                case Controller.Modes.Replaying:
                    if (Controller.MoveNext()) {
                        input = Controller.Current;
                    } else {
                        Controller.Mode = Controller.Modes.None;
                        ProcessInputs();
                    }
                    break;
            }
            return input;
        }

        public static void Start() {
            X.StartPolling(gamepad);
        }

        public static void Stop() {
            X.StopPolling();
        }
    }
}
