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
            Reset();
        }

        public void Reset() {
            inputs.Clear();
            Current = Actions.None;
            framesToNext = 0;
        }

        public bool WriteFile(string fileName) {
            if (framesToNext != 0) {
                inputs.Add(new Input{framesToNext, Current});
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
            for (int i = 0; i < 5; ++i) {
    			try {
        			inputs.Clear();
                    ReadFile(fileName);
                    Reset();
                    return true;
                } catch {
                    System.Threading.Thread.Sleep(50);
                }
            }
            return false;
        }

		private bool ReadFile(string fileName, string relativePath = "", int startLine = 1, int lastLine = int.MaxValue) {
            if (!File.Exists(relativePath + extraFile)) {
                throw new FileNoptFoundException($"{relativePath}{extraFile} not found.");
            }

            int subLine = 1;
			using (StreamReader sr = new StreamReader(fileName)) {
				while (!sr.EndOfStream) {
					string line = sr.ReadLine();
					if (subLine < startLine) { continue; }
					if (subLine > lastLine) { break; }

                    if (line.StartsWith("#")) { continue; }
					if (line.StartsWith("Read")) {
                        // Tokenize line
                        string[] options = line.Split(new char[]{','}, count : 4);
                        // Parse line and setup values
                        string extraFile = options.ElementAtOrDefault(1);
                        // Get relative path from full filename
                        int indexPath = extraFile.LastIndexOf("\\");
                        if (indexPath != -1){
                            relativePath += extraFile.Remove(indexPath);
                            extraFile = extraFile.Substring(0, ++indexPath);
                        }
                        // Try parsing as int, fallback to 0 if fail
                        int.TryParse(options.ElementAtOrDefault(2), out startLine);
                        // Try parsing as int, fallback to MaxValue
                        if (!int.TryParse(options.ElementAtOrDefault(3), out lastLine)) {
                            lastLine = int.MaxValue;
                        }
                        // Recursive call of file reading
						ReadFile(extraFile, relativePath, startLine, lastLine);
					} else {
                        Input input = new Input(line);
                        if (input.Frames != 0) {
                        	inputs.Add(input);
                        }
                    }
                    subLine++;
				}
			}
		}
    }
}
