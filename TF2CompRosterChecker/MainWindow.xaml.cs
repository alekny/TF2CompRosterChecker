/**
 * 
 *  Costura.Fody
 *  Copyright (c) Simon Cropp, Cameron MacFarland
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 * -----------------------------------------------------------------------------
 * 
 *  Fody
 *  Copyright (c) Simon Cropp
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 * -----------------------------------------------------------------------------
 * 
 * 
 *  Html Agility Pack (HAP)
 *  Copyright (c)ZZZ Projects, Simon Mourrier, Jeff Klawiter, Stephan Grell
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 * -----------------------------------------------------------------------------
 * 
 *  Newtonsoft.Json
 *  Copyright (c) James Newton-King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TF2CompRosterChecker
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            int marginleft = 0;
            int margintop = 0;
            bool switcher = true;
            string baseUrl = "";
            string baseTeamUrl = "";
            string league = "";
            string statusOutputText = new TextRange(statusOutput.Document.ContentStart, statusOutput.Document.ContentEnd).Text;
            Color color = Colors.White;
            List<Player> result = new List<Player>();

            DisableUI();
            if (leagueSelector.SelectedIndex == 0)
            {
                ETF2LChecker ec = new ETF2LChecker(statusOutputText);
                await Task.Run(() => result = ec.parseJSON(ETF2LChecker.HL, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = ETF2LChecker.baseUrl;
                baseTeamUrl = ETF2LChecker.baseTeamUrl;
                league = "ETF2L";
            }
            else if (leagueSelector.SelectedIndex == 1)
            {
                ETF2LChecker ec = new ETF2LChecker(statusOutputText);
                await Task.Run(() => result = ec.parseJSON(ETF2LChecker.Sixes, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = ETF2LChecker.baseUrl;
                baseTeamUrl = ETF2LChecker.baseTeamUrl;
                league = "ETF2L";
            }
            else if (leagueSelector.SelectedIndex == 2)
            {
                RGLChecker rc = new RGLChecker(statusOutputText);
                await Task.Run(() => result = rc.ParseJSON(RGLChecker.HL, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = RGLChecker.baseUrl;
                baseTeamUrl = RGLChecker.baseTeamUrl;
                league = "RGL";
            }
            else if (leagueSelector.SelectedIndex == 3)
            {
                RGLChecker rc = new RGLChecker(statusOutputText);
                await Task.Run(() => result = rc.ParseJSON(RGLChecker.PL, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = RGLChecker.baseUrl;
                baseTeamUrl = RGLChecker.baseTeamUrl;
                league = "RGL";
            }
            else if (leagueSelector.SelectedIndex == 4)
            {
                RGLChecker rc = new RGLChecker(statusOutputText);
                await Task.Run(() => result = rc.ParseJSON(RGLChecker.TradSixes, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = RGLChecker.baseUrl;
                baseTeamUrl = RGLChecker.baseTeamUrl;
                league = "RGL";
            }
            else if (leagueSelector.SelectedIndex == 5)
            {
                RGLChecker rc = new RGLChecker(statusOutputText);
                await Task.Run(() => result = rc.ParseJSON(RGLChecker.TradSixes, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = RGLChecker.baseUrl;
                baseTeamUrl = RGLChecker.baseTeamUrl;
                league = "RGL";
            }
            else if (leagueSelector.SelectedIndex == 6)
            {
                UGCChecker uc = new UGCChecker(statusOutputText);
                await Task.Run(() => result = uc.parseUGCPlayerPage(UGCChecker.HL, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = UGCChecker.baseUrl;
                baseTeamUrl = UGCChecker.baseTeamUrl;
                league = "UGC";
            }
            else if (leagueSelector.SelectedIndex == 7)
            {
                UGCChecker uc = new UGCChecker(statusOutputText);
                await Task.Run(() => result = uc.parseUGCPlayerPage(UGCChecker.Sixes, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = UGCChecker.baseUrl;
                baseTeamUrl = UGCChecker.baseTeamUrl;
                league = "UGC";
            }
            else if (leagueSelector.SelectedIndex == 8)
            {
                UGCChecker uc = new UGCChecker(statusOutputText);
                await Task.Run(() => result = uc.parseUGCPlayerPage(UGCChecker.FourVeeFour, progressBar, submitButton).OrderBy(o => o.Team).ToList());
                baseUrl = UGCChecker.baseUrl;
                baseTeamUrl = UGCChecker.baseTeamUrl;
                league = "UGC";
            }

            if (result.Any())
            {
                statusOutput.Visibility = Visibility.Hidden;
                foreach (var player in result)
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
                        grid1.Children.Add(TBGen(new Run(player.Name), new Run("[!]"), Brushes.Red, marginleft, margintop, true));
                    }
                    else
                    {
                        grid1.Children.Add(TBGen(player.Name, marginleft, margintop, true));
                    }

                    if (player.Teamid != "")
                    {
                        Hyperlink teamlink = new Hyperlink(new Run("[+]"))
                        {
                            NavigateUri = new Uri(baseTeamUrl + player.Teamid),
                        };
                        teamlink.RequestNavigate += Hyperlink_RequestNavigate;
                        grid2.Children.Add(TBGen(new Run(player.Team), teamlink, marginleft, margintop, false));
                    }
                    else
                    {
                        grid2.Children.Add(TBGen(player.Team, marginleft, margintop, false));
                    }
                    grid3.Children.Add(TBGen(player.Div, marginleft, margintop, false));
                    if (player.Leagueid != "")
                    {
                        Hyperlink leaguelink = new Hyperlink(new Run(league))
                        {
                            NavigateUri = new Uri(baseUrl + player.Leagueid),
                        };
                        leaguelink.RequestNavigate += Hyperlink_RequestNavigate;
                        grid4.Children.Add(TBGen(leaguelink, marginleft, margintop, false));
                    }

                    Hyperlink logslink = new Hyperlink(new Run("Logs"))
                    {
                        NavigateUri = new Uri(SteamIDTools.baseLogsUrl + player.Profileid),
                    };
                    logslink.RequestNavigate += Hyperlink_RequestNavigate;
                    grid5.Children.Add(TBGen(logslink, marginleft, margintop, false));

                    Hyperlink profilelink = new Hyperlink(new Run("Steam"))
                    {
                        NavigateUri = new Uri(SteamIDTools.baseUrl + player.Profileid),
                    };
                    profilelink.RequestNavigate += Hyperlink_RequestNavigate;
                    grid6.Children.Add(TBGen(profilelink, marginleft, margintop, false));


                    outputGrid.Children.Add(grid1);
                    outputGrid.Children.Add(grid2);
                    outputGrid.Children.Add(grid3);
                    outputGrid.Children.Add(grid4);
                    outputGrid.Children.Add(grid5);
                    outputGrid.Children.Add(grid6);

                    counter++;
                }
                statusOutput.Document.Blocks.Clear();
                statusOutput.Document.Blocks.Add(new Paragraph(new Run("")));
                foundIDs.Text = "";
                header.Text = "Results";
                outputFrame.Visibility = Visibility.Visible;
                submitButton.Content = "Reset";
                submitButton.Click -= submitButton_Click;
                submitButton.Click += ResetTextBox;
            }
            else
            {
                statusOutput.Document.Blocks.Clear();
                statusOutput.Document.Blocks.Add(new Paragraph(new Run("No SteamIDs found")));
            }
            EnableUI();
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
                FontSize = 13,
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
            tb.FontSize = 13;
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
            tb.FontSize = 13;
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
            tb.FontSize = 13;
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
            header.Text = "Paste Status Output here:";
            outputGrid.Children.Clear();
            submitButton.Content = "Check Roster";
            progressBar.Value = 0;
            statusOutput.Document.Blocks.Clear();
            submitButton.Click -= ResetTextBox;
            submitButton.Click += submitButton_Click;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        public static IEnumerable<TextRange> GetAllWordRanges(FlowDocument document)
        {
            string pattern = SteamIDTools.steamID3regex + "|" + SteamIDTools.profileUrlregex /*+ "|" + SteamIDTools.profileCustomUrlregex*/;
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    MatchCollection matches = Regex.Matches(textRun, pattern);
                    foreach (Match match in matches)
                    {
                        int startIndex = match.Index;
                        int length = match.Length;
                        TextPointer start = pointer.GetPositionAtOffset(startIndex);
                        TextPointer end = start.GetPositionAtOffset(length);
                        yield return new TextRange(start, end);
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!new TextRange(statusOutput.Document.ContentStart, statusOutput.Document.ContentEnd).Text.Equals(""))
            {
                statusOutput.Foreground = Brushes.Black;
                IEnumerable<TextRange> wordRanges = null;
                await Task.Run(() => wordRanges = GetAllWordRanges(statusOutput.Document));
                if (wordRanges.Any())
                {
                    foreach (TextRange wordRange in wordRanges)
                    {
                        wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
                        //statusOutput.Select(match.Index, match.Length);
                    }
                    foundIDs.Text = wordRanges.Count() + " SteamID3s/Profile Urls found";
                }
            }
        }

        private void Show_Licenses(object sender, RoutedEventArgs e)
        {
            outputFrame.Visibility = Visibility.Hidden;
            statusOutput.Visibility = Visibility.Visible;
            header.Text = "Paste Status Output here:";
            outputGrid.Children.Clear();
            submitButton.Content = "Check Roster";
            progressBar.Value = 0;
            string License = "Fody\n"
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
            +"\n"
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
            + "IN THE SOFTWARE.\n";
            statusOutput.Document.Blocks.Clear();
            statusOutput.Document.Blocks.Add(new Paragraph(new Run(License)));
            submitButton.Content = "Reset";
            submitButton.Click -= submitButton_Click;
            submitButton.Click += ResetTextBox;
        }

    }
}
