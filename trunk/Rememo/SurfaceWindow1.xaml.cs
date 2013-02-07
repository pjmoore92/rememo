using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.Collections.Generic;
using System.Timers;

namespace Rememo
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow, ReminderObserver
    {

        #region Constants
        /// <summary>
        /// Constants Required for Rememo
        /// </summary>
        /// 
        
        public DiaryConfig diaryConfig;
        public int selectedWeek;

        

        public const int REMINDER_OFFSET_X = 80;
        public const int REMINDER_WIDTH = 350;
        public const int REMINDER_HEIGHT = 23;

        public const int MORNING_OFFSET_Y = 35;
        public const int AFTERNOON_OFFSET_Y = 75;
        public const int EVENING_OFFSET_Y = 115;
        public const int FOUR_OFFSET_Y = 155;

        private String enteredText = "";
        private bool underline;
        private bool asterisk;
        private bool circle;
        

        private String oldText;

        System.Timers.Timer clock = new System.Timers.Timer(1000);

        private DiaryEntryCollection diaryEntryCollection;

        DiaryEntry newEntry;

        #region Reminder constants
        // Reminders & notes
        private bool remindersToShow;
        private Reminder reminderToDeliver;
        private ReminderCollection reminders;
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            Console.WriteLine("Does the config exist at line 67?", DiaryManager.ConfigExists().ToString());
            if (DiaryManager.ConfigExists())
            {
                InitializeComponent();

                clock.Elapsed += new System.Timers.ElapsedEventHandler(clock_Elapsed);
                Console.WriteLine("Config found!");
                clock.Enabled = true;
                Console.WriteLine("clock enabled");
                Console.WriteLine("Welcome Text set");
            }
            else
            {  
                
                InitializeComponent();

                clock.Elapsed += new System.Timers.ElapsedEventHandler(clock_Elapsed);
                Console.WriteLine("No config file!");
                clock.Enabled = true;
                Console.WriteLine("clock enabled");
                welcomeText(false);
                
                
            }
            // Add handlers for Application activation events
            AddActivationHandlers();

        }
        #endregion

        #region Closing Logic + Other Stuff
        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for Application activation events
            RemoveActivationHandlers();
        }

        /// <summary>
        /// Adds handlers for Application activation events.
        /// </summary>
        private void AddActivationHandlers()
        {
            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Removes handlers for Application activation events.
        /// </summary>
        private void RemoveActivationHandlers()
        {
            // Unsubscribe from surface application activation events
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }
        #endregion

        #region Startup
        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
     
            InitDiary();
            InitReminders();
        }

        #endregion

        #region Application Start/Stop
        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>

        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        #endregion
        
        #region Calendar Display
        /// <summary>
        /// Show The Calendar from HomeScreen
        /// </summary>
        private void surfaceButton3_Click(object sender, RoutedEventArgs e)
        {
            //Change to Calendar View
            tabControl1.SelectedIndex = 1;
        }
        #endregion

        #region Initialise Diary

        private void welcomeText(bool configured)
        {
            if (configured){
                welcome.Text = "Welcome to ReMemo, " + diaryConfig.Name;
            }else{
                welcome.Text = "Please click settings above to customise your experience. ";
            }
        }

        private void InitDiary()
        {
            diaryConfig = DiaryManager.ReadConfig();
            diaryEntryCollection = DiaryEntryCollection.Load();

            Console.WriteLine("Initialised Diary.");
            

            DateTime now = DateTime.Now;
            DateTime start = diaryConfig.StartDate;

            bool configured = DiaryManager.ConfigExists();

            if (configured)
                welcomeText(true);
            

            if (now.DayOfYear - start.DayOfYear > 7)
            {
                // We're in week 2 now
                //button_nextWeek.Background = (SolidColorBrush)Resources["Colour6"];

                selectedWeek = 2;
            }
            else
            {
                // Still in week 1
                //button_prevWeek.Background = (SolidColorBrush)Resources["Colour6"];

                selectedWeek = 1;
            }

            // Update diary UI
            UpdateDiaryUI();

            //editingNote = false;
        }

        private void InitReminders()
        {
            remindersToShow = false;

            // Create a new reminder collection and start the clock
            reminders = new ReminderCollection();
            reminders.RegisterObserver(this);
            reminders.Start();

            // Copy reminders from diary collection to reminder manager
            foreach (DiaryEntry entry in diaryEntryCollection)
            {
                reminders.AddReminder(entry.Reminder);
                entry.Reminder.Entry = entry;
            }
        }

        public DiaryConfig GetDiaryConfig()
        {
            return diaryConfig;
        }
        #endregion

        /// <summary>
        /// Renders the given diary entry in the appropriate area of the diary.
        /// </summary>
        private void RenderDiaryEntry(DiaryEntry entry)
        {
            Canvas canvas = GetCanvasForDay(entry.Date.DayOfWeek);
            //ClearOldEntry(canvas);
            Console.WriteLine("The canvas in RenderDiaryEntry is: " + canvas.ToString());
            
            int y = 0;

            switch (entry.Time)
            {
                case TimeOfDay.Morning: y = MORNING_OFFSET_Y; break;
                case TimeOfDay.Afternoon: y = AFTERNOON_OFFSET_Y; break;
                case TimeOfDay.Evening: y = EVENING_OFFSET_Y; break;
                case TimeOfDay.Day: y = FOUR_OFFSET_Y; break;
                default: y = MORNING_OFFSET_Y; break;
            }
            Console.WriteLine("It is getting to this line!! 227");
            Console.WriteLine("Canvas is: " + canvas.Name.ToString());
            entry.Render(canvas, REMINDER_OFFSET_X, y, REMINDER_WIDTH, REMINDER_HEIGHT, false);
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Change to HomeScreen View
            tabControl1.SelectedIndex = 0;
        }
        
        private void ClearOldEntry()
        {
            Canvas[] canvases = { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };

            foreach (Canvas canvas in canvases)
            {
                List<UIElement> toRemove = new List<UIElement>();

                foreach (UIElement child in canvas.Children)
                {
                    if (!(child is FrameworkElement))
                        continue;

                    FrameworkElement fe = (FrameworkElement)child;

                    if (((String)fe.Tag) == "Reminder")
                        toRemove.Add(child);
                        Console.WriteLine("Something here is breaking it");
                }

                foreach (UIElement child in toRemove)
                {
                    canvas.Children.Remove(child);
                }
            }
        }


        private void UpdateDiaryUI()
        {
            ClearOldEntry();
            //ClearAllNotes();
            
            UpdateDateLabels();
            

            DateTime firstDayOfWeek = GetFirstDayOfSelectedWeek();
            DateTime lastDayOfWeek = firstDayOfWeek.AddDays(7);

            // Render diary entries which are for this week
             foreach (DiaryEntry entry in diaryEntryCollection)
             {
                 if (entry.Date.CompareTo(firstDayOfWeek) >= 0 && entry.Date.CompareTo(lastDayOfWeek) <= 0)
                 {
                     Console.WriteLine("The entry is: " + entry.Text.ToString());
                     RenderDiaryEntry(entry);
                 }
             }

             // Render notes
            // IEnumerable<DiaryNote> notes = diaryEntryCollection.GetNotesForWeek(selectedWeek);

             /*foreach (DiaryNote note in notes)
             {
                 RenderDiaryNote(note);
             }*/
        }

        private void UpdateDateLabels()
        {
            DateTime startOfSelectedWeek = GetStartOfSelectedWeek();
            DateTime firstDayOfSelectedWeek = GetFirstDayOfSelectedWeek();
            DateTime now = DateTime.Now;

            TextBlock[] textBlocks = { MondayTextBlock, TuesdayTextBlock, WednesdayTextBlock, ThursdayTextBlock, FridayTextBlock, SaturdayTextBlock, SundayTextBlock };
            String[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            DateTime temp = firstDayOfSelectedWeek;
            for (int i = 0; i < 7; i++)
            {
                textBlocks[i].Text = String.Format("{0} {1}{2} {3}", temp.DayOfWeek, temp.Day, GetDatePostfix(temp), months[temp.Month - 1]);

                // Highlight the label representing "today"
                if (temp.Day == now.Day && temp.Month == now.Month && temp.Year == now.Year)
                    //textBlocks[i].Foreground = (SolidColorBrush)Resources["Colour3"];
                    textBlocks[i].Foreground = Brushes.Gold;
                else
                    textBlocks[i].Foreground = Brushes.Black;

                temp = temp.AddDays(1);
            }
        }

        private static String GetDatePostfix(DateTime dt)
        {
            return (dt.Day == 1) ? "st" : (dt.Day == 2) ? "nd" : (dt.Day == 3) ? "rd" : "th";

        }


        private DateTime GetStartOfSelectedWeek()
        {
            return diaryConfig.StartDate.AddDays(7 * (selectedWeek - 1));
        }

        /// <summary>
        /// Gets the first day (the Monday) of the selected week.
        /// </summary>
        /// <returns></returns>
        private DateTime GetFirstDayOfSelectedWeek()
        {
            DateTime startOfSelectedWeek = GetStartOfSelectedWeek();

            int daysIntoWeek = 0;

            switch (startOfSelectedWeek.DayOfWeek)
            {
                case DayOfWeek.Monday: daysIntoWeek = 0; break;
                case DayOfWeek.Tuesday: daysIntoWeek = 1; break;
                case DayOfWeek.Wednesday: daysIntoWeek = 2; break;
                case DayOfWeek.Thursday: daysIntoWeek = 3; break;
                case DayOfWeek.Friday: daysIntoWeek = 4; break;
                case DayOfWeek.Saturday: daysIntoWeek = 5; break;
                case DayOfWeek.Sunday: daysIntoWeek = 6; break;
            }

            return startOfSelectedWeek.AddDays(-daysIntoWeek);
        }


        void clock_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                hours.Text = DateTime.Now.Hour.ToString();
                minutes.Text = DateTime.Now.Minute.ToString();
                seconds.Text = DateTime.Now.Second.ToString();
            }));
        }

        private Canvas ClickLocation(Point point)
        {
            Rect mondayRect = new Rect(0, 0, Monday.ActualWidth, Monday.ActualHeight);
            Rect tuesdayRect = new Rect(0, mondayRect.Bottom, Tuesday.ActualWidth, Tuesday.ActualHeight);
            Rect wednesdayRect = new Rect(0, tuesdayRect.Bottom, Wednesday.ActualWidth, Wednesday.ActualHeight);
            Rect thursdayRect = new Rect(0, wednesdayRect.Bottom, Thursday.ActualWidth, Thursday.ActualHeight);
            Rect fridayRect = new Rect(mondayRect.Right, 0, Friday.ActualWidth, Friday.ActualHeight);
            Rect saturdayRect = new Rect(mondayRect.Right, fridayRect.Bottom, Saturday.ActualWidth, Saturday.ActualHeight);
            Rect sundayRect = new Rect(mondayRect.Right, saturdayRect.Bottom, Sunday.ActualWidth, Sunday.ActualHeight);
            Rect notesRect = new Rect(mondayRect.Right, sundayRect.Bottom, Notes.ActualWidth, Notes.ActualHeight);

            if (mondayRect.Contains(point))
            {
                return Monday;
            }
            else if (tuesdayRect.Contains(point))
            {
                return Tuesday;
            }
            else if (wednesdayRect.Contains(point))
            {
                return Wednesday;
            }
            else if (thursdayRect.Contains(point))
            {
                return Thursday;
            }
            else if (fridayRect.Contains(point))
            {
                return Friday;
            }
            else if (saturdayRect.Contains(point))
            {
                return Saturday;
            }
            else if (sundayRect.Contains(point))
            {
                return Sunday;
            }
            else if (notesRect.Contains(point))
            {
                return Notes;
            }
            else
            {
                return null;
            }
        }

        private int ClickRow(Point point)
        {
            Rect row1 = new Rect(0, RowLine1.Y1 - 30, RowLine1.ActualWidth, 30);
            Rect row2 = new Rect(0, RowLine2.Y1 - 30, RowLine2.ActualWidth, 30);
            Rect row3 = new Rect(0, RowLine2.Y1 + 1, RowLine2.ActualWidth, 30);

            if (row1.Contains(point))
                return 1;
            else if (row2.Contains(point))
                return 2;
            else if (row3.Contains(point))
                return 3;
            else
                return -1;
        }

        private DateTime GetDateForDayInSelectedWeek(DayOfWeek day)
        {
            DateTime theMonday = GetFirstDayOfSelectedWeek();
            DateTime temp = theMonday;

            while (temp.DayOfWeek != day)
            {
                temp = temp.AddDays(1);
            }

            return temp;
        }

        /// <summary>
        /// Gets the day of the week which represents the given canvas.
        /// </summary>
        private DayOfWeek GetDayForCanvas(Canvas canvas)
        {
            if (canvas.Name == "Monday")
                return DayOfWeek.Monday;
            else if (canvas.Name == "Tuesday")
                return DayOfWeek.Tuesday;
            else if (canvas.Name == "Wednesday")
                return DayOfWeek.Wednesday;
            else if (canvas.Name == "Thursday")
                return DayOfWeek.Thursday;
            else if (canvas.Name == "Friday")
                return DayOfWeek.Friday;
            else if (canvas.Name == "Saturday")
                return DayOfWeek.Saturday;
            else if (canvas.Name == "Sunday")
                return DayOfWeek.Sunday;

            // This function would never be called in a situation where it's not one of the previous values
            else
                return DayOfWeek.Monday;
        }


        private void AddEntry(Canvas canvas, TimeOfDay when)
        {
            if (when == TimeOfDay.None)
                return;
            
            //editingNote = false;

            // Get the date and time corresponding to the canvas which was clicked on.
            DateTime date = GetDateForDayInSelectedWeek(GetDayForCanvas(canvas));
           
            DateTime listboxTime = GetTimefromListbox();
            
            // Get any existing diary entry.
            DiaryEntry entry = diaryEntryCollection.GetEntry(date, when);

            Console.WriteLine("Checking if null line 454");
            if (entry == null)
            {
                // No existing entry - create one!
                Console.WriteLine("Null entry - Creating an entry!");
               // ResetKeyboard();
                enteredText = diaryEntryCanvas.Text;

                // Reset annotations
               // underline = false;
               // asterisk = false;
               // circle = false;

                // Initialise new entry
                newEntry = new DiaryEntry();
                newEntry.Time = when;
                newEntry.Date = date;
                newEntry.TimeSelected = listboxTime;
                
                newEntry.Text = diaryEntryCanvas.Text;
                Console.WriteLine("This is what is is in diaryEntryCanvas.text when renderEntry is called from line 480: " + diaryEntryCanvas.Text);
                Console.WriteLine("This is what is is in newEntry.text when renderEntry is called from line 480: " + newEntry.Text);
                TabAddEntry.IsSelected = true;
                RenderEntry();
            }
            else
            {
                // An entry exists for this date - let's edit it!

                //ResetKeyboard();
                diaryEntryCanvas.Text = entry.Text;
                //enteredText = entry.Text;
                //oldText = entry.Text;

                // Reset annotations
                //underline = entry.Underline;
                //asterisk = entry.Asterisk;
                //circle = entry.Circle;

                // Initialise new entry
                newEntry = entry;
                newEntry.Text = entry.Text;
                newEntry.Time = entry.Time;
                newEntry.Date = entry.Date;
                newEntry.TimeSelected = entry.TimeSelected;
                newEntry.Underline = entry.Underline;
                newEntry.Asterisk = entry.Asterisk;
                newEntry.Circle = entry.Circle;

                //UpdateAnnotationButtons();

                
                TabAddEntry.IsSelected = true;
                //RenderEntry();


                //log.Write("Add entry loaded");
            }
        }

        private DateTime GetTimefromListbox()
        {
            //ContentSelector.GetValue();
            return new DateTime();
        }

        private void AddNote(int row)
        {
            //editingNote = true;

            int week = selectedWeek;
            //DiaryNote note = diaryEntryCollection.GetNoteForWeek(week, row);

           /* if (note == null)
            {
              //  ResetKeyboard();
              //  enteredText = "";

                // Initialise new entry
              //  newNote = new DiaryNote();
              //  newNote.Week = selectedWeek;
              //  newNote.Row = row;

               // TabAddNote.IsSelected = true;
            }
            else
            {
              //  ResetKeyboard();
              //  enteredText = note.Text;
              //  oldText = note.Text;

                // Initialise new entry
              //  newNote = note;
              //  newNote.Week = note.Week;
              //  newNote.Text = note.Text;
              //  newNote.Row = note.Row;

               // TabAddNote.IsSelected = true;
              //  RenderNote();
            }
            */
            //log.Write("Add note loaded");
        }

        private Canvas GetCanvasForDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return Monday;
                case DayOfWeek.Tuesday: return Tuesday;
                case DayOfWeek.Wednesday: return Wednesday;
                case DayOfWeek.Thursday: return Thursday;
                case DayOfWeek.Friday: return Friday;
                case DayOfWeek.Saturday: return Saturday;
                case DayOfWeek.Sunday: return Sunday;
                default: return null;
            }
        }

        private void RenderEntry()
        {
            if (newEntry != null)
            {
                oldText = newEntry.Text;

                newEntry.Text = enteredText;
                newEntry.Asterisk = asterisk;
                newEntry.Circle = circle;
                newEntry.Underline = underline;
                // Not needed anymore - newEntry.Render(canvas, 0, 0, DiaryEntryCanvasColumn.ActualWidth, DiaryEntryCanvasRow.ActualHeight, true);
            }

            TabAddEntry.IsSelected = true;
        }

        private TimeOfDay ClickArea(Point point)
        {
            Rect morning = new Rect(0, Sep1.Y1 - 30, Sep1.ActualWidth, 30);
            Rect afternoon = new Rect(0, Sep2.Y1 - 30, Sep2.ActualWidth, 30);
            Rect evening = new Rect(0, Sep2.Y1 + 1, Sep2.ActualWidth, 30);
            
            if (morning.Contains(point))
                return TimeOfDay.Morning;
            else if (afternoon.Contains(point))
                return TimeOfDay.Afternoon;
            else if (evening.Contains(point))
                return TimeOfDay.Evening;
            else
            //Forced code - MUST FIX!    
                return TimeOfDay.Day;
        }

     
        private void Grid_ContactDown(object sender, ContactEventArgs e)
        {
            UIElement source = sender as UIElement;
            Point point = e.GetPosition(source);
            Console.WriteLine("This is the point = " + point.ToString());
            
            Canvas clicked = ClickLocation(point);
            Console.WriteLine("This is the clicked variable", clicked.ToString());
            
            if (clicked == Notes)
            {
                int row = ClickRow(e.GetPosition(clicked));

                if (row != -1)
                    AddNote(row);
            }
            else if (clicked != null)
            {
                TimeOfDay area = ClickArea(e.GetPosition(clicked));
                AddEntry(clicked, area);
            }
        }


        private TimeSpan GetTimeForTimeOfDay(TimeOfDay time)
        {
            //CHANGE THIS
            int h = 0, m = 0;
            Console.WriteLine("Time of day is: " + time.ToString());

            switch (time)
            {
                case TimeOfDay.Morning: h = (Int16)Application.Current.Resources["MorningTime"]; m = (Int16)Application.Current.Resources["MorningTimeMins"]; break;
                case TimeOfDay.Afternoon: h = (Int16)Application.Current.Resources["AfternoonTime"]; m = (Int16)Application.Current.Resources["AfternoonTimeMins"]; break;
                case TimeOfDay.Evening: h = (Int16)Application.Current.Resources["EveningTime"]; m = (Int16)Application.Current.Resources["EveningTimeMins"]; break;
                default: h = (Int16)Application.Current.Resources["MorningTime"]; m = (Int16)Application.Current.Resources["MorningTimeMins"]; break;
            }

            return new TimeSpan(h, m, 0);
        }

        private void button_acceptEntry_Click(object sender, RoutedEventArgs e)
        {
            newEntry.Text = diaryEntryCanvas.Text;

            if (newEntry == null || newEntry.Text.Length == 0)
            {
                Console.WriteLine("New entry was null or 0");
                Console.WriteLine("I think its because text length is too small:" + newEntry.Text.Length.ToString());
                return;
            }

            #region Create reminder for entry
            // Create a new reminder for the chosen time and date
            TimeSpan time = GetTimeForTimeOfDay(newEntry.Time);

            // Take the day, month and year from the existing reminder. Calculate time based on the TimeOfDay.
            DateTime date = new DateTime(newEntry.Date.Year, newEntry.Date.Month, newEntry.Date.Day, time.Hours, time.Minutes, time.Seconds);

            Console.WriteLine("Date and time of reminder: {0} at {1}", date.ToLongDateString(), date.ToShortTimeString());

            // See if the reminder has already occurred, i.e. someone added something to the past
            bool reminderInPast = date.Ticks < DateTime.Now.Ticks;

            // Unregister any old reminders for this entry first
            
            reminders.RemoveReminder(newEntry.Reminder);

            // Assign a high priority if annotations are used
           
            ReminderPriority priority = (newEntry.Underline || newEntry.Circle || newEntry.Asterisk) ? ReminderPriority.High : ReminderPriority.Low;

            // Then register a new one
           
            Reminder reminder = new Reminder(newEntry.Text, date, reminderInPast, reminderInPast, priority, newEntry);
            newEntry.Reminder = reminder;
            reminders.AddReminder(reminder);
            #endregion

            // Save newEntry

            Console.WriteLine("Have just added a new entry?? " + DateTime.Now.ToString());
            diaryEntryCollection.AddEntry(newEntry);

            // Update diary UI

            UpdateDiaryUI();

            // Clear canvas and switch tab
            //ResetAnnotationButtons();
            diaryEntryCanvas.Clear();
            TabDiary.IsSelected = true;

            //log.Write("Accepted new diary entry");
        }

        private void button_cancelEntry_Click(object sender, RoutedEventArgs e)
        {
           // ResetKeyboard();
           // diaryEntryCanvas.Children.Clear();
           // enteredText = "";

           // if (newEntry != null && oldText != null)
            //    newEntry.Text = oldText;

           // ResetAnnotationButtons();

            TabDiary.IsSelected = true;

           // log.Write("Cancelling new diary entry");
        }

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void surfaceButton2_Click(object sender, RoutedEventArgs e)
        {
            userName.Text = diaryConfig.Name;
            TabConfig.IsSelected = true;
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String temp = ContentSelector.SelectedItem.ToString();
            timeSelect.Content = temp.Substring(temp.IndexOf(':')+1,6);

        }

        private void userName_Loaded(object sender, RoutedEventArgs e)
        {
            if (DiaryManager.ConfigExists())
            {
                userName.Text = diaryConfig.Name;
            }
        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            diaryConfig.Name = userName.Text;
            DiaryManager.WriteConfig(diaryConfig);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            welcomeText(true);
            tabItem1.IsSelected = true;
        }

        #region Reminder delivery
        /// <summary>
        /// Callback function for the observable object to
        /// notify that a reminder is due for delivery.
        /// </summary>
        public void DeliverReminders(List<Reminder> reminders)
        {
            if (reminders == null || reminders.Count == 0)
                return;

           // Logger.Write("Reminder", String.Format("Received {0} reminders to deliver.", reminders.Count));

            Reminder max = reminders[0];

            for (int i = 1; i < reminders.Count; i++)
            {
                if (reminders[i].Time.Ticks > max.Time.Ticks)
                    max = reminders[i];
            }

            if (max.Time.Day < DateTime.Now.Day)
            {
             //   Logger.Write("Reminder", "Most recent reminder was yesterday. Not delivering.");

                for (int i = 1; i < reminders.Count; i++)
                {
                    reminders[i].Delivered = true;
                    reminders[i].Done = true;
                    diaryEntryCollection.Save();
                }

                return;
            }

           // Logger.Write("Reminder", "Delivered most recent reminder.");

            remindersToShow = true;
            reminderToDeliver = max;

            // This gets called from a non-UI thread, so use the dispatcher
            Dispatcher.Invoke(new Action(delegate()
            {
                // Update the UI
              
                //DisplaySomething();
                UserNotifications.RequestNotification("The following reminder is due", reminderToDeliver.ToString());

                //AddAbstractReminder(canvas);

                if (reminderToDeliver.Priority == ReminderPriority.High)
                {
                    // Play musicon
                   // MusiconManager.Play();
                }
            }));
        }
        #endregion

    }
}