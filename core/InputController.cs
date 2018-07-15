using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAS
{
    class InputController{
		private List<Input> inputs = new List<Input>();
	    private string filePath;
		public InputController(string filePath = "Cuphead.tas") {
			this.filePath = filePath;
		}

        public bool Read(){
            bool temp = this.ReadFile();
            return temp;
        }

		private bool ReadFile() {
			try {
				inputs.Clear();
				if (!File.Exists(filePath)) { return false; }

				int lines = 0;
				using (StreamReader sr = new StreamReader(filePath)) {
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
				return true;
			} catch {
				return false;
			}
		}
		private void ReadFile(string extraFile, int lines, string relativePath = "") {
			int index = extraFile.IndexOf(',');
			string filePath = index > 0 ? extraFile.Substring(0, index) : extraFile;
            int relativePathIndex = filePath.LastIndexOf('\\') + 1;
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
                relativePath = relativePath + filePath.Substring(0, relativePathIndex);
                filePath = filePath.Substring(relativePathIndex);
            }

			if (!File.Exists(relativePath + filePath)) { return; }

			int subLine = 0;
			using (StreamReader sr = new StreamReader(relativePath + filePath)) {
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

		public override string ToString() {
			return string.Join("\n", inputs);
		}
    }
    class Program
    {
        public static void Main(string[] args)
        {
            InputController manager = new InputController(filePath: "Cuphead.tas");
            manager.Read();
            Console.WriteLine(manager);
        }
    }
}
