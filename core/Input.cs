using System;
using System.Text;
namespace TAS {
	[Flags]
	public enum Actions {
		None,
		Left   = 1,
		Right  = 2,
		Up     = 4,
		Down   = 8,
		Jump   = 16,
		Dash   = 32,
		Change = 64,
		Start  = 128,
		Attack = 256,
		Ex     = 512,
		Lock   = 1024,
		Analog = 2048
	}
	public class Input {
		public int Frames { get; set; }
		public Actions Actions { get; set; }
		public float Angle { get; set; }
		public bool Slowdown { get; set; }
		public Input() { }
        public Input(int frames, Actions actions) {
            Frames = frames;
            Actions = actions;
        }
		public Input(string line) {
			int index = 0;
			Frames = ReadFrames(line, ref index);
			if (Frames == 0) {
				if (line.StartsWith("***")) {
					Slowdown = true;
					index = 3;
					Frames = ReadFrames(line, ref index);
				}
				return;
			}

			while (index < line.Length) {
				char c = line[index];

				switch (char.ToUpper(c)) {
					case '<': Actions ^= Actions.Left;   break;
					case '>': Actions ^= Actions.Right;  break;
					case '^': Actions ^= Actions.Up;     break;
					case 'V': Actions ^= Actions.Down;   break;
					case 'J': Actions ^= Actions.Jump;   break;
					case 'D': Actions ^= Actions.Dash;   break;
					case 'C': Actions ^= Actions.Change; break;
					case 'S': Actions ^= Actions.Start;  break;
					case 'A': Actions ^= Actions.Attack; break;
					case 'E': Actions ^= Actions.Ex;     break;
					case 'L': Actions ^= Actions.Lock;   break;
					case 'X':
						Actions ^= Actions.Analog;
						index++;
						Angle = ReadAngle(line, ref index);
						continue;
				}

				index++;
			}

			if (HasActions(Actions.Analog)) {
				Actions &= ~Actions.Right & ~Actions.Left & ~Actions.Up & ~Actions.Down;
			} else {
				Angle = 0;
			}
		}
		private int ReadFrames(string line, ref int start) {
			bool foundFrames = false;
			int frames = 0;

			while (start < line.Length) {
				char c = line[start];

				if (!foundFrames) {
					if (char.IsDigit(c)) {
						foundFrames = true;
						frames = c ^ 0x30;
					} else if (c != ' ') {
						return frames;
					}
				} else if (char.IsDigit(c)) {
					if (frames < 9999) {
						frames = frames * 10 + (c ^ 0x30);
					} else {
						frames = 9999;
					}
				} else if (c != ' ') {
					return frames;
				}

				start++;
			}

			return frames;
		}
		private float ReadAngle(string line, ref int start) {
			bool foundAngle = false;
			bool foundDecimal = false;
			int decimalPlaces = 1;
			int angle = 0;
			bool negative = false;

			while (start < line.Length) {
				char c = line[start];

				if (!foundAngle) {
					if (char.IsDigit(c)) {
						foundAngle = true;
						angle = c ^ 0x30;
					} else if (c == ',') {
						foundAngle = true;
					} else if (c == '.') {
						foundAngle = true;
						foundDecimal = true;
					} else if (c == '-') {
						negative = true;
					}
				} else if (char.IsDigit(c)) {
					angle = angle * 10 + (c ^ 0x30);
					if (foundDecimal) {
						decimalPlaces *= 10;
					}
				} else if (c == '.') {
					foundDecimal = true;
				} else if (c != ' ') {
					return (negative ? (float)-angle : (float)angle) / (float)decimalPlaces;
				}

				start++;
			}

			return (negative ? (float)-angle : (float)angle) / (float)decimalPlaces;
		}
		public float GetX() {
			if (HasActions(Actions.Right)) {
				return 1f;
			} else if (HasActions(Actions.Left)) {
				return -1f;
			} else if (!HasActions(Actions.Analog)) {
				return 0f;
			}
			return (float)Math.Sin(Angle * Math.PI / 180.0);
		}
		public float GetY() {
			if (HasActions(Actions.Up)) {
				return 1f;
			} else if (HasActions(Actions.Down)) {
				return -1f;
			} else if (!HasActions(Actions.Analog)) {
				return 0f;
			}
			return (float)Math.Cos(Angle * Math.PI / 180.0);
		}
		public bool HasActions(Actions actions) {
			return (Actions & actions) != 0;
		}
		public override string ToString() {
			return Slowdown? "***" + Frames.ToString() : Frames == 0 ? "" : Frames.ToString().PadLeft(4, ' ') + ActionsToString();
		}
		public string ActionsToString() {
			StringBuilder sb = new StringBuilder();
			if (HasActions(Actions.Left))   { sb.Append(",<"); }
			if (HasActions(Actions.Right))  { sb.Append(",>"); }
			if (HasActions(Actions.Up))     { sb.Append(",^"); }
			if (HasActions(Actions.Down))   { sb.Append(",v"); }
			if (HasActions(Actions.Jump))   { sb.Append(",J"); }
			if (HasActions(Actions.Dash))   { sb.Append(",D"); }
			if (HasActions(Actions.Change)) { sb.Append(",C"); }
			if (HasActions(Actions.Start))  { sb.Append(",S"); }
			if (HasActions(Actions.Attack)) { sb.Append(",A"); }
			if (HasActions(Actions.Ex))     { sb.Append(",E"); }
			if (HasActions(Actions.Lock))   { sb.Append(",L"); }
			if (HasActions(Actions.Analog)) { sb.Append(",X,").Append(Angle.ToString("0")); }
			return sb.ToString();
		}
		public override bool Equals(object obj) {
			return obj is Input && ((Input)obj) == this;
		}
		public override int GetHashCode() {
			return Frames ^ (int)Actions;
		}
		public static bool operator ==(Input one, Input two) {
			bool oneNull = (object)one == null;
			bool twoNull = (object)two == null;
			if (oneNull != twoNull) {
				return false;
			} else if (oneNull && twoNull) {
				return true;
			}
			return one.Actions == two.Actions && one.Angle == two.Angle;
		}
		public static bool operator !=(Input one, Input two) {
			bool oneNull = (object)one == null;
			bool twoNull = (object)two == null;
			if (oneNull != twoNull) {
				return true;
			} else if (oneNull && twoNull) {
				return false;
			}
			return one.Actions != two.Actions || one.Angle != two.Angle;
		}
	}
}
