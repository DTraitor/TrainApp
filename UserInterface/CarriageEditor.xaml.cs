using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLogic;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace UserInterface;

public partial class CarriageEditor : Window
{
    public CarriageEditor(IEnumerable<Train.Carriage> trainCarriages, OnCarriageChanged onCarriageChanged)
    {
        InitializeComponent();
        carriages = trainCarriages;
        carriageChanged += onCarriageChanged;
        UpdateList();
    }

    private void UpdateList()
    {
        CarriagesList.Children.Clear();
        CarriagesList.RowDefinitions.Clear();
        foreach (Train.Carriage carriage in carriages)
        {
            CarriagesList.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            IntegerUpDown numberBox = new IntegerUpDown
            {
                Value = carriage.Number,
                VerticalAlignment = VerticalAlignment.Center,
            };
            numberBox.ValueChanged += (sender, args) =>
            {
                carriageChanged.Invoke(carriage.Id, numberBox.Value.Value, carriage.Capacity);
            };
            IntegerUpDown capacityBox = new IntegerUpDown
            {
                Value = carriage.Capacity,
                Margin = new Thickness(2,0,2,0),
                VerticalAlignment = VerticalAlignment.Center,
            };
            capacityBox.ValueChanged += (sender, args) =>
            {
                carriageChanged.Invoke(carriage.Id, carriage.Number, capacityBox.Value.Value);
            };
            TextBlock freeSeats = new TextBlock
            {
                Text = "Free: " + carriage.Free,
                VerticalAlignment = VerticalAlignment.Center,
            };
            TextBlock reservedSeats = new TextBlock
            {
                Text = "Reserved: " + carriage.Reserved,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Button deleteButton = new Button
            {
                Content = "Delete",
                VerticalAlignment = VerticalAlignment.Center,
            };
            deleteButton.Click += (sender, args) =>
            {
                carriageChanged.Invoke(carriage.Id, carriage.Number, -1);
            };

            int row = CarriagesList.RowDefinitions.Count - 1;
            Grid.SetRow(numberBox, row);
            Grid.SetColumn(numberBox, 0);
            Grid.SetRow(capacityBox, row);
            Grid.SetColumn(capacityBox, 1);
            Grid.SetRow(freeSeats, row);
            Grid.SetColumn(freeSeats, 2);
            Grid.SetRow(reservedSeats, row);
            Grid.SetColumn(reservedSeats, 3);
            Grid.SetRow(deleteButton, row);
            Grid.SetColumn(deleteButton, 4);
            CarriagesList.Children.Add(numberBox);
            CarriagesList.Children.Add(capacityBox);
            CarriagesList.Children.Add(freeSeats);
            CarriagesList.Children.Add(reservedSeats);
            CarriagesList.Children.Add(deleteButton);
        }
    }

    private void OnAddNewCarriage(object sender, RoutedEventArgs e)
    {
        if (CarriageNumber.Value == null)
        {
            MessageBox.Show("Please enter a number for the carriage.");
            return;
        }
        if (MaxSeats.Value == null)
        {
            MessageBox.Show("Please enter a number for the maximum capacity.");
            return;
        }
        carriageChanged.Invoke(Guid.Empty, CarriageNumber.Value.Value, MaxSeats.Value.Value);
        UpdateList();
    }

    public delegate void OnCarriageChanged(Guid id, int carriageNumber, int maxCapacity);
    private OnCarriageChanged carriageChanged;
    private IEnumerable<Train.Carriage> carriages;
}