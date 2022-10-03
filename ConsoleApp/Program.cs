using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    // TODO: perform code cleanup
    class Program
    {
        public struct BufferItem
        {
            public char Value { get; set; }

            public bool IsWriteable { get; set; }

            public int Id { get; set; }

            public int Index { get; set; }
        }

        public class ConsoleItem
        {
            public ConsoleItem(string title, uint length) : this(title, string.Empty, length)
            {
            }

            public ConsoleItem(string title, string value, uint length)
            {
                Title = title + ": ";
                Value = value;
                Length = length;
            }

            public string Title { get; }

            public string Value { get; set; }

            public uint Length { get; }
        }

        private static BufferItem[,] Arr;

        static void Main(string[] args)
        {
            Console.Title = "Factory: Console";
            Console.TreatControlCAsInput = true;

            var items = new Dictionary<int, ConsoleItem>
            {
                { 1, new ConsoleItem("Name", "Hiro", 15) },
                { 2, new ConsoleItem("Hobby", 100) },
                { 3, new ConsoleItem("Age", "20", 3) },
            };

            Console.Clear();
            ConsoleItem last = null;
            foreach (var item in items)
            {
                Console.WriteLine(item.Value.Title + item.Value.Value);
                last = item.Value;
            }

            Console.SetCursorPosition(last.Title.Length + last.Value.Length, Console.CursorTop - 1);

            while (true)
            {
                var input = Console.ReadKey(true);

                var left = Console.CursorLeft;
                var top = Console.CursorTop;

                if (input.Key == ConsoleKey.Enter)
                {
                    break;
                }

                var currentElement = GetCurrentElement(items, left, top);

                switch (input.Key)
                {
                    case ConsoleKey.LeftArrow:
                        left--;
                        TrySetCursorPosition(left, top, true);
                        break;
                    case ConsoleKey.RightArrow:
                        left++;
                        TrySetCursorPosition(left, top, true);
                        break;
                    case ConsoleKey.UpArrow:
                        top--;
                        TrySetCursorPosition(left, top, false);
                        break;
                    case ConsoleKey.DownArrow:
                        top++;
                        TrySetCursorPosition(left, top, false);
                        break;
                    case ConsoleKey.Backspace:
                        ClearCharacter(currentElement, true);
                        break;
                    case ConsoleKey.Delete:
                        ClearCharacter(currentElement, false);
                        break;
                    case ConsoleKey.Insert:
                        break;
                    case ConsoleKey.Home:
                        left = 0;
                        TrySetCursorPosition(left, top, false);
                        break;
                    case ConsoleKey.End:
                        left = Console.BufferWidth - 1;
                        TrySetCursorPosition(left, top, false);
                        break;
                    case ConsoleKey.PageUp:
                        top = 0;
                        TrySetCursorPosition(left, top, false);
                        break;
                    case ConsoleKey.PageDown:
                        top = Console.BufferHeight;
                        TrySetCursorPosition(left, top, false);
                        break;
                    default:
                        TryWrite(input.KeyChar, currentElement);
                        break;
                }
            }

            Console.ReadLine();
        }

        private static void TryWrite(char keyChar, ConsoleItem item)
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            if (Arr[left, top].IsWriteable)
            {
                if (Arr[left, top].Value == default)
                {
                    Console.Write(keyChar);
                    item.Value += keyChar;
                }
                else
                {
                    if (item.Value.Length < item.Length - 1)
                    {
                        Console.Write(keyChar);
                        for (var i = Arr[left, top].Index; i < item.Value.Length; i++)
                        {
                            Console.Write(item.Value[i]);
                        }

                        item.Value = item.Value.Substring(0, Arr[left, top].Index) + keyChar + item.Value.Substring(Arr[left, top].Index, item.Value.Length - Arr[left, top].Index);
                        left++;
                        if (left == Console.BufferWidth)
                        {
                            left = 0;
                            top++;
                        }
                        Console.SetCursorPosition(left, top);
                    }
                }
            }
        }

        private static ConsoleItem GetCurrentElement(Dictionary<int, ConsoleItem> items, int left, int top)
        {
            var width = Console.BufferWidth;
            var height = 0;
            foreach (var item in items)
            {
                height += ((item.Value.Value.Length + item.Value.Title.Length) / width + 1);
            }

            Arr = new BufferItem[width, height];

            width = 0;
            height = 0;

            foreach (var item in items)
            {
                for (var i = 0; i < item.Value.Title.Length; i++)
                {
                    Arr[width, height] = new BufferItem { Id = item.Key, Value = item.Value.Title[i] };
                    width++;
                    if (width == Console.BufferWidth)
                    {
                        height++;
                        width = 0;
                    }
                }

                for (var i = 0; i < item.Value.Value.Length; i++)
                {
                    Arr[width, height] = new BufferItem { Id = item.Key, Value = item.Value.Value[i], IsWriteable = true, Index = i };
                    width++;
                    if (width == Console.BufferWidth)
                    {
                        height++;
                        width = 0;
                    }
                }

                if (item.Value.Value.Length < item.Value.Length)
                {
                    Arr[width, height].IsWriteable = true;
                    Arr[width, height].Id = item.Key;

                    width++;
                    if (width == Console.BufferWidth)
                    {
                        height++;
                        width = 0;
                    }
                }

                for (var i = width; i < Console.BufferWidth; i++)
                {
                    Arr[i, height].Id = item.Key;
                }
                width = 0;
                height++;
            }

            return items[Arr[left, top].Id];
        }

        private static void TrySetCursorPosition(int left, int top, bool reversed)
        {
            var maxTop = Arr.Length / Console.BufferWidth - 1;
            if (left < 0)
            {
                left = Console.BufferWidth - 1;
                top--;
            }

            if (left > Console.BufferWidth - 1)
            {
                left = 0;
                top++;
            }

            if (top < 0)
            {
                top = 0;
            }

            if (top > maxTop)
            {
                top = maxTop;
            }

            int direction;
            if (Arr[left, top].Value == default)
            {
                direction = reversed ? 1 : -1;
            }
            else
            {
                direction = reversed ? -1 : 1;
            }


            while (!Arr[left, top].IsWriteable)
            {

                left += direction;

                if (left == 0)
                {
                    left = Console.BufferWidth - 1;
                    top--;
                }

                if (left == Console.BufferWidth)
                {
                    left = 0;
                    top++;
                }

                if (top == -1)
                {
                    top++;
                    left = 0;
                    direction *= -1;
                }

                if (top > maxTop)
                {
                    top--;
                    left = Console.BufferWidth - 1;
                    direction *= -1;
                }
            }

            Console.SetCursorPosition(left, top);
        }

        private static void ClearCharacter(ConsoleItem item, bool moveMe)
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;


            var prev = left - 1;
            var next = left + 1;
            if (prev < 0)
            {
                prev = Console.BufferWidth - 1;
                top--;
            }

            if(next > Console.BufferWidth - 1)
            {
                next = 0;
                top++;
            }

            if (moveMe && Arr[prev, top].IsWriteable)
            {
                if (Arr[prev, top].Index < item.Value.Length - 1)
                {
                    Console.SetCursorPosition(prev, top);
                    for (var i = Arr[left, top].Index; i < item.Value.Length; i++)
                    {
                        Console.Write(item.Value[i]);
                    }
                    Console.Write(' ');
                    Console.SetCursorPosition(prev, top);
                    item.Value = item.Value.Substring(0, Arr[left, top].Index) + item.Value.Substring(Arr[left, top].Index + 1, item.Value.Length - Arr[left, top].Index - 1);
                }
                else
                {
                    Console.Write("\b \b");
                    item.Value = item.Value.Substring(0, item.Value.Length - 1);
                }
            }
            else if (!moveMe && Arr[next, top].IsWriteable)
            {
                for (var i = Arr[left, top].Index + 1; i < item.Value.Length; i++)
                {
                    Console.Write(item.Value[i]);
                }

                Console.Write(' ');

                Console.SetCursorPosition(left, top);
                item.Value = item.Value.Substring(0, Arr[left, top].Index) + item.Value.Substring(Arr[left, top].Index + 1, item.Value.Length - Arr[left, top].Index - 1);
            }
        }
    }
}
