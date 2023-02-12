using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
namespace Ejednevnik
{
    public class Note
    {
        public int id;
        public DateTime date;
        public string title;
        public string description;
        public Note(int id, string title, string description, DateTime date)
        {
            this.title = title;
            this.description = description;
            this.date = date;
            this.id = id;
        }

    }
    public class Timetable
    {
        public List<Note> todayNotes;
        public DateTime selectedDate;
        public int selectedTaskId = -1;
        public List<Note> allNotes;
        public Timetable(DateTime date)
        {
            this.todayNotes = Data.LoadNotes(date);
            this.allNotes = Data.LoadNotes(default);
            selectedDate = date;
        }
        public void RefreshNotes()
        {
            this.todayNotes = Data.LoadNotes(this.selectedDate);
        }
        public void UpdateNotes()
        {
            Data.SaveNotes(this.allNotes);
        }
        public void NewNote(string title, string desc, DateTime date)
        {

            Note note = new Note(this.allNotes.Count, title, desc, date);
            this.allNotes.Add(note);
            Data.SaveNotes(this.allNotes);
            this.RefreshNotes();
        }
        public void EditNote(string title, string desc, DateTime date)
        {
            if (selectedTaskId != -1)
            {
                Note note = new Note(this.todayNotes[selectedTaskId].id, title, desc, date);
                DeleteNote(this.todayNotes[selectedTaskId].id);
                this.allNotes.Add(note);
                UpdateNotes();
                this.RefreshNotes();
                this.selectedTaskId = -1;
            }
        }
        public void DeleteNote(int id=-1, int todayId=-1)
        {
            if (todayId != -1)
                id = this.todayNotes[todayId].id;
            List<Note> newNotes = new List<Note>();
            foreach (Note note in this.allNotes)
            {
                if (note.id != id)
                {
                    newNotes.Add(note);
                }
            }
            this.allNotes = newNotes;
            RefreshNotes();
            UpdateNotes();
        }
    }
    public class Data
    {
        public static void SaveNotes(List<Note> notes)
        {
            File.WriteAllText("notes.json", JsonConvert.SerializeObject(notes));
        }
        public static List<Note> LoadNotes(DateTime date = default)
        {
            List<Note> notes = new List<Note>();
            try
            {
                List<Note> raw_notes = JsonConvert.DeserializeObject<List<Note>>(File.ReadAllText("notes.json"));
                foreach (Note note in raw_notes)
                {
                    if (note.date == date || date == default)
                        notes.Add(note);
                }
            }
            catch
            {
                notes = new List<Note>();
                File.WriteAllText("notes.json", JsonConvert.SerializeObject(notes));

            }
            return notes;
        }
    }
}
