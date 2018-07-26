using System;
using System.Text;
using System.Linq;

namespace TAS {
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
                Actions |= Collection.Convert(stringChar);
            }
        }

        public string ActionsToString() {
            // Builds string from Actions flag
            StringBuilder sb = new StringBuilder();
            foreach (var entry in Collection.ActionsToString) {
                if (Actions.HasFlag(entry.Key)) {
                    sb.Append(",");
                    sb.Append(entry.Value);
                }
            }
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
