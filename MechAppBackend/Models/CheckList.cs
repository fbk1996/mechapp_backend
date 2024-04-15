using System;
using System.Collections.Generic;

namespace MechAppBackend.Models
{
    public partial class CheckList
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public short? SuspensionFrontStatus { get; set; }
        public string? SuspensionFrontDescription { get; set; }
        public short? BrakesFrontStatus { get; set; }
        public string? BrakesFrontDescription { get; set; }
        public short? EngineSuspensionStatus { get; set; }
        public string? EngineSuspensionDescription { get; set; }
        public short? LeaksStatus { get; set; }
        public string? LeaksDescription { get; set; }
        public short? SuspensionRearStatus { get; set; }
        public string? SuspensionRearDescription { get; set; }
        public short? BrakesRearStatus { get; set; }
        public string? BrakesRearDescription { get; set; }
        public short? ExhoustSystemStatus { get; set; }
        public string? ExhoustSystemDescription { get; set; }
        public short? FluidLevelStatus { get; set; }
        public string? FluidLevelDescription { get; set; }
        public short? EngineStatus { get; set; }
        public string? EngineDescription { get; set; }
        public short? ElectricSystemStatus { get; set; }
        public string? ElectricSystemDescription { get; set; }

        public virtual Order? Order { get; set; }
    }
}
