﻿using Blazor.IndexedDB.Framework;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
{
    public class DataService
    {
        public Settings Settings = new Settings();

        public List<Category> categoryList { get; set; } = new List<Category>();
        public List<Habit> habitList { get; set; } = new List<Habit>();
        public List<Time> timeList { get; set; } = new List<Time>();

        public Dictionary<long, Category> categoryDict { get; set; } = new Dictionary<long, Category>();
        public Dictionary<long, Habit> habitDict { get; set; } = new Dictionary<long, Habit>();
        public Dictionary<long, Time> timeDict { get; set; } = new Dictionary<long, Time>();

        [Inject]
        IIndexedDbFactory DbFactory { get; set; } = null!;

        public async Task LoadData()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            if (db.Settings.Count == 0)
            {
                db.Settings.Add(new Settings());
                await db.SaveChanges();
            }

            Settings = db.Settings.First();

            if (db.Categories.Count == 0)
            {
                db.Categories.Add(new Category());
                await db.SaveChanges();
            }

            categoryList = db.Categories.ToList();
            habitList = db.Habits.ToList();
            timeList = db.Times.ToList();

            categoryDict = categoryList.ToDictionary(category => category.Id);
            habitDict = habitList.ToDictionary(habit => habit.Id);
            timeDict = timeList.ToDictionary(time => time.Id);

            foreach (Time time in timeList)
            {
                if (habitDict.ContainsKey(time.HabitId))
                    habitDict[time.HabitId].TimeList.Add(time);
            }

            foreach (Habit habit in habitList)
            {
                if (categoryDict.ContainsKey(habit.CategoryId))
                    categoryDict[habit.CategoryId].HabitList.Add(habit);
            }
        }

        async Task SaveCategory(Category category)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

            if (category.Id == 0)
            {
                db.Categories.Add(category);
            }

            await db.SaveChanges();

            await LoadData();
        }

        async Task DeleteCategory(Category category)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Categories.Remove(category);
            await db.SaveChanges();

            await LoadData();
        }

        async Task SaveHabit(Habit habit)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

            if (habit.Id == 0)
            {
                db.Habits.Add(habit);
            }

            await db.SaveChanges();

            await LoadData();
        }

        async Task DeleteHabit(Habit habit)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Habits.Remove(habit);
            await db.SaveChanges();

            await LoadData();
        }

        async Task DeleteTime(Time time)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();
            db.Times.Remove(time);
            await db.SaveChanges();

            await LoadData();
        }

        async Task AddTime(Habit habit)
        {
            using IndexedDatabase db = await this.DbFactory.Create<IndexedDatabase>();

            Time time = new Time
            {
                HabitId = habit.Id,
                DateTime = DateTime.Now
            };

            db.Times.Add(time);

            await db.SaveChanges();

            await LoadData();
        }
    }
}