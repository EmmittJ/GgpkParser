using GgpkParser.DataTypes.Specifications;
using GgpkParser.Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GgpkDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GgpkRecordLoader GgpkRecordLoader { get; }
        public MainWindow()
        {
            InitializeComponent();
            GgpkRecordLoader = new GgpkRecordLoader(@"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\Content.ggpk");
        }

        private Dictionary<string, HashSet<string>> Directory { get; } = new Dictionary<string, HashSet<string>>()
        {
            { "ROOT", new HashSet<string>() }
        };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                GgpkRecordLoader.Load();
                if (GgpkRecordLoader.IndexBin is null)
                {
                    return;
                }

                var paths = new HashSet<string>();

                var index = GgpkRecordLoader.IndexBin;
                for (var i = 0; i < index.FileCount; i++)
                {
                    paths.Add(index.FileInfos[i].Path);
                }

                foreach (var record in GgpkRecordLoader.Records[RecordType.File].OfType<FileRecord>())
                {
                    paths.Add(record.Path);
                }

                foreach (var path in paths)
                {
                    if (path.EndsWith(".bundle.bin") || path.EndsWith(".index.bin"))
                    {
                        continue;
                    }

                    var exploded = path.Split(Path.AltDirectorySeparatorChar);
                    if (exploded.Length > 1)
                    {
                        Directory["ROOT"].Add(exploded[0]);
                    }

                    for (var j = 1; j < exploded.Length; j++)
                    {
                        var dir = string.Join(Path.AltDirectorySeparatorChar, exploded[..j]);
                        if (!Directory.ContainsKey(dir))
                        {
                            Directory[dir] = new HashSet<string>();
                        }
                        Directory[dir].Add(string.Join(Path.AltDirectorySeparatorChar, exploded[..(j + 1)]));
                    }
                }

                GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var root = CreateTreeViewItem("ROOT");
                    GgpkFileExplorer.Items.Add(root);

                    root.Items.Add(null);
                    root.Expanded += TreeViewItem_Expanded;
                    root.IsExpanded = true;
                }));
            }).Start();
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem view && view.Tag is string parent)
            {
                if (view.Items.Count == 1 && view.Items[0] is null)
                {
                    view.Items.Clear();
                    foreach (var child in Directory[parent].OrderBy(x => Directory.ContainsKey(x) ? $"!!{x}" : x))
                    {
                        var sub = CreateTreeViewItem(child);

                        view.Items.Add(sub);
                        if (Directory.ContainsKey(child))
                        {
                            sub.Items.Add(null);
                            sub.Expanded += TreeViewItem_Expanded;
                        }
                        else
                        {
                            sub.MouseDoubleClick += Sub_MouseDoubleClick;
                        }
                    }
                }
            }
        }

        private void Sub_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem item && item.Tag is string file)
            {
                new Thread(() =>
                {
                    var spec = GgpkRecordLoader.LoadRecord(file);
                    MemoryStream? old = null;

                    GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        old = GgpkFileHexEditor.Stream;
                        FileLabel.Text = $"File: {file}";
                        GgpkFileHexEditor.Visibility = Visibility.Hidden;
                        GgpkTextEditor.Visibility = Visibility.Hidden;
                    }));

                    switch (spec)
                    {
                        case IndexBinSpecification indexBinSpec:
                            if (indexBinSpec.IndexBin is null) break;

                            var builder = new StringBuilder();
                            foreach (var (hash, filename) in indexBinSpec.IndexBin.HashToFileName)
                            {
                                builder.Append($"Hash: {hash}, Filename: {filename}");
                            }
                            var text = builder.ToString();
                            GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                GgpkTextEditor.Visibility = Visibility.Visible;
                                GgpkTextEditor.Text = text;
                            }));
                            break;
                        default:
                            GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                GgpkFileHexEditor.Visibility = Visibility.Visible;
                                GgpkFileHexEditor.Stream = new MemoryStream(spec.RawData);
                            }));
                            break;
                    }

                    if (!(old is null))
                    {
                        old.Flush();
                        old.Dispose();
                    }
                }).Start();
            }
        }

        private TreeViewItem CreateTreeViewItem(string name)
        {
            var item = new TreeViewItem()
            {
                Tag = name,
                FontWeight = FontWeights.Normal,
                Header = name.Split(Path.AltDirectorySeparatorChar)[^1]
            };

            return item;
        }
    }
}
