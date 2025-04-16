namespace vlc_works015
{
    public struct ListAndRange
    {
        public string List { get; set; }
        public string StartCell { get; set; }
        public string FinalCell { get; set; }

        public static ListAndRange New(string startCell, string finalCell = null, string list = null) =>
            new ListAndRange()
            {
                List = list,
                StartCell = startCell,
                FinalCell = finalCell
            };

        public string FromList(string list)
        {
            List = list;
            return ToString();
        }

        public override string ToString() =>
            List == null
            ? throw new System.Exception(
                $"НЕ БЫЛ УКАЗАН list В SheetAndRange\n" +
                $"ИЗВЕСТНЫЕ ЗНАЧЕНИЯ:\n\tStartCell[{StartCell}]\n\tFinalCell[{FinalCell}]")

            : $"{List}!{(FinalCell == null ? StartCell : $"{StartCell}:{FinalCell}")}";
    }
}
