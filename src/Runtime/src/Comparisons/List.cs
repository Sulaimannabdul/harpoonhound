using System.Collections.Generic;

namespace csharp_garbage_collector
{
    internal static partial class Program
    {
        private static void ListFoo()
        {
            List<int> list = new List<int>();

            for (int i = 0; i < items; i++)
            {
                list.Add(i);
            }

            GetGCGenerationAndCollect(list, $"List");
        }

        private static void PreSizedList()
        {
            List<int> list = new List<int>(items);

            for (int i = 0; i < items; i++)
            {
                list.Add(i);
            }

            GetGCGenerationAndCollect(list, $"Pre-sized list");
        }
    }
}