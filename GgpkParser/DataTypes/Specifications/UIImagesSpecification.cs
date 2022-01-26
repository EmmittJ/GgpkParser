using GgpkParser.DataTypes.UIImages;
using GgpkParser.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GgpkParser.DataTypes.Specifications
{
    [Specification(FileExtension = "UIImages1.txt", Priority = 1)]
    [Specification(FileExtension = "UIDivinationImages.txt", Priority = 1)]
    [Specification(FileExtension = "UIPS4.txt", Priority = 1)]
    [Specification(FileExtension = "UIShopImages.txt", Priority = 1)]
    [Specification(FileExtension = "UIXbox.txt", Priority = 1)]
    public class UIImagesSpecification : IDataSpecification
    {
        public byte[] RawData { get; private set; } = Array.Empty<byte>();
        public DataTable DataTable { get; private set; } = new DataTable();
        public string Name { get; }

        public UIImagesSpecification(string name = "") => Name = name;

        public void LoadFrom(in Stream stream, Data data)
        {
            stream.Position = data.Offset;
            RawData = stream.Read<byte>(data.Length);

            using var memory = new MemoryStream(RawData);
            var rows = Encoding.Unicode.GetString(memory.Read<byte>(data.Length)).Split('\n');

            foreach (var row in rows)
            {
                var parse = row.Split('"').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (parse.Length < 3)
                {
                    continue;
                }

                var path = parse[0];
                var texture = parse[1];
                var rect = parse[2].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (!int.TryParse(rect[0], out var x))
                {
                    throw new InvalidDataException($"Could not get {nameof(x)} out of ${row}");
                }

                if (!int.TryParse(rect[1], out var y))
                {
                    throw new InvalidDataException($"Could not get {nameof(y)} out of ${row}");
                }

                if (!int.TryParse(rect[2], out var w))
                {
                    throw new InvalidDataException($"Could not get {nameof(w)} out of ${row}");
                }

                if (!int.TryParse(rect[3], out var h))
                {
                    throw new InvalidDataException($"Could not get {nameof(h)} out of ${row}");
                }

                UIImages.Add(new UIImage(path, texture, new System.Drawing.Rectangle(x, y, w - x, h - y)));
            }

            SetupDataTableColumns();
            foreach (var uiimage in UIImages)
            {
                var row = DataTable.NewRow();
                row["Texture"] = $"\"{uiimage.Texture}\"";
                row["Path"] = $"\"{uiimage.Path}\"";
                row["X"] = uiimage.Rectangle.X;
                row["Y"] = uiimage.Rectangle.Y;
                row["Width"] = uiimage.Rectangle.Width;
                row["Height"] = uiimage.Rectangle.Height;
                DataTable.Rows.Add(row);
            }
        }

        private void SetupDataTableColumns()
        {

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "#",
                Caption = "#",
                DataType = typeof(int),
                ReadOnly = true,
                AutoIncrement = true,
            });

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "Texture",
                Caption = "Texture",
                DataType = typeof(string),
                ReadOnly = true,
            });

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "Path",
                Caption = "Path",
                DataType = typeof(string),
                ReadOnly = true,
            });

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "X",
                Caption = "X",
                DataType = typeof(int),
                ReadOnly = true,
            });

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "Y",
                Caption = "Y",
                DataType = typeof(int),
                ReadOnly = true,
            });

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "Width",
                Caption = "Width",
                DataType = typeof(int),
                ReadOnly = true,
            });

            DataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "Height",
                Caption = "Height",
                DataType = typeof(int),
                ReadOnly = true,
            });
        }

        public List<UIImage> UIImages = new();
    }
}
