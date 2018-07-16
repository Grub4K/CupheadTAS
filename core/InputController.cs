using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAS {
    class InputRecorder {
        private List<Input> inputs = new List<Input>();
        private Actions Current;
        private int framesToNext;
		public InputRecorder() {
            Current = Actions.None;
            framesToNext = 0;
        }

        public bool WriteFile(string fileName) {
            try {
        		using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.ASCII)){
        				foreach (var input in inputs) {
        					writer.WriteLine(input);
        				}
                    }
        	    }
                return true;
            } catch {
                return false;
            }
        }

        public void Add(Actions toAdd) {
            if (toAdd != Current) {
                Input input = new Input(framesToNext, Current);
                inputs.Add(input);
                framesToNext = 1;
                Current = toAdd;
            } else {
                framesToNext++;
            }
        }
    }

    class InputPlayer {
		private List<Input> inputs = new List<Input>();
        public Input Current {
            get {
                Input Previous = Current;
                Current = inputs?[++inputIndex];
                return Previous;
            }
            set {
                Current = value;
            }
        }
        private int inputIndex, framesToNext;
		public InputPlayer() {
            inputIndex = 0;
        }

		public bool ReadFile(string fileName) {
			try {
				inputs.Clear();
				if (!File.Exists(fileName)) { return false; }

				int lines = 0;
				using (StreamReader sr = new StreamReader(fileName)) {
					while (!sr.EndOfStream) {
						string line = sr.ReadLine();

						if (line.IndexOf("Read", System.StringComparison.OrdinalIgnoreCase) == 0 && line.Length > 5) {
							ReadFile(line.Substring(5), ++lines);
							lines--;
						}

						Input input = new Input(++lines, line);
						if (input.Frames != 0) {
							inputs.Add(input);
						}
					}
				}
                Current = inputs?[0];
                framesToNext = Current.Equals(null)? Current.Frames : 0;
				return true;
			} catch {
				return false;
			}
		}

		private void ReadFile(string extraFile, int lines, string relativePath = "") {
			int index = extraFile.IndexOf(',');
			string fileName = index > 0 ? extraFile.Substring(0, index) : extraFile;
            int relativePathIndex = fileName.LastIndexOf('\\') + 1;
			int startLine = 0;
			int linesToRead = int.MaxValue;
			if (index > 0) {
				int indexLen = extraFile.IndexOf(',', index + 1);
				if (indexLen > 0) {
					int.TryParse(extraFile.Substring(index + 1, indexLen - index - 1), out startLine);
					int.TryParse(extraFile.Substring(indexLen + 1), out linesToRead);
				} else {
					int.TryParse(extraFile.Substring(index + 1), out startLine);
				}
			}

            if (relativePathIndex != 0){
                relativePath = relativePath + fileName.Substring(0, relativePathIndex);
                fileName = fileName.Substring(relativePathIndex);
            }

			if (!File.Exists(relativePath + fileName)) { return; }

			int subLine = 0;
			using (StreamReader sr = new StreamReader(relativePath + fileName)) {
				while (!sr.EndOfStream) {
					string line = sr.ReadLine();

                    subLine++;
					if (subLine < startLine) { continue; }
					if (subLine > linesToRead) { break; }

					if (line.IndexOf("Read", System.StringComparison.OrdinalIgnoreCase) == 0 && line.Length > 5) {
						ReadFile(line.Substring(5), lines, relativePath);
					}

					Input input = new Input(lines, line);
					if (input.Frames != 0) {
						inputs.Add(input);
					}
				}
			}
		}
    }
}
