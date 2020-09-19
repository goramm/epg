using EPGWPF.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EPGWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EPG epg;
        public MainWindow()
        {
            InitializeComponent();
            LoadChannels();
            LoadCategories();
        }

        private void ChannelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeProgrammes();
        }

        private void InfoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDescription();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchChannels();
        }


        private void ChannelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddChannels();
        }
        
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadChannels();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Save();
            base.OnClosing(e);
        }

        private void LoadChannels()
        {
            epg = XmlParser.Load(UrlTextBox.Text);
            AddChannels();
        }

        private void LoadCategories()
        {
            ChannelComboBox.Items.Add("All");
            foreach (Category category in epg.Categories)
            {
                ChannelComboBox.Items.Add(category.Name);
            }
            if (ChannelComboBox.Items.Count > 0)
            {
                ChannelComboBox.SelectedIndex = 0;
            }
        }

        private void AddChannels()
        {
            ChannelListBox.Items.Clear();
            InfoListBox.Items.Clear();
            DescriptionTextBox.Text = "";

            List<Channel> channels = epg.Channels;
            if (SearchTextBox.Text.Length > 0)
            {
                channels = channels.Where(c => c.Name.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
            }

            object category = ChannelComboBox.SelectedValue;
            if (category != null && category.ToString() != "All")
            {
                channels = channels.Where(c => c.Category == category.ToString()).ToList();
            }

            foreach (Channel channel in channels)
            {
                ChannelListBox.Items.Add(new ListBoxItem { Content = channel, Tag = channel });
            }

            if (ChannelListBox.Items.Count > 0)
            {
                ChannelListBox.SelectedIndex = 0;
            }
        }

        private void ChangeProgrammes()
        {
            if (ChannelListBox.SelectedItem == null) return;
            InfoListBox.Items.Clear();

            ListBoxItem listItem = ChannelListBox.SelectedItem as ListBoxItem;
            Channel channel = listItem.Tag as Channel;
            int day = -1;
            int selectedIndex = -1;
            foreach (Programme programme in channel.Programmes)
            {
                if (day != programme.Start.Day)
                {
                    InfoListBox.Items.Add(new ListBoxItem { Content = $"{programme.Start:dd.MM.yyyy}", Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)), Foreground = Brushes.Black, IsEnabled = false });
                }
                if (DateTime.Now > programme.Start && DateTime.Now < programme.End)
                {
                    selectedIndex = InfoListBox.Items.Count;
                    InfoListBox.Items.Add(new ListBoxItem { Content = programme, ToolTip = programme.Title, Tag = programme, IsSelected = true, FontWeight = FontWeight.FromOpenTypeWeight(700) });
                }
                else
                {
                    InfoListBox.Items.Add(new ListBoxItem { Content = programme, ToolTip = programme.Title, Tag = programme });
                }

                day = programme.Start.Day;
            }

            if (selectedIndex > -1)
            {
                var item = InfoListBox.Items[selectedIndex];
                InfoListBox.ScrollIntoView(InfoListBox.Items[InfoListBox.Items.Count - 1]);
                UpdateLayout();
                InfoListBox.ScrollIntoView(item);
            }

        }

        private void SearchChannels()
        {
            AddChannels();
        }

        private void ChangeDescription()
        {
            if (InfoListBox.SelectedItem != null)
            {
                ListBoxItem item = InfoListBox.SelectedItem as ListBoxItem;
                Programme programme = item.Tag as Programme;
                DescriptionTextBox.Text = String.IsNullOrWhiteSpace(programme.Description) ? "No description" : programme.Description;
            }
        }
    }
}
