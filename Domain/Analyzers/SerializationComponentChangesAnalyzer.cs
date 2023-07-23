using Domain.Components;
using Domain.DocumentComponents;
using Domain.DocumentComponentsChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Domain.Analyzers
{
    public class SerializationComponentChangesAnalyzer : ChangesAnalyzer<SerializationComponent?, SerializationComponentChanges?>
    {
        public override SerializationComponentChanges? GetChanges(SerializationComponent? newVersion, SerializationComponent? prevVersion)
        {
            if (newVersion == null && prevVersion == null)
                return null;

            SerializationComponentChanges serializationComponentChanges = new SerializationComponentChanges();
            serializationComponentChanges.AssemblyName = newVersion?.AssemblyName ?? prevVersion!.AssemblyName;
            serializationComponentChanges.ClassName = newVersion?.ClassName ?? prevVersion!.ClassName;
            serializationComponentChanges.SysID = newVersion?.SysID ?? prevVersion!.SysID;

            if (newVersion != null && prevVersion != null)
            {
                serializationComponentChanges.Properties = newVersion.Properties.Join(prevVersion.Properties,
                                                            n => n.Name,
                                                            p => p.Name,
                                                            (n, p) => new SerializationItemChanges()
                                                            {
                                                                Name = n.Name,
                                                                IsListValue = n.IsListValue,
                                                                NewValue = n.Value,
                                                                OldValue = p.Value,
                                                                SysState = n.Value != p.Value ? SysState.Modified : SysState.None
                                                            }).ToList();

                serializationComponentChanges.SysState = serializationComponentChanges.Properties.Any(x => x.SysState == SysState.Modified) ? SysState.Modified : SysState.None;

                serializationComponentChanges.Components = new List<SerializationComponentChanges>();
                decimal stepNew = 0.0m;
                foreach (var newComponent in newVersion.Components)
                {
                    SerializationComponent? prevComponent = prevVersion.Components?.FirstOrDefault(x => x.SysID == newComponent.SysID);

                    SerializationComponentChanges? changes = GetChanges(newComponent, prevComponent);
                    changes.OrderNum = ++stepNew;
                    changes.SysState = prevComponent == null ? SysState.New : SysState.None;

                    
                    serializationComponentChanges.Components.Add(changes);

                }

                decimal stepOld = 0.000000001m;
                decimal curStep = 0.0m;
                foreach (var prevComponent in prevVersion.Components)
                {
                    SerializationComponent? newComponent = newVersion.Components.FirstOrDefault(x => x.SysID == prevComponent.SysID);

                    if (newComponent != null)
                    {
                        decimal orderNum = serializationComponentChanges.Components.First(x => x.SysID == newComponent.SysID).OrderNum;
                        curStep = orderNum;
                        stepOld = 0.000000001m;
                        continue;
                    }
                    else
                    {
                        SerializationComponentChanges? changes = GetChanges(null, prevComponent);
                        changes.SysState = SysState.Deleted;
                        changes.OrderNum = curStep + stepOld;
                        stepOld += 0.000000001m;

                        serializationComponentChanges.Components.Add(changes);

                    }

                }

                

                var placementSwaps = prevVersion.Components 
                            .Join(serializationComponentChanges.Components
                                    .Where(s => s.SysState == SysState.None)
                                    , c => c.SysID
                                    , scCh => scCh.SysID
                                    , (c, scCh) => new { scCh.SysID, scCh.OrderNum, scCh.SysState}).ToList();

                foreach (var pSwap in placementSwaps)
                {
                    bool isPlacementSwap = pSwap.SysState == SysState.None
                        && placementSwaps.Any(x => x.OrderNum < pSwap.OrderNum && placementSwaps.IndexOf(x) > placementSwaps.IndexOf(pSwap));

                    if (isPlacementSwap)
                    {
                        var component = serializationComponentChanges.Components.First(x => x.SysID == pSwap.SysID);
                        component.SysState = SysState.Modified;
                    }
                    
                }

                serializationComponentChanges.Components = serializationComponentChanges.Components.OrderBy(x => x.OrderNum).ToList();

            }
            else if (newVersion != null)
            {
                serializationComponentChanges.SysState = SysState.New;
                serializationComponentChanges.Properties = newVersion.Properties.Select(x => new SerializationItemChanges()
                                                                                        {
                                                                                            Name = x.Name,
                                                                                            IsListValue = x.IsListValue,
                                                                                            NewValue = x.Value
                                                                                        }).ToList();


                foreach (var newComponent in newVersion.Components)
                {
                    SerializationComponentChanges? changes = GetChanges(newComponent, null);
                    serializationComponentChanges.Components.Add(changes);

                }
            }
            else if (prevVersion != null)
            {
                serializationComponentChanges.SysState = SysState.Deleted;
                serializationComponentChanges.Properties = prevVersion.Properties.Select(x => new SerializationItemChanges()
                                                                                        {
                                                                                            Name = x.Name,
                                                                                            IsListValue = x.IsListValue,
                                                                                            OldValue = x.Value
                                                                                        }).ToList();
                foreach (var prevComponent in prevVersion.Components)
                {
                    SerializationComponentChanges? changes = GetChanges(null, prevComponent);
                    serializationComponentChanges.Components.Add(changes);
                }
            }

            return serializationComponentChanges;
        }

        public override bool IsDifferent(SerializationComponent newVersion, SerializationComponent prevVersion)
        {
            throw new NotImplementedException();
        }
    }
}
