using System.Collections.Generic;

namespace csharp_garbage_collector
{
    internal static partial class Program
    {
        private static void ClassCall(List<ClassFoo> list)
        {
            for (int i = 0; i < items; i++)
            {
                list.Add(new ClassFoo(i, i.ToString()));
            }

            list.Clear();

            GetGCGenerationAndCollect(list, $"Operations with classes");
        }

        private static void StructCall(List<StructFoo> list)
        {
            for (int i = 0; i < items; i++)
            {
                list.Add(new StructFoo(i, i.ToString()));
            }
            list.Clear();

            GetGCGenerationAndCollect(list, $"Operations with structs");
        }
    }
}