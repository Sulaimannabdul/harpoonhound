using System;
using System.Collections.Generic;

namespace csharp_garbage_collector
{
    internal static partial class Program
    {
        private const int items = 3000;
        private const int iterations = int.MaxValue / 2;

        private static void GetGCGenerationAndCollect<T>(T obj, string op)
        {
            long memorySnapshot1 = GC.GetTotalMemory(false);

            GC.SuppressFinalize(obj);
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            long memorySnapshot2 = GC.GetTotalMemory(false);

            Console.WriteLine(
                @$"
                    Operation: {op}
                    Difference of memory used before and after GC {Math.Abs(memorySnapshot1 - memorySnapshot2)} bytes
                    Object deallocated on Gen: {GC.GetGeneration(obj)}
                 ");
        }

        private static void Main()
        {
            bool showMenu = true;
            while (showMenu)
            {
                Console.WriteLine("\n----------------------");
                showMenu = MainMenu();
            }
        }

        private static bool MainMenu()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1 - Boxing and Unboxing operation");
            Console.WriteLine("2 - String and String Builder concatenation");
            Console.WriteLine("3 - List and Pre-sized List");
            Console.WriteLine("4 - Class and Struct");

            Console.WriteLine("5 - Exit");

            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    WithBoxingUnboxing(items);
                    WithoutBoxingUnboxing(items);
                    return true;

                case "2":
                    ConcatenateString();
                    StringBuilder();
                    return true;

                case "3":
                    PreSizedList();
                    ListFoo();
                    return true;

                case "4":
                    ClassCall(new List<ClassFoo>());
                    StructCall(new List<StructFoo>());
                    return true;

                case "5":
                    return false;

                default:
                    return true;
            }
        }
    }
}