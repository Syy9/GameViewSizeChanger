namespace Syy.GameViewSizeChanger
{
    public interface IGameViewSizeApplyer
    {
        void Apply();
        bool IsSelect();
        void NoticeChangedOtherSize();
    }
}
