﻿using Blazor.IndexedDB.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TheLastTime.Data;

namespace TheLastTime.Shared
{
    public partial class NavMenu
    {
        protected string bootswatchTheme = "superhero";

        readonly Dictionary<string, string> bootswatchThemeDict = new Dictionary<string, string>()
        {
            //{ "cerulean", "sha384-3fdgwJw17Bi87e1QQ4fsLn4rUFqWw//KU0g8TvV6quvahISRewev6/EocKNuJmEw" },
            { "cosmo", "sha384-5QFXyVb+lrCzdN228VS3HmzpiE7ZVwLQtkt+0d9W43LQMzz4HBnnqvVxKg6O+04d" },
            //{ "cyborg", "sha384-nEnU7Ae+3lD52AK+RGNzgieBWMnEfgTbRHIwEvp1XXPdqdO6uLTd/NwXbzboqjc2" },
            //{ "darkly", "sha384-nNK9n28pDUDDgIiIqZ/MiyO3F4/9vsMtReZK39klb/MtkZI3/LtjSjlmyVPS3KdN" },
            //{ "flatly", "sha384-qF/QmIAj5ZaYFAeQcrQ6bfVMAh4zZlrGwTPY7T/M+iTTLJqJBJjwwnsE5Y0mV7QK" },
            //{ "journal", "sha384-QDSPDoVOoSWz2ypaRUidLmLYl4RyoBWI44iA5agn6jHegBxZkNqgm2eHb6yZ5bYs" },
            //{ "litera", "sha384-enpDwFISL6M3ZGZ50Tjo8m65q06uLVnyvkFO3rsoW0UC15ATBFz3QEhr3hmxpYsn" },
            //{ "lumen", "sha384-GzaBcW6yPIfhF+6VpKMjxbTx6tvR/yRd/yJub90CqoIn2Tz4rRXlSpTFYMKHCifX" },
            { "lux", "sha384-9+PGKSqjRdkeAU7Eu4nkJU8RFaH8ace8HGXnkiKMP9I9Te0GJ4/km3L1Z8tXigpG" },
            //{ "materia", "sha384-B4morbeopVCSpzeC1c4nyV0d0cqvlSAfyXVfrPJa25im5p+yEN/YmhlgQP/OyMZD" },
            //{ "minty", "sha384-H4X+4tKc7b8s4GoMrylmy2ssQYpDHoqzPa9aKXbDwPoPUA3Ra8PA5dGzijN+ePnH" },
            { "pulse", "sha384-L7+YG8QLqGvxQGffJ6utDKFwmGwtLcCjtwvonVZR/Ba2VzhpMwBz51GaXnUsuYbj" },
            //{ "sandstone", "sha384-zEpdAL7W11eTKeoBJK1g79kgl9qjP7g84KfK3AZsuonx38n8ad+f5ZgXtoSDxPOh" },
            //{ "simplex", "sha384-FYrl2Nk72fpV6+l3Bymt1zZhnQFK75ipDqPXK0sOR0f/zeOSZ45/tKlsKucQyjSp" },
            //{ "sketchy", "sha384-RxqHG2ilm4r6aFRpGmBbGTjsqwfqHOKy1ArsMhHusnRO47jcGqpIQqlQK/kmGy9R" },
            //{ "slate", "sha384-8iuq0iaMHpnH2vSyvZMSIqQuUnQA7QM+f6srIdlgBrTSEyd//AWNMyEaSF2yPzNQ" },
            //{ "solar", "sha384-NCwXci5f5ZqlDw+m7FwZSAwboa0svoPPylIW3Nf+GBDsyVum+yArYnaFLE9UDzLd" },
            //{ "spacelab", "sha384-F1AY0h4TrtJ8OCUQYOzhcFzUTxSOxuaaJ4BeagvyQL8N9mE4hrXjdDsNx249NpEc" },
            { "superhero", "sha384-HnTY+mLT0stQlOwD3wcAzSVAZbrBp141qwfR4WfTqVQKSgmcgzk+oP0ieIyrxiFO" },
            //{ "united", "sha384-JW3PJkbqVWtBhuV/gsuyVVt3m/ecRJjwXC3gCXlTzZZV+zIEEl6AnryAriT7GWYm" },
            { "yeti", "sha384-mLBxp+1RMvmQmXOjBzRjqqr0dP9VHU2tb3FK6VB0fJN/AOu7/y+CAeYeWJZ4b3ii" },
        };

        private bool collapseNavMenu = true;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        async Task ImportFile(InputFileChangeEventArgs e)
        {
            Stream stream = e.File.OpenReadStream();

            using StreamReader streamReader = new StreamReader(stream);

            string jsonString = await streamReader.ReadToEndAsync();
        }

        [Inject]
        IIndexedDbFactory DbFactory { get; set; } = null!;

        [Inject]
        IJSRuntime JSRuntime { get; set; } = null!;

        async Task ExportFile()
        {
            using IndexedDatabase db = await DbFactory.Create<IndexedDatabase>();

            List<Category> categoryList = db.Categories.ToList();
            List<Habit> habitList = db.Habits.ToList();
            List<Time> timeList = db.Times.ToList();

            Dictionary<long, Category> categoryDict = categoryList.ToDictionary(category => category.Id);
            Dictionary<long, Habit> habitDict = habitList.ToDictionary(habit => habit.Id);

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

            string jsonString = JsonSerializer.Serialize(categoryList, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

            await SaveAs(JSRuntime, "TheLastTime.json", bytes);
        }

        static async Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        }
    }
}