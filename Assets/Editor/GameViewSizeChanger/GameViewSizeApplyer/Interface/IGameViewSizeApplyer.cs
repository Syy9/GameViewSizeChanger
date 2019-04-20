namespace Syy.Tools
{
    public interface IGameViewSizeApplyer
    {
        void Apply();
        bool IsSelect();
        void NoticeChangedOtherSize();
    }
}
