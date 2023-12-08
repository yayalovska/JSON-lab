using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace l3
{
    public class Event
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string Specialty { get; set; }
        public string Timing { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        private List<Event> _events = new List<Event>();
        private const string JsonFilePath = "assets/events.json";


        public MainPage()
        {
            InitializeComponent();
            LoadJsonFile();
        }

        private void LoadJsonFile()
        {
            try
            {
                if (File.Exists(JsonFilePath))
                {
                    string jsonString = File.ReadAllText(JsonFilePath);
                    var loadedEvents = JsonSerializer.Deserialize<List<Event>>(jsonString);
                    if (loadedEvents != null)
                    {
                        _events.Clear();
                        _events.AddRange(loadedEvents);
                    }
                }
                else
                {
                    Console.WriteLine($"JSON file does not exist at path: {JsonFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON file: {ex.Message}");
            }
        }

        private async void OnOpenFileClicked(object sender, EventArgs e)
        {
            try
            {
                var openFileResult = await FilePicker.PickAsync();
                if (openFileResult != null)
                {
                    using (var stream = await openFileResult.OpenReadAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        string jsonString = await reader.ReadToEndAsync();
                        var loadedEvents = JsonSerializer.Deserialize<List<Event>>(jsonString);
                        if (loadedEvents != null)
                        {
                            _events.Clear();
                            _events.AddRange(loadedEvents);
                            RefreshListView();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening JSON file: {ex.Message}");
            }
        }

        private void SaveJsonToFile()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(_events, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(JsonFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON file: {ex.Message}");
            }
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            var newEvent = new Event
            {
                Name = nameEntry.Text,
                Faculty = facultyEntry.Text,
                Department = departmentEntry.Text,
                Specialty = specialtyEntry.Text,
                Timing = timingEntry.Text
            };

            _events.Add(newEvent);
            RefreshListView();
            SaveJsonToFile();
        }

        private void OnUpdateClicked(object sender, EventArgs e)
        {
            if (eventsListView.SelectedItem is Event selectedEvent)
            {
                selectedEvent.Name = nameEntry.Text;
                selectedEvent.Faculty = facultyEntry.Text;
                selectedEvent.Department = departmentEntry.Text;
                selectedEvent.Specialty = specialtyEntry.Text;
                selectedEvent.Timing = timingEntry.Text;

                RefreshListView();
                SaveJsonToFile();
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (eventsListView.SelectedItem is Event selectedEvent)
            {
                _events.Remove(selectedEvent);
                RefreshListView();
                SaveJsonToFile();
            }
        }

        private void RefreshListView()
        {
            eventsListView.ItemsSource = null;
            eventsListView.ItemsSource = _events;
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            SaveJsonToFile();
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            nameEntry.Text = string.Empty;
            facultyEntry.Text = string.Empty;
            departmentEntry.Text = string.Empty;
            specialtyEntry.Text = string.Empty;
            timingEntry.Text = string.Empty;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Event selectedEvent)
            {
                nameEntry.Text = selectedEvent.Name;
                facultyEntry.Text = selectedEvent.Faculty;
                departmentEntry.Text = selectedEvent.Department;
                specialtyEntry.Text = selectedEvent.Specialty;
                timingEntry.Text = selectedEvent.Timing;
            }
        }

        private void OnExitButtonClicked(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}