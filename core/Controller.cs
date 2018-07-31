using System;
using System.IO;
using System.Linq;
using System.Text;
using XInput.Wrapper;
using System.Collections.Generic;

namespace TAS {
    internal static class Controller {
        public enum Modes {
            None,
            Replaying,
            Recording
        }
        private static List<Input> inputs = new List<Input>();
        private static int framesToNext, inputIndex;
        private static Modes _mode;
        public static X.Gamepad.GamepadButtons Current { get; private set; }
        public static Modes Mode {
            get => _mode;
            set {
                switch (_mode) {
                    case Modes.Recording:
                        Write("Cuphead.rec.tas");
                        break;
                }
                _mode = value;
                switch (value) {
                    case Modes.None:
                        Reset();
                        break;
                    case Modes.Replaying:
                        Read("Cuphead.tas");
                        break;
                }
            }
        }
		static Controller() {
            Reset();
            _mode = Modes.None;
        }

        public static string Stringify() {
            return string.Join("\n", inputs);
        }

        public static void Reset() {
            inputs.Clear();
            Current = (X.Gamepad.GamepadButtons)0;
            framesToNext = 0;
            inputIndex = -1;
        }

         // **************
        // Recorder Part

        public static bool Write(string fileName) {
            if (_mode == Modes.Recording) {
                if (framesToNext != 0) {
                    inputs.Add(new Input {
                        Actions = Current,
                        Frames = framesToNext
                    });
                }
                for (int i = 0; i < 5; ++i) {
                    try {
                        WriteFile(fileName);
                        return true;
                    } catch {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
            return false;
        }

        public static void Add(X.Gamepad.GamepadButtons toAdd) {
            if (_mode == Modes.Recording) {
                if (toAdd != Current) {
                    if (framesToNext != 0) {
                        inputs.Add(new Input {
                            Actions = Current,
                            Frames = framesToNext
                        });
                    }
                    framesToNext = 1;
                    Current = toAdd;
                } else {
                    framesToNext++;
                }
            }
        }

        private static void WriteFile(string fileName) {
            // Create writer
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.ASCII)){
                // Write every Input to File
        		foreach (var input in inputs) {
        			writer.WriteLine(input);
        		}
            }
        }

         // **************
        // Player Part

        public static bool MoveNext() {
            if (_mode == Modes.Replaying) {
                if (--framesToNext <= 0) {
                    if (++inputIndex == inputs.Count) {
                        return false;
                    }
                    Current = inputs[inputIndex].Actions;
                    framesToNext = inputs[inputIndex].Frames;
                }
                return true;
            }
            return false;
        }

        public static bool Read(string fileName) {
            if (_mode == Modes.Replaying) {
                for (int i = 0; i < 5; ++i) {
            		try {
                        Reset();
                        ReadFile(fileName);
                        return true;
                    } catch {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
            return false;
        }

        private static void ReadFile(string fileName, string relativePath = "", int startLine = 1, int lastLine = int.MaxValue) {
            if (!File.Exists(relativePath + fileName)) {
                throw new FileNotFoundException($"{relativePath}{fileName} not found.");
            }

            int subLine = 1;
        	using (StreamReader sr = new StreamReader($"{relativePath}{fileName}")) {
        		while (!sr.EndOfStream) {
        			string line = sr.ReadLine();
        			if (subLine < startLine) { continue; }
        			if (subLine > lastLine) { break; }

                    // Comment or segment info, ignore
                    // TODO add SegInfo to file structuring and make it useful
                    // maybe something for TASstudio (segment tree?)
                    if (line.StartsWith("#")) { continue; }
                    // Load next file
        			if (line.StartsWith("Read")) {
                        // Tokenize line
                        string[] options = line.Split(new char[]{','}, count : 4);
                        // Parse line and setup values
                        string extraFile = options.ElementAtOrDefault(1);
                        // Get relative path from full filename
                        int indexPath = fileName.LastIndexOf("\\");
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
