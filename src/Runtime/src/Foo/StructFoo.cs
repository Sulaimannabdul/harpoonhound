namespace csharp_garbage_collector
{
    internal struct StructFoo
    {
        public readonly int Index;
        public string Name;

        public StructFoo(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}