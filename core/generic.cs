using System;
using XInput.Wrapper;
using System.Globalization;
using System.Collections.Generic;

namespace TAS {
    internal static class Collection {
        public static readonly Dictionary<string, X.Gamepad.GamepadButtons> StringToActions = new Dictionary<string, X.Gamepad.GamepadButtons> {
            {"<", X.Gamepad.GamepadButtons.Dpad_Left },
            {">", X.Gamepad.GamepadButtons.Dpad_Right},
            {"^", X.Gamepad.GamepadButtons.Dpad_Up   },
            {"v", X.Gamepad.GamepadButtons.Dpad_Down },
            {"J", X.Gamepad.GamepadButtons.A         },
            {"D", X.Gamepad.GamepadButtons.Y         },
            {"C", X.Gamepad.GamepadButtons.LBumper   },
            {"S", X.Gamepad.GamepadButtons.Start     },
            {"A", X.Gamepad.GamepadButtons.X         },
            {"E", X.Gamepad.GamepadButtons.B         },
            {"L", X.Gamepad.GamepadButtons.RBumper   }
        };
        public static bool Convert(string str, out X.Gamepad.GamepadButtons action) {
            return StringToActions.TryGetValue(
                       str.ToUpper(new CultureInfo("en-US", false)),
                       out action
            );
        }
    }
}
