using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLogic;

namespace UserInterface
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateTrainsList();
            UpdateReservationsList();
        }

        private void UpdateReservationsList()
        {
            ReservationsList.Children.Clear();
            ReservationsList.RowDefinitions.Clear();
            foreach (Reservation reservation in logicHandler.GetReservations())
            {
                ReservationsList.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});

                TextBlock nameBlock = new TextBlock
                {
                    Text = reservation.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                TextBlock trainNameBlock = new TextBlock
                {
                    Text = logicHandler.GetTrain(reservation.TrainId).Name,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                int totalReservedSeats = 0;
                foreach (var (carriageId, seats) in reservation.ReservedSeats)
                {
                    totalReservedSeats += seats;
                }
                TextBlock totalReservedSeatsBlock = new TextBlock
                {
                    Text = totalReservedSeats.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Button editButton = new Button
                {
                    Content = "Edit",
                    Margin = new Thickness(2,0,2,0),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                editButton.Click += (sender, args) =>
                {
                    ReservationsWindow reservationsWindow = new(reservation, logicHandler);
                    reservationsWindow.ShowDialog();
                };
                Button deleteButton = new Button
                {
                    Content = "Delete",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                deleteButton.Click += (sender, args) =>
                {
                    logicHandler.DeleteReservation(reservation.Id);
                    UpdateReservationsList();
                };

                int row = ReservationsList.RowDefinitions.Count - 1;
                Grid.SetRow(nameBlock, row);
                Grid.SetColumn(nameBlock, 0);
                Grid.SetRow(trainNameBlock, row);
                Grid.SetColumn(trainNameBlock, 1);
                Grid.SetRow(totalReservedSeatsBlock, row);
                Grid.SetColumn(totalReservedSeatsBlock, 2);
                Grid.SetRow(editButton, row);
                Grid.SetColumn(editButton, 3);
                Grid.SetRow(deleteButton, row);
                Grid.SetColumn(deleteButton, 4);

                ReservationsList.Children.Add(nameBlock);
                ReservationsList.Children.Add(trainNameBlock);
                ReservationsList.Children.Add(totalReservedSeatsBlock);
                ReservationsList.Children.Add(editButton);
                ReservationsList.Children.Add(deleteButton);
            }
        }

        private void UpdateTrainsList()
        {
            TrainsList.Children.Clear();
            TrainsList.RowDefinitions.Clear();
            foreach (Train train in logicHandler.GetTrains())
            {
                TrainsList.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
                TextBox nameBox = new TextBox
                {
                    Text = train.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                nameBox.TextChanged += (sender, args) =>
                {
                    logicHandler.ChangeTrainName(train.Id, nameBox.Text);
                };
                Button carriageButton = new Button
                {
                    Content = "Carriages",
                    Margin = new Thickness(2,0,2,0),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                carriageButton.Click += (sender, args) =>
                {
                    CarriageEditor carriageWindow = new(train.Carriages, (id, number, capacity) =>
                    {
                        try
                        {
                            logicHandler.TrainCarriageChanged(train.Id, id, number, capacity);
                        }
                        catch (ApplicationException e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                    carriageWindow.ShowDialog();
                };
                Button scheduleButton = new Button
                {
                    Content = "Schedule",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                scheduleButton.Click += (sender, args) =>
                {
                    ScheduleEditor scheduleWindow = new(train.Schedule, (id, name, time) =>
                    {
                        logicHandler.TrainStationChanged(train.Id, id, name, time);
                    });
                    scheduleWindow.ShowDialog();
                };
                DatePicker startDatePicker = new DatePicker
                {
                    SelectedDate = train.StartDate,
                    Margin = new Thickness(2,0,2,0),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                startDatePicker.SelectedDateChanged += (sender, args) =>
                {
                    if (startDatePicker.SelectedDate == null)
                    {
                        startDatePicker.SelectedDate = train.StartDate;
                        return;
                    }
                    if (startDatePicker.SelectedDate.Value > train.EndDate)
                    {
                        startDatePicker.SelectedDate = train.EndDate;
                    }
                    logicHandler.ChangeTrainStartDate(train.Id, startDatePicker.SelectedDate.Value);
                };
                DatePicker endDatePicker = new DatePicker
                {
                    SelectedDate = train.EndDate,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                endDatePicker.SelectedDateChanged += (sender, args) =>
                {
                    if (endDatePicker.SelectedDate == null)
                    {
                        endDatePicker.SelectedDate = train.EndDate;
                        return;
                    }
                    if (endDatePicker.SelectedDate.Value < train.StartDate)
                    {
                        endDatePicker.SelectedDate = train.StartDate;
                    }
                    logicHandler.ChangeTrainEndDate(train.Id, endDatePicker.SelectedDate.Value);
                };
                Button reservationsButton = new Button
                {
                    Content = "Add Reservation",
                    Margin = new Thickness(2,0,2,0),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                reservationsButton.Click += (sender, args) =>
                {
                    Reservation reservation = logicHandler.AddNewReservation(train.Id, new Dictionary<Guid, int>());
                    ReservationsWindow reservationsWindow = new(reservation, logicHandler);
                    reservationsWindow.ShowDialog();
                    UpdateReservationsList();
                };
                Button deleteButton = new Button
                {
                    Content = "Delete",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                deleteButton.Click += (sender, args) =>
                {
                    try
                    {
                        logicHandler.DeleteTrain(train.Id);
                    }
                    catch (ApplicationException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    UpdateTrainsList();
                };

                int row = TrainsList.RowDefinitions.Count - 1;
                Grid.SetRow(nameBox, row);
                Grid.SetColumn(nameBox, 0);
                Grid.SetRow(carriageButton, row);
                Grid.SetColumn(carriageButton, 1);
                Grid.SetRow(scheduleButton, row);
                Grid.SetColumn(scheduleButton, 2);
                Grid.SetRow(startDatePicker, row);
                Grid.SetColumn(startDatePicker, 3);
                Grid.SetRow(endDatePicker, row);
                Grid.SetColumn(endDatePicker, 4);
                Grid.SetRow(reservationsButton, row);
                Grid.SetColumn(reservationsButton, 5);
                Grid.SetRow(deleteButton, row);
                Grid.SetColumn(deleteButton, 6);

                TrainsList.Children.Add(nameBox);
                TrainsList.Children.Add(carriageButton);
                TrainsList.Children.Add(scheduleButton);
                TrainsList.Children.Add(startDatePicker);
                TrainsList.Children.Add(endDatePicker);
                TrainsList.Children.Add(reservationsButton);
                TrainsList.Children.Add(deleteButton);
            }
        }

        private void AddNewTrain(object sender, RoutedEventArgs e)
        {
            if (NewTrainName.Text == "")
            {
                MessageBox.Show("Train name cannot be empty");
                return;
            }
            if (NewTrainStartDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a start date");
                return;
            }
            if (NewTrainEndDate.SelectedDate == null)
            {
                MessageBox.Show("Please select an end date");
                return;
            }
            if (NewTrainStartDate.SelectedDate.Value > NewTrainEndDate.SelectedDate.Value)
            {
                MessageBox.Show("Start date cannot be after end date");
                return;
            }
            logicHandler.AddNewTrain(
                NewTrainName.Text,
                stations,
                carriages,
                NewTrainStartDate.SelectedDate.Value,
                NewTrainEndDate.SelectedDate.Value
                );
            UpdateTrainsList();
        }

        private List<Train.Carriage> carriages = new();
        private List<Train.Station> stations = new();
        private LogicHandler logicHandler = new LogicHandler();
    }
}
