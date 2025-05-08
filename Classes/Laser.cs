using System.Drawing;
using System.Windows.Forms;

namespace vlc_works015
{
    public class Laser
    {
        public ushort Value { get; set; } = 10;
        public bool LastIsIntersected { get; set; } = false;
        private Label Label { get; set; }

        public Laser(Label label)
        {
            Label = label;
        }

        public bool IsIntersected() => Value > 2045; // (4090 / 2);

        public void SetValue(ushort value)
        {
            Value = value;
            Label.Text = Value.ToString();
        }

        public bool SetColor()
        {
            bool isIntersected = IsIntersected();
            Label.BackColor =
                isIntersected
                ? Color.LightGreen
                : Color.LightCoral;
            return isIntersected;
        }

        public bool SetValueAndColor(ushort value)
        {
            SetValue(value);
            return SetColor();
        }
    }
}
