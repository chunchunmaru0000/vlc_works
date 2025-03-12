namespace vlc_works
{
    public struct SheetAndRange
    {
        public string Sheet { get; set; }
        public string StartCell { get; set; }
        public string FinalCell { get; set; }

        public static SheetAndRange New(string sheet, string startCell, string finalCell = null) =>
            new SheetAndRange()
            {
                Sheet = sheet,
                StartCell = startCell,
                FinalCell = finalCell
            };

        public override string ToString() =>
            $"{Sheet}!{(FinalCell == null ? StartCell : $"{StartCell}:{FinalCell}")}";
    }
}
