using System.Drawing;

namespace GgpkParser.DataTypes.UIImages
{
    public class UIImage
    {
        public string Path { get; set; }
        public string Texture { get; set; }
        public Rectangle Rectangle { get; set; }
        public UIImage(string path, string texture, Rectangle rectangle) => (Path, Texture, Rectangle) = (path, texture, rectangle);
    }
}
