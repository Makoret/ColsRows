using System;
using System.Threading;

namespace COLSnROWS {
    
    class Program {

        static void Main(string[] args) {
            Console.Title = "Cols&Rows";
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetWindowSize(33, 6);//Always first
            Console.SetBufferSize(33, 6);//Always second
            //Console.WriteLine("Ancho del buffer: {0}", Console.BufferWidth);
            //Console.WriteLine("Alto del buffer: {0}", Console.BufferHeight);
            //Console.WriteLine("Ancho de la ventana: {0}", Console.WindowWidth);
            //Console.WriteLine("Alto de la ventana: {0}", Console.WindowHeight);

            string[,] board = new string[5, 5];
            int X_score = 0, O_score = 0;
            
            Game(board, X_score, O_score);
            
        }

        public static void CheckScoreBoard() {
            for (int i = 0; i < 3; i++) {
                Console.WriteLine("Working thread...");
                Thread.Sleep(100);
            }
        }

        static void Game(string [,] Array, int X_score, int O_score) {
            Start:
            BoardInit(Array);
            ShowBoard(Array, X_score, O_score);
            bool side = true;
            int turn = 0;
            String veredict = "continue";
            String Consult = ""; 
            do {
                string coord = Move();
                string[] pos = coord.Split(',');
                int Ypos = Convert.ToInt32(pos[0]);
                int Xpos = Convert.ToInt32(pos[1]);
                Consult = Array[Xpos, Ypos];
                if (Consult.Equals(" ")) {
                    if (side) {
                        Array[Xpos, Ypos] = "X";
                        side = false;
                    } else {
                        Array[Xpos, Ypos] = "O";
                        side = true;
                    }
                    ShowBoard(Array, X_score, O_score);
                    turn++;
                    if (turn >= 5) {
                        veredict = Check(Array, turn);
                    }
                }
            } while (veredict.Equals("continue"));
            if (veredict.Equals("DRAW!")) {
                AskAgain:
                Console.SetCursorPosition(0, 5);
                Console.Write("Continue?(y/n)");
                ConsoleKeyInfo opt = Console.ReadKey(true);
                if (opt.Key == ConsoleKey.Y) {
                    goto Start;
                }
                if (opt.Key == ConsoleKey.N) {
                    Console.SetCursorPosition(0, 0);
                    Environment.Exit(0);
                } else {
                    goto AskAgain;
                }
            } else {
                veredict = veredict.Remove(1);
                if (veredict.Equals("X")) {
                    X_score++;
                } else {
                    O_score++;
                }
                AskAgain:
                Console.SetCursorPosition(0, 5);
                Console.Write("Continue?(y/n)");
                ConsoleKeyInfo opt = Console.ReadKey(true);
                if (opt.Key == ConsoleKey.Y) {
                    goto Start;
                }
                if (opt.Key == ConsoleKey.N) {
                    Console.SetCursorPosition(0, 0);
                    Environment.Exit(0);
                } else {
                    goto AskAgain;
                }
            }
        }

        static string[,] BoardInit(string[,] Array) {
            
            bool floor = false;
            bool wall = false;

            for (int x = 0; x < 5; x++) {
                if (floor == false) {
                    for (int y = 0; y < 5; y++) {
                        if (wall == true) {
                            Array[x, y] = "|";
                            wall = false;
                        } else {
                            Array[x, y] = " ";
                            wall = true;
                        }
                    }
                    wall = false;
                    floor = true;
                } else {
                    for (int y = 0; y < 5; y++) {
                        Array[x, y] = "-";
                    }
                    floor = false;
                }
            }
            return Array;
        }

        static void ShowBoard(string[,] Array, int X_score, int O_score) {
            Console.Clear();
            for (int i = 0; i < 5; i++) {
                for (int z = 0; z < 5; z++) {
                    //Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write(Array[i, z]);
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(17, 0);
            Console.Write("Scoreboard");
            Console.SetCursorPosition(20, 1);
            Console.Write("X: {0}", X_score);
            Console.SetCursorPosition(20, 3);
            Console.Write("O: {0}", O_score);
            //Console.BackgroundColor = ConsoleColor.Black;

        }

        static string Move() {
            Console.SetCursorPosition(2, 2);
            again:
            int Xpos = Console.CursorLeft;
            int Ypos = Console.CursorTop;
            ConsoleKeyInfo direction = Console.ReadKey(true);
            try {
                do {
                switch (direction.Key) {
                        case ConsoleKey.LeftArrow:
                            if (Xpos==0) {
                                Console.SetCursorPosition(Xpos, Ypos);
                                //Console.WriteLine("izquierda!");
                                goto again;
                            } else {
                                Console.SetCursorPosition(Xpos - 2, Ypos);
                                //Console.WriteLine("izquierda!");
                                goto again;
                            }
                        case ConsoleKey.UpArrow:
                            if (Ypos==0) {
                                Console.SetCursorPosition(Xpos, Ypos);
                                //Console.WriteLine("arriba!");
                                goto again;
                            } else {
                                Console.SetCursorPosition(Xpos, Ypos - 2);
                                //Console.WriteLine("arriba!");
                                goto again;
                            }
                        case ConsoleKey.RightArrow:
                            if (Xpos==4) {
                                Console.SetCursorPosition(Xpos, Ypos);
                                //Console.WriteLine("derecha!");
                                goto again;
                            } else {
                                Console.SetCursorPosition(Xpos + 2, Ypos);
                                //Console.WriteLine("derecha!");
                                goto again;
                            }
                        case ConsoleKey.DownArrow:
                            if (Ypos==4) {
                                Console.SetCursorPosition(Xpos, Ypos);
                                //Console.WriteLine("abajo!");
                                goto again;
                            } else {
                                Console.SetCursorPosition(Xpos, Ypos + 2);
                                //Console.WriteLine("abajo!");
                                goto again;
                            }
                    }
                } while (direction.Key != ConsoleKey.Spacebar);
                return Console.CursorLeft + "," + Console.CursorTop;
            } catch (Exception) {
                goto again;
            }
        }

        static string Check(string [,] Array, int turn) {
            string A = Array[0, 0];
            string B = Array[0, 2];     //A|B|C
            string C = Array[0, 4];     //-----
            string D = Array[2, 0];     //D|E|F
            string E = Array[2, 2];     //-----
            string F = Array[2, 4];     //G|H|I
            string G = Array[4, 0];
            string H = Array[4, 2];
            string I = Array[4, 4];

            if (E == A && E == I) {
                if (E.Equals(" ")) {
                    return ("continue");
                } else {
                    return (E + " WON!");
                }
            }
            if (E == D && E == F) {
                if (E.Equals(" ")) {
                    return ("continue");
                } else {
                    return (E + " WON!");
                }
            }
            if (E == G && E == C) {
                if (E.Equals(" ")) {
                    return ("continue");
                } else {
                    return (E + " WON!");
                }
            }
            if (E == H && E == B) {
                if (E.Equals(" ")) {
                    return ("continue");
                } else {
                    return (E + " WON!");
                }
            }
            if (F == C && F == I) {
                return (F + " WON!");
            }
            if (D == A && D == G) {
                if (D.Equals(" ")) {
                    return ("continue");
                } else {
                    return (D + " WON!");
                }
            }
            if (B == A && B == C) {
                if (B.Equals(" ")) {
                    return ("continue");
                } else {
                    return (B + " WON!");
                }
            }
            if (H == G && H == I) {
                if (H.Equals(" ")) {
                    return ("continue");
                } else {
                    return (H + " WON!");
                }
            }
            if (turn == 9) {
                return ("DRAW!");
            } else {
                return ("continue");
            }

        }

        //private void StartObserving() {
        //    thread = new Thread(this.Game());
        //    thread.Start();
        //}
    }
}