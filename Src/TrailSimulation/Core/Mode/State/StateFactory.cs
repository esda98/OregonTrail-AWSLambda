﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TrailSimulation.Game;
using TrailSimulation.Utility;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Keeps track of all the possible states a given game mode can have by using attributes and reflection to keep track
    ///     of which user data object gets mapped to which particular state.
    /// </summary>
    public sealed class StateFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.StateFactory" /> class.
        /// </summary>
        public StateFactory()
        {
            // Create dictionaries for reference tracking for what states belong to what game modes.
            LoadedStates = new Dictionary<Type, Mode>();

            // Collect all of the states with the custom attribute decorated on them.
            var foundStates = AttributeHelper.GetTypesWith<RequiredModeAttribute>(false);
            foreach (var stateType in foundStates)
            {
                // GetModule the attribute itself from the state we are working on, which gives us the game mode enum.
                var stateAttribute = stateType.GetAttributes<RequiredModeAttribute>(false).First();
                var stateParentMode = stateAttribute.ParentMode;

                // Add the state reference list for lookup and instancing later during runtime.
                LoadedStates.Add(stateType, stateParentMode);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the reflected state types.
        /// </summary>
        private Dictionary<Type, Mode> LoadedStates { get; set; }

        /// <summary>
        ///     Creates and adds the specified type of state to currently active game mode.
        /// </summary>
        /// <param name="stateType">Role object that is the actual type of state that needs created.</param>
        /// <param name="activeMode">Current active game mode passed to factory so no need to call game simulation singleton.</param>
        /// <returns>Created state instance from reference types build on startup.</returns>
        public IStateProduct CreateStateFromType(Type stateType, IModeProduct activeMode)
        {
            // Check if the state exists in our reference list.
            if (!LoadedStates.ContainsKey(stateType))
                throw new ArgumentException(
                    "State factory cannot create state from type that does not exist in reference states! " +
                    "Perhaps developer forgot [RequiredMode] attribute on state?!");

            // States are based on abstract class, but never should be one.
            if (stateType.IsAbstract)
                return null;

            // Create the state, it will have constructor with one parameter.
            var stateInstance = Activator.CreateInstance(
                stateType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[]
                {
                    // Grab the user data object from active mode
                    activeMode
                },
                CultureInfo.InvariantCulture);

            // Pass the created state back to caller.
            return stateInstance as IStateProduct;
        }

        /// <summary>
        ///     Called when primary simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            LoadedStates.Clear();
            LoadedStates = null;
        }
    }
}