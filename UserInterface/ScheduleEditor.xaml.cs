using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLogic;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace UserInterface;

public partial class ScheduleEditor : Window
{
    public ScheduleEditor(IEnumerable<Train.Station> data, OnStationChanged onStationChanged)
    {
        InitializeComponent();
        stationChanged += onStationChanged;
        stations = data;
        UpdateList();
    }

    private void UpdateList()
    {
        StationsList.Children.Clear();
        StationsList.RowDefinitions.Clear();
        foreach (Train.Station station in stations)
        {
            StationsList.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            TextBox nameBox = new TextBox
            {
                Text = station.Name,
                VerticalAlignment = VerticalAlignment.Center,
            };
            nameBox.TextChanged += (sender, args) =>
            {
                stationChanged.Invoke(station.Id, nameBox.Text, station.ArrivalTime);
            };
            TimePicker timePicker = new TimePicker
            {
                Value = station.ArrivalTime,
                Margin = new Thickness(2,0,2,0),
                VerticalAlignment = VerticalAlignment.Center,
            };
            timePicker.ValueChanged += (sender, args) =>
            {
                stationChanged.Invoke(station.Id, station.Name, timePicker.Value.Value);
            };
            Button deleteButton = new Button
            {
                Content = "Delete",
                VerticalAlignment = VerticalAlignment.Center,
            };
            deleteButton.Click += (sender, args) =>
            {
                stationChanged.Invoke(station.Id, station.Name, null);
            };

            int row = StationsList.RowDefinitions.Count - 1;
            Grid.SetRow(nameBox, row);
            Grid.SetColumn(nameBox, 0);
            Grid.SetRow(timePicker, row);
            Grid.SetColumn(timePicker, 1);
            Grid.SetRow(deleteButton, row);
            Grid.SetColumn(deleteButton, 2);

            StationsList.Children.Add(nameBox);
            StationsList.Children.Add(timePicker);
            StationsList.Children.Add(deleteButton);
        }
    }

    private void OnAddNewStation(object sender, RoutedEventArgs e)
    {
        if (NewStationName.Text == "")
        {
            MessageBox.Show("Please enter a name for the station.");
            return;
        }
        if (NewStationTime.Value == null)
        {
            MessageBox.Show("Please choose time of arrival to the station.");
            return;
        }

        stationChanged.Invoke(Guid.Empty, NewStationName.Text, NewStationTime.Value.Value);
        UpdateList();
    }

    public delegate void OnStationChanged(Guid id, string name, DateTime? time);
    private OnStationChanged stationChanged;
    private IEnumerable<Train.Station> stations;
}