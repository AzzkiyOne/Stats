namespace Stats;

internal interface IWidget_TableCell : IWidget
{
    bool IsPinned { get; }
    float Width { get; set; }
}
