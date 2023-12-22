using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLogic;
using Xceed.Wpf.Toolkit;

namespace UserInterface;

public partial class ReservationsWindow : Window
{
    public ReservationsWindow(Reservation reservation, LogicHandler handler)
    {
        InitializeComponent();
        this.reservation = reservation;
        logicHandler = handler;
        ReservationName.Text = reservation.Name;
        TrainName.Text = logicHandler.GetTrain(reservation.TrainId).Name;
        UpdateList();
    }

    private void UpdateList()
    {
        CarriagesList.Children.Clear();
        CarriagesList.RowDefinitions.Clear();
        foreach (Train.Carriage carriage in logicHandler.GetTrain(reservation.TrainId).Carriages)
        {
            CarriagesList.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            TextBlock carriageNumber = new TextBlock
            {
                Text = "Carriage #" + carriage.Number,
                VerticalAlignment = VerticalAlignment.Center,
            };
            TextBlock seatsNumber = new TextBlock
            {
                Text = "Free: " + logicHandler.GetFreeSeats(reservation.TrainId, carriage.Id),
                VerticalAlignment = VerticalAlignment.Center,
            };
            IntegerUpDown seatsCount = new IntegerUpDown
            {
                Value = carriage.Reserved,
                Minimum = 0,
                Maximum = logicHandler.GetFreeSeats(reservation.TrainId, carriage.Id) + carriage.Reserved,
            };
            seatsCount.ValueChanged += (sender, args) =>
            {
                logicHandler.ChangeReservationSeats(reservation.Id, carriage.Id, seatsCount.Value.Value);
                seatsNumber.Text = "Free: " + logicHandler.GetFreeSeats(reservation.TrainId, carriage.Id);
            };
            int row = CarriagesList.RowDefinitions.Count - 1;
            Grid.SetRow(carriageNumber, row);
            Grid.SetColumn(carriageNumber, 0);
            Grid.SetRow(seatsNumber, row);
            Grid.SetColumn(seatsNumber, 1);
            Grid.SetRow(seatsCount, row);
            Grid.SetColumn(seatsCount, 2);
            CarriagesList.Children.Add(carriageNumber);
            CarriagesList.Children.Add(seatsNumber);
            CarriagesList.Children.Add(seatsCount);
        }
    }

    private Reservation reservation;
    private LogicHandler logicHandler;
}