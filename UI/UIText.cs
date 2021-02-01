namespace HabiCS.UI
{
    public class UIText
    {
        private bool _textChanged;

        private string _text;

        public string Text {
            get { return _text;}
            set {
                _text = value;
                _textChanged = true;
            }
        }

        public bool Updated { get { return _textChanged;} set { _textChanged = value; } }

        public UIText(string text)
        {
            _text = text;
            _textChanged = true;
        }
    }
}