namespace Syy.Tools
{
    public interface IGameViewSizeData
    {
        string Title { get; set; }
        string Aspect { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Orientation orientation { get; set; }

        string ToText();
    }
}
