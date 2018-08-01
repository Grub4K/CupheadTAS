using System;
using System.Text;
using System.Linq;
using XInput.Wrapper;

namespace TAS {
    internal class Input {
        public int Frames { get; set; }
        public X.Gamepad.GamepadButtons Actions { get; set; }
        public bool Slowdown { get; set; }
        // TODO add field for SegInfo
        public Input() {  }

        private string ActionsToString() {
            StringBuilder sb = new StringBuilder();
            // Build string from Actions flags
            foreach (var entry in Collection.StringToActions) {
                if (Actions.HasFlag(entry.Value)) {
                    sb.Append($",{entry.Key}");
                }
            }
            return sb.ToString();
        }

        public override string ToString() {
            return Slowdown?
                $"*** {Frames.ToString().PadLeft(2, ' ')}" :
                $"{Frames.ToString().PadLeft(4, ' ')}{ActionsToString()}";
        }

        public override bool Equals(object obj) {
            return obj is Input && (obj as Input) == this;
        }

        public override int GetHashCode() {
            return (int)Actions;
        }

        public static bool operator ==(Input one, Input two) {
            return one?.Actions == two?.Actions;
        }

        public static bool operator !=(Input one, Input two) {
            return one?.Actions != two?.Actions;
        }
    }
}
