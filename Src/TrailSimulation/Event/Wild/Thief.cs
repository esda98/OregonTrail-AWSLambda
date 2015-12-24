﻿// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@6:43 AM

namespace TrailSimulation.Event
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Entity;
    using Game;

    /// <summary>
    ///     Robber who can come in the middle of the night and steal things from the vehicle inventory. He is also very
    ///     dangerous and will do whatever it takes to get what he wants, so there is a chance some of your party members may
    ///     get murdered.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Thief : EventItemDestroyer
    {
        /// <summary>Fired by the item destroyer event prefab before items are destroyed.</summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        /// <returns>The <see cref="string" />.</returns>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            // Change event text depending on if items were destroyed or not.
            return destroyedItems.Count > 0
                ? TryKillPassengers("murdered")
                : "no loss of items.";
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnPreDestroyItems()
        {
            var theifPrompt = new StringBuilder();
            theifPrompt.Clear();
            theifPrompt.AppendLine("Thief comes in the");
            theifPrompt.Append("night resulting in ");
            return theifPrompt.ToString();
        }
    }
}