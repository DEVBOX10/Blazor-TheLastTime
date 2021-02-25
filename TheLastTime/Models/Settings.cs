﻿using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Models
{
    public enum Ratio
    {
        ElapsedToAverage,
        ElapsedToDesired,
        AverageToDesired
    }

    public enum Sort
    {
        Index,
        Description,
        ElapsedTime,
        SelectedRatio
    }

    public class Settings
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long ShowPercentMin { get; set; }

        [Required]
        public bool? ShowStarred { get; set; }

        [Required]
        public bool? ShowTwoMinute { get; set; }

        [Required]
        public bool? ShowNeverDone { get; set; }

        [Required]
        public bool? ShowDoneOnce { get; set; }

        [Required]
        public bool? ShowRatioOverPercentMin { get; set; }

        [Required]
        public bool ShowHabitId { get; set; }

        [Required]
        public bool ShowHabitIdUpDownButtons { get; set; }

        [Required]
        public bool ShowAllSelectOptions { get; set; }

        [Required]
        public bool ShowCategoriesInHeader { get; set; }

        [Required]
        public string Size { get; set; } = string.Empty;

        [Required]
        public string Theme { get; set; } = string.Empty;

        [Required]
        public Ratio Ratio { get; set; }

        [Required]
        public Sort Sort { get; set; }
    }
}
