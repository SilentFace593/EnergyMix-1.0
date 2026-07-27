using System.Collections.Generic;
using Colossal.Serialization.Entities;

namespace EnergyMix
{
    public struct EnergySnapshot
    {
        public double GameDays;
        public float Wind;
        public float Coal;
        public float Gas;
        public float Incineration;
        public float Solar;
        public float Geothermal;
        public float Nuclear;
        public float Hydro;

        public float GetValue(EnergyCategory cat)
        {
            switch (cat)
            {
                case EnergyCategory.Wind: return Wind;
                case EnergyCategory.Coal: return Coal;
                case EnergyCategory.Gas: return Gas;
                case EnergyCategory.Incineration: return Incineration;
                case EnergyCategory.Solar: return Solar;
                case EnergyCategory.Geothermal: return Geothermal;
                case EnergyCategory.Nuclear: return Nuclear;
                case EnergyCategory.Hydro: return Hydro;
                default: return 0f;
            }
        }

        public void SetValue(EnergyCategory cat, float value)
        {
            switch (cat)
            {
                case EnergyCategory.Wind: Wind = value; break;
                case EnergyCategory.Coal: Coal = value; break;
                case EnergyCategory.Gas: Gas = value; break;
                case EnergyCategory.Incineration: Incineration = value; break;
                case EnergyCategory.Solar: Solar = value; break;
                case EnergyCategory.Geothermal: Geothermal = value; break;
                case EnergyCategory.Nuclear: Nuclear = value; break;
                case EnergyCategory.Hydro: Hydro = value; break;
            }
        }

        public static EnergySnapshot Merge(EnergySnapshot a, EnergySnapshot b)
        {
            return new EnergySnapshot
            {
                GameDays = (a.GameDays + b.GameDays) * 0.5,
                Wind = (a.Wind + b.Wind) * 0.5f,
                Coal = (a.Coal + b.Coal) * 0.5f,
                Gas = (a.Gas + b.Gas) * 0.5f,
                Incineration = (a.Incineration + b.Incineration) * 0.5f,
                Solar = (a.Solar + b.Solar) * 0.5f,
                Geothermal = (a.Geothermal + b.Geothermal) * 0.5f,
                Nuclear = (a.Nuclear + b.Nuclear) * 0.5f,
                Hydro = (a.Hydro + b.Hydro) * 0.5f,
            };
        }
    }

    public static class EnergySnapshotIO
    {
        public static void Write<TWriter>(TWriter writer, in EnergySnapshot s) where TWriter : IWriter
        {
            writer.Write(s.GameDays);
            writer.Write(s.Wind);
            writer.Write(s.Coal);
            writer.Write(s.Gas);
            writer.Write(s.Incineration);
            writer.Write(s.Solar);
            writer.Write(s.Geothermal);
            writer.Write(s.Nuclear);
            writer.Write(s.Hydro);
        }

        public static EnergySnapshot Read<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out double gameDays);
            reader.Read(out float wind);
            reader.Read(out float coal);
            reader.Read(out float gas);
            reader.Read(out float incineration);
            reader.Read(out float solar);
            reader.Read(out float geothermal);
            reader.Read(out float nuclear);
            reader.Read(out float hydro);
            return new EnergySnapshot
            {
                GameDays = gameDays,
                Wind = wind,
                Coal = coal,
                Gas = gas,
                Incineration = incineration,
                Solar = solar,
                Geothermal = geothermal,
                Nuclear = nuclear,
                Hydro = hydro,
            };
        }
    }

    /// <summary>
    /// Stockage managé simple (listes), porté par EnergyMixSimulationSystem
    /// qui implémente ISerializable directement (mécanisme de sérialisation
    /// "au niveau système", confirmé par la trace de pile de PrefabSystem.Serialize).
    /// </summary>
    public class EnergyMixData
    {
        public EnergySnapshot Current;

        private const int FineCapacity = 200;
        private const int MediumCapacity = 150;
        private const int CoarseCapacity = 150;

        public readonly List<EnergySnapshot> Fine = new List<EnergySnapshot>(FineCapacity + 1);
        public readonly List<EnergySnapshot> Medium = new List<EnergySnapshot>(MediumCapacity + 1);
        public readonly List<EnergySnapshot> Coarse = new List<EnergySnapshot>(CoarseCapacity + 1);

        public void PushSnapshot(EnergySnapshot snapshot)
        {
            Current = snapshot;
            Fine.Add(snapshot);
            if (Fine.Count > FineCapacity)
            {
                EnergySnapshot merged = EnergySnapshot.Merge(Fine[0], Fine[1]);
                Fine.RemoveAt(0);
                Fine.RemoveAt(0);
                PushToMedium(merged);
            }
        }

        private void PushToMedium(EnergySnapshot snapshot)
        {
            Medium.Add(snapshot);
            if (Medium.Count > MediumCapacity)
            {
                EnergySnapshot merged = EnergySnapshot.Merge(Medium[0], Medium[1]);
                Medium.RemoveAt(0);
                Medium.RemoveAt(0);
                PushToCoarse(merged);
            }
        }

        private void PushToCoarse(EnergySnapshot snapshot)
        {
            Coarse.Add(snapshot);
            if (Coarse.Count > CoarseCapacity)
            {
                EnergySnapshot merged = EnergySnapshot.Merge(Coarse[0], Coarse[1]);
                Coarse.RemoveAt(0);
                Coarse.RemoveAt(0);
                Coarse.Insert(0, merged);
            }
        }

        private const int SerializationVersion = 1;

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write(SerializationVersion);
            EnergySnapshotIO.Write(writer, in Current);
            WriteList(writer, Fine);
            WriteList(writer, Medium);
            WriteList(writer, Coarse);
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out int version);
            if (version != SerializationVersion)
            {
                Colossal.Logging.LogManager.GetLogger("EnergyMix")
                    .Warn($"EnergyMixData: version {version} inconnue (attendu {SerializationVersion}). Historique réinitialisé.");
                return;
            }
            Current = EnergySnapshotIO.Read(reader);
            ReadList(reader, Fine);
            ReadList(reader, Medium);
            ReadList(reader, Coarse);
        }

        private static void WriteList<TWriter>(TWriter writer, List<EnergySnapshot> list) where TWriter : IWriter
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                EnergySnapshot s = list[i];
                EnergySnapshotIO.Write(writer, in s);
            }
        }

        private static void ReadList<TReader>(TReader reader, List<EnergySnapshot> list) where TReader : IReader
        {
            reader.Read(out int count);
            list.Clear();
            for (int i = 0; i < count; i++)
                list.Add(EnergySnapshotIO.Read(reader));
        }
    }
}