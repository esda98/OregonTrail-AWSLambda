﻿using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public sealed class GameSimulationApp : SimulationApp, IGameSimulation
    {
        /// <summary>
        ///     Manages weather, temperature, humidity, and current grazing level for living animals.
        /// </summary>
        private ClimateSimulation _climate;

        /// <summary>
        ///     Manages time in a linear since from the provided ticks in base simulation class. Handles days, months, and years.
        /// </summary>
        private TimeSimulation _time;

        /// <summary>
        ///     Current vessel which the player character and his party are traveling inside of, provides means of transportation
        ///     other than walking.
        /// </summary>
        private Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        public GameSimulationApp(SimulationType simulationType) : base(simulationType)
        {
            // Only setup game logic on server.
            if (simulationType != SimulationType.Server)
                return;

            _time = new TimeSimulation(1985, Months.May, 5, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            _climate = new ClimateSimulation(this, ClimateClassification.Moderate);
            TrailSimulation = new TrailSimulation();
            TotalTurns = 0;
            _vehicle = new Vehicle(this);
        }

        public TrailSimulation TrailSimulation { get; private set; }

        public override void OnDestroy()
        {
            if (SimulationType == SimulationType.Server)
            {
                // Unhook delegates from events.
                _time.DayEndEvent -= TimeSimulation_DayEndEvent;
                _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
                _time.YearEndEvent -= TimeSimulation_YearEndEvent;
                _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;

                // Destroy all instances.
                _time = null;
                _climate = null;
                TrailSimulation = null;
                TotalTurns = 0;
                _vehicle = null;
            }

            base.OnDestroy();
        }

        public ITimeSimulation Time
        {
            get { return _time; }
        }

        public IClimateSimulation Climate
        {
            get { return _climate; }
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        public uint TotalTurns { get; private set; }

        public void TakeTurn()
        {
            // Only servers can increment turns and tick time logic.
            if (SimulationType != SimulationType.Server)
                return;

            TotalTurns++;
            _time.TickTime();
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            // Title and current game mode.
            var title = new StringBuilder();
            title.Append($"Oregon Trail {SimulationType} - ");
            title.Append($"Mode: {ActiveModeName} - ");

            // Server has some extra pieces of information in title.
            if (SimulationType == SimulationType.Server)
            {
                title.Append($"Turns: {TotalTurns.ToString("D4")} - ");
                title.Append($"Clients: {TotalClients} - ");
            }

            // Tick spinner phase to show program tick activity.
            title.Append($"[{TickPhase}]");
            return title.ToString();
        }

        protected override void OnFirstTick()
        {
            base.OnFirstTick();

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            if (SimulationType == SimulationType.Server)
            {
                AddMode(ModeType.NewGame);
            }
        }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="mode">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        protected override GameMode OnModeChanging(ModeType mode)
        {
            switch (mode)
            {
                case ModeType.Travel:
                    return new TravelMode(this);
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode(this);
                case ModeType.Hunt:
                    return new HuntMode(this);
                case ModeType.Landmark:
                    return new LandmarkMode(this);
                case ModeType.NewGame:
                    return new NewGameMode(this);
                case ModeType.RandomEvent:
                    return new RandomEventMode(this);
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode(this);
                case ModeType.Settlement:
                    return new SettlementMode(this);
                case ModeType.Store:
                    return new StoreMode(this);
                case ModeType.Trade:
                    return new TradeMode(this);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private void TimeSimulation_SpeedChangeEvent()
        {
            // Only servers process speed change events.
            if (SimulationType != SimulationType.Server)
                return;

            Console.WriteLine("Travel pace changed to " + _vehicle.Pace);
        }

        private void TimeSimulation_YearEndEvent(uint yearCount)
        {
            // Only servers process logic at the end of the year.
            if (SimulationType != SimulationType.Server)
                return;

            Console.WriteLine("Year end!");
        }

        private void TimeSimulation_DayEndEvent(uint dayCount)
        {
            // Only servers process logic at the end of the day.
            if (SimulationType != SimulationType.Server)
                return;

            _climate.TickClimate();
            Vehicle.UpdateVehicle();
            TrailSimulation.ReachedPointOfInterest();
            _vehicle.DistanceTraveled += (uint) Vehicle.Pace;

            Console.WriteLine("Day end!");
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
            // Only servers process logic at the end of the month.
            if (SimulationType != SimulationType.Server)
                return;

            Console.WriteLine("Month end!");
        }
    }
}