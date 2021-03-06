﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using VirusTotalNET.Results;
using System.Collections.Generic;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Net;
using System.Windows.Controls;

namespace VTHelper
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DomainReport domainReport;
        private IPReport ipReport;
        private UrlReport urlReport;
        private FileReport fileReport;
        private static BitmapImage safeIcon;
        private static BitmapImage warningIcon;
        private static BitmapImage dangerIcon;

        public MainWindow()
        {
            InitializeComponent();

            safeIcon = new BitmapImage();
            safeIcon.BeginInit();
            safeIcon.CacheOption = BitmapCacheOption.None;
            safeIcon.CacheOption = BitmapCacheOption.OnLoad;
            safeIcon.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            safeIcon.UriSource = new Uri("Resources/256_safe.png", UriKind.Relative);
            safeIcon.EndInit();

            dangerIcon = new BitmapImage();
            dangerIcon.BeginInit();
            dangerIcon.CacheOption = BitmapCacheOption.None;
            dangerIcon.CacheOption = BitmapCacheOption.OnLoad;
            dangerIcon.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            dangerIcon.UriSource = new Uri("Resources/256_danger.png", UriKind.Relative);
            dangerIcon.EndInit();

            warningIcon = new BitmapImage();
            warningIcon.BeginInit();
            warningIcon.CacheOption = BitmapCacheOption.None;
            warningIcon.CacheOption = BitmapCacheOption.OnLoad;
            warningIcon.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            warningIcon.UriSource = new Uri("Resources/256_warning.png", UriKind.Relative);
            warningIcon.EndInit();
        }
        const string urlScanLinkStart = "https://www.virustotal.com/#/url/";
        const string fileScanLinkStart = "https://www.virustotal.com/#/file/";
        const string urlScanLinkEnd = "/detection";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        private async void ParseDomainReportAsync(string domain)
        {
            var converter = new ImageSourceConverter();
            domainReport = await App.GetDomainReportAsync(domain);
            
            ForcePointDomainCat_Lbl.Content = domainReport.ForcePointThreatSeekerCategory;
            AlexaDomainInfo_Lbl.Content = domainReport.AlexaDomainInfo;
            WHOIS_Lbl.Text = domainReport.WhoIs;
            string subdomains = String.Join("\n", domainReport.SubDomains.ToArray());
            Subdomains_Lbl.Content = subdomains;
            WebutationDomainInfo_Lbl.Content = domainReport.WebutationDomainInfo.Verdict + "[Score: " + domainReport.WebutationDomainInfo.SafetyScore + "]";
            TrendMicroCategory_Lbl.Content = domainReport.TrendMicroCategory;
            OperaDomainInfo_Lbl.Content = domainReport.OperaDomainInfo;
            DrWebCategory_Lbl.Content = domainReport.DrWebCategory;
            BitDefenderDomainInfo_Lbl.Content = domainReport.BitDefenderDomainInfo;

            DomainScorePanel.Visibility = Visibility.Visible;
            DomainDetails_Stack.Visibility = Visibility.Visible;
            DomainBigPicture.Visibility = Visibility.Collapsed;

            if (domainReport.Resolutions.Count > 0)
            {
                for (int i = 0; i < domainReport.Resolutions.Count; i++) {
                    Resolutions_Lbl.Content += domainReport.Resolutions[i].IPAddress + " (" + domainReport.Resolutions[i].LastResolved + ")" + "\n";
                }
            }

            if (domainReport.WebutationDomainInfo.SafetyScore > 80)
            {
                DomainScorePanel.Background = System.Windows.Media.Brushes.LightGreen;
                DomainScore_Lbl.Content = "(Safe)";
                DomainScore_Icon.Source = safeIcon;
            }
            else if (domainReport.WebutationDomainInfo.SafetyScore > 60)
            {
                DomainScorePanel.Background = System.Windows.Media.Brushes.Orange;
                DomainScore_Lbl.Content = "(Be carefull)";
                DomainScore_Icon.Source = warningIcon;
            } else {
                DomainScorePanel.Background = System.Windows.Media.Brushes.Red;
                DomainScore_Lbl.Content = "(Dangerous)";
                DomainScore_Icon.Source = dangerIcon;
            }

            if (domainReport.DetectedUrls.Count > 0)
            {
                DomainReportURLDetectedPositives_Lbl.Content = domainReport.DetectedUrls[0].Positives;
                DomainReportURLDetectedTotalEngines_Lbl.Content = domainReport.DetectedUrls[0].Total;
                DomainReportURLDetectedDate_Lbl.Content = domainReport.DetectedUrls[0].ScanDate;
                DomainReportURLDetectedURL_Lbl.Content = domainReport.DetectedUrls[0].Url;
                DomainRowDetectedURL.Height = new GridLength(1, GridUnitType.Auto);
            } else {
                DomainRowDetectedURL.Height = new GridLength(0, GridUnitType.Pixel);
            }

            if (domainReport.DetectedDownloadedSamples.Count > 0)
            {
                DomainReportDownloadSamplesPosisives_Lbl.Content = domainReport.DetectedDownloadedSamples[0].Positives;
                DomainReportDownloadSamplesTotal_Lbl.Content = domainReport.DetectedDownloadedSamples[0].Total;
                DomainReportDownloadSamplesDate_Lbl.Content = domainReport.DetectedDownloadedSamples[0].Date;
                DomainRowDetectedDownload.Height = new GridLength(1, GridUnitType.Auto);
            } else {
                DomainRowDetectedDownload.Height = new GridLength(0, GridUnitType.Pixel);
            }

            if(domainReport.UndetectedUrls.Count > 0)
            {
                DomainReportURLUndetectedPositives_Lbl.Content = domainReport.UndetectedUrls[0][2];
                DomainReportURLUndetectedTotalEngines_Lbl.Content = domainReport.UndetectedUrls[0][3];
                DomainReportURLUndetectedDate_Lbl.Content = domainReport.UndetectedUrls[0][4];
                DomainRowUndetectedURL.Height = new GridLength(1, GridUnitType.Auto);
            } else {
                DomainRowDetectedDownload.Height = new GridLength(0, GridUnitType.Pixel);
            }

            if(domainReport.UndetectedDownloadedSamples.Count > 0)
            {
                DomainReportUndetectedDownloadSamplesPosisives_Lbl.Content = domainReport.UndetectedDownloadedSamples[0].Positives;
                DomainReportUndetectedDownloadSamplesTotal_Lbl.Content = domainReport.UndetectedDownloadedSamples[0].Total;
                DomainReportUndetectedDownloadSamplesDate_Lbl.Content = domainReport.UndetectedDownloadedSamples[0].Date;
                DomainRowUndetectedDownload.Height = new GridLength(1, GridUnitType.Auto);
            } else {
                DomainRowUndetectedDownload.Height = new GridLength(0, GridUnitType.Pixel);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        private async void ScanDomainAsync(string domain)
        {
            UrlScanResult scanResult = await App.ScanDomainAsync(domain);
            DomainScanPermlink_Lbl.Content = scanResult.Permalink;
            if(scanResult.ResponseCode.ToString() == "Queued")
            {
                DomainPermlinkPanel.Height = 32;
                DomainPermlinkPanel.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        private async void ScanURLAsync(string url)
        {
            UrlReport urlReport = await App.GetUrlReportAsync(url);
        }
        /// <summary>
        /// URL report
        /// </summary>
        /// <param name="url"></param>
        private async void ParseURLReportAsync(string url)
        {
            URLScan_Grid.Children.RemoveRange(3, URLScan_Grid.Children.Count - 1);
            urlReport = await App.GetUrlReportAsync(url);

            URLDetails_Stack.Visibility = Visibility.Visible;
            URLScorePanel.Visibility = Visibility.Visible;
            URLBigPicture.Visibility = Visibility.Collapsed;

            URLReportURLDetectedPositives_Lbl.Content = urlReport.Positives;
            URLReportURLDetectedTotalEngines_Lbl.Content = urlReport.Total;
            URLReportURLDetectedDate_Lbl.Content = urlReport.ScanDate;

            if (urlReport.Positives < 3)
            {
                URLScorePanel.Background = System.Windows.Media.Brushes.LightGreen;
                URLScore_Lbl.Content = "(Safe)";
                URLScore_Icon.Source = safeIcon;
            }
            else if (urlReport.Positives < 5)
            {
                URLScorePanel.Background = System.Windows.Media.Brushes.Orange;
                URLScore_Lbl.Content = "(Be carefull)";
                URLScore_Icon.Source = warningIcon;
            }
            else
            {
                URLScorePanel.Background = System.Windows.Media.Brushes.Red;
                URLScore_Lbl.Content = "(Dangerous)";
                URLScore_Icon.Source = dangerIcon;
            }

            int row = 2;
            foreach (var item in urlReport.Scans)
            {
                if (item.Value.Detected)
                {
                    System.Windows.Controls.Label name = new System.Windows.Controls.Label();
                    System.Windows.Controls.Label virus = new System.Windows.Controls.Label();
                    System.Windows.Controls.Label detail = new System.Windows.Controls.Label();

                    name.Content = item.Key;
                    virus.Content = item.Value.Result;
                    detail.Content = item.Value.Detail;

                    name.SetValue(Grid.RowProperty, row);
                    name.SetValue(Grid.ColumnProperty, 0);

                    virus.SetValue(Grid.RowProperty, row);
                    virus.SetValue(Grid.ColumnProperty, 1);

                    detail.SetValue(Grid.RowProperty, row);
                    detail.SetValue(Grid.ColumnProperty, 2);

                    URLScan_Grid.Children.Add(name);
                    URLScan_Grid.Children.Add(virus);
                    URLScan_Grid.Children.Add(detail);

                    ++row;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private async void ScanFileAsync(string filePath)
        {
            ScanResult scanResult = await App.ScanFileAsync(filePath);
            FileReportMD5_Lbl.Content = scanResult.MD5;
            FileReportSHA256_Lbl.Content = scanResult.SHA256;
            FileReportSHA1_Lbl.Content = scanResult.SHA1;
            FileScanPermlink_Lbl.Content = scanResult.Permalink;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private async void ParseFileReportAsync(string filePath)
        {
            fileReport = await App.GetFileReportAsync(filePath);
            FileReportMD5_Lbl.Content = fileReport.MD5;
            FileReportSHA256_Lbl.Content = fileReport.SHA256;
            FileReportSHA1_Lbl.Content = fileReport.SHA1;

            FileDetails_Stack.Visibility = Visibility.Visible;
            FileScorePanel.Visibility = Visibility.Visible;
            FileBigPicture.Visibility = Visibility.Collapsed;

            FileReportDate_Lbl.Content = fileReport.ScanDate;
            FileReportPositives_Lbl.Content = fileReport.Positives;
            FileReportTotalEngines_Lbl.Content = fileReport.Total;

            if (fileReport.Positives < 3)
            {
                FileScorePanel.Background = System.Windows.Media.Brushes.LightGreen;
                FileScore_Lbl.Content = "(Safe)";
                FileScore_Icon.Source = safeIcon;
            }
            else if (fileReport.Positives < 5)
            {
                FileScorePanel.Background = System.Windows.Media.Brushes.Orange;
                FileScore_Lbl.Content = "(Be carefull)";
                FileScore_Icon.Source = warningIcon;
            }
            else
            {
                FileScorePanel.Background = System.Windows.Media.Brushes.Red;
                FileScore_Lbl.Content = "(Dangerous)";
                FileScore_Icon.Source = dangerIcon;
            }

            int row = 2;
            foreach (var item in fileReport.Scans)
            {
                if (item.Value.Detected)
                {
                    System.Windows.Controls.Label name = new System.Windows.Controls.Label();
                    System.Windows.Controls.Label virus = new System.Windows.Controls.Label();
                    System.Windows.Controls.Label update = new System.Windows.Controls.Label();
                    System.Windows.Controls.Label version = new System.Windows.Controls.Label();

                    name.Content = item.Key;
                    virus.Content = item.Value.Result;
                    update.Content = item.Value.Update;
                    version.Content = item.Value.Version;

                    name.SetValue(Grid.RowProperty, row);
                    name.SetValue(Grid.ColumnProperty, 0);

                    virus.SetValue(Grid.RowProperty, row);
                    virus.SetValue(Grid.ColumnProperty, 1);

                    update.SetValue(Grid.RowProperty, row);
                    update.SetValue(Grid.ColumnProperty, 2);

                    version.SetValue(Grid.RowProperty, row);
                    version.SetValue(Grid.ColumnProperty, 3);

                    FileReport_Grid.Children.Add(name);
                    FileReport_Grid.Children.Add(virus);
                    FileReport_Grid.Children.Add(update);
                    FileReport_Grid.Children.Add(version);

                    ++row;
                }
            }
        }
        public bool IsValidIPAddress(string addr)
        {
            IPAddress address;
            bool valid = IPAddress.TryParse(addr, out address);
            return valid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        private async void ParseIPReportAsync(string ip)
        {
            bool valid = IsValidIPAddress(ip);
            if (valid == false)
            {
                MessageBoxResult ipInvalidError = MessageBox.Show("Invalid IP address !",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                return;
            }

            ipReport = await App.GetIPReportAsync(ip);
            IPCountry_Lbl.Content = ipReport.Country;
            IPOwner_Lbl.Content = ipReport.AsOwner;

            if (ipReport.Resolutions != null && ipReport.Resolutions.Count > 0)
            {
                for (int i = 0; i < ipReport.Resolutions.Count; i++)
                {
                    IPResolutions_Lbl.Content += ipReport.Resolutions[i].Hostname + " (" + ipReport.Resolutions[i].LastResolved + ")" + "\n";
                }
            }

            int detectedURLPositives = 0;
            if (ipReport.DetectedUrls != null && ipReport.DetectedUrls.Count > 0)
            {
                detectedURLPositives = Convert.ToInt32(ipReport.DetectedUrls[0].Positives);
                IPReportURLDetectedPositives_Lbl.Content = ipReport.DetectedUrls[0].Positives;
                IPReportURLDetectedTotalEngines_Lbl.Content = ipReport.DetectedUrls[0].Total;
                IPReportURLDetectedDate_Lbl.Content = ipReport.DetectedUrls[0].ScanDate;
                IPReportURLDetectedURL_Lbl.Content = ipReport.DetectedUrls[0].Url;
            }

            int detectedDownloadPositives = 0;
            if (ipReport.DetectedDownloadedSamples != null && ipReport.DetectedDownloadedSamples.Count > 0)
            {
                detectedDownloadPositives = Convert.ToInt32(ipReport.DetectedDownloadedSamples[0].Positives);
                IPReportDownloadSamplesPosisives_Lbl.Content = ipReport.DetectedDownloadedSamples[0].Positives;
                IPReportDownloadSamplesTotal_Lbl.Content = ipReport.DetectedDownloadedSamples[0].Total;
                IPReportDownloadSamplesDate_Lbl.Content = ipReport.DetectedDownloadedSamples[0].Date;
            }

            if (ipReport.UndetectedCommunicatingSamples != null && ipReport.UndetectedDownloadedSamples.Count > 0)
            {
                IPReportUndetectedDownloadSamplesTotal_Lbl.Content = ipReport.UndetectedDownloadedSamples[0].Total;
                IPReportUndetectedDownloadSamplesPosisives_Lbl.Content = ipReport.UndetectedDownloadedSamples[0].Positives;
                IPReportUndetectedDownloadSamplesDate_Lbl.Content = ipReport.UndetectedDownloadedSamples[0].Date;
            }

            
            if (ipReport.UndetectedUrls != null && ipReport.UndetectedUrls.Count > 0)
            {
                IPReportURLUndetectedPositives_Lbl.Content = ipReport.UndetectedUrls[0][2];
                IPReportURLUndetectedTotalEngines_Lbl.Content = ipReport.UndetectedUrls[0][3];
                IPReportURLUndetectedDate_Lbl.Content = ipReport.UndetectedUrls[0][4];
            }

            if (detectedURLPositives < 5 && detectedDownloadPositives < 5)
            {
                IPScorePanel.Background = System.Windows.Media.Brushes.LightGreen;
                IPScore_Lbl.Content = "(Safe)";
                IPScore_Icon.Source = safeIcon;
            }
            else if (detectedURLPositives < 10 && detectedDownloadPositives < 10)
            {
                IPScorePanel.Background = System.Windows.Media.Brushes.Orange;
                IPScore_Lbl.Content = "(Be careful)";
                IPScore_Icon.Source = warningIcon;
            }
            else
            {
                IPScorePanel.Background = System.Windows.Media.Brushes.Red;
                IPScore_Lbl.Content = "(Dangerous)";
                IPScore_Icon.Source = dangerIcon;
            }

            IPDetails_Stack.Visibility = Visibility.Visible;
            IPScorePanel.Visibility = Visibility.Visible;
            IPBigPicture.Visibility = Visibility.Collapsed;
        }
        
        private void GetDomainReportBtn_Click(object sender, RoutedEventArgs e)
        {
            ParseDomainReportAsync(DomainName_TextBox.Text);
        }
        
        private void ScanDomainBtn_Click(object sender, RoutedEventArgs e)
        {
            ScanDomainAsync(DomainName_TextBox.Text);
        }

        private void GetURLReportBtn_Click(object sender, RoutedEventArgs e)
        {
            ParseURLReportAsync(URL_TextBox.Text);
        }

        private void ScanURLBtn_Click(object sender, RoutedEventArgs e)
        {
            ScanURLAsync(URL_TextBox.Text);
        }

        private void ScanFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ScanFileAsync(FileName_TextBox.Text);
        }

        private void GetFileReportBtn_Click(object sender, RoutedEventArgs e)
        {
            ParseFileReportAsync(FileName_TextBox.Text);
        }

        private void GetIPReportBtn_Click(object sender, RoutedEventArgs e)
        {
            ParseIPReportAsync(IP_TextBox.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                string sha256 = VirusTotalNET.Helpers.HashHelper.GetSHA256(fileInfo);
                FileName_TextBox.Text = fileInfo.FullName;
            }
        }


        private void showTab(System.Windows.Controls.StackPanel panel)
        {
            ScanFile_Panel.Visibility = Visibility.Hidden;
            ScanDomain_Panel.Visibility = Visibility.Hidden;
            Settings_Panel.Visibility = Visibility.Hidden;
            About_Panel.Visibility = Visibility.Hidden;
            ScanIP_Panel.Visibility = Visibility.Hidden;
            ScanURL_Panel.Visibility = Visibility.Hidden;

            // "pack://siteoforigin:,,,/Resources/big1.jpg"
            // "pack://siteoforigin:,,,/Resources/big2.jpg"
            // "pack://siteoforigin:,,,/Resources/big3.jpg"
            // "pack://siteoforigin:,,,/Resources/big4.jpg"
            panel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanFileTabBtn_Click(object sender, RoutedEventArgs e)
        {
            showTab(ScanFile_Panel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanDomainTabBtn_Click(object sender, RoutedEventArgs e)
        {
            showTab(ScanDomain_Panel);
        }

        private void ScanURLTabBtn_Click(object sender, RoutedEventArgs e)
        {
            showTab(ScanURL_Panel);
        }

        private void SettingsTabBtn_Click(object sender, RoutedEventArgs e)
        {
            showTab(Settings_Panel);
        }

        private void AboutTabBtn_Click(object sender, RoutedEventArgs e)
        {
            showTab(About_Panel);
        }

        private void ScanIPTabBtn_Click(object sender, RoutedEventArgs e)
        {
            showTab(ScanIP_Panel);
        }

        private void DomainReportURLDetectedLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(domainReport.DetectedUrls[0].Url);
        }

        private void DomainReportDownloadSamplesLink_Click(object sender, RoutedEventArgs e)
        {
            if (domainReport.DetectedDownloadedSamples.Count > 0)
            {
                string hash = domainReport.DetectedDownloadedSamples[0].Sha256;
                string link = String.Concat(fileScanLinkStart, hash, urlScanLinkEnd);

                System.Diagnostics.Process.Start(link);
            }
        }

        private void DomainReportURLUndetectedLink_Click(object sender, RoutedEventArgs e)
        {
            string hash = domainReport.UndetectedUrls[0][1];
            string link = String.Concat(urlScanLinkStart, hash, urlScanLinkEnd);

            System.Diagnostics.Process.Start(link);
        }

        private void DomainReportUndetectedDownloadSamplesLink_Click(object sender, RoutedEventArgs e)
        {
            string hash = domainReport.UndetectedDownloadedSamples[0].Sha256;
            string link = String.Concat(fileScanLinkStart, hash, urlScanLinkEnd);

            System.Diagnostics.Process.Start(link);
        }

        private void IPReportDownloadSamplesLink_Click(object sender, RoutedEventArgs e)
        {
            string hash = ipReport.DetectedDownloadedSamples[0].Sha256;
            string link = String.Concat(fileScanLinkStart, hash, urlScanLinkEnd);

            System.Diagnostics.Process.Start(link);
        }

        private void IPReportURLUndetectedLink_Click(object sender, RoutedEventArgs e)
        {
            string hash = ipReport.UndetectedUrls[0][1];
            string link = String.Concat(urlScanLinkStart, hash, urlScanLinkEnd);

            System.Diagnostics.Process.Start(link);
        }

        private void IPReportUndetectedDownloadSamplesLink_Click(object sender, RoutedEventArgs e)
        {
            string hash = ipReport.UndetectedDownloadedSamples[0].Sha256;
            string link = String.Concat(fileScanLinkStart, hash, urlScanLinkEnd);

            System.Diagnostics.Process.Start(link);
        }

        private void FileReportLink_Click(object sender, RoutedEventArgs e)
        {
            string hash = fileReport.SHA256;
            string link = String.Concat(fileScanLinkStart, hash, urlScanLinkEnd);

            System.Diagnostics.Process.Start(link);
        }

        private void URLReportFileScanLink_Click(object sender, RoutedEventArgs e)
        {
            if (urlReport != null)
            {
                string hash = urlReport.FileScanId;
                if (hash != null && hash.Length > 0)
                {
                    var arr = hash.Split('-');
                    string link = String.Concat(fileScanLinkStart, arr[0], urlScanLinkEnd);
                    System.Diagnostics.Process.Start(link);
                }
            }
        }

        private void GithubLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/arturfog/");
        }

        private void URLReportURLDetectedURLLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(urlReport.Permalink);
        }

        private void GetApiKey_Btn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.virustotal.com/#/join-us");
        }
    }
}
