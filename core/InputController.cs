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
            Start();
        }

        public void Start() {
            inputs.Clear();
            Current = Actions.None;
            framesToNext = 0;
        }

        public bool WriteFile(string fileName) {
            if (framesToNext != 0) {
                inputs.Add(new Input(framesToNext, Current));
            }
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
                if (framesToNext != 0) {
                    inputs.Add(new Input(framesToNext, Current));
                }
                framesToNext = 1;
                Current = toAdd;
            } else {
                framesToNext++;
            }
        }
    }

    class InputPlayer {
		private List<Input> inputs = new List<Input>();
        private int inputIndex, framesToNext;
		public InputPlayer() {
            Reset();
        }

        public void Reset() {
            framesToNext = 1;
            inputIndex = -1;
        }

        public Input Current {
            get {
                return inputs[inputIndex];
            }
        }

        public bool MoveNext() {
            if (--framesToNext == 0 || inputs[inputIndex].Slowdown) {
                if (++inputIndex == inputs.Count) {
                    return false;
                }
                framesToNext = inputs[inputIndex].Frames;
            }
            return true;
        }

        public void Load(string fileName) {
            int trycount = 5;
            while (!ReadFile(fileName) && --trycount >= 0) {
                System.Threading.Thread.Sleep(50);
            }

            Reset();
        }

		private bool ReadFile(string fileName) {
			try {
				inputs.Clear();
				if (!File.Exists(fileName)) { return false; }

				using (StreamReader sr = new StreamReader(fileName)) {
					while (!sr.EndOfStream) {
						string line = sr.ReadLine();

						if (line.StartsWith("Read") && line.Length > 5) {
							ReadFile(line.Substring(5), relativePath : "");
						}

						Input input = new Input(line);
						if (input.Frames != 0) {
							inputs.Add(input);
						}
					}
				}
				return true;
			} catch {
				return false;
			}
		}

		private void ReadFile(string extraFile, string relativePath) {
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

					if (line.StartsWith("Read") && line.Length > 5) {
						ReadFile(line.Substring(5), relativePath);
					}

					Input input = new Input(line);
					if (input.Frames != 0) {
						inputs.Add(input);
					}
				}
			}
		}
    }
}
