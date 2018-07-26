using System;
using System.Linq;

namespace TAS {
    [Flags]
    public enum Actions {
        None,
        Left   = 0x001,
        Right  = 0x002,
        Up     = 0x004,
        Down   = 0x008,
        Jump   = 0x010,
        Dash   = 0x020,
        Change = 0x040,
        Start  = 0x080,
        Attack = 0x100,
        Ex     = 0x200,
        Lock   = 0x400
    }
    public class Input {
        public int Frames { get; set; }
        public Actions Actions { get; set; }
        public bool Slowdown { get; private set; }
        public Input() {  }
        public Input(string line) {
            // Tokenize line
            string[] stringTokens = line.Split(',');
            if (line.StartsWith("***")) {
                Slowdown = true;
                stringTokens[0] = stringTokens[0].Remove(0, 3);
            }
            // Shitty solution for attribute setting...
            // Set frame count for input
            int.TryParse(stringTokens.FirstOrDefault(), out int temp);
            Frames = temp;
            // Process each entered character afterwards
            foreach (var stringChar in stringTokens.Skip(1)) {
                switch (stringChar) {
                    // TODO Use lookup for this
                    case "<": Actions |= Actions.Left;   break;
                    case ">": Actions |= Actions.Right;  break;
                    case "^": Actions |= Actions.Up;     break;
                    case "V": Actions |= Actions.Down;   break;
                    case "J": Actions |= Actions.Jump;   break;
                    case "D": Actions |= Actions.Dash;   break;
                    case "C": Actions |= Actions.Change; break;
                    case "S": Actions |= Actions.Start;  break;
                    case "A": Actions |= Actions.Attack; break;
                    case "E": Actions |= Actions.Ex;     break;
                    case "L": Actions |= Actions.Lock;   break;
                }
            }
        }

        public string ActionsToString() {
            // Builds string from Actions flag
            StringBuilder sb = new StringBuilder();
            // TODO Use lookup for this
            if (Actions.HasFlag(Actions.Left))   { sb.Append(",<"); }
            if (Actions.HasFlag(Actions.Right))  { sb.Append(",>"); }
            if (Actions.HasFlag(Actions.Up))     { sb.Append(",^"); }
            if (Actions.HasFlag(Actions.Down))   { sb.Append(",v"); }
            if (Actions.HasFlag(Actions.Jump))   { sb.Append(",J"); }
            if (Actions.HasFlag(Actions.Dash))   { sb.Append(",D"); }
            if (Actions.HasFlag(Actions.Change)) { sb.Append(",C"); }
            if (Actions.HasFlag(Actions.Start))  { sb.Append(",S"); }
            if (Actions.HasFlag(Actions.Attack)) { sb.Append(",A"); }
            if (Actions.HasFlag(Actions.Ex))     { sb.Append(",E"); }
            if (Actions.HasFlag(Actions.Lock))   { sb.Append(",L"); }
            return sb.ToString();
        }

        public override string ToString() {
            return Slowdown? "***" + Frames.ToString().PadLeft(2, ' ') : Frames.ToString().PadLeft(4, ' ') + ActionsToString();
        }

        public override bool Equals(object obj) {
            return obj is Input && ((Input)obj) == this;
        }

        public override int GetHashCode() {
            return Frames ^ (int)Actions;
        }

        public static bool operator ==(Input one, Input two) {
            return one?.Actions == two?.Actions;
        }

        public static bool operator !=(Input one, Input two) {
            return one?.Actions != two?.Actions;
        }
    }
}
