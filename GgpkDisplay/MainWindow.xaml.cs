using GgpkParser.DataTypes.Dat;
using GgpkParser.DataTypes.Specifications;
using GgpkParser.Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private Dictionary<string, HashSet<string>> Directory { get; set; } = new Dictionary<string, HashSet<string>>()
        {
            { "ROOT", new HashSet<string>() }
        };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                lock (buildlock)
                {
                    GgpkRecordLoader.Load();
                }

                BuildDirectory();
            }).Start();
        }

        private object buildlock = new object();
        private void BuildDirectory(string pattern = "")
        {
            lock (buildlock)
            {
                try
                {
                    var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    Directory = new Dictionary<string, HashSet<string>>() { { "ROOT", new HashSet<string>() } };
                    var paths = new HashSet<string>();

                    if (!(GgpkRecordLoader.IndexBin is null))
                    {
                        for (var i = 0; i < GgpkRecordLoader.IndexBin.FileCount; i++)
                        {
                            paths.Add(GgpkRecordLoader.IndexBin.FileInfos[i].Path);
                        }
                    }

                    foreach (var record in GgpkRecordLoader.Records[RecordType.File].OfType<FileRecord>())
                    {
                        paths.Add(record.Path);
                    }

                    paths = string.IsNullOrWhiteSpace(pattern) ? paths : paths.Where(x => regex.IsMatch(x)).ToHashSet();
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
                        GgpkFileExplorer.Items.Clear();
                        var root = CreateTreeViewItem("ROOT");
                        GgpkFileExplorer.Items.Add(root);

                        root.Items.Add(null);
                        root.Expanded += TreeViewItem_Expanded;
                        root.IsExpanded = true;
                    }));
                }
                catch { }
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem view && view.Tag is string parent && Directory.ContainsKey(parent))
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
                var hexTab = GgpkFileTab.Items.OfType<TabItem>().Where(x => x.Header.ToString() == "HexEditor").First();
                var dataTab = GgpkFileTab.Items.OfType<TabItem>().Where(x => x.Header.ToString() == "DataGrid").First();
                new Thread(() =>
                {
                    var spec = GgpkRecordLoader.LoadRecord(file);
                    MemoryStream? old = null;

                    GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        old = GgpkHexEditor.Stream;
                        FileLabel.Text = $"File: {file}";
                        hexTab.IsSelected = true;
                        dataTab.Visibility = Visibility.Hidden;
                    }));

                    switch (spec)
                    {
                        case DatSpecification datSpec when !(datSpec.Specification is null) && datSpec.DataTable.Columns.Count > 1:
                            GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                dataTab.IsSelected = true;
                                dataTab.Visibility = Visibility.Visible;

                                GgpkDataGrid.ItemsSource = datSpec.DataTable.DefaultView;
                                GgpkHexEditor.Stream = new MemoryStream(spec.RawData);
                            }));
                            break;
                        default:
                            GgpkFileExplorer.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                hexTab.IsSelected = true;

                                GgpkHexEditor.Stream = new MemoryStream(spec.RawData);
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

        System.Timers.Timer? _filterTimer = null;
        System.Timers.Timer FilterTimer
        {
            get
            {
                if (_filterTimer is null)
                {
                    _filterTimer = new System.Timers.Timer()
                    {
                        Interval = 500
                    };

                    _filterTimer.Elapsed += FilterTimer_Elapsed;
                }

                return _filterTimer;
            }
        }

        private void GgpkFilter_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            FilterTimer.Stop();
            FilterTimer.Start();
        }

        private void FilterTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FilterTimer.Stop();

            var text = string.Empty;
            GgpkFileExplorer.Dispatcher.Invoke(new Action(() =>
            {
                text = GgpkFilter.Text;
            }));

            new Thread(() => { BuildDirectory(text); }).Start();
        }
    }
}
