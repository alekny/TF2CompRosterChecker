
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
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
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            int marginleft = 0;
            int margintop = 0;
            bool switcher = true;
            string baseUrl = "";
            string baseTeamUrl = "";
            string league = "";
            string statusOutputText = statusOutput.Text;
            Color color = Colors.White;
            List<Player> result = new List<Player>();

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
                        break;
                    }
                case 1:
                    {
                        ETF2LChecker ec = new ETF2LChecker(statusOutputText);
                        _ = await Task.Run(() => result = ec.ParseData(Checker.LeagueFormat.HL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = ec.BaseUrl;
                        baseTeamUrl = ec.BaseTeamUrl;
                        league = "ETF2L";
                        break;
                    }
                case 2:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.Sixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        break;
                    }
                case 3:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.NRSixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        break;
                    }
                case 4:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.HL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        break;
                    }
                case 5:
                    {
                        RGLChecker rc = new RGLChecker(statusOutputText);
                        _ = await Task.Run(() => result = rc.ParseData(Checker.LeagueFormat.PL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = rc.BaseUrl;
                        baseTeamUrl = rc.BaseTeamUrl;
                        league = "RGL";
                        break;
                    }
                case 6:
                    {
                        UGCChecker uc = new UGCChecker(statusOutputText);
                        _ = await Task.Run(() => result = uc.ParseData(Checker.LeagueFormat.Sixes, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = uc.BaseUrl;
                        baseTeamUrl = uc.BaseTeamUrl;
                        league = "UGC";
                        break;
                    }
                case 7:
                    {
                        UGCChecker uc = new UGCChecker(statusOutputText);
                        _ = await Task.Run(() => result = uc.ParseData(Checker.LeagueFormat.HL, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = uc.BaseUrl;
                        baseTeamUrl = uc.BaseTeamUrl;
                        league = "UGC";
                        break;
                    }
                case 8:
                    {
                        UGCChecker uc = new UGCChecker(statusOutputText);
                        _ = await Task.Run(() => result = uc.ParseData(Checker.LeagueFormat.FourVeeFour, progress).OrderBy(o => o.Team).ThenBy(o => o.Name).ToList());
                        baseUrl = uc.BaseUrl;
                        baseTeamUrl = uc.BaseTeamUrl;
                        league = "UGC";
                        break;
                    }

                default:
                    break;
            }

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
                        //Only ETF2L-Checker will set this to a non-null object yet.
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
                            };
                            popup.Tag = "Show Bans";
                            popup.Foreground = Brushes.Red;
                            popup.ToolTip = "Show Bans";
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
                statusOutput.Text = "";
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
                Placement = PlacementMode.Mouse
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
                FontSize = 14
            };
            _ = sp.Children.Add(popupText);
            foreach (Ban ban in bans)
            {
                TextBlock banline = new TextBlock
                {
                    Foreground = Brushes.Black
                };
                _ = sp.Children.Add(banline);
                //Display Permabans in a better way (currently only used by RGL)
                if (ban.End.Equals("2147483647"))
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        public static int GetAllSteamIDs(string input)
        {
            string pattern = SteamIDTools.steamID3regex + "|" + SteamIDTools.profileUrlregex + "|"
                + SteamIDTools.steamIDregex + "|" + SteamIDTools.profileCustomUrlregex;
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

        private void Show_Licenses(object sender, RoutedEventArgs e)
        {
            outputFrame.Visibility = Visibility.Hidden;
            statusOutput.Visibility = Visibility.Visible;
            header.Text = "Paste Status Output here:";
            outputGrid.Children.Clear();
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
            + "SOFTWARE.\n";

            statusOutput.Text = license;
            submitButton.Content = "Reset";
            submitButton.Click -= SubmitButton_Click;
            submitButton.Click += ResetTextBox;
        }
    }
}
