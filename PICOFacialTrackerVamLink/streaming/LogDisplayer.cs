namespace PICOFacialTrackerVamLink;

public interface LogDisplayer
{
    public interface ButtonPressed
    {
        void OnButtonPressed();
    }

    public enum Color
    {
        BLACK, RED
    }

    void ShowText(string text, Color color = Color.BLACK);
    void HideButton();
    void ShowButton(string btnMsg, ButtonPressed callback);
}