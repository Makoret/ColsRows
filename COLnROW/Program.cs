using System;
using System.Threading;
using System.Threading.Tasks;
using WMPLib;

namespace COLSnROWS {

    class Program {

        public static void Main(string[] args) {
            Console.Title = "Cols&Rows";
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetWindowSize(33, 6);//Always first
            Console.SetBufferSize(33, 6);//Always second
            System.Media.SoundPlayer SP = new System.Media.SoundPlayer();

            MainMenu();
            //it will only continue if the player presses spacebar
            string[,] board = new string[5, 5];//creates the board array
            int X_score = 0, O_score = 0;//sets score to 0

            Game(board, X_score, O_score);//game starts

        }

        //First thing to show up waitting for a player
        public static void MainMenu() {
            MainMenu:
            Console.Clear();
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(12, 2);
            Console.Write("Cols&Rows");
            ConsoleKeyInfo Activeness;
            //Calls the method press to run parallel to this method without interfering
            Thread t = new Thread(press);
            t.Start();
            Activeness = Console.ReadKey(true);
            switch (Activeness.Key) {
                case ConsoleKey.Spacebar:
                    try {
                        t.Abort();
                        Console.CursorVisible = true;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    } catch (Exception) {
                    }
                    break;
                default://In case the user press anything but the spacebar key
                    t.Abort();
                    goto MainMenu;
            }
        }
        //Initialize the board
        public static string[,] BoardInit(string[,] Array) {
            
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
        //The game loop
        public static void Game(string [,] Array, int X_score, int O_score) {
            
            Start:
            BoardInit(Array);
            bool side = true;
            side = ShowBoard(Array, X_score, O_score, side);
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
                        MusicPlay(Environment.CurrentDirectory + @"\Audio\Turn_bell.wav");
                    } else {
                        Array[Xpos, Ypos] = "O";
                        side = true;
                        MusicPlay(Environment.CurrentDirectory + @"\Audio\Turn_bell.wav");
                    }
                    ShowBoard(Array, X_score, O_score, side);
                    turn++;
                    if (turn >= 5) {
                        veredict = Check(Array, turn);
                    }
                }
            } while (veredict.Equals("continue"));
            if (veredict.Equals("DRAW!")) {
                MusicPlay(Environment.CurrentDirectory + @"\Audio\End_Turn_bell.wav");
                AskAgain:
                Console.SetCursorPosition(0, 5);
                Console.Write(veredict);
                Console.SetCursorPosition(10, 5);
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
                Console.SetCursorPosition(0, 5);
                Console.Write(veredict);
                veredict = veredict.Remove(1);
                if (veredict.Equals("X")) {
                    X_score++;
                    MusicPlay(Environment.CurrentDirectory + @"\Audio\End_Turn_bell.wav");
                } else {
                    O_score++;
                    MusicPlay(Environment.CurrentDirectory + @"\Audio\End_Turn_bell.wav");
                }
                AskAgain:
                Console.SetCursorPosition(10, 5);
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
        //Method to show up the board and score values
        public static bool ShowBoard(string[,] Array, int X_score, int O_score, bool player) {
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
            if (player) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(20, 1);
                Console.Write("X: {0}", X_score);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(20, 3);
                Console.Write("O: {0}", O_score);
                Console.ResetColor();
                return true;

            } else {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(20, 1);
                Console.Write("X: {0}", X_score);
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(20, 3);
                Console.Write("O: {0}", O_score);
                Console.ResetColor();
                return false;
            }
            //Console.BackgroundColor = ConsoleColor.Black;
        }
        //Method looking for rows or columns to be completed by only one player
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
        //Allows the movement within the board
        public static string Move() {
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
                        case ConsoleKey.Spacebar:
                            break;
                        default:
                            goto again;
                    }
                } while (direction.Key != ConsoleKey.Spacebar);
                return Console.CursorLeft + "," + Console.CursorTop;
            } catch (Exception) {
                goto again;
            }
        }
        //Makes a small delay
        public static void SleepTime(int miliseconds) {
            var t = Task.Run(async delegate {
                await Task.Delay(miliseconds);
                return 42;
            });
            t.Wait();
        }
        //A small effect changing the color of a text
        public static void press() {
            while (true) {
                SleepTime(200);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(10, 4);
                Console.Write("Press spacebar");
                SleepTime(150);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(10, 4);
                Console.Write("Press spacebar");
                SleepTime(100);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(10, 4);
                Console.Write("Press spacebar");
                SleepTime(50);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(10, 4);
                Console.Write("Press spacebar");
            }
        }

        public static void MusicPlay(String Address) {
            WindowsMediaPlayer Player = new WindowsMediaPlayer();
            Player.URL = Address;
            Player.controls.play();
            Player.settings.volume = 10;
        }
    }
}