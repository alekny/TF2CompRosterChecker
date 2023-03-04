
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Xml;

namespace TF2CompRosterChecker
{

    public sealed partial class MainWindow : Window
    {
        //We use this to trace the recheck-functionality.
        private bool checkCompleted = false;

        //Store when our last query was finished.
        private DateTimeOffset completedAt;

        //Store the last checked league here.
        private string league = "";

        //Store the last checked leagueformat here.
        private Checker.LeagueFormat checkedFormat;

        //Let other methods and handlers use this.
        private List<Player> result = null;

        public MainWindow()
        {
            InitializeComponent();
            
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("TF2CompRosterChecker.SteamIDSyntaxHL.xml"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    statusOutput.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            statusOutput.Options.EnableHyperlinks = false;

            //Enable Right-Click Menu
            ContextMenu cm = new ContextMenu();
            MenuItem undo = new MenuItem();
            undo.Name = "Undo";
            undo.Header = "Undo";
            undo.Click += (sender, e) => statusOutput.Undo();

            MenuItem redo = new MenuItem();
            redo.Name = "Redo";
            redo.Header = "Redo";
            redo.Click += (sender, e) => statusOutput.Redo();

            MenuItem cut = new MenuItem();
            cut.Name = "Cut";
            cut.Header = "Cut";
            cut.Click += (sender, e) => statusOutput.Cut();

            MenuItem copy = new MenuItem();
            copy.Name = "Copy";
            copy.Header = "Copy";
            copy.Click += (sender, e) => statusOutput.Copy();

            MenuItem paste = new MenuItem();
            paste.Name = "Paste";
            paste.Header = "Paste";
            paste.Click += (sender, e) => statusOutput.Paste();

            MenuItem selectall = new MenuItem();
            selectall.Name = "Select_All";
            selectall.Header = "Select All";
            selectall.Click += (sender, e) => statusOutput.SelectAll();

            cm.Items.Add(undo);
            cm.Items.Add(redo);
            cm.Items.Add(new Separator());
            cm.Items.Add(cut);
            cm.Items.Add(copy);
            cm.Items.Add(paste);
            cm.Items.Add(new Separator());
            cm.Items.Add(selectall);

            //Only make MenuItems accessible if it makes sense.
            cm.Opened += (sender, e) =>
            {
                undo.IsEnabled = !statusOutput.IsReadOnly && statusOutput.CanUndo;
                redo.IsEnabled = !statusOutput.IsReadOnly && statusOutput.CanRedo;
                cut.IsEnabled = !statusOutput.IsReadOnly && statusOutput.SelectionLength > 0;
                copy.IsEnabled = statusOutput.SelectionLength > 0;
                paste.IsEnabled = !statusOutput.IsReadOnly && Clipboard.ContainsText();
                selectall.IsEnabled = statusOutput.Text.Length > 0 && statusOutput.SelectionLength < statusOutput.Text.Length;
            };

            statusOutput.ContextMenu = cm;
        }
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            int marginleft = 0;
            int margintop = 0;
            bool switcher = true;
            string baseUrl = "";
            string baseTeamUrl = "";
            league = "";
            string statusOutputText = statusOutput.Text;
            Color color = Colors.White;
            result = new List<Player>();

            //Update UI progressbar
            Progress<int> progress = new Progress<int>();
            progress.ProgressChanged += ReportProgress;

            DisableUI();
            switch (leagueSelector.SelectedIndex)
            {
                case 0:
                    {
                        ETF2LChecker ec = new ETF2LChecker(statusOutputText);
                        _ = await Task.Run(() => result = ec.ParseData(Checker.LeagueFormat.Sixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = ec.BaseUrl;
                        baseTeamUrl = ec.BaseTeamUrl;
                        league = "ETF2L";
                        checkedFormat = Checker.LeagueFormat.Sixes;
                        break;
                    }
                case 1:
                    {
                        ETF2LChecker ec = new ETF2LChecker(statusOutputText);
                        _ = await Task.Run(() => result = ec.ParseData(Checker.LeagueFormat.HL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = ec.BaseUrl;
                        baseTeamUrl = ec.BaseTeamUrl;
                        league = "ETF2L";
                        checkedFormat = Checker.LeagueFormat.HL;
                        break;
                    }
                case 2:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.Sixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        checkedFormat = Checker.LeagueFormat.Sixes;
                        break;
                    }
                case 3:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.NRSixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        checkedFormat = Checker.LeagueFormat.NRSixes;
                        break;
                    }
                case 4:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.HL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        checkedFormat = Checker.LeagueFormat.HL;
                        break;
                    }
                case 5:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.PL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        checkedFormat = Checker.LeagueFormat.PL;
                        break;
                    }
                case 6:
                    {
                        UGCChecker uc = new UGCChecker(statusOutputText);
                        _ = await Task.Run(() => result = uc.ParseData(Checker.LeagueFormat.Sixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = uc.BaseUrl;
                        baseTeamUrl = uc.BaseTeamUrl;
                        league = "UGC";
                        checkedFormat = Checker.LeagueFormat.Sixes;
                        break;
                    }
                case 7:
                    {
                        UGCChecker uc = new UGCChecker(statusOutputText);
                        _ = await Task.Run(() => result = uc.ParseData(Checker.LeagueFormat.HL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = uc.BaseUrl;
                        baseTeamUrl = uc.BaseTeamUrl;
                        league = "UGC";
                        checkedFormat = Checker.LeagueFormat.HL;
                        break;
                    }
                case 8:
                    {
                        UGCChecker uc = new UGCChecker(statusOutputText);
                        _ = await Task.Run(() => result = uc.ParseData(Checker.LeagueFormat.FourVeeFour, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = uc.BaseUrl;
                        baseTeamUrl = uc.BaseTeamUrl;
                        league = "UGC";
                        checkedFormat = Checker.LeagueFormat.FourVeeFour;
                        break;
                    }

                default:
                    break;
            }
            this.completedAt = DateTimeOffset.Now;
            outputGrid.Children.Clear();
            if (result.Any())
            {
                statusOutput.Visibility = Visibility.Hidden;
                foreach (Player player in result)
                {
                    if (switcher)
                    {
                        color = Colors.White;
                        switcher = false;
                    }
                    else
                    {
                        color = Color.FromRgb(240, 240, 240);
                        switcher = true;
                    }
                    RowDefinition rd = new RowDefinition();
                    rd.Height = GridLength.Auto;
                    outputGrid.RowDefinitions.Add(rd);


                    Grid grid1 = new Grid();
                    grid1.Background = new SolidColorBrush(color);
                    Grid.SetColumn(grid1, 0);
                    Grid.SetRow(grid1, counter);

                    Grid grid2 = new Grid();
                    grid2.Background = new SolidColorBrush(color);
                    Grid.SetColumn(grid2, 1);
                    Grid.SetRow(grid2, counter);

                    Grid grid3 = new Grid();
                    grid3.Background = new SolidColorBrush(color);
                    Grid.SetColumn(grid3, 2);
                    Grid.SetRow(grid3, counter);

                    Grid grid4 = new Grid();
                    grid4.Background = new SolidColorBrush(color);
                    Grid.SetColumn(grid4, 3);
                    Grid.SetRow(grid4, counter);

                    Grid grid5 = new Grid();
                    grid5.Background = new SolidColorBrush(color);
                    Grid.SetColumn(grid5, 4);
                    Grid.SetRow(grid5, counter);

                    Grid grid6 = new Grid();
                    grid6.Background = new SolidColorBrush(color);
                    Grid.SetColumn(grid6, 5);
                    Grid.SetRow(grid6, counter);


                    if (player.HasBans)
                    {
                        //Only ETF2L- and RGL-Checker will set this to a non-null object yet.
                        if (player.Bans?.Any() != true)
                        {
                            Hyperlink displayid = new Hyperlink(new Run("[i]")) { };
                            Hyperlink displayid3 = new Hyperlink(new Run("[3]")) { };
                            _ = grid1.Children.Add(TBGen(new Run(player.Name), new Run("[!]"), Brushes.Red, displayid, displayid3, marginleft, margintop, true));
                            displayid.Click += (senders, es) => TextToClipboard(senders, es, player.Steamid);
                            displayid3.Click += (senders, es) => TextToClipboard(senders, es, player.Steamid3);
                        }
                        else
                        {
                            Hyperlink popup = new Hyperlink(new Run("[!]"))
                            {
                                Tag = "Show Bans",
                                Foreground = Brushes.Red,
                                ToolTip = "Show Bans"
                            };
                            Hyperlink displayid = new Hyperlink(new Run("[i]")) { };
                            Hyperlink displayid3 = new Hyperlink(new Run("[3]")) { };
                            _ = grid1.Children.Add(TBGen(new Run(player.Name), popup, displayid, displayid3, marginleft, margintop, true));
                            //Route necessary info into the EventHandler. 
                            popup.Click += (senders, es) => OpenPopup(senders, es, player.Bans);
                            displayid.Click += (senders, es) => TextToClipboard(senders, es, player.Steamid);
                            displayid3.Click += (senders, es) => TextToClipboard(senders, es, player.Steamid3);
                        }

                    }
                    else
                    {
                        Hyperlink displayid = new Hyperlink(new Run("[i]")) { };
                        Hyperlink displayid3 = new Hyperlink(new Run("[3]")) { };
                        _ = grid1.Children.Add(TBGen(new Run(player.Name), displayid, displayid3, marginleft, margintop, true));
                        displayid.Click += (senders, es) => TextToClipboard(senders, es, player.Steamid);
                        displayid3.Click += (senders, es) => TextToClipboard(senders, es, player.Steamid3);
                    }
                    if (player.Teamid != "")
                    {
                        Hyperlink teamlink = new Hyperlink(new Run("[+]"))
                        {
                            NavigateUri = new Uri(baseTeamUrl + player.Teamid),
                        };
                        teamlink.RequestNavigate += Hyperlink_RequestNavigate;
                        _ = grid2.Children.Add(TBGen(new Run(player.Team), teamlink, marginleft, margintop, false));
                    }
                    else
                    {
                        _ = grid2.Children.Add(TBGen(player.Team, marginleft, margintop, false));
                    }
                    _ = grid3.Children.Add(TBGen(player.Div, marginleft, margintop, false));
                    if (player.Leagueid != "")
                    {
                        Hyperlink leaguelink = new Hyperlink(new Run(league))
                        {
                            NavigateUri = new Uri(baseUrl + player.Leagueid),
                        };
                        leaguelink.RequestNavigate += Hyperlink_RequestNavigate;
                        _ = grid4.Children.Add(TBGen(leaguelink, marginleft, margintop, false));
                    }

                    Hyperlink logslink = new Hyperlink(new Run("Logs"))
                    {
                        NavigateUri = new Uri(SteamIDTools.baseLogsUrl + player.Profileid),
                    };
                    logslink.RequestNavigate += Hyperlink_RequestNavigate;
                    _ = grid5.Children.Add(TBGen(logslink, marginleft, margintop, false));

                    Hyperlink profilelink = new Hyperlink(new Run("Steam"))
                    {
                        NavigateUri = new Uri(SteamIDTools.baseUrl + player.Profileid),
                    };
                    profilelink.RequestNavigate += Hyperlink_RequestNavigate;
                    _ = grid6.Children.Add(TBGen(profilelink, marginleft, margintop, false));


                    _ = outputGrid.Children.Add(grid1);
                    _ = outputGrid.Children.Add(grid2);
                    _ = outputGrid.Children.Add(grid3);
                    _ = outputGrid.Children.Add(grid4);
                    _ = outputGrid.Children.Add(grid5);
                    _ = outputGrid.Children.Add(grid6);

                    counter++;
                }
                checkCompleted = true;
                MenuSave.IsEnabled = true;
                MenuJson.IsEnabled = true;
                header.Text = "Results";
                outputFrame.Visibility = Visibility.Visible;
                submitButton.Content = "Reset";
                submitButton.Click -= SubmitButton_Click;
                submitButton.Click += ResetTextBox;
            }
            else
            {
                statusOutput.Text = "No SteamIDs found";
            }
            foundIDs.Text = "";
            EnableUI();
        }

        //Callback to update the UI
        private void ReportProgress(object sender, int e)
        {
            progressBar.Value += e;
            submitButton.Content = "Checking: " + progressBar.Value + "%";
        }

        //Popup for the ban-details
        private void OpenPopup(object sender, EventArgs e, List<Ban> bans)
        {
            Popup codePopup = new Popup
            {
                Placement = PlacementMode.Mouse,
                MaxWidth = 700
            };
            StackPanel sp = new StackPanel
            {
                Background = Brushes.Salmon
            };
            TextBlock popupText = new TextBlock
            {
                Foreground = Brushes.Black,
                Text = bans.Count + " Ban(s) on Record:",
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(2, 2, 2, 8),
                TextWrapping = TextWrapping.Wrap
            };
            _ = sp.Children.Add(popupText);
            foreach (Ban ban in bans.OrderByDescending(o => o.Start).ToList())
            {
                TextBlock banline = new TextBlock
                {
                    Foreground = Brushes.Black,
                    FontSize = 13,
                    Margin = new Thickness(2, 0, 2, 2),
                    TextWrapping = TextWrapping.Wrap
                };
                _ = sp.Children.Add(banline);

                //TODO: Search ban reason for urls and make them clickable (RGL likes to do this...)

                //Display Permabans in a better way (currently only used by RGL & ETF2L)
                //RGL uses UTS of 2147483647 and ETF2L 2145826800
                if (ban.End.Equals("2147483647") || ban.End.Equals("2145826800"))
                {
                    banline.Text = UnixTimeStampToDateTime(ban.Start).ToString("dd.MM.yyyy") + " - "
                    + "Permanent" + ", Reason: " + ban.Reason;
                }
                else
                {
                    banline.Text = UnixTimeStampToDateTime(ban.Start).ToString("dd.MM.yyyy") + " - "
                    + UnixTimeStampToDateTime(ban.End).ToString("dd.MM.yyyy") + ", Reason: " + ban.Reason;
                }
            }
            codePopup.Child = sp;
            codePopup.StaysOpen = false;
            codePopup.IsOpen = true;
        }

        private void TextToClipboard(object sender, EventArgs e, string text)
        {
            //No evil stuff like phishing links possible, we calculated this id directly from the
            //steamid64.
            Clipboard.SetText(text);
            copiedNotice.Visibility = Visibility.Visible;
            Storyboard sb = Resources["copiedNoticeAnimation"] as Storyboard;
            sb.Begin(copiedNotice);
        }

        //Thanks: https://stackoverflow.com/a/250400
        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }

        private void EnableUI()
        {
            submitButton.IsEnabled = true;
            statusOutput.IsEnabled = true;
        }

        private void DisableUI()
        {
            submitButton.IsEnabled = false;
            statusOutput.IsEnabled = false;
        }

        public static TextBlock TBGen(string content, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock
            {
                Text = content,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.NoWrap,
                Height = 20
            };
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }

        public static TextBlock TBGen(Run content, Run additional, Brush color, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock();
            tb.Inlines.Clear();
            tb.Inlines.Add(content);
            additional.Foreground = color;
            tb.Inlines.Add(new Run(" "));
            tb.Inlines.Add(additional);
            tb.FontSize = 14;
            tb.Height = 20;
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.TextWrapping = TextWrapping.NoWrap;
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }

        public static TextBlock TBGen(Run content, Run additional, Brush color, Hyperlink link1, Hyperlink link2, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock();
            tb.Inlines.Clear();
            tb.Inlines.Add(content);
            additional.Foreground = color;
            tb.Inlines.Add(new Run(" "));
            tb.Inlines.Add(additional);
            tb.Inlines.Add(link1);
            tb.Inlines.Add(link2);
            tb.FontSize = 14;
            tb.Height = 20;
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.TextWrapping = TextWrapping.NoWrap;
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }

        public static TextBlock TBGen(Hyperlink link, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock();
            tb.Inlines.Clear();
            tb.Inlines.Add(link);
            tb.FontSize = 14;
            tb.Height = 20;
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.TextWrapping = TextWrapping.NoWrap;
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }

        public static TextBlock TBGen(Run text, Hyperlink link, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock();
            tb.Inlines.Clear();
            tb.Inlines.Add(text);
            tb.Inlines.Add(new Run(" "));
            tb.Inlines.Add(link);
            tb.FontSize = 14;
            tb.Height = 20;
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.TextWrapping = TextWrapping.NoWrap;
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }

        public static TextBlock TBGen(Run text, Hyperlink link1, Hyperlink link2, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock();
            tb.Inlines.Clear();
            tb.Inlines.Add(text);
            tb.Inlines.Add(new Run(" "));
            tb.Inlines.Add(link1);
            tb.Inlines.Add(link2);
            tb.FontSize = 14;
            tb.Height = 20;
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.TextWrapping = TextWrapping.NoWrap;
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }
        public static TextBlock TBGen(Run text, Hyperlink link1, Hyperlink link2, Hyperlink link3, int marginleft, int margintop, bool bold)
        {
            TextBlock tb = new TextBlock();
            tb.Inlines.Clear();
            tb.Inlines.Add(text);
            tb.Inlines.Add(new Run(" "));
            tb.Inlines.Add(link1);
            tb.Inlines.Add(link2);
            tb.Inlines.Add(link3);
            tb.FontSize = 14;
            tb.Height = 20;
            if (bold)
            {
                tb.FontWeight = FontWeights.Bold;
            }
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.TextWrapping = TextWrapping.NoWrap;
            Thickness margin = tb.Margin;
            margin.Left = marginleft;
            margin.Top = margintop;
            tb.Margin = margin;
            return tb;
        }

        private void ResetTextBox(object sender, RoutedEventArgs e)
        {
            checkCompleted = false;
            MenuSave.IsEnabled = false;
            MenuJson.IsEnabled = false;
            outputFrame.Visibility = Visibility.Hidden;
            statusOutput.Visibility = Visibility.Visible;
            foundIDs.Visibility = Visibility.Visible;
            copiedNotice.Visibility = Visibility.Hidden;
            header.Text = "Paste Status Output here:";
            outputGrid.Children.Clear();
            submitButton.Content = "Check Roster";
            progressBar.Value = 0;
            statusOutput.Text = "";
            submitButton.Click -= ResetTextBox;
            submitButton.Click += SubmitButton_Click;
        }

        private void RecheckTextBox(object sender, SelectionChangedEventArgs e)
        {
            //Only offer a recheck after one check already went through successfully
            if (checkCompleted)
            {
                progressBar.Value = 0;
                submitButton.Content = "Recheck Roster";
                submitButton.Click -= ResetTextBox;
                submitButton.Click += SubmitButton_Click;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        public static int GetAllSteamIDs(string input)
        {
            string pattern = SteamIDTools.steamID3regex + "|" + SteamIDTools.profileUrlregex + "|"
                + SteamIDTools.steamIDregex + "|" + SteamIDTools.profileCustomUrlregex + "|"
                + SteamIDTools.etf2lProfileUrl + "|" + SteamIDTools.ugcProfileUrl + "|" 
                + SteamIDTools.rglProfileUrl + "|" + SteamIDTools.tf2centerProfileUrl;
            MatchCollection matches = Regex.Matches(input, pattern);
            return matches.Count;
        }
        private async void statusOutput_TextChanged(object sender, EventArgs e)
        {
            if (!statusOutput.Text.Equals(""))
            {
                string input = statusOutput.Text;
                int counter = 0;
                _ = await Task.Run(() => counter = GetAllSteamIDs(input));

                if (counter <= Checker.RATECTRL)
                {
                    foundIDs.Text = counter + " SteamIDs/Profile Urls found";
                    submitButton.IsEnabled = true;
                }
                else
                {
                    foundIDs.Text = "Too many SteamIDs entered (" + counter + "), max " + Checker.RATECTRL + " allowed per request!";
                    submitButton.IsEnabled = false;
                }

            }
        }

        private void statusOutput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Do the same as with changed Text.
            statusOutput_TextChanged(sender, null);
        }

        //Remove any previous formatting from the pasted content.
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            e.DataObject = new DataObject(DataFormats.Text, e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty);
        }

        //This actually got quite a lot, problby putting it into a separate resource later.
        private void Show_Licenses(object sender, RoutedEventArgs e)
        {
            outputFrame.Visibility = Visibility.Hidden;
            statusOutput.Visibility = Visibility.Visible;
            header.Text = "Paste Status Output here:";
            submitButton.Content = "Check Roster";
            progressBar.Value = 0;
            string license = "Fody\n"
            + "Copyright (c) Simon Cropp\n"
            + "\n"
            + "Permission is hereby granted, free of charge, to any person obtaining a copy \n"
            + "of this software and associated documentation files (the \"Software\"), to deal \n"
            + "in the Software without restriction, including without limitation the rights \n"
            + "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell \n"
            + "copies of the Software, and to permit persons to whom the Software is \n"
            + "furnished to do so, subject to the following conditions:\n"
            + "\n"
            + "The above copyright notice and this permission notice shall be included in \n"
            + "all copies or substantial portions of the Software.\n"
            + "\n"
            + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR \n"
            + "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, \n"
            + "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE \n"
            + "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER \n"
            + "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING \n"
            + "FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS \n"
            + "IN THE SOFTWARE.\n"
            + "\n"
            + "-----------------------------------------------------------------------------\n"
            + "\n"
            + "Costura.Fody\n"
            + "Copyright (c) Simon Cropp, Cameron MacFarland\n"
            + "\n"
            + "Permission is hereby granted, free of charge, to any person obtaining a copy \n"
            + "of this software and associated documentation files (the \"Software\"), to deal \n"
            + "in the Software without restriction, including without limitation the rights \n"
            + "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell \n"
            + "copies of the Software, and to permit persons to whom the Software is \n"
            + "furnished to do so, subject to the following conditions:\n"
            + "\n"
            + "The above copyright notice and this permission notice shall be included in \n"
            + "all copies or substantial portions of the Software.\n"
            + "\n"
            + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR \n"
            + "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, \n"
            + "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE \n"
            + "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER \n"
            + "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING \n"
            + "FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS \n"
            + "IN THE SOFTWARE.\n"
            + "\n"
            + "-----------------------------------------------------------------------------\n"
            + "\n"
            + "Html Agility Pack (HAP)\n"
            + "Copyright (c) ZZZ Projects, Simon Mourrier, Jeff Klawiter, Stephan Grell\n"
            + "\n"
            + "Permission is hereby granted, free of charge, to any person obtaining a copy \n"
            + "of this software and associated documentation files (the \"Software\"), to deal \n"
            + "in the Software without restriction, including without limitation the rights \n"
            + "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell \n"
            + "copies of the Software, and to permit persons to whom the Software is \n"
            + "furnished to do so, subject to the following conditions:\n"
            + "\n"
            + "The above copyright notice and this permission notice shall be included in \n"
            + "all copies or substantial portions of the Software.\n"
            + "\n"
            + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR \n"
            + "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, \n"
            + "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE \n"
            + "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER \n"
            + "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING \n"
            + "FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS \n"
            + "IN THE SOFTWARE.\n"
            + "\n"
            + "-----------------------------------------------------------------------------\n"
            + "\n"
            + "Newtonsoft.Json\n"
            + "Copyright (c) James Newton-King\n"
            + "\n"
            + "Permission is hereby granted, free of charge, to any person obtaining a copy \n"
            + "of this software and associated documentation files (the \"Software\"), to deal \n"
            + "in the Software without restriction, including without limitation the rights \n"
            + "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell \n"
            + "copies of the Software, and to permit persons to whom the Software is \n"
            + "furnished to do so, subject to the following conditions:\n"
            + "\n"
            + "The above copyright notice and this permission notice shall be included in \n"
            + "all copies or substantial portions of the Software.\n"
            + "\n"
            + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR \n"
            + "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, \n"
            + "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE \n"
            + "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER \n"
            + "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING \n"
            + "FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS \n"
            + "IN THE SOFTWARE.\n"
            + "\n"
            + "-----------------------------------------------------------------------------\n"
            + "\n"
            + "AvalonEdit\n"
            + "Copyright (c) AvalonEdit Contributors\n"
            + "\n"
            + "Permission is hereby granted, free of charge, to any person obtaining a copy\n"
            + "of this software and associated documentation files (the \"Software\"), to deal\n"
            + "in the Software without restriction, including without limitation the rights\n"
            + "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell\n"
            + "copies of the Software, and to permit persons to whom the Software is\n"
            + "furnished to do so, subject to the following conditions:\n"
            + "\n"
            + "The above copyright notice and this permission notice shall be included in all\n"
            + "copies or substantial portions of the Software.\n"
            + "\n"
            + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\n"
            + "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\n"
            + "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\n"
            + "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\n"
            + "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\n"
            + "OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\n"
            + "SOFTWARE.\n"
            + "-----------------------------------------------------------------------------\n"
            + "\n"
            + "TF2CompRosterChecker\n"
            + "https://github.com/alekny/TF2CompRosterChecker\n"
            + "\n"
            + "MIT License\n"
            + "\n"
            + "Copyright (c) alekny 2023\n"
            + "\n"
            + "Permission is hereby granted, free of charge, to any person obtaining a copy\n"
            + "of this software and associated documentation files (the \"Software\"), to deal\n"
            + "in the Software without restriction, including without limitation the rights\n"
            + "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell\n"
            + "copies of the Software, and to permit persons to whom the Software is\n"
            + "furnished to do so, subject to the following conditions:\n"
            + "\n"
            + "The above copyright notice and this permission notice shall be included in all\n"
            + "copies or substantial portions of the Software.\n"
            + "\n"
            + "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\n"
            + "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\n"
            + "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\n"
            + "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\n"
            + "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\n"
            + "OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\n"
            + "SOFTWARE.\n";

            statusOutput.Text = license;
            submitButton.Content = "Reset";

            //From this point on we cannot recheck anymore, since we are overwriting the
            //statusOutput content. This is a corner-case i might fix at some point, but rn
            //it doesn't affect the intended recheck functionality.
            checkCompleted = false;
            MenuSave.IsEnabled = false;
            MenuJson.IsEnabled = false;

            //In case the event-handler already got set by the finished check.
            submitButton.Click -= ResetTextBox;

            submitButton.Click -= SubmitButton_Click;
            submitButton.Click += ResetTextBox;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OpenGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/alekny/TF2CompRosterChecker");
        }

        private static string Sha256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", "");
                return hash;
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (this.result != null && this.result.Count > 0)
            {
                try
                {
                    string tempFilePath = Path.GetTempPath() + "Report_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + ".txt";
                    string output = "";
                    string bantable = "";
                    int maxwidthname = "Name".Length, maxwidthleagueid = "LeagueId/ProfileId".Length, maxwidthsteamid3 = "SteamId3".Length, maxwidthteam = "Team".Length,
                        maxwidthteamid = "TeamId".Length, maxwidthdiv = "Divsion".Length;
                    bool bansfound = false;

                    //We have to iterate twice anyway, so use this to find the max lengths for each column.
                    //Check for bans as well.
                    foreach (Player player in this.result)
                    {
                        maxwidthname = (player.Name.Length > maxwidthname) ? player.Name.Length : maxwidthname;
                        maxwidthleagueid = (player.Leagueid.Length > maxwidthleagueid) ? player.Leagueid.Length : maxwidthleagueid;
                        maxwidthsteamid3 = (player.Steamid3.Length > maxwidthsteamid3) ? player.Steamid3.Length : maxwidthsteamid3;
                        maxwidthteam = (player.Team.Length > maxwidthteam) ? player.Team.Length : maxwidthteam;
                        maxwidthteamid = (player.Teamid.Length > maxwidthteamid) ? player.Teamid.Length : maxwidthteamid;
                        maxwidthdiv = (player.Div.Length > maxwidthdiv) ? player.Div.Length : maxwidthdiv;
                        if (player.HasBans)
                        {
                            bansfound = true;
                        }
                    }

                    string tableformat = "{0,-" + (maxwidthname + 3) + "}{1,-" + (maxwidthleagueid + 3) + "}{2,-" + (maxwidthsteamid3 + 3)
                        + "}{3,-" + (maxwidthteam + 3) + "}{4,-" + (maxwidthteamid + 3) + "}{5,-" + (maxwidthdiv + 3) + "}{6,-9}";


                    output += ".: Query Report of " + this.league + " / " + Checker.FormatToString(this.checkedFormat) + " at "
                        + this.completedAt.ToString("dd.MM.yyyy HH:mm:ss ('UTC'zzz)") + " :.\n";

                    if (this.result.Count == 1)
                    {
                        output += "Showing results for 1 player:\n\n";
                    }
                    else
                    {
                        output += "Showing results for " + this.result.Count + " players:\n\n";
                    }
                    
                    if (bansfound)
                    {
                        output += "!! Warning: Players in this list have active or past bans on record !!\n\n";
                    }

                    output += String.Format(tableformat, "Name", "LeagueId/ProfileId", "SteamId3", "Team", "TeamId", "Division", "Bans?") + "\n";
                    for (int i = 0; i <= maxwidthname + maxwidthleagueid + maxwidthsteamid3 + maxwidthteam + maxwidthteamid + maxwidthdiv + 23; i++)
                    {
                        output += "-";
                    }

                    output += "\n";

                    foreach (Player player in this.result)
                    {
                        output += String.Format(tableformat, player.Name, player.Leagueid, player.Steamid3, player.Team, player.Teamid, player.Div,
                            player.HasBans ? "!" : "") + "\n";
                        if (player.HasBans && !"UGC".Equals(this.league))
                        {
                            bantable += "\n";
                            bantable += "Bans for player \"" + player.Name + "\":\n";
                            foreach (Ban ban in player.Bans)
                            {
                                if (ban.End.Equals("2147483647") || ban.End.Equals("2145826800"))
                                {
                                    bantable += "* " + UnixTimeStampToDateTime(ban.Start).ToString("dd.MM.yyyy") + " - "
                                    + "Permanent" + ", Reason: " + ban.Reason;
                                }
                                else
                                {
                                    bantable += "* " + UnixTimeStampToDateTime(ban.Start).ToString("dd.MM.yyyy") + " - "
                                    + UnixTimeStampToDateTime(ban.End).ToString("dd.MM.yyyy") + ", Reason: " + ban.Reason;
                                }
                                bantable += "\n";
                            }
                        }
                    }

                    if (bansfound && !"UGC".Equals(this.league))
                    {
                        output += "\n\nFound Bans:\n";
                        output += bantable;
                    }

                    string copy = String.Copy(output);
                    string hash = Sha256Hash(copy);

                    //Now this is no pgp signature, just simple and designed to be tedious to do by hand ;>
                    while (copy.Length > 7)
                    {
                        copy = copy.Substring(7);
                        hash = Sha256Hash(hash + copy);
                    }

                    output += "\n\n" + hash;

                    File.WriteAllText(tempFilePath, output);
                    Process.Start("notepad.exe", tempFilePath);
                }
                catch(Exception ex) 
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void VerifyReport_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.Title = "Open report file";

            bool? dialogResultesult = openFileDialog.ShowDialog();
            if (dialogResultesult == true)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.Length < 102400)
                    {
                        string input = File.ReadAllText(fileName);

                        if(input.Length > 66)
                        {
                            string hash = input.Split('\n').Last();
                            string data = input.Substring(0, input.Length - 66);

                            string newhash = Sha256Hash(data);

                            while (data.Length > 7)
                            {
                                data = data.Substring(7);
                                newhash = Sha256Hash(newhash + data);
                            }

                            if (newhash.Equals(hash))
                            {
                                MessageBox.Show("The report was successfully verified", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Could not verify the given report", "Failed!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The given file is too small!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The given file is too big!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.Title = "Save json file";

            bool? dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    File.WriteAllText(fileName, JsonConvert.SerializeObject(this.result, Newtonsoft.Json.Formatting.Indented));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
