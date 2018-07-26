using System;
using System.Collections.Generic;

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
    internal static class Collection {
        public static readonly Dictionary<Actions, string> ActionsToString = new Dictionary<Actions, string> {
            {Actions.Left,   "<"},
            {Actions.Right,  ">"},
            {Actions.Up,     "^"},
            {Actions.Down,   "v"},
            {Actions.Jump,   "J"},
            {Actions.Dash,   "D"},
            {Actions.Change, "C"},
            {Actions.Start,  "S"},
            {Actions.Attack, "A"},
            {Actions.Ex,     "E"},
            {Actions.Lock,   "L"}
        };
        public static readonly Dictionary<string, Actions> StringToActions = new Dictionary<string, Actions> {
            {"<", Actions.Left  },
            {">", Actions.Right },
            {"^", Actions.Up    },
            {"v", Actions.Down  },
            {"J", Actions.Jump  },
            {"D", Actions.Dash  },
            {"C", Actions.Change},
            {"S", Actions.Start },
            {"A", Actions.Attack},
            {"E", Actions.Ex    },
            {"L", Actions.Lock  }
        };
        public static string Convert(Actions act) {
            ActionsToString.TryGetValue(act, out string temp);
            return temp;
        }
        public static Actions Convert(string str) {
            StringToActions.TryGetValue(str, out Actions temp);
            return temp;
        }
    }
}
