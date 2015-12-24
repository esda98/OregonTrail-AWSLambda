﻿// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 11/14/2015@3:12 AM

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     The pace at which you travel can change.
    /// </summary>
    public enum TravelPace
    {
        /// <summary>
        ///     8 hours a day, taking frequent rests. You take care not to get too tired.
        /// </summary>
        Steady = 1,

        /// <summary>
        ///     12 hours a day, stopping only when necessary. Finish each day feeling very tired.
        /// </summary>
        Strenuous = 2,

        /// <summary>
        ///     16 hours a day, you never rest and barley sleep. You are absolutely exhausted, and your health suffers.
        /// </summary>
        Grueling = 3
    }
}