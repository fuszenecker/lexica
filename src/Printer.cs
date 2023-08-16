namespace LsReader;

internal static class Printer
{
    const char ESC = '¤';
    const char RETURN = '÷';
    static readonly Stack<char> _colorStack = new();

    public static void PrintInColors(string value)
    {
        var parts = value.Split(ESC);

        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            var color = part[0];

            if (color == RETURN)
            {
                _colorStack.Pop();
                color = _colorStack.Peek();
            }
            else
            {
                _colorStack.Push(color);
            }

            switch (color)
            {
                case 'W':
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'G':
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'g':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'B':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'Y':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'D':
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'R':
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'M':
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'y':
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'C':
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case 'r':
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(part[1..]);
                    break;

                case '_':
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(part[1..]);
                    break;

                case '=':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(part[1..]);
                    break;

                default:
                    Console.Write(part);
                    break;
            }
        }
    }
}